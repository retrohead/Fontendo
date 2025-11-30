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

        public ActionResult Parse(BinaryReader br)
        {
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
    }
}