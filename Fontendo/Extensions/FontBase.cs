using Fontendo.Formats.NFTR;
using Fontendo.Interfaces;

namespace Fontendo.Extensions
{
    public class FontBase
    {
        public class Sheets
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public List<Bitmap> Items { get; set; }

            public Sheets(int Width, int Height)
            {
                this.Width = Width;
                this.Height = Height;
                Items = new List<Bitmap>();
            }
        }

        public IFontendoFont Font;
        public ITextureCodec TextureCodec;
        public FileSystem.FileType LoadedFontFileType;
        public string LoadedFontFilePath = "";
        public enum Platform { NTR, RVL, CTR }
        public FontBase(Platform platform)
        {
            switch (platform)
            {
                case Platform.RVL:
                    TextureCodec = new Fontendo.Codecs.RVL.RVLTextureCodec();
                    // TODO: Implement RVL fonts
                    throw new NotImplementedException("RVL font not implemented yet");
                    //Font = new BCFNT();
                    //break;
                case Platform.CTR:
                    TextureCodec = new Fontendo.Codecs.CTR.CTRTextureCodec();
                    Font = new BCFNT();
                    break;
                case Platform.NTR:
                    // TODO: Implement NTR fonts
                    TextureCodec = new Fontendo.Codecs.NTR.NTRTextureCodec();
                    throw new NotImplementedException("NTR font not implemented yet");
                //Font = new BCFNT();
                //break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform));
            }
        }
        public ActionResult LoadFont(string path)
        {
            FileSystem.FileType fontFileType = FileSystem.GetFileTypeFromPath(path);
            if (fontFileType == FileSystem.FileType.All)
                return new ActionResult(false, "File extension is not recognised");
            ActionResult result = Font.Load(path);
            // TODO: verify the font is of the expected type
            LoadedFontFileType = fontFileType;
            if (result.Success)
                LoadedFontFilePath = path;
            return result;
        }

        public bool IsLoaded()
        {
            return LoadedFontFilePath != "";
        }
    }
}
