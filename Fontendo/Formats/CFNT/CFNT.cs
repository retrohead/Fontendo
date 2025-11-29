public class CFNT
{

    public UInt32 magic; //Leave magic as a writable field because this class will be inherited for future versions of the format
    public UInt16 BOM; //uint8_t order mark, FFFE is Little and FEFF is Big Endian
    public UInt16 headerSize; //Size of the CFNT header
    public UInt32 version; //Version
    public UInt32 fileSize; //Size of the full file in bytes
    public UInt16 dataBlocks; //Number of data blocks in the file
    public UInt16 reserved; //Aka two bytes of null padding

    public ActionResult Parse(BinaryReader br)
    {
        try
        {
            magic = br.ReadUInt32();
            BOM = br.ReadUInt16();
            headerSize = br.ReadUInt16();
            version = br.ReadUInt32();
            fileSize = br.ReadUInt32();
            dataBlocks = br.ReadUInt16();
            reserved = br.ReadUInt16();
        }
        catch (Exception e)
        {
            return new ActionResult(false, $"CFNT exception {e.Message}");
        }
        if (!ValidateSignature())
            return new ActionResult(false, "Invalid CFNT signature!!!");
        if (version != 0x3000000)
            return new ActionResult(false, $"Unknown CFNT format version 0x{version.ToString("X8")}");
        return new ActionResult(true, "OK");
    }

    private bool ValidateSignature()
    {
        if (magic != 0x43464E54U && magic != 0x544E4643U)
            return false;
        return true;
    }
}