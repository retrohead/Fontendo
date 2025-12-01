using Fontendo.Extensions;
using static Fontendo.Extensions.FontBase;
using static Fontendo.FontProperties.PropertyList;
using static Fontendo.FontProperties.FontPropertyList;
using static Fontendo.FontProperties.GlyphProperties;

namespace Fontendo.Interfaces
{
    public interface IFontendoFont
    {
        public ITextureCodec Codec { get; set; }
        public Sheets? Sheets { get; set; }
        public List<Glyph>? Glyphs { get; set; }
        public List<CharImage>? CharImages { get; set; }
        public FontPropertyRegistry Properties { get; set; }
        public ActionResult Load(string filename);
        public ActionResult Save(string filename);
        public void Dispose();
    }
}
