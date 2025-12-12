using Fontendo.Extensions.BinaryTools;
using Fontendo.UI;
using System.Data;
using static Fontendo.Extensions.FontBase;

namespace Fontendo.Formats.CTR
{
    public class CWDH
    {
        private UInt32 EntriesOffset; //Used to keep track of where to read the entries from

        public UInt32 Magic; //Should always be 0x43574448, CWDH in ASCII
        public UInt32 Length; //CWDH section length in bytes
        public UInt16 IndexBegin; //First glyph index
        public UInt16 IndexEnd; //Last glyph index
        public UInt32 PtrNext; //Pointer to the next (next CWDH + 0x8), 0x0 if last
        public List<CharWidths> Entries = new List<CharWidths>(); //CharWidths entries are stored after the header

        private readonly BinaryReaderX? br;

        public CWDH(BinaryReaderX br)
        {
            this.br = br;
        }
        public CWDH(List<CharWidths> Entries, UInt32 Magic = 0x43574448U)
        {
            this.Magic = Magic;
            this.Length = 0x0U;
            this.IndexBegin = 0x0;
            this.IndexEnd = (ushort)(Entries.Count() - 1);
            this.PtrNext = 0x0U;
            this.Entries = Entries;
        }

        public ActionResult Parse()
        {
            if (br == null)
                return new ActionResult(false, "Binary reader not attached to CWDH");
            try
            {
                Magic = br.ReadUInt32();
                Length = br.ReadUInt32();
                IndexBegin = br.ReadUInt16();
                IndexEnd = br.ReadUInt16();
                PtrNext = br.ReadUInt32();
                EntriesOffset = (UInt32)br.BaseStream.Position;
            }
            catch (Exception e)
            {
                return new ActionResult(false, $"CWDH exception {e.Message}");
            }
            if (!ValidateSignature())
                return new ActionResult(false, "Invalid CWDH signature!!!");

            // read entries
            ActionResult result = ReadEntries(br);
            if (!result.Success)
                return result;

            return new ActionResult(true, "OK");
        }

        private bool ValidateSignature()
        {
            if (Magic != 0x43574448U && Magic != 0x48445743U)
                return false;
            return true;
        }

        private ActionResult ReadEntries(BinaryReaderX br)
        {
            Entries = new List<CharWidths>();
            if (EntriesOffset == 0U)
            {
                // TODO: 
                // not really sure what the original author meant by this next line
                // presuming this is an error or meant for new file fornow
                //return; //The case if the first contructor was chosen
                return new ActionResult(false, "Entry offset is zero");
            }
            br.BaseStream.Position = EntriesOffset;
            //Read all the entries
            for (int i = IndexBegin; i <= IndexEnd; i++)
            {
                CharWidths charwidth = new CharWidths(br);
                ActionResult result = charwidth.Parse();
                if (!result.Success)
                    return new ActionResult(false, $"Failed parsing CWDH charwidth at index {i}\n\n{result.Message}");
                Entries.Add(charwidth);
            }
            return new ActionResult(true, "OK");
        }

        public void Serialize(BinaryWriterX bw, BlockLinker linker)
        {
            linker.IncLookupValue(FontPointerType.blockCount, 1);
            linker.IncLookupValue(FontPointerType.CWHD, 1);

            long startPos = bw.BaseStream.Position;
            long sectionNum = linker.GetLookupValue(FontPointerType.CWHD);

            linker.AddLookupValueByName($"{nameof(FontPointerType.CWHD)}Len{sectionNum}", startPos);

            bw.WriteUInt32(Magic);

            linker.AddPatchAddrByName(bw.BaseStream.Position, $"{nameof(FontPointerType.CWHD)}Len{sectionNum}");
            bw.WriteUInt32(Length);

            if (sectionNum == 1)
                linker.AddLookupValue(FontPointerType.ptrWidth, bw.BaseStream.Position);

            bw.WriteUInt16(IndexBegin);
            bw.WriteUInt16(IndexEnd);

            linker.AddPatchAddrByName(bw.BaseStream.Position, $"{nameof(FontPointerType.CWHD)}{sectionNum + 1}");
            bw.WriteUInt32(PtrNext);

            UI_MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} Wrote {Entries.Count()} Entries at offset");
            foreach (var entry in Entries)
            {
                entry.Serialize(bw);
            }
            UI_MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")}  Entries end rest is padding");

            // Padding to 4-byte boundary, not sure this is needed
            long padBytes = 0x4 - (bw.BaseStream.Position % 0x4);
            if (padBytes != 0x4)
            {
                for (uint i = 0; i < padBytes; i++)
                    bw.WriteByte((byte)0x0);
            }

            linker.AddLookupValueByName($"{nameof(FontPointerType.CWHD)}Len{sectionNum}", (uint)bw.BaseStream.Position - startPos);
        }
    }
}