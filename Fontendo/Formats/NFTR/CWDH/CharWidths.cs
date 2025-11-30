namespace Fontendo.Formats.CTR
{
    public class CharWidths
    {
        public sbyte Left; //left space width of character
        public byte GlyphWidth; //glyph width of character
        public byte CharWidth; //character width = left space width + glyph width + right space width

        public ActionResult Parse(BinaryReader br)
        {
            try
            {
                Left = br.ReadSByte();
                GlyphWidth = br.ReadByte();
                CharWidth = br.ReadByte();

            }
            catch (Exception e)
            {
                return new ActionResult(false, $"CharWidths exception {e.Message}");
            }
            return new ActionResult(true, "OK");
        }
    }
}