using Fontendo.Extensions;
using Fontendo.Extensions.BinaryTools;
using static Fontendo.Formats.GlyphProperties;

namespace Fontendo.Formats.CTR
{
    public class CharWidths
    {
        public sbyte Left; //left space width of character
        public byte GlyphWidth; //glyph width of character
        public byte CharWidth; //character width = left space width + glyph width + right space width

        private BinaryReaderX? br;
        public CharWidths(BinaryReaderX br)
        {
            this.br = br;
        }
        public CharWidths(sbyte Left, byte GlyphWidth, byte CharWidth)
        {
            this.Left = Left;
            this.GlyphWidth = GlyphWidth;
            this.CharWidth = CharWidth;
        }

        public ActionResult Parse()
        {
            if (br == null)
                return new ActionResult(false, "Binary reader not attached to CharWidths");
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
        public void Serialize(BinaryWriterX bw)
        {
            bw.WriteSbyte(Left);
            bw.WriteByte(GlyphWidth);
            bw.WriteByte(CharWidth);
        }
    }
}