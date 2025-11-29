public class TGLP
{
    public UInt32 magic; //Should always be 0x54474C50, TGLP in ASCII
    public UInt32 length; //TGLP section length in bytes
    public byte cellWidth; //Glyph cell width, in pixels
    public byte cellHeight; //Glyph cell height, in pixels
    public byte baselinePos; //Baseline position
    public byte maxCharWidth; //Maximum character width
    public UInt32 sheetSize; //Single sheet size in bytes
    public UInt16 sheetCount; //Sheet count
    public UInt16 sheetFormat; //Sheet image format
    public UInt16 cellsPerRow; //Glyph cells per row
    public UInt16 cellsPerColumn; //Glyph cells per column
    public UInt16 sheetWidth; //Sheet width, in pixels
    public UInt16 sheetHeight; //Sheet height, in pixels
    public UInt32 sheetPtr; //Sheet data pointer

    public ActionResult Parse(BinaryReader br)
    {
        try
        {
            magic = br.ReadUInt32();
            length = br.ReadUInt32();
            cellWidth = br.ReadByte();
            cellHeight = br.ReadByte();
            baselinePos = br.ReadByte();
            maxCharWidth = br.ReadByte();
            sheetSize = br.ReadUInt32();
            sheetCount = br.ReadUInt16();
            sheetFormat = br.ReadUInt16();
            cellsPerRow = br.ReadUInt16();
            cellsPerColumn = br.ReadUInt16();
            sheetWidth = br.ReadUInt16();
            sheetHeight = br.ReadUInt16();
            sheetPtr = br.ReadUInt32();
        }
        catch (Exception e)
        {
            return new ActionResult(false, $"TGLP exception {e.Message}");
        }
        if (!ValidateSignature())
            return new ActionResult(false, "Invalid TGLP signature!!!");
        return new ActionResult(true, "OK");
    }

    private bool ValidateSignature()
    {
        if (magic != 0x54474C50U && magic != 0x504C4754U)
            return false;
        return true;
    }
}