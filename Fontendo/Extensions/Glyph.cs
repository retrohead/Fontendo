using Fontendo.FontProperties;
using static Fontendo.FontProperties.GlyphProperties;
using static Fontendo.FontProperties.PropertyList;

namespace Fontendo.Extensions
{
    public class Glyph
    {
        public GlyphPropertyRegistry Properties { get; private set; }
        public Bitmap? Pixmap { get; private set; }

        public int Index { get; set; }
        public int CodePoint { get; set; }


        public Glyph(int index, Bitmap pixmap, Dictionary<GlyphProperty, object>? propertyValues = null)
        {
            Index = index;

            Properties = new GlyphPropertyRegistry();

            Properties = new GlyphPropertyRegistry();
            Properties.AddProperty(GlyphProperty.Index, "Index", PropertyValueType.UInt16);
            Properties.AddProperty(GlyphProperty.Code, "Code point", PropertyValueType.UInt16);
            Properties.AddProperty(GlyphProperty.Left, "Left", PropertyValueType.SByte, (-0x7F, 0x7F));
            Properties.AddProperty(GlyphProperty.GlyphWidth, "Glyph width", PropertyValueType.Byte, (0x0, 0xFF));
            Properties.AddProperty(GlyphProperty.CharWidth, "Char width", PropertyValueType.Byte, (0x0, 0xFF));

            if(propertyValues != null)
            {
                foreach (var kvp in propertyValues)
                {
                    Properties.SetValue(kvp.Key, kvp.Value);
                }
            }

            Pixmap = new Bitmap(16, 16);
            Pixmap = pixmap ?? new Bitmap(16, 16);
        }

        public Rectangle SelectionRect =>
            new Rectangle(-1, -1, Pixmap?.Width ?? 0 + 1, Pixmap?.Height ?? 0 + 1);

        public Rectangle BoundingRect =>
            new Rectangle(0, 0, Pixmap?.Width ?? 0, Pixmap?.Height ?? 0);

        public bool Selected { get; set; }
    }
}
