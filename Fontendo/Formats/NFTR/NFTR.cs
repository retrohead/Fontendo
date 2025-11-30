namespace Fontendo.Formats.NFTR
{
    public class NFTR
    {
        public UInt32 magic; //Leave magic as a writable field because this class will be inherited for future versions of the format
        public UInt16 BOM; //uint8_t order mark, FFFE is Little and FEFF is Big Endian
        public UInt16 version; //Version
        public UInt32 fileSize; //Size of the full file in bytes
        public UInt16 ptrInfo; //Pointer to the begining of FINF section
        public UInt16 dataBlocks; //Number of data blocks in the file

        public ActionResult Parse(BinaryReader br)
        {
            try
            {
                magic = br.ReadUInt32();
                BOM = br.ReadUInt16();
                version = br.ReadUInt16();
                fileSize = br.ReadUInt32();
                ptrInfo = br.ReadUInt16();
                dataBlocks = br.ReadUInt16();
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
            if (magic != 0x4E465452U && magic != 0x5254464EU)
                return false;
            return true;
        }
    }
}