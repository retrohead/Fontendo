using Fontendo.Extensions;
using static Fontendo.Extensions.FontBase.FontSettings;

namespace Fontendo.Interfaces
{
    public interface IFontendoFont
    {
        public ITextureCodec Codec { get; set; }
        public FontBase? FontBase { get; }
        public SheetsType? Sheets { get; set; }
        public List<Glyph>? Glyphs { get; set; }
        public ActionResult Load(FontBase fontbase, string filename);
        public ActionResult Save(string filename);
        public void RecreateSheetFromGlyphs(int i);
        public void RecreateGlyphsFromSheet(int i);
        public void Dispose();
    }
}
