using Fontendo.Extensions;
using Fontendo.Extensions.BinaryTools;
using Fontendo.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using static Fontendo.Extensions.FontBase;
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

                UI_MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} CMAP CodeBegin={CodeBegin}, CodeEnd={CodeEnd}, MappingMethod={MappingMethod}");

                UI_MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} CMAP Entries Start");
                // read entries
                ActionResult result = ReadEntries(br);
                UI_MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} CMAP Entries={Entries.Count}");
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

        private ActionResult ReadEntries(BinaryReaderX br)
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
        public static List<(Glyph First, Glyph Second)> CreateDirectEntries(ref LinkedList<Glyph> glyphs)
        {
            var result = new List<(Glyph First, Glyph Second)>();
            var r = new Stride();
            LinkedListNode<Glyph>? pos = glyphs.First;

        begin:
            if (pos == glyphs.Last?.Next)
                return result;
            while (GetNextStride(ref r, glyphs.Last, pos))
            {
                if(r.First == null || r.Second == null)
                    break;
                pos = r.Second.Next;

                int spanLength = r.Second.Value.Settings.CodePoint - r.First.Value.Settings.CodePoint + 1;
                if (spanLength >= 80)
                {
                    // Add the pair of glyphs marking the stride
                    result.Add((r.First.Value, r.Second.Value));

                    // Remove the stride range from the glyph list
                    var current = r.First;
                    var second = r.Second;
                    while (current != second)
                    {
                        var next = current.Next;
                        glyphs.Remove(current);
                        current = next;
                    }
                    glyphs.Remove(second);
                    goto begin;
                }
            }

            return result;
        }

        public class Stride
        {
            public LinkedListNode<Glyph>? First;
            public LinkedListNode<Glyph>? Second;
        }

        public static List<List<CMAPEntry>> CreateTableEntries(ref LinkedList<Glyph> glyphs)
        {
            var result = new List<List<CMAPEntry>>();
            Stride r1 = new Stride();
            LinkedListNode<Glyph>? pos = glyphs.First; // index into glyphs

            while (pos != glyphs.Last.Next)
            {
                // r1 is a pair of indices delimiting a stride
                GetNextStride(ref r1, glyphs.Last, pos);
                if (r1.First == glyphs.Last.Next)
                    break;

                int num1 = r1.Second.Value.Settings.CodePoint - r1.First.Value.Settings.CodePoint + 1;
                int num2 = 0;

                while (true)
                {
                    Stride r2 = new Stride();
                    Stride r3 = new Stride();
                    float num3 = 0.0f;
                    float num4 = 0.0f;

                    // forward merge
                    if (r1.Second != glyphs.Last.Next)
                    {
                        LinkedListNode<Glyph> next = r1.Second.Next;
                        if (GetNextStride(ref r2, glyphs.Last, next))
                        {
                            int num5 = r2.Second.Value.Settings.CodePoint - r2.First.Value.Settings.CodePoint + 1;
                            int num6 = r2.First.Value.Settings.CodePoint - r1.Second.Value.Settings.CodePoint - 1;
                            int num7 = num1 + num5;
                            int num8 = num7 + num2 + num6;
                            num3 = (float)num7 / (float)num8;
                        }
                    }

                    // backward merge
                    if (r1.First != glyphs.First)
                    {
                        LinkedListNode<Glyph> prev = r1.First.Previous;
                        if (GetPrevStride(ref r3, glyphs.First, prev))
                        {
                            int num5 = r3.Second.Value.Settings.CodePoint - r3.First.Value.Settings.CodePoint + 1;
                            int num6 = r1.First.Value.Settings.CodePoint - r3.Second.Value.Settings.CodePoint - 1;
                            int num7 = num1 + num5;
                            int num8 = num7 + num2 + num6;
                            num4 = (float)num7 / num8;
                        }
                    }

                    if (num3 > num4)
                    {
                        if (num3 >= 0.5f)
                        {
                            int num5 = r2.Second.Value.Settings.CodePoint - r2.First.Value.Settings.CodePoint + 1;
                            int num6 = r2.First.Value.Settings.CodePoint - r1.Second.Value.Settings.CodePoint - 1;
                            num1 += num5;
                            num2 += num6;
                            r1.Second = r2.Second;
                        }
                        else break;
                    }
                    else if (num4 >= 0.5f)
                    {
                        int num5 = r3.Second.Value.Settings.CodePoint - r3.First.Value.Settings.CodePoint + 1;
                        int num6 = r1.First.Value.Settings.CodePoint - r3.Second.Value.Settings.CodePoint - 1;
                        num1 += num5;
                        num2 += num6;
                        r1.First = r3.First;
                    }
                    else break;
                }

                if (num1 < 40)
                {
                    pos = r1.Second.Next;
                }
                else if (r1.Second != null)
                {
                    UInt16 code1 = r1.First.Value.Settings.CodePoint;
                    UInt16 code2 = r1.Second.Value.Settings.CodePoint;
                    UInt16 length = (UInt16)(code2 - code1 + 1);

                    var entries = new List<CMAPEntry>();
                    for (UInt16 offset = 0; offset < length; ++offset)
                    {
                        Glyph? glyph = GetGlyphByCodePoint(glyphs, (UInt16)(code1 + offset));
                        if (glyph != null)
                        {
                            entries.Add(new CMAPEntry(glyph.Settings.CodePoint, (ushort)glyph.Settings.Index));
                        }
                        else
                        {
                            entries.Add(new CMAPEntry((UInt16)(code1 + offset), 0xFFFF));
                        }
                    }

                    result.Add(entries);
                    pos = r1.Second.Next;

                    // Remove the range from the glyph list (inclusive)
                    var current = r1.First;
                    var second = r1.Second;
                    while (current != second)
                    {
                        var next = current.Next;
                        glyphs.Remove(current);
                        current = next;
                    }
                    glyphs.Remove(second);
                }
            }

            return result;
        }

        /// <summary>
        /// Find the next contiguous stride of glyphs with sequential code/index values.
        /// </summary>
        public static bool GetNextStride(ref Stride r, LinkedListNode<Glyph> last, LinkedListNode<Glyph> pos)
        {
            if (pos == last.Next)
                return false;

            ushort code = (ushort)(pos.Value.Settings.CodePoint + 1U);
            ushort index = (ushort)(pos.Value.Settings.Index + 1U);

            LinkedListNode<Glyph> node = pos;
            for (var next = node.Next;
                 next != last.Next &&
                 next.Value.Settings.CodePoint == code &&
                 next.Value.Settings.Index == index;
                 ++index)
            {
                node = next;
                next = node.Next;
                ++code;
            }

            r.First = pos;
            r.Second = node;
            return true;
        }

        /// <summary>
        /// Find the previous contiguous stride of glyphs with sequential code values.
        /// </summary>
        public static bool GetPrevStride(ref Stride r, LinkedListNode<Glyph> begin, LinkedListNode<Glyph> pos)
        {
            if (pos == begin) return false;

            var prev1 = pos.Previous;
            if (prev1 == begin) return false;

            ushort code = (ushort)(prev1.Value.Settings.CodePoint - 1U);
            LinkedListNode<Glyph> prev2;

            for (prev2 = prev1.Previous;
                 prev2 != begin && prev2.Value.Settings.CodePoint == code;
                 --code)
            {
                prev2 = prev2.Previous;
            }

            r.First = prev2.Next;
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
            linker.IncLookupValue(FontPointerType.blockCount, 1);
            linker.IncLookupValue(FontPointerType.CMAP, 1);

            long startPos = bw.BaseStream.Position;
            long sectionNum = linker.GetLookupValue(FontPointerType.CMAP);
            UI_MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} CMAP {sectionNum - 1} start");

            linker.AddLookupValueByName($"{nameof(FontPointerType.CMAP)}{sectionNum}", startPos + 0x8);

            bw.WriteUInt32(Magic);

            linker.AddPatchAddrByName(bw.BaseStream.Position, $"{nameof(FontPointerType.CMAP)}Len{sectionNum}");
            bw.WriteUInt32(Length);

            bw.WriteUInt16(CodeBegin);
            bw.WriteUInt16(CodeEnd);
            bw.WriteUInt16(MappingMethod);
            bw.WriteUInt16(Reserved);

            linker.AddPatchAddrByName(bw.BaseStream.Position, $"{nameof(FontPointerType.CMAP)}{sectionNum + 1}");
            bw.WriteUInt32(PtrNext);

            UI_MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} CMAP Entries CodeBegin={CodeBegin}, CodeEnd={CodeEnd}, MappingMethod={MappingMethod}, Entries={Entries.Count()}");

            switch (MappingMethod)
            {
                case 0: // Direct
                    bw.WriteUInt16(Entries[0].Index);
                    UI_MainWindow.Log($"0x{bw.BaseStream.Position.ToString()} Wrote {(CodeEnd - CodeBegin) + 1}");
                    break;

                case 1: // Table
                    foreach (var entry in Entries)
                    {
                        bw.WriteUInt16(entry.Index);
                    }
                    UI_MainWindow.Log($"0x{bw.BaseStream.Position.ToString()} Wrote {Entries.Count()} of which {Entries.FindAll(e => e.Index != 0xFFFFU).Count()} where not just 0xFFFF");
                    break;

                case 2: // Scan
                    bw.WriteUInt16((ushort)Entries.Count());
                    foreach (var entry in Entries)
                    {
                        entry.Serialize(bw);
                    }
                    UI_MainWindow.Log($"0x{bw.BaseStream.Position.ToString()} Wrote {Entries.Count()} of which {Entries.FindAll(e => e.Index != 0xFFFFU).Count()} where not just 0xFFFF");
                    break;
            }

            // Padding to 4-byte boundary
            uint padBytes = 0x4 - ((uint)bw.BaseStream.Position % 0x4);
            if (padBytes != 0x4)
            {
                for (uint i = 0; i < padBytes; i++)
                    bw.WriteByte((byte)0x0);
            }

            linker.AddLookupValueByName($"{nameof(FontPointerType.CMAP)}Len{sectionNum}", (uint)bw.BaseStream.Position - startPos);
        }

    }
}