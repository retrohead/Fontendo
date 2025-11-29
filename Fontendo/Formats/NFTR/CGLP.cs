public class CGLP
{
    public UInt32 bitmapPtr; //Here just for keeping track of where the image data is located, not actually present in the file
    public UInt32 magic; //Section magic, should always be 0x43474C50, CGLP in ASCII
    public UInt32 length; //Length of the section including bitmap data itself in bytes
    public byte cellWidth; //Glyph cell width, in pixels
    public byte cellHeight; //Glyph cell height, in pixels
    public UInt16 cellSize; //IDK even, maybe cell size in bytes?
    public sbyte baselinePos; //Baseline position
    public byte maxCharWidth; //Maximum character width
    public byte bpp; //Bitmap Bits Per Pixel
    public byte flags; //Flags, see the enum below. Called "reserved" since RVL SDK
                       //Enums
    public enum fontGlyphFlags : byte
    {
        verticalWriting = (1 << 0),
        rot0Deg = (0 << 1),
        rot90Deg = (1 << 1),
        rot180Deg = (2 << 1),
        rot270Deg = (3 << 1),
        rotMask = (3 << 1)
    };

    public ActionResult Parse(BinaryReader br)
    {
        try
        {
            magic = br.ReadUInt32();
            length = br.ReadUInt32();
            cellWidth = br.ReadByte();
            cellHeight = br.ReadByte();
            cellSize = br.ReadUInt16();
            baselinePos = br.ReadSByte();
            maxCharWidth = br.ReadByte();
            bpp = br.ReadByte();
            flags = br.ReadByte();
            bitmapPtr = (UInt32)br.BaseStream.Position;
        } catch(Exception e)
        {
            return new ActionResult(false, $"CGLP exception {e.Message}");
        }
        if (!ValidateSignature())
            return new ActionResult(false, "Invalid CGLP signature!!!");
        return new ActionResult(true, "OK");
    }

    private bool ValidateSignature()
    {
        if (magic != 0x43474C50U && magic != 0x504C4743U)
            return false;
        return true;
    }
}