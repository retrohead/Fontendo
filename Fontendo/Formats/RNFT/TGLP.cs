public class TGLP
{
    public UInt32 Magic; //Should always be 0x54474C50, TGLP in ASCII
    public UInt32 Length; //TGLP section length in bytes
    public byte CellWidth; //Glyph cell width, in pixels
    public byte CellHeight; //Glyph cell height, in pixels
    public byte BaselinePos; //Baseline position
    public byte MaxCharWidth; //Maximum character width
    public UInt32 SheetSize; //Single sheet size in bytes
    public UInt16 SheetCount; //Sheet count
    public UInt16 SheetFormat; //Sheet image format
    public UInt16 CellsPerRow; //Glyph cells per row
    public UInt16 CellsPerColumn; //Glyph cells per column
    public UInt16 SheetWidth; //Sheet width, in pixels
    public UInt16 SheetHeight; //Sheet height, in pixels
    public UInt32 SheetPtr; //Sheet data pointer

    public ActionResult Parse(BinaryReader br)
    {
        try
        {
            Magic = br.ReadUInt32();
            Length = br.ReadUInt32();
            CellWidth = br.ReadByte();
            CellHeight = br.ReadByte();
            BaselinePos = br.ReadByte();
            MaxCharWidth = br.ReadByte();
            SheetSize = br.ReadUInt32();
            SheetCount = br.ReadUInt16();
            SheetFormat = br.ReadUInt16();
            CellsPerRow = br.ReadUInt16();
            CellsPerColumn = br.ReadUInt16();
            SheetWidth = br.ReadUInt16();
            SheetHeight = br.ReadUInt16();
            SheetPtr = br.ReadUInt32();
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
        if (Magic != 0x54474C50U && Magic != 0x504C4754U)
            return false;
        return true;
    }
}