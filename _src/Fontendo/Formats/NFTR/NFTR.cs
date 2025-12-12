using Fontendo.Extensions.BinaryTools;

namespace Fontendo.Formats.CTR
{
    public class NFTR
    {
        public UInt32 Magic; //Leave magic as a writable field because this class will be inherited for future versions of the format
        public UInt16 BOM; //uint8_t order mark, FFFE is Little and FEFF is Big Endian
        public UInt16 Version; //Version
        public UInt32 FileSize; //Size of the full file in bytes
        public UInt16 PtrInfo; //Pointer to the begining of FINF section
        public UInt16 DataBlocks; //Number of data blocks in the file

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

        private bool ValidateSignature()
        {
            if (Magic != 0x4E465452U && Magic != 0x5254464EU)
                return false;
            return true;
        }
    }
}