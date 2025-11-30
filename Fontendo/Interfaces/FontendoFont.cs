using Fontendo.Extensions;
using static Fontendo.Extensions.FontBase;

namespace Fontendo.Interfaces
{
    public interface IFontendoFont
    {
        public ActionResult Load(string filename);
        public ActionResult Save(string filename);
        public Sheets GetSheets();
        public List<CharImage> GetCharImages(int? sheet = null);
        public List<Glyph> GetGlyphDetails(int? sheet = null);
    }
}
