using static Fontendo.Extensions.PropertyList;
using static Fontendo.Formats.GlyphProperties;

namespace Fontendo.Extensions
{
    public class Glyph
    {
        public Dictionary<GlyphProperty, PropertyBase>? Properties { get; private set; }
        public Bitmap? Pixmap { get; private set; }

        public int Index { get; set; }
        public int CodePoint { get; set; }

        public Glyph()
        {
            Properties = new Dictionary<GlyphProperty, PropertyBase>();
            Pixmap = new Bitmap(16, 16);
        }

        public Glyph(int index, Dictionary<GlyphProperty, PropertyBase> props, Bitmap pixmap)
        {
            Index = index;
            Properties = props ?? new Dictionary<GlyphProperty, PropertyBase>();
            Pixmap = pixmap ?? new Bitmap(16, 16);
        }

        public Rectangle SelectionRect =>
            new Rectangle(-1, -1, Pixmap?.Width ?? 0 + 1, Pixmap?.Height ?? 0 + 1);

        public Rectangle BoundingRect =>
            new Rectangle(0, 0, Pixmap?.Width ?? 0, Pixmap?.Height ?? 0);

        public bool Selected { get; set; }
    }
}
