using Fontendo.Extensions;
using static Fontendo.Formats.GlyphProperties;

namespace Fontendo.Formats.CTR
{
    public class CharWidths
    {
        public sbyte Left; //left space width of character
        public byte GlyphWidth; //glyph width of character
        public byte CharWidth; //character width = left space width + glyph width + right space width

        public CharWidths()
        {
        }
        public CharWidths(sbyte t_left, byte t_glyphWidth, byte t_charWidth)
        {
            Left = t_left;
            GlyphWidth = t_glyphWidth;
            CharWidth = t_charWidth;
        }

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

        /// <summary>
        /// Equivalent of C++ CharWidths::createWidthEntries
        /// </summary>
        public static List<CharWidths> CreateWidthEntries(IEnumerable<Glyph> glyphs)
        {
            var entries = new List<CharWidths>();

            foreach (var glyph in glyphs)
            {
                // Assuming glyph.Props is a List<PropertyBase> in the same order as in your Load
                var leftProp = glyph.Properties[GlyphProperty.Left] as PropertyList.Property<sbyte>;
                var glyphWidthProp = glyph.Properties[GlyphProperty.GlyphWidth] as PropertyList.Property<byte>;
                var charWidthProp = glyph.Properties[GlyphProperty.CharWidth] as PropertyList.Property<byte>;

                if (leftProp == null || glyphWidthProp == null || charWidthProp == null)
                    throw new InvalidOperationException("Glyph properties not set correctly.");

                entries.Add(new CharWidths(leftProp.Value, glyphWidthProp.Value, charWidthProp.Value));
            }

            return entries;
        }

        /// <summary>
        /// Equivalent of C++ CharWidths::serialize
        /// </summary>
        public void Serialize(BinaryWriter bw)
        {
            bw.Write(Left);
            bw.Write(GlyphWidth);
            bw.Write(CharWidth);
        }
    }
}