using Fontendo.Extensions;
using Fontendo.Extensions.BinaryTools;
using System;
using System.Reflection.PortableExecutable;
using static Fontendo.Formats.GlyphProperties;
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
        public CMAP((Glyph, Glyph) Maps, UInt32 Magic = 0x434D4150U)
        {
            this.Magic = Magic;
            this.Length = 0x0U;
            this.CodeBegin = GetGlyphPropertyValue<UInt16>(Maps.Item1, GlyphProperty.Code);
            this.CodeEnd = GetGlyphPropertyValue<UInt16>(Maps.Item2, GlyphProperty.Code);
            this.MappingMethod = 0; //Direct
            this.Reserved = 0;
            this.PtrNext = 0x0U;
            this.Entries.Add(new CMAPEntry(CodeBegin, GetGlyphPropertyValue<UInt16>(Maps.Item1, GlyphProperty.Index)));
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
        public static List<(Glyph Start, Glyph End)> CreateDirectEntries(List<Glyph> glyphs)
        {
            var glyphLinkedList = new LinkedList<Glyph>(glyphs);
            var result = new List<(Glyph Start, Glyph End)>();

            var pos = glyphLinkedList.First;

            while (true)
            {
                if (pos == null) return result;

                bool restart = false;

                while (GetNextStride(out var stride, glyphLinkedList.Last, pos))
                {
                    pos = stride.End.Next;

                    ushort startCode = GetGlyphPropertyValue<ushort>(stride.Start.Value, GlyphProperty.Code);
                    ushort endCode = GetGlyphPropertyValue<ushort>(stride.End.Value, GlyphProperty.Code);
                    int codeSpan = endCode - startCode + 1;

                    if (codeSpan >= 80)
                    {
                        result.Add((stride.Start.Value, stride.End.Value));

                        // Remove the glyphs in this stride from the linked list
                        var node = stride.Start;
                        while (node != stride.End.Next)
                        {
                            var next = node.Next;
                            glyphLinkedList.Remove(node);
                            node = next;
                        }

                        // Reset position and mark restart
                        pos = glyphLinkedList.First;
                        restart = true;
                        break;
                    }
                }

                if (!restart) break;
            }

            return result;
        }


        public static List<List<CMAPEntry>> CreateTableEntries(List<Glyph> glyphs)
        {
            var glyphLinkedList = new LinkedList<Glyph>(glyphs);
            var result = new List<List<CMAPEntry>>();
            var pos = glyphLinkedList.First;

            while (pos != null)
            {
                // Find initial stride starting at pos
                if (!GetNextStride(out var stride1, glyphLinkedList.Last, pos))
                    break;

                int glyphSpan = GetGlyphPropertyValue<UInt16>(stride1.End.Value, GlyphProperty.Code) - GetGlyphPropertyValue<UInt16>(stride1.Start.Value, GlyphProperty.Code) + 1;
                int gapSpan = 0;

                // Try to extend stride forward/backward
                while (true)
                {
                    (LinkedListNode<Glyph> Start, LinkedListNode<Glyph> End) stride2;
                    (LinkedListNode<Glyph> Start, LinkedListNode<Glyph> End) stride3;
                    float forwardRatio = 0.0f;
                    float backwardRatio = 0.0f;

                    // Forward stride
                    if (stride1.End != glyphLinkedList.Last)
                    {
                        var next = stride1.End.Next;
                        if (GetNextStride(out stride2, glyphLinkedList.Last, next))
                        {
                            int span2 = GetGlyphPropertyValue<UInt16>(stride2.End.Value, GlyphProperty.Code) - GetGlyphPropertyValue<UInt16>(stride2.Start.Value, GlyphProperty.Code) + 1;
                            int gap2 = GetGlyphPropertyValue<UInt16>(stride2.Start.Value, GlyphProperty.Code) - GetGlyphPropertyValue<UInt16>(stride1.End.Value, GlyphProperty.Code) - 1;
                            int combinedSpan = glyphSpan + span2;
                            int totalSpan = combinedSpan + gapSpan + gap2;
                            forwardRatio = (float)combinedSpan / totalSpan;

                            if (forwardRatio >= 0.5f)
                            {
                                glyphSpan += span2;
                                gapSpan += gap2;
                                stride1.End = stride2.End;
                                continue;
                            }
                        }
                    }

                    // Backward stride
                    if (stride1.Start != glyphLinkedList.First)
                    {
                        var prev = stride1.Start.Previous;
                        if (GetPrevStride(out stride3, glyphLinkedList.First, prev))
                        {
                            int span3 = GetGlyphPropertyValue<UInt16>(stride3.End.Value, GlyphProperty.Code) - GetGlyphPropertyValue<UInt16>(stride3.Start.Value, GlyphProperty.Code) + 1;
                            int gap3 = GetGlyphPropertyValue<UInt16>(stride1.Start.Value, GlyphProperty.Code) - GetGlyphPropertyValue<UInt16>(stride3.End.Value, GlyphProperty.Code) - 1;
                            int combinedSpan = glyphSpan + span3;
                            int totalSpan = combinedSpan + gapSpan + gap3;
                            backwardRatio = (float)combinedSpan / totalSpan;

                            if (backwardRatio >= 0.5f)
                            {
                                glyphSpan += span3;
                                gapSpan += gap3;
                                stride1.Start = stride3.Start;
                                continue;
                            }
                        }
                    }

                    // If neither forward nor backward extended, stop
                    break;
                }

                if (glyphSpan < 40)
                {
                    pos = stride1.End.Next;
                }
                else
                {
                    ushort codeStart = GetGlyphPropertyValue<UInt16>(stride1.Start.Value, GlyphProperty.Code);
                    ushort codeEnd = GetGlyphPropertyValue<UInt16>(stride1.End.Value, GlyphProperty.Code);
                    int length = codeEnd - codeStart + 1;

                    var entries = new List<CMAPEntry>();
                    for (int offset = 0; offset < length; offset++)
                    {
                        var glyph = GetGlyphByCodePoint(glyphs, (ushort)(codeStart + offset));
                        if (glyph != null)
                        {
                            entries.Add(new CMAPEntry(GetGlyphPropertyValue<UInt16>(glyph, GlyphProperty.Code), GetGlyphPropertyValue<UInt16>(glyph, GlyphProperty.Index)));
                        }
                        else
                        {
                            entries.Add(new CMAPEntry((ushort)(codeStart + offset), 0xFFFF));
                        }
                    }

                    result.Add(entries);

                    // Advance pos and remove stride nodes
                    pos = stride1.End.Next;
                    var node = stride1.Start;
                    while (node != stride1.End.Next)
                    {
                        var next = node.Next;
                        glyphLinkedList.Remove(node);
                        node = next;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Find the next contiguous stride of glyphs with sequential code/index values.
        /// </summary>
        private static bool GetNextStride(out (LinkedListNode<Glyph> Start, LinkedListNode<Glyph> End) stride,
                                          LinkedListNode<Glyph> last,
                                          LinkedListNode<Glyph> pos)
        {
            stride = (null, null);

            if (pos == last?.Next) return false;

            ushort code = (ushort)(GetGlyphPropertyValue<UInt16>(pos.Value, GlyphProperty.Code) + 1);
            ushort index = (ushort)(GetGlyphPropertyValue<UInt16>(pos.Value, GlyphProperty.Index) + 1);

            var node = pos;
            var next = node.Next;

            while (next != last?.Next &&
                   GetGlyphPropertyValue<UInt16>(next.Value, GlyphProperty.Code) == code &&
                   GetGlyphPropertyValue<UInt16>(next.Value, GlyphProperty.Index) == index)
            {
                node = next;
                next = node.Next;
                code++;
                index++;
            }

            stride = (pos, node);
            return true;
        }

        /// <summary>
        /// Find the previous contiguous stride of glyphs with sequential code values backwards.
        /// </summary>
        private static bool GetPrevStride(out (LinkedListNode<Glyph>? Start, LinkedListNode<Glyph>? End) stride,
                                          LinkedListNode<Glyph> begin,
                                          LinkedListNode<Glyph> pos)
        {
            stride = (null, null);

            if (pos == begin) return false;

            var prev1 = pos.Previous;
            if (prev1 == begin) return false;

            ushort code = (ushort)(GetGlyphPropertyValue<UInt16>(prev1.Value, GlyphProperty.Code) - 1);
            var prev2 = prev1.Previous;

            while (prev2 != begin && GetGlyphPropertyValue<UInt16>(prev2.Value, GlyphProperty.Code) == code)
            {
                prev2 = prev2.Previous;
                code--;
            }

            stride = (prev2.Next, prev1);
            return true;
        }

        /// <summary>
        /// Find a glyph by its code point.
        /// </summary>
        public static Glyph? GetGlyphByCodePoint(IEnumerable<Glyph> glyphs, ushort codePoint)
        {
            foreach (var g in glyphs)
            {
                if (GetGlyphPropertyValue<UInt16>(g, GlyphProperty.Code) == codePoint)
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
                    GetGlyphPropertyValue<UInt16>(g, GlyphProperty.Code), 
                    GetGlyphPropertyValue<UInt16>(g, GlyphProperty.Code))
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