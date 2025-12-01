using Fontendo.Extensions;
using Fontendo.Extensions.BinaryTools;
using System;
using System.Reflection.PortableExecutable;
using static Fontendo.FontProperties.GlyphProperties;
using static System.Net.Mime.MediaTypeNames;
namespace Fontendo.Formats.CTR
{
    public class CMAP
    {
        private UInt32 EntriesOffset;

        public UInt32 Magic; //Should always be 0x434D4150, CMAP in ASCII
        public UInt32 Length; //CMAP section length in bytes
        public UInt16 CodeBegin; //First character code
        public UInt16 CodeEnd; //Last character code
        public UInt16 MappingMethod; //Mapping method, 0x0 - Direct, 0x1 - Table, 0x2 - Scan
        public UInt16 Reserved; //Reserved (this is referred to it as so even in the official SDK software)
        public UInt32 PtrNext; //Pointer to the next CMAP(next CMAP offset + 0x8), 0x0 if last
        public List<CMAPEntry> Entries = new List<CMAPEntry>();

        private readonly BinaryReaderX? br;
        public CMAP(List<Glyph> Maps, UInt32 Magic = 0x434D4150U)
        {
            this.Magic = Magic;
            this.Length = 0x0U;
            this.CodeBegin = Maps[0].Properties.GetValue<UInt16>(GlyphProperty.Code);
            this.CodeEnd = Maps[Maps.Count() - 1].Properties.GetValue<UInt16>(GlyphProperty.Code);
            this.MappingMethod = 0; //Direct
            this.Reserved = 0;
            this.PtrNext = 0x0U;
            this.Entries = new List<CMAPEntry>();
            foreach (var glyph in Maps)
            {
                this.Entries.Add(new CMAPEntry(CodeBegin, glyph.Properties.GetValue<UInt16>(GlyphProperty.Index)));
            }
        }
        public CMAP(UInt16 MappingMethod, List<CMAPEntry> Maps, UInt32 Magic = 0x434D4150U)
        {
            this.Magic = Magic;
            this.Length = 0x0U;
            this.MappingMethod = MappingMethod;
            this.Reserved = 0;
            this.PtrNext = 0x0U;
            Entries = Maps;
            switch (MappingMethod)
            {
                case 0x1:
                    CodeBegin = Entries[0].Code;
                    CodeEnd = Entries[Entries.Count() - 1].Code;
                    break;
                case 0x2:
                    CodeBegin = 0x0;
                    CodeEnd = 0xFFFF;
                    break;
            }
        }
        public CMAP(BinaryReaderX br)
        {
            this.br = br;
        }

        public ActionResult Parse()
        {
            if (br == null)
                return new ActionResult(false, "Binary reader not attached to CMAP");
            try
            {
                Magic = br.ReadUInt32();
                Length = br.ReadUInt32();
                CodeBegin = br.ReadUInt16();
                CodeEnd = br.ReadUInt16();
                MappingMethod = br.ReadUInt16();
                Reserved = br.ReadUInt16();
                PtrNext = br.ReadUInt32();
                EntriesOffset = (UInt32)br.BaseStream.Position;


                MainForm.Log($"CMAP Entries Start {br.BaseStream.Position}");
                // read entries
                ActionResult result = ReadEntries(br);
                MainForm.Log($"CMAP Read {Entries.Count} entries using method {MappingMethod} ending at {br.BaseStream.Position}");
                if (!result.Success)
                    return result;
            }
            catch (Exception e)
            {
                return new ActionResult(false, $"CMAP exception {e.Message}");
            }
            if (!ValidateSignature())
                return new ActionResult(false, "Invalid CMAP signature!!!");

            return new ActionResult(true, "OK");
        }

        private bool ValidateSignature()
        {
            if (Magic != 0x434D4150U && Magic != 0x50414D43)
                return false;
            return true;
        }

        private ActionResult ReadEntries(BinaryReader br)
        {
            Entries = new List<CMAPEntry>();
            if (EntriesOffset == 0)
                return new ActionResult(false, "CMAP Entry Offset is zero");
            br.BaseStream.Position = EntriesOffset;
            UInt16 temp = 0;
            switch (MappingMethod)
            {
                case 0: //Direct
                    temp = br.ReadUInt16(); //indexOffset
                    for (UInt16 i = CodeBegin; i <= CodeEnd; i++)
                    {
                        Entries.Add(new CMAPEntry(i, temp));
                        temp++;
                    }
                    break;
                case 1: //Table
                    for (UInt16 i = CodeBegin; i <= CodeEnd; i++)
                    {
                        temp = br.ReadUInt16(); //index
                        if (temp != 0xFFFFU) Entries.Add(new CMAPEntry(i, temp));
                    }
                    break;
                case 2: //Scan
                    temp = br.ReadUInt16(); //count
                    for (UInt16 i = 0; i < temp; i++)
                    {
                        CMAPEntry entry = new CMAPEntry(br);
                        ActionResult result = entry.Parse();
                        if (!result.Success)
                            return new ActionResult(false, $"Failed reading CMAP Entry at index {i}\n\n{result.Message}");
                        Entries.Add(entry);
                    }
                    break;
            }
            return new ActionResult(true, "OK");
        }


        /// <summary>
        /// Create "direct" CMAP entries: contiguous runs of glyphs with sequential codes and indices.
        /// </summary>
        public static List<List<Glyph>> CreateDirectEntries(List<Glyph> glyphs)
        {
            var result = new List<List<Glyph>>();
            int pos = 0;

            while (pos < glyphs.Count)
            {
                bool restart = false;

                // Try to find strides starting at current position
                while (pos < glyphs.Count && GetNextStride(out var stride, glyphs, glyphs[pos]))
                {
                    // Advance position past this stride
                    pos = glyphs.IndexOf(stride[^1]) + 1;

                    ushort startCode = stride[0].Properties.GetValue<ushort>(GlyphProperty.Code);
                    ushort endCode = stride[^1].Properties.GetValue<ushort>(GlyphProperty.Code); // last element in stride
                    int codeSpan = endCode - startCode + 1;

                    if (codeSpan >= 80)
                    {
                        // Add the full stride (list of glyphs) to results
                        result.Add(stride);

                        // Remove those glyphs from the source list so they aren’t reused
                        foreach (var g in stride)
                            glyphs.Remove(g);

                        // Reset position and mark restart
                        pos = 0;
                        restart = true;
                        break;
                    }
                }

                if (!restart)
                    break;
            }

            return result;
        }

