public class FINF
{
    public UInt32 magic; //Should always be 0x46494E46, FINF in ASCII
    public UInt32 length; //FINF section length in ushorts
    public byte fontType; //Font type, 0x0 - Bitmap, 0x1 - TGLP
    public byte lineFeed; //Line feed, == leading
    public UInt16 defaultCharIndex; //Index of default character, used for when a program requests a character that doesn't exist in the font
    public CharWidths defaultWidths = new CharWidths(); //Default character widths, ? fallback for characters that don't have width information
    public byte encoding; //Font encoding, 0x0 - UTF-8, 0x1 - UTF-16, 0x2 - ShiftJIS, 0x3 - CP1252
    public UInt32 ptrGlyph; //TGLP section data pointer (TGLP + 0x8)
    public UInt32 ptrWidth; //CWDH section data pionter (CWDH + 0x8)
    public UInt32 ptrMap; //CMAP section data pointer (CMAP + 0x8)
    public byte height; //Font height, v1.2+ and all later revisions only
    public byte width; //Font width, v1.2+ and all later revisions only
    public byte ascent; //Ascent, v1.2+ and all later revisions only
    public byte padding; //Padding ushort, v1.2+ and all later revisions only

    public ActionResult Parse(BinaryReader br)
    {
        try
        {
            magic = br.ReadUInt32();
            length = br.ReadUInt32();
            fontType = br.ReadByte();
            lineFeed = br.ReadByte();
            defaultCharIndex = br.ReadUInt16();
            defaultWidths = new CharWidths();
            ActionResult cwhdresult = defaultWidths.Parse(br);
            if (!cwhdresult.Success)
                return new ActionResult(false, $"Invalid FINF charwidths {cwhdresult.Message}");

            encoding = br.ReadByte();
            ptrGlyph = br.ReadUInt32();
            ptrWidth = br.ReadUInt32();
            ptrMap = br.ReadUInt32();
            if (length == 0x20U)
            {
                height = br.ReadByte();
                width = br.ReadByte();
                ascent = br.ReadByte();
                padding = br.ReadByte();
            }
            else if (length == 0x1CU)  //Old version has a size of 0x1C, doesn't have the four bytes below
            {
                height = 0xFF;
                width = 0xFF;
                ascent = 0xFF;
                padding = 0xFF;
            }
            else
            {
                return new ActionResult(false, $"Invalid FINF section length 0x{length.ToString("X8")}");
            }
        }
        catch (Exception e)
        {
            return new ActionResult(false, $"FINF exception {e.Message}");
        }
        if (!ValidateSignature())
            return new ActionResult(false, "Invalid FINF signature!!!");
        return new ActionResult(true, "OK");
    }

    private bool ValidateSignature()
    {
        if (magic != 0x46494E46U && magic != 0x464E4946U)
            return false;
        return true;
    }
}