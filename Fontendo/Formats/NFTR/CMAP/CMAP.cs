using Fontendo.Extensions;
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

        public CMAP((Glyph, Glyph) t_entries, UInt32 t_magic)
        {
            Magic = t_magic;
            Length = 0x0U;
            CodeBegin = GetGlyphPropertyValue<UInt16>(t_entries.Item1, GlyphProperty.Code);
            CodeEnd = GetGlyphPropertyValue<UInt16>(t_entries.Item2, GlyphProperty.Code);
            MappingMethod = 0; //Direct
            Reserved = 0;
            PtrNext = 0x0U;
            Entries = new List<CMAPEntry>();
            Entries.Add(new CMAPEntry(CodeBegin, GetGlyphPropertyValue<UInt16>(t_entries.Item1, GlyphProperty.Index)));
        }
        public CMAP()
        {

        }

        public ActionResult Parse(BinaryReader br)
        {
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

                // read entries
                ActionResult result = ReadEntries(br);
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
                        CMAPEntry entry = new CMAPEntry();
                        ActionResult result = entry.Parse(br);
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
    }
}