using Fontendo.Extensions;
using Fontendo.Extensions.BinaryTools;
using System;
using System.Collections.Generic;
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
        public CMAP((Glyph First,Glyph Last) Entries, UInt32 Magic = 0x434D4150U)
        {
            this.Magic = Magic;
            this.Length = 0x0U;
            this.CodeBegin = Entries.First.Settings.GetValue<UInt16>(GlyphProperty.Code);
            this.CodeEnd = Entries.Last.Settings.GetValue<UInt16>(GlyphProperty.Code);
            this.MappingMethod = 0; //Direct
            this.Reserved = 0;
            this.PtrNext = 0x0U;
            this.Entries = new List<CMAPEntry>();
            this.Entries.Add(new CMAPEntry(CodeBegin, Entries.First.Settings.GetValue<UInt16>(GlyphProperty.Index)));
        }
        public CMAP(UInt16 MappingMethod, List<CMAPEntry> Entries, UInt32 Magic = 0x434D4150U)
        {
            this.Magic = Magic;
            this.Length = 0x0U;
            this.MappingMethod = MappingMethod;
            this.Reserved = 0;
            this.PtrNext = 0x0U;
            this.Entries = Entries;
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

                MainForm.Log($"Parsing CMAP: Start={br.BaseStream.Position}, CodeBegin={CodeBegin}, CodeEnd={CodeEnd}, MappingMethod={MappingMethod}");

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
        public static List<(Glyph First, Glyph Last)> CreateDirectEntries(List<Glyph> glyphs)
        {
            var result = new List<(Glyph First, Glyph Last)>();
            int pos = 0;

        begin:
            if (pos >= glyphs.Count)
                return result;

            Stride r = new Stride();
            while (GetNextStride(glyphs, ref r, glyphs.Count - 1, pos))
            {
                if(r.First == null || r.Second == null)
                    break;
                pos = (int)r.Second + 1;

                int spanLength = glyphs[(int)r.Second].Settings.GetValue<UInt16>(GlyphProperty.Code) - glyphs[(int)r.First].Settings.GetValue<UInt16>(GlyphProperty.Code) + 1;
                if (spanLength >= 80)
                {
                    // Add the pair of glyphs marking the stride
                    result.Add((glyphs[(int)r.First], glyphs[(int)r.Second]));

                    // Remove the stride range from the glyph list
                    glyphs.RemoveRange((int)r.First, (int)r.Second - (int)r.First + 1);

                    // Restart from beginning (like the C++ goto)
                    pos = 0;
                    goto begin;
                }
            }

            return result;
        }

        public class Stride
        {
            public int? First;
            public int? Second;
        }

        public static List<List<CMAPEntry>> CreateTableEntries(List<Glyph> glyphs)
        {
            var result = new List<List<CMAPEntry>>();
            var pos = 0; // index into glyphs

            Stride r1 = new Stride();
            while (pos < glyphs.Count() - 1)
            {
                // r1 is a pair of indices delimiting a stride
                if (!GetNextStride(glyphs, ref r1, glyphs.Count - 1, pos))
                    break;
                if(r1.First == glyphs.Count() - 1 || r1.First == null || r1.Second == null)
                    break;

                int num1 = glyphs[(int)r1.Second].Settings.GetValue<UInt16>(GlyphProperty.Code) - glyphs[(int)r1.First].Settings.GetValue<UInt16>(GlyphProperty.Code) + 1;
                int num2 = 0;

                while (true)
                {
                    Stride r2 = new Stride();
                    Stride r3 = new Stride();
                    float num3 = 0.0f;
                    float num4 = 0.0f;

                    if (r1.Second < glyphs.Count() - 1)
                    {
                        int next = (int)r1.Second + 1;
                        if (GetNextStride(glyphs, ref r2, glyphs.Count - 1, next))
                        {
                            if (r2.First != null && r2.Second != null)
                            {
                                int num5 = glyphs[(int)r2.Second].Settings.GetValue<UInt16>(GlyphProperty.Code) - glyphs[(int)r2.First].Settings.GetValue<UInt16>(GlyphProperty.Code) + 1;
                                int num6 = glyphs[(int)r2.First].Settings.GetValue<UInt16>(GlyphProperty.Code) - glyphs[(int)r1.Second].Settings.GetValue<UInt16>(GlyphProperty.Code) - 1;
                                int num7 = num1 + num5;
                                int num8 = num7 + num2 + num6;
                                num3 = (float)num7 / num8;
                            }
                        }
                    }

                    if (r1.First > 0 && r1.Second != null)
                    {
                        int prev = (int)r1.Second - 1;
                        if (GetPrevStride(glyphs, ref r3, 0, prev))
                        {
                            if (r3.First != null && r3.Second != null)
                            {
                                int num5 = glyphs[(int)r3.Second].Settings.GetValue<UInt16>(GlyphProperty.Code) - glyphs[(int)r3.First].Settings.GetValue<UInt16>(GlyphProperty.Code) + 1;
                                int num6 = glyphs[(int)r1.First].Settings.GetValue<UInt16>(GlyphProperty.Code) - glyphs[(int)r3.Second].Settings.GetValue<UInt16>(GlyphProperty.Code) - 1;
                                int num7 = num1 + num5;
                                int num8 = num7 + num2 + num6;
                                num4 = (float)num7 / num8;
                            }
                        }
                    }

                    if (num3 > num4)
                    {
                        if (num3 >= 0.5f && r2.First != null && r2.Second != null && r1.Second != null)
                        {
                            int num5 = glyphs[(int)r2.Second].Settings.GetValue<UInt16>(GlyphProperty.Code) - glyphs[(int)r2.First].Settings.GetValue<UInt16>(GlyphProperty.Code) + 1;
                            int num6 = glyphs[(int)r2.First].Settings.GetValue<UInt16>(GlyphProperty.Code) - glyphs[(int)r1.Second].Settings.GetValue<UInt16>(GlyphProperty.Code) - 1;
                            num1 += num5;
                            num2 += num6;
                            r1.Second = r2.Second;
                        }
                        else break;
                    }
                    else if (num4 >= 0.5f && r3.First != null && r3.Second != null)
                    {
                        int num5 = glyphs[(int)r3.Second].Settings.GetValue<UInt16>(GlyphProperty.Code) - glyphs[(int)r3.First].Settings.GetValue<UInt16>(GlyphProperty.Code) + 1;
                        int num6 = glyphs[(int)r1.First].Settings.GetValue<UInt16>(GlyphProperty.Code) - glyphs[(int)r3.Second].Settings.GetValue<UInt16>(GlyphProperty.Code) - 1;
                        num1 += num5;
                        num2 += num6;
                        r1.First = r3.First;
                    }
                    else break;
                }

                if (num1 < 40 && r1.Second != null)
                {
                    pos = (int)r1.Second + 1;
                }
                else if(r1.Second != null)
                {
                    UInt16 code1 = glyphs[(int)r1.First].Settings.GetValue<UInt16>(GlyphProperty.Code);
                    UInt16 code2 = glyphs[(int)r1.Second].Settings.GetValue<UInt16>(GlyphProperty.Code);
                    UInt16 length = (UInt16)(code2 - code1 + 1);

                    var entries = new List<CMAPEntry>();
                    for (UInt16 offset = 0; offset < length; ++offset)
                    {
                        var glyph = GetGlyphByCodePoint(glyphs, (UInt16)(code1 + offset));
                        if (glyph != null)
                        {
                            entries.Add(new CMAPEntry(glyph.Settings.GetValue<UInt16>(GlyphProperty.Code), glyph.Settings.GetValue<UInt16>(GlyphProperty.Index)));
                        }
                        else
                        {
                            entries.Add(new CMAPEntry((UInt16)(code1 + offset), 0xFFFF));
                        }
                    }

                    result.Add(entries);
                    pos = (int)r1.Second + 1;

                    // Erase range [r1.first, r1.second]
                    glyphs.RemoveRange((int)r1.First, (int)r1.Second - (int)r1.First + 1);
                }
            }

            return result;
        }



        /// <summary>
        /// Find the next contiguous stride of glyphs with sequential code/index values.
        /// </summary>
        public static bool GetNextStride(List<Glyph> glyphs, ref Stride r, int lastIndex, int posIndex)
        {
            if (posIndex == lastIndex + 1)
                return false;
            r = new Stride();
            ushort code = (ushort)(glyphs[posIndex].Settings.GetValue<UInt16>(GlyphProperty.Code) + 1U);
            ushort index = (ushort)(glyphs[posIndex].Settings.GetValue<UInt16>(GlyphProperty.Index) + 1U);

            int node = posIndex;
            int next = node + 1;

            for(var next2 = node+1; next != lastIndex + 1 &&
                   glyphs[next].Settings.GetValue<UInt16>(GlyphProperty.Code) == code &&
                   glyphs[next].Settings.GetValue<UInt16>(GlyphProperty.Index) == index; index++)
            {
                node = next2;
                next = node + 1;
                code++;
            }
            if (r.First == r.Second)
                return false;
            r.First = posIndex;
            r.Second = node;
            return true;
        }

        public static bool GetPrevStride(List<Glyph> glyphs, ref Stride r, int beginIndex, int posIndex)
        {
            if (posIndex == beginIndex)
                return false;

            int prev1 = posIndex - 1;
            if (prev1 == beginIndex)
                return false;

            ushort code = (ushort)(glyphs[prev1].Settings.GetValue<UInt16>(GlyphProperty.Code) - 1U);

            int prev2 = prev1 - 1;
            while (prev2 >= beginIndex &&
                   glyphs[prev2].Settings.GetValue<UInt16>(GlyphProperty.Code) == code)
            {
                prev2--;
                code--;
            }

            if (r.First == r.Second)
                return false;
            r.First = prev2 + 1;
            r.Second = prev1;
            return true;
        }


        /// <summary>
        /// Find a glyph by its code point.
        /// </summary>
        public static Glyph? GetGlyphByCodePoint(IEnumerable<Glyph> glyphs, UInt16 codePoint)
        {
            foreach (var g in glyphs)
            {
                if (g.Settings.GetValue<UInt16>(GlyphProperty.Code) == codePoint)
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
                    g.Settings.GetValue<UInt16>(GlyphProperty.Code), 
                    g.Settings.GetValue<UInt16>(GlyphProperty.Index))
                );
            }

            if (entries.Count > 0)
                result.Add(entries);

            return result;
        }

        public void Serialize(BinaryWriterX bw, BlockLinker linker)
        {
            MainForm.Log($"Serializing CMAP: Start={bw.BaseStream.Position}, CodeBegin={CodeBegin}, CodeEnd={CodeEnd}, MappingMethod={MappingMethod}, Entries={Entries.Count()}");

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
                    bw.WriteUInt16((ushort)Entries.Count());
                    foreach (var entry in Entries)
                    {
                        entry.Serialize(bw);
                    }
                    break;
            }
            MainForm.Log($"Wrote {Entries.Count()} ending at {bw.BaseStream.Position}");

            // Padding to 4-byte boundary
            uint padBytes = 0x4 - ((uint)bw.BaseStream.Position % 0x4);
            if (padBytes != 0x4)
            {
                for (uint i = 0; i < padBytes; i++)
                    bw.WriteByte((byte)0x0);
            }

            linker.AddLookupValue($"CMAPLen{sectionNum}", (uint)bw.BaseStream.Position - startPos);
        }

    }
}