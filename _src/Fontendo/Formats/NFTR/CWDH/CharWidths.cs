using Fontendo.Extensions;
using Fontendo.Extensions.BinaryTools;
using static Fontendo.FontProperties.GlyphProperties;

namespace Fontendo.Formats
{
    public class CharWidths
    {
        public sbyte Left; //left space width of character
        public byte GlyphWidth; //glyph width of character
        public byte CharWidth; //character width = left space width + glyph width + right space width

        BinaryReaderX? br;
        public CharWidths(BinaryReaderX? br)
        {
            this.br = br;
        }
        public CharWidths(sbyte t_left, byte t_glyphWidth, byte t_charWidth)
        {
            Left = t_left;
            GlyphWidth = t_glyphWidth;
            CharWidth = t_charWidth;
        }

        public ActionResult Parse()
        {
            if (br == null)
                return new ActionResult(false, "Binary reader not attached to CWDH");
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
                var leftProp = glyph.Settings.GetValue<sbyte>(GlyphProperty.Left);
                var glyphWidthProp = glyph.Settings.GetValue<byte>(GlyphProperty.GlyphWidth);
                var charWidthProp = glyph.Settings.GetValue<byte>(GlyphProperty.CharWidth);

                entries.Add(new CharWidths(leftProp, glyphWidthProp, charWidthProp));
            }

            return entries;
        }

        /// <summary>
        /// Equivalent of C++ CharWidths::serialize
        /// </summary>
        public void Serialize(BinaryWriterX bw)
        {
            bw.WriteSbyte(Left);
            bw.WriteByte(GlyphWidth);
            bw.WriteByte(CharWidth);
        }
    }
}