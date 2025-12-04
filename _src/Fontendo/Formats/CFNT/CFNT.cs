using Fontendo.Extensions.BinaryTools;
using System;
using System.Reflection.PortableExecutable;

namespace Fontendo.Formats.CTR
{
    public class CFNT
    {

        public UInt32 Magic; //Leave magic as a writable field because this class will be inherited for future versions of the format
        public UInt16 BOM; //uint8_t order mark, FFFE is Little and FEFF is Big Endian
        public UInt16 HeaderSize; //Size of the CFNT header
        public UInt32 Version; //Version
        public UInt32 FileSize; //Size of the full file in bytes
        public UInt16 DataBlocks; //Number of data blocks in the file
        public UInt16 Reserved; //Aka two bytes of null padding

        private readonly BinaryReaderX? br;

        public CFNT(BinaryReaderX br)
        {
            this.br = br;
        }
        public CFNT(UInt32 Version = 0x3000000U, UInt32 Magic = 0x544E4643U)
        {
            this.Magic = Magic;
            this.BOM = 0xFEFF;
            this.HeaderSize = 0x0;
            this.Version = Version;
            this.FileSize = 0x0U;
            this.DataBlocks = 0x0;
            this.Reserved = 0x0;
        }

        public ActionResult Parse()
        {
            if (br == null)
                return new ActionResult(false, "Binary reader not attached to CFNT");
            try
            {
                Magic = br.ReadUInt32();
                BOM = br.ReadUInt16();
                HeaderSize = br.ReadUInt16();
                Version = br.ReadUInt32();
                FileSize = br.ReadUInt32();
                DataBlocks = br.ReadUInt16();
                Reserved = br.ReadUInt16();
            }
            catch (Exception e)
            {
                return new ActionResult(false, $"CFNT exception {e.Message}");
            }
            if (!ValidateSignature())
                return new ActionResult(false, "Invalid CFNT signature!!!");
            if (Version != 0x3000000)
                return new ActionResult(false, $"Unknown CFNT format version 0x{Version.ToString("X8")}");
            return new ActionResult(true, "OK");
        }

        private bool ValidateSignature()
        {
            if (Magic != 0x43464E54U && Magic != 0x544E4643U)
                return false;
            return true;
        }


        /// <summary>
        /// Serialize CFNT header into the binary stream, recording patch addresses in the linker.
        /// </summary>
        public void Serialize(BinaryWriterX bw, BlockLinker linker)
        {
            // Record the starting position of the font block
            linker.AddLookupValue("ptrFont", bw.BaseStream.Position);

            bw.WriteUInt32(Magic);
            bw.WriteUInt16(BOM);

            // Patch pointer to FINF block
            linker.AddShortPatchAddr(bw.BaseStream.Position, "ptrInfo");
            bw.WriteUInt16(HeaderSize);

            bw.WriteUInt32(Version);

            // Patch file size
            linker.AddPatchAddr(bw.BaseStream.Position, "fileSize");
            bw.WriteUInt32(FileSize);

            // Patch block count
            linker.AddShortPatchAddr(bw.BaseStream.Position, "blockCount");
            bw.WriteUInt16(DataBlocks);

            bw.WriteUInt16(Reserved);
        }
    }
}