using Fontendo.Extensions;
using static Fontendo.Extensions.FontBase;
using static Fontendo.Extensions.PropertyList;
using static Fontendo.Formats.FontProperties;
using static Fontendo.Formats.GlyphProperties;

namespace Fontendo.Interfaces
{
    public interface IFontendoFont
    {
        public ITextureCodec Codec { get; set; }
        public Sheets? Sheets { get; set; }
        public List<Glyph>? Glyphs { get; set; }
        public List<CharImage>? CharImages { get; set; }

        public Dictionary<FontProperty, FontPropertyListEntryDescriptor> FontPropertyDescriptors { get; set; }
        public Dictionary<FontProperty, PropertyBase> FontProperties { get; set; }

        public Dictionary<GlyphProperty, FontPropertyListEntryDescriptor> GlyphPropertyDescriptors { get; set; }
        public Dictionary<GlyphProperty, PropertyBase> GlyphProperties { get; set; }

        public ActionResult Load(string filename);
        public ActionResult Save(string filename);
    }
}