        public static List<List<CMAPEntry>> CreateTableEntries(List<Glyph> glyphs)
        {
            var result = new List<List<CMAPEntry>>();

            int pos = 0;
            while (pos < glyphs.Count)
            {
                // Find initial stride starting at glyphs[pos]
                if (!GetNextStride(out var stride1, glyphs, glyphs[pos]))
                    break;

                int glyphSpan = stride1[^1].Properties.GetValue<UInt16>(GlyphProperty.Code) -
                                stride1[0].Properties.GetValue<UInt16>(GlyphProperty.Code) + 1;
                int gapSpan = 0;

                // Try to extend stride forward/backward
                while (true)
                {
                    float forwardRatio = 0.0f;
                    float backwardRatio = 0.0f;

                    // Forward stride
                    if (stride1[^1] != glyphs[^1])
                    {
                        var nextGlyph = glyphs[glyphs.IndexOf(stride1[^1]) + 1];
                        if (GetNextStride(out var stride2, glyphs, nextGlyph))
                        {
                            int span2 = stride2[^1].Properties.GetValue<UInt16>(GlyphProperty.Code) -
                                        stride2[0].Properties.GetValue<UInt16>(GlyphProperty.Code) + 1;
                            int gap2 = stride2[0].Properties.GetValue<UInt16>(GlyphProperty.Code) -
                                       stride1[^1].Properties.GetValue<UInt16>(GlyphProperty.Code) - 1;
                            int combinedSpan = glyphSpan + span2;
                            int totalSpan = combinedSpan + gapSpan + gap2;
                            forwardRatio = (float)combinedSpan / totalSpan;

                            if (forwardRatio >= 0.5f)
                            {
                                glyphSpan += span2;
                                gapSpan += gap2;
                                stride1.AddRange(stride2);
                                continue;
                            }
                        }
                    }

                    // Backward stride
                    if (stride1[0] != glyphs[0])
                    {
                        var prevGlyph = glyphs[glyphs.IndexOf(stride1[0]) - 1];
                        if (GetPrevStride(out var stride3, glyphs, prevGlyph))
                        {
                            int span3 = stride3[^1].Properties.GetValue<UInt16>(GlyphProperty.Code) -
                                        stride3[0].Properties.GetValue<UInt16>(GlyphProperty.Code) + 1;
                            int gap3 = stride1[0].Properties.GetValue<UInt16>(GlyphProperty.Code) -
                                       stride3[^1].Properties.GetValue<UInt16>(GlyphProperty.Code) - 1;
                            int combinedSpan = glyphSpan + span3;
                            int totalSpan = combinedSpan + gapSpan + gap3;
                            backwardRatio = (float)combinedSpan / totalSpan;

                            if (backwardRatio >= 0.5f)
                            {
                                glyphSpan += span3;
                                gapSpan += gap3;
                                stride1.InsertRange(0, stride3);
                                continue;
                            }
                        }
                    }

                    // If neither forward nor backward extended, stop
                    break;
                }

                if (glyphSpan < 40)
                {
                    pos = glyphs.IndexOf(stride1[^1]) + 1;
                }
                else
                {
                    ushort codeStart = stride1[0].Properties.GetValue<UInt16>(GlyphProperty.Code);
                    ushort codeEnd = stride1[^1].Properties.GetValue<UInt16>(GlyphProperty.Code);
                    int length = codeEnd - codeStart + 1;

                    var entries = new List<CMAPEntry>();
                    for (int offset = 0; offset < length; offset++)
                    {
                        var glyph = GetGlyphByCodePoint(glyphs, (ushort)(codeStart + offset));
                        if (glyph != null)
                        {
                            entries.Add(new CMAPEntry(
                                glyph.Properties.GetValue<UInt16>(GlyphProperty.Code),
                                glyph.Properties.GetValue<UInt16>(GlyphProperty.Index)));
                        }
                        else
                        {
                            entries.Add(new CMAPEntry((ushort)(codeStart + offset), 0xFFFF));
                        }
                    }

                    result.Add(entries);

                    // Advance pos past the stride
                    pos = glyphs.IndexOf(stride1[^1]) + 1;

                    // Optionally: remove stride glyphs if you don’t want them reused
                    foreach (var g in stride1)
                        glyphs.Remove(g);
                }
            }

            return result;
        }



        /// <summary>
        /// Find the next contiguous stride of glyphs with sequential code/index values.
        /// </summary>
        public static bool GetNextStride(out List<Glyph> stride, List<Glyph> glyphs, Glyph current)
        {
            stride = new List<Glyph>();

            // Find the index of the current glyph in the list
            int pos = glyphs.IndexOf(current);
            if (pos == -1 || pos >= glyphs.Count)
                return false; // glyph not found or invalid position


            // Start with the current glyph
            var currentGlyph = glyphs[pos];
            stride.Add(currentGlyph);

            ushort code = currentGlyph.Properties.GetValue<UInt16>(GlyphProperty.Code);
            ushort index = currentGlyph.Properties.GetValue<UInt16>(GlyphProperty.Index);

            // Walk forward while codes and indices are consecutive
            int i = pos + 1;
            while (i < glyphs.Count)
            {
                var next = glyphs[i];
                ushort nextCode = next.Properties.GetValue<UInt16>(GlyphProperty.Code);
                ushort nextIndex = next.Properties.GetValue<UInt16>(GlyphProperty.Index);

                if (nextCode == code + 1 && nextIndex == index + 1)
                {
                    stride.Add(next);
                    code = nextCode;
                    index = nextIndex;
                    i++;
                }
                else
                {
                    break;
                }
            }

            return true;
        }


        /// <summary>
        /// Find the previous contiguous stride of glyphs with sequential code values backwards.
        /// </summary>
        public static bool GetPrevStride(out List<Glyph> stride, List<Glyph> glyphs, Glyph current)
        {
            stride = new List<Glyph>();
            int pos = glyphs.IndexOf(current);
            if (pos <= 0)
                return false; // can't go back

            int prev1Index = pos - 1;
            if (prev1Index <= 0)
                return false; // no stride possible

            // Start with prev1
            var prev1 = glyphs[prev1Index];
            stride.Add(prev1);

            UInt16 code = prev1.Properties.GetValue<UInt16>(GlyphProperty.Code);
            if(code != 0x0)
                code = (UInt16)(code - 0x1);

            int i = prev1Index - 1;
            while (i >= 0)
            {
                var prev2 = glyphs[i];
                ushort prevCode = prev2.Properties.GetValue<UInt16>(GlyphProperty.Code);

                if (prevCode == code)
                {
                    stride.Insert(0, prev2); // prepend to stride
                    code--;
                    i--;
                }
                else
                {
                    break;
                }
            }

            return true;
        }


        /// <summary>
        /// Find a glyph by its code point.
        /// </summary>
        public static Glyph? GetGlyphByCodePoint(IEnumerable<Glyph> glyphs, ushort codePoint)
        {
            foreach (var g in glyphs)
            {
                if (g.Properties.GetValue<UInt16>(GlyphProperty.Code) == codePoint)
                    return g;
            }
            return null;
        }

        /// <summary>
        /// Create "scan" CMAP entries: every glyph as a CMAPEntry.
        /// </summary>
        public static List<List<CMAPEntry>> CreateScanEntries(IEnumerable<Glyph> glyphs)
        {
            var result = new List<List<CMAPEntry>>();
            var entries = new List<CMAPEntry>();

            foreach (var g in glyphs)
            {
                entries.Add(new CMAPEntry(
                    g.Properties.GetValue<UInt16>(GlyphProperty.Code), 
                    g.Properties.GetValue<UInt16>(GlyphProperty.Index))
                );
            }

            if (entries.Count > 0)
                result.Add(entries);

            return result;
        }

        public void Serialize(BinaryWriterX bw, BlockLinker linker)
        {
            linker.IncLookupValue("blockCount", 1);
            linker.IncLookupValue("CMAP", 1);

            long startPos = bw.BaseStream.Position;
            long sectionNum = linker.GetLookupValue("CMAP");

            linker.AddLookupValue($"CMAP{sectionNum}", startPos + 0x8);

            bw.WriteUInt32(Magic);

            linker.AddPatchAddr(bw.BaseStream.Position, $"CMAPLen{sectionNum}");
            bw.WriteUInt32(Length);

            bw.WriteUInt16(CodeBegin);
            bw.WriteUInt16(CodeEnd);
            bw.WriteUInt16(MappingMethod);
            bw.WriteUInt16(Reserved);

            linker.AddPatchAddr(bw.BaseStream.Position, $"CMAP{sectionNum + 1}");
            bw.WriteUInt32(PtrNext);

            MainForm.Log($"CMAP Entries start {bw.BaseStream.Position}");
            switch (MappingMethod)
            {
                case 0: // Direct
                    bw.WriteUInt16(Entries[0].Index);
                    break;

                case 1: // Table
                    foreach (var entry in Entries)
                    {
                        bw.WriteUInt16(entry.Index);
                    }
                    break;

                case 2: // Scan
                    bw.WriteUInt16((ushort)Entries.Count);
                    foreach (var entry in Entries)
                    {
                        entry.Serialize(bw);
                    }
                    break;
            }

            // Padding to 4-byte boundary
            uint padBytes = 4U - ((uint)bw.BaseStream.Position % 4U);
            if (padBytes != 4U)
            {
                for (uint i = 0; i < padBytes; i++)
                    bw.WriteByte((byte)0x0);
            }

            linker.AddLookupValue($"CMAPLen{sectionNum}", (uint)bw.BaseStream.Position - startPos);
        }

    }
}