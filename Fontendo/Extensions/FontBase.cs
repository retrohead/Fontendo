using Fontendo.Formats.CTR;
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
        public class CharImage
        {
            public int Index { get; set; }
            public int Sheet { get; set; }
            public Bitmap Image { get; set; }

            public CharImage(int Index, int Sheet, Bitmap Image)
            {
                this.Index = Index;
                this.Sheet = Sheet;
                this.Image = Image;
            }
        }

        public IFontendoFont Font;
        public ITextureCodec TextureCodec;
        public FileSystem.FileType LoadedFontFileType;
        public string LoadedFontFilePath = "";
        public enum Platform
        {
            Dolphin,
            Revolution,
            Cafe,
            NX,
            Nitro,
            CTR
        }
        public enum CharEncodings
        {
            UTF8,
            UTF16,
            ShiftJIS,
            CP1252,
            Num
        };
        public enum ImageFormats
        {
            None, //Fallback option, just in case
            L2, //DS gen 4 pokemon fonts, 2 bit luminance
            L2A,
            L4, //I4, L4, 4 bit luminance
            L4A, //IA4, LA4, 4 bit luminance + 4 bit alpha
            L8, //I8, L8, 8 bit luminance
            HL8, //CTR HL8
            A8, //CTR A8, 8 bit alpha-only, identical to L8
            A4, //CTR a4, 4 bit alpha-only, identical to L4
            L8A, //IA8, LA8, 8 bit luminance + 8 bit alpha
            RGB565, //5 bit red + 6 bit green + 5 bit blue
            RGB5A1, //5 bpc RGB + 1 bit alpha
            RGB5A3, //4 bpc RGB + 3 bit alpha or 5 bpc RGB, based on the first bit
            RGBA4, //4 bpc RGBA
            RGB8, //8 bpc RGB
            RGBA8, //8 bpc RBGBA
            ETC1, //CTR ETC1, Ericsson Texture Compression 1
            ETC1A4, //CTR ETC1A4, ETC1 with 4 bits of alpha
        };
        public FontBase(Platform platform)
        {
            switch (platform)
            {
                case Platform.Revolution:
                    TextureCodec = new Fontendo.Codecs.RVL.RVLTextureCodec();
                    // TODO: Implement RVL fonts
                    throw new NotImplementedException("RVL font not implemented yet");
                    //Font = new BCFNT();
                    //break;
                case Platform.CTR:
                    TextureCodec = new Fontendo.Codecs.CTR.CTRTextureCodec();
                    Font = new BCFNT();
                    break;
                case Platform.Nitro:
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
