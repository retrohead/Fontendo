public class CharWidths
{
    public sbyte left; //left space width of character
    public byte glyphWidth; //glyph width of character
    public byte charWidth; //character width = left space width + glyph width + right space width

    public ActionResult Parse(BinaryReader br)
    {
        try
        {
            left = br.ReadSByte();
            glyphWidth = br.ReadByte();
            charWidth = br.ReadByte();

        }
        catch (Exception e)
        {
            return new ActionResult(false, $"CharWidths exception {e.Message}");
        }
        return new ActionResult(true, "OK");
    }
}