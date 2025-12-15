using Fontendo.Extensions;
using Fontendo.Extensions.BinaryTools;

namespace Fontendo.Formats
{
    public class NFTR
    {
        public UInt32 Magic; //Leave magic as a writable field because this class will be inherited for future versions of the format
        public UInt16 BOM; //uint8_t order mark, FFFE is Little and FEFF is Big Endian
        public UInt16 Version; //Version
        public UInt32 FileSize; //Size of the full file in bytes
        public UInt16 PtrInfo; //Pointer to the begining of FINF section
        public UInt16 DataBlocks; //Number of data blocks in the file

        private readonly BinaryReaderX? br;

        public NFTR(BinaryReaderX br)
        {
            this.br = br;
        }


        public NFTR(UInt16 version = 0x0102, UInt32 magic = 0x4E465452U)
        {
            this.Version = version;
            this.Magic = magic;
            this.BOM = 0xFEFF;
            this.FileSize = 0;
            this.PtrInfo = 0;
            this.DataBlocks = 0;
        }


        public ActionResult Parse(BinaryReaderX br)
        {
            try
            {
                Magic = br.ReadUInt32();
                BOM = br.ReadUInt16();
                Version = br.ReadUInt16();
                FileSize = br.ReadUInt32();
                PtrInfo = br.ReadUInt16();
                DataBlocks = br.ReadUInt16();
            }
            catch (Exception e)
            {
                return new ActionResult(false, $"NFTR exception {e.Message}");
            }
            if (!ValidateSignature())
                return new ActionResult(false, "Invalid NFTR signature!!!");
            return new ActionResult(true, "OK");
        }

        public bool ValidateSignature()
        {
            if (Magic != 0x4E465452U && Magic != 0x5254464EU)
                return false;
            return true;
        }

        public void Serialize(BinaryWriterX bw, BlockLinker linker)
        {
            linker.AddLookupValue(FontBase.FontPointerType.ptrFont, bw.BaseStream.Position);
            bw.WriteUInt32(Magic);
            bw.WriteUInt16(BOM);
            bw.WriteUInt16(Version);
            linker.AddPatchAddr(bw.BaseStream.Position, FontBase.FontPointerType.fileSize);
            bw.WriteUInt32(FileSize);
            linker.AddShortPatchAddr(bw.BaseStream.Position, FontBase.FontPointerType.ptrInfo);
            bw.WriteUInt16(PtrInfo);
            linker.AddShortPatchAddr(bw.BaseStream.Position, FontBase.FontPointerType.blockCount);
            bw.WriteUInt16(DataBlocks);
        }
    }
}