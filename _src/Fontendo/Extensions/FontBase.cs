using Fontendo.Formats.CTR;
using Fontendo.Interfaces;
using System.ComponentModel;
using System.Windows.Data;
using System.Drawing;
using static Fontendo.FontProperties.FontPropertyList;
using static Fontendo.FontProperties.PropertyList;
using Color = System.Drawing.Color;
using Fontendo.Formats;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Fontendo.Extensions
{
    public class FontBase : INotifyPropertyChanged, IDisposable
    {
        public enum FontPointerType
        {
            ptrFont,
            ptrWidth,
            ptrMap,
            ptrInfo,
            blockCount,
            fileSize,
            glyphLength,
            ptrGlyph,
            sheetPtr,
            CMAP,
            CWHD
        }
        public class FontSettings : INotifyPropertyChanged
        {
            private FontPropertyRegistry Properties { get; set; } = new FontPropertyRegistry();

            private List<FontProperty> CreatedProperties = new List<FontProperty>();

            public class SheetsType : INotifyPropertyChanged
            {
                private int _width;
                public int Width
                {
                    get
                    { return _width; }
                    set
                    {
                        if (_width != value)
                        {
                            _width = value; OnPropertyChanged(nameof(Width));
                        }
                    }
                }
                private int _height;
                public int Height
                {
                    get { return _height; }
                    set
                    {
                        if (_height != value)
                        {
                            _height = value; OnPropertyChanged(nameof(Height));

                        }
                    }
                }

                public List<Bitmap> Images
                {
                    get; set;
                }
                public List<Bitmap?> MaskImages
                {
                    get; set;
                }

                public bool HasMaskImages
                {
                    get
                    {
                        foreach (var img in MaskImages)
                        {
                            if (img != null)
                                return true;
                        }
                        return false;
                    }
                }

                public SheetsType(int Width, int Height)
                {
                    this.Width = Width;
                    this.Height = Height;
                    MaskImages = new List<Bitmap?>();
                    Images = new List<Bitmap>();
                }

                public event PropertyChangedEventHandler? PropertyChanged;
                protected void OnPropertyChanged(string propertyName) =>
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            public class CharImageType : INotifyPropertyChanged
            {
                private int _index;
                public int Index
                {
                    get
                    { return _index; }
                    set
                    {
                        if (_index != value)
                        {
                            _index = value; OnPropertyChanged(nameof(Index));
                        }
                    }
                }

                private int _sheet;
                public int Sheet
                {
                    get { return _sheet; }
                    set
                    {
                        if (_sheet != value)
                        {
                            _sheet = value; OnPropertyChanged(nameof(Sheet));
                        }
                    }
                }

                private Bitmap _image;
                public Bitmap Image
                {
                    get { return _image; }
                    set
                    {
                        if (_image != value)
                        {
                            _image = value; OnPropertyChanged(nameof(Image));
                        }
                    }
                }

                public CharImageType(int Index, int Sheet, Bitmap Image)
                {
                    this.Index = Index;
                    this.Sheet = Sheet;
                    this.Image = Image;
                }

                public event PropertyChangedEventHandler? PropertyChanged;
                protected void OnPropertyChanged(string propertyName) =>
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            // font properties
            public Endianness.Endian Endianness
            {
                get
                {
                    return Properties.GetValue<Endianness.Endian>(FontProperty.Endianness);
                }
                set
                {
                    if (!CreatedProperties.Contains(FontProperty.Endianness))
                    {
                        Properties.SetValue(FontProperty.Endianness, value);
                        CreatedProperties.Add(FontProperty.Endianness);
                        return;
                    }
                    if (Endianness != value)
                    {
                        Properties.SetValue(FontProperty.Endianness, value);
                        OnPropertyChanged(nameof(Endianness));
                    }
                }
            }
            public CharEncodings CharEncoding
            {
                get
                {
                    return Properties.GetValue<CharEncodings>(FontProperty.CharEncoding);
                }
                set
                {
                    if (!CreatedProperties.Contains(FontProperty.CharEncoding))
                    {
                        Properties.SetValue(FontProperty.CharEncoding, value);
                        CreatedProperties.Add(FontProperty.CharEncoding);
                        return;
                    }
                    if (CharEncoding != value)
                    {
                        Properties.SetValue(FontProperty.CharEncoding, value);
                        OnPropertyChanged(nameof(CharEncoding));
                    }
                }
            }
            public byte LineFeed
            {
                get
                {
                    return Properties.GetValue<byte>(FontProperty.LineFeed);
                }
                set
                {
                    if (!CreatedProperties.Contains(FontProperty.LineFeed))
                    {
                        Properties.SetValue(FontProperty.LineFeed, value);
                        CreatedProperties.Add(FontProperty.LineFeed);
                        return;
                    }
                    if (LineFeed != value)
                    {
                        Properties.SetValue(FontProperty.LineFeed, value);
                        OnPropertyChanged(nameof(LineFeed));
                    }
                }
            }
            public byte Height
            {
                get
                {
                    return Properties.GetValue<byte>(FontProperty.Height);
                }
                set
                {
                    if (!CreatedProperties.Contains(FontProperty.Height))
                    {
                        Properties.SetValue(FontProperty.Height, value);
                        CreatedProperties.Add(FontProperty.Height);
                        return;
                    }
                    if (Height != value)
                    {
                        Properties.SetValue(FontProperty.Height, value);
                        OnPropertyChanged(nameof(Height));
                    }
                }
            }
            public byte Width
            {
                get
                {
                    return Properties.GetValue<byte>(FontProperty.Width);
                }
                set
                {
                    if (!CreatedProperties.Contains(FontProperty.Width))
                    {
                        Properties.SetValue(FontProperty.Width, value);
                        CreatedProperties.Add(FontProperty.Width);
                        return;
                    }
                    if (Width != value)
                    {
                        Properties.SetValue(FontProperty.Width, value);
                        OnPropertyChanged(nameof(Width));
                    }
                }
            }
            public byte Ascent
            {
                get
                {
                    return Properties.GetValue<byte>(FontProperty.Ascent);
                }
                set
                {
                    if (!CreatedProperties.Contains(FontProperty.Ascent))
                    {
                        Properties.SetValue(FontProperty.Ascent, value);
                        CreatedProperties.Add(FontProperty.Ascent);
                        return;
                    }
                    if (Ascent != value)
                    {
                        Properties.SetValue(FontProperty.Ascent, value);
                        OnPropertyChanged(nameof(Ascent));
                    }
                }
            }
            public byte Baseline
            {
                get
                {
                    return Properties.GetValue<byte>(FontProperty.Baseline);
                }
                set
                {
                    if (!CreatedProperties.Contains(FontProperty.Baseline))
                    {
                        Properties.SetValue(FontProperty.Baseline, value);
                        CreatedProperties.Add(FontProperty.Baseline);
                        return;
                    }
                    if (Baseline != value)
                    {
                        Properties.SetValue(FontProperty.Baseline, value);
                        OnPropertyChanged(nameof(Baseline));
                    }
                }
            }
            public uint Version
            {
                get
                {
                    return Properties.GetValue<uint>(FontProperty.Version);
                }
                set
                {
                    if (!CreatedProperties.Contains(FontProperty.Version))
                    {
                        Properties.SetValue(FontProperty.Version, value);
                        CreatedProperties.Add(FontProperty.Version);
                        return;
                    }
                    if (Version != value)
                    {
                        Properties.SetValue(FontProperty.Version, value);
                        OnPropertyChanged(nameof(Version));
                    }
                }
            }
            public SheetsType? Sheets
            {
                get
                {
                    return Font.Sheets;
                }
                set
                {
                    if (Font.Sheets != value)
                    {
                        Font.Sheets = value;
                        OnPropertyChanged(nameof(Sheets));
                    }
                }
            }
            public List<Glyph>? Glyphs
            {
                get
                {
                    return Font.Glyphs;
                }
                set
                {
                    if (Font.Glyphs != value)
                    {
                        Font.Glyphs = value;
                        OnPropertyChanged(nameof(Glyphs));
                    }
                }
            }

            // NTR-specific

            public byte NtrBpp
            {
                get
                {
                    return Properties.GetValue<byte>(FontProperty.NtrBpp);
                }
                set
                {
                    if (!CreatedProperties.Contains(FontProperty.NtrBpp))
                    {
                        Properties.SetValue(FontProperty.NtrBpp, value);
                        CreatedProperties.Add(FontProperty.NtrBpp);
                        return;
                    }
                    if (NtrBpp != value)
                    {
                        Properties.SetValue(FontProperty.NtrBpp, value);
                        OnPropertyChanged(nameof(NtrBpp));
                    }
                }
            }

            public byte NtrVertical
            {
                get
                {
                    return Properties.GetValue<byte>(FontProperty.NtrVertical);
                }
                set
                {
                    if (!CreatedProperties.Contains(FontProperty.NtrVertical))
                    {
                        Properties.SetValue(FontProperty.NtrVertical, value);
                        CreatedProperties.Add(FontProperty.NtrVertical);
                        return;
                    }
                    if (NtrVertical != value)
                    {
                        Properties.SetValue(FontProperty.NtrVertical, value);
                        OnPropertyChanged(nameof(NtrVertical));
                    }
                }
            }

            public byte NtrRotation
            {
                get
                {
                    return Properties.GetValue<byte>(FontProperty.NtrRotation);
                }
                set
                {
                    if (!CreatedProperties.Contains(FontProperty.NtrRotation))
                    {
                        Properties.SetValue(FontProperty.NtrRotation, value);
                        CreatedProperties.Add(FontProperty.NtrRotation);
                        return;
                    }
                    if (NtrRotation != value)
                    {
                        Properties.SetValue(FontProperty.NtrRotation, value);
                        OnPropertyChanged(nameof(NtrRotation));
                    }
                }
            }

            public bool NtrGameFreak
            {
                get
                {
                    return Properties.GetValue<bool>(FontProperty.NtrGameFreak);
                }
                set
                {
                    if (!CreatedProperties.Contains(FontProperty.NtrGameFreak))
                    {
                        Properties.SetValue(FontProperty.NtrGameFreak, value);
                        CreatedProperties.Add(FontProperty.NtrGameFreak);
                        return;
                    }
                    if (NtrGameFreak != value)
                    {
                        Properties.SetValue(FontProperty.NtrGameFreak, value);
                        OnPropertyChanged(nameof(NtrGameFreak));
                    }
                }
            }

            //NtrRotation,
            //NtrGameFreak,

            // RVL only
            public ImageFormats NtrRvlImageFormat
            {
                get
                {
                    return Properties.GetValue<ImageFormats>(FontProperty.NtrRvlImageFormat);
                }
                set
                {
                    if (!CreatedProperties.Contains(FontProperty.NtrRvlImageFormat))
                    {
                        Properties.SetValue(FontProperty.NtrRvlImageFormat, value);
                        CreatedProperties.Add(FontProperty.NtrRvlImageFormat);
                        return;
                    }
                    if (NtrRvlImageFormat != value)
                    {
                        Properties.SetValue(FontProperty.NtrRvlImageFormat, value);
                        OnPropertyChanged(nameof(NtrRvlImageFormat));
                    }
                }
            }

            private IFontendoFont Font;
            public FontSettings(IFontendoFont Font)
            {
                this.Font = Font;
            }


            public T GetValue<T>(FontProperty property)
            {
                return Properties.GetValue<T>(property);
            }


            public Dictionary<FontProperty, FontPropertyDescriptor> PropertyDescriptors
            {
                get
                {
                    return Properties.FontPropertyDescriptors;
                }
                set
                {
                    if (Properties.FontPropertyDescriptors != value)
                    {
                        Properties.FontPropertyDescriptors.Clear();
                        foreach (var kvp in value)
                        {
                            Properties.FontPropertyDescriptors.Add(kvp.Key, kvp.Value);
                        }
                        OnPropertyChanged(nameof(PropertyDescriptors));
                    }
                }
            }
            public void AddProperty(
                FontProperty property,
                string name,
                PropertyValueType type,
                EditorType editorType = EditorType.None,
                (long Min, long Max)? range = null)
            {
                Properties.AddProperty(property, name, type, editorType, range);
            }


            public bool GetBindingsForObject(FontProperty property, out List<Binding> bindings)
            {
                bindings = new List<Binding>();

                switch (property)
                {
                    case FontProperty.Endianness:
                        bindings.Add(new Binding(nameof(Endianness))
                        {
                            Source = this,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });
                        return true;

                    case FontProperty.CharEncoding:
                        bindings.Add(new Binding(nameof(CharEncoding))
                        {
                            Source = this,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });
                        return true;

                    case FontProperty.LineFeed:
                        bindings.Add(new Binding(nameof(LineFeed))
                        {
                            Source = this,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });
                        return true;

                    case FontProperty.Height:
                        bindings.Add(new Binding(nameof(Height))
                        {
                            Source = this,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });
                        return true;

                    case FontProperty.Width:
                        bindings.Add(new Binding(nameof(Width))
                        {
                            Source = this,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });
                        return true;

                    case FontProperty.Ascent:
                        bindings.Add(new Binding(nameof(Ascent))
                        {
                            Source = this,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });
                        return true;

                    case FontProperty.Baseline:
                        bindings.Add(new Binding(nameof(Baseline))
                        {
                            Source = this,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });
                        return true;

                    case FontProperty.NtrRvlImageFormat:
                        bindings.Add(new Binding(nameof(NtrRvlImageFormat))
                        {
                            Source = this,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });
                        return true;

                    case FontProperty.Version:
                        var vb = new Binding(nameof(Version))
                        {
                            Source = this,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            Converter = new HexValueConverter() // implement IValueConverter for hex formatting/parsing
                        };
                        bindings.Add(vb);
                        return true;

                    case FontProperty.NtrBpp:
                        bindings.Add(new Binding(nameof(NtrBpp))
                        {
                            Source = this,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });
                        return true;

                    case FontProperty.NtrRotation:
                        bindings.Add(new Binding(nameof(NtrRotation))
                        {
                            Source = this,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });
                        return true;

                    case FontProperty.NtrVertical:
                        bindings.Add(new Binding(nameof(NtrVertical))
                        {
                            Source = this,
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });
                        return true;

                    default:
                        throw new NotImplementedException($"No binding implemented for property {property}");
                }
            }

            public void UpdateValueRange(FontProperty property, (long Min, long Max) range)
            {
                Properties.UpdateValueRange(property, range);
            }
            public void UpdatePreferredControl(FontProperty property, EditorType editor)
            {
                Properties.UpdatePreferredControl(property, editor);
            }

            public event PropertyChangedEventHandler? PropertyChanged;
            protected void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public FontSettings Settings
        {
            get;
            private set;
        }

        private IFontendoFont Font;
        public ITextureCodec TextureCodec;
        public FileSystemHelper.FileType LoadedFontFileType;

        private string _loadedFontPath = "";
        public string LoadedFontFilePath
        {
            get { return _loadedFontPath; }
            private set
            {
                if (_loadedFontPath != value)
                {
                    _loadedFontPath = value;
                    OnPropertyChanged(nameof(LoadedFontFilePath));
                    OnPropertyChanged(nameof(IsLoaded));
                }
            }
        }
        public bool IsLoaded
        {
            get
            {
                return LoadedFontFilePath != "";
            }
        }
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
                    Font = new BCFNT(this);
                    Settings = new FontSettings(Font);
                    Settings.AddProperty(FontProperty.Endianness, "Endianness", PropertyValueType.Bool, EditorType.EndiannessPicker);
                    Settings.AddProperty(FontProperty.CharEncoding, "Char encoding", PropertyValueType.CharEncoding, EditorType.Label);
                    Settings.AddProperty(FontProperty.LineFeed, "Line feed", PropertyValueType.Byte, EditorType.NumberBox, (0, 0xFF));
                    Settings.AddProperty(FontProperty.Height, "Height", PropertyValueType.Byte, EditorType.NumberBox, (0, 0xFF));
                    Settings.AddProperty(FontProperty.Width, "Width", PropertyValueType.Byte, EditorType.NumberBox, (0, 0xFF));
                    Settings.AddProperty(FontProperty.Ascent, "Ascent", PropertyValueType.Byte, EditorType.NumberBox, (0, 0xFF));
                    Settings.AddProperty(FontProperty.Baseline, "Baseline", PropertyValueType.Byte, EditorType.NumberBox, (0, 0xFF));
                    Settings.AddProperty(FontProperty.Version, "Version", PropertyValueType.UInt32, EditorType.Label, (0, 0xFFFFFFFF));
                    Settings.AddProperty(FontProperty.NtrRvlImageFormat, "Image encoding", PropertyValueType.ImageFormat, EditorType.Label);

                    break;
                case Platform.Nitro:
                    TextureCodec = new Fontendo.Codecs.NTR.NTRTextureCodec();
                    Font = new NitroFontResource(this);
                    Settings = new FontSettings(Font);
                    Settings.AddProperty(FontProperty.Endianness, "Endianness", PropertyValueType.Bool, EditorType.EndiannessPicker);
                    Settings.AddProperty(FontProperty.CharEncoding, "Char encoding", PropertyValueType.CharEncoding, EditorType.Label);
                    Settings.AddProperty(FontProperty.LineFeed, "Line feed", PropertyValueType.Byte, EditorType.NumberBox, (0, 0xFF));
                    Settings.AddProperty(FontProperty.Height, "Height", PropertyValueType.Byte, EditorType.NumberBox, (0, 0xFF));
                    Settings.AddProperty(FontProperty.Width, "Width", PropertyValueType.Byte, EditorType.NumberBox, (0, 0xFF));
                    Settings.AddProperty(FontProperty.Ascent, "Ascent", PropertyValueType.Byte, EditorType.NumberBox, (0, 0xFF));
                    Settings.AddProperty(FontProperty.Baseline, "Baseline", PropertyValueType.Byte, EditorType.NumberBox, (0, 0xFF));
                    Settings.AddProperty(FontProperty.Version, "Version", PropertyValueType.UInt32, EditorType.Label, (0, 0xFFFFFFFF));
                    // ntr only
                    Settings.AddProperty(FontProperty.NtrBpp, "Image bit depth", PropertyValueType.Byte, EditorType.Label);
                    Settings.AddProperty(FontProperty.NtrVertical, "Vertical", PropertyValueType.Byte, EditorType.CheckBox);
                    Settings.AddProperty(FontProperty.NtrRotation, "Rotation", PropertyValueType.Byte, EditorType.Label);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform));
            }
        }

        public static FontBase? CreateFontBase(string path)
        {
            FontBase fontBase;
            FileSystemHelper.FileType fontFileType = FileSystemHelper.GetFileTypeFromPath(path);
            if (fontFileType == FileSystemHelper.FileType.All)
                return null;
            // establish codec from file extension
            switch (fontFileType)
            {
                case FileSystemHelper.FileType.BinaryCrustFont:
                    fontBase = new FontBase(Platform.CTR);
                    break;
                case FileSystemHelper.FileType.NitroFontResource:
                    fontBase = new FontBase(Platform.Nitro);
                    break;
                default:
                    return null;
            }
            fontBase.LoadedFontFileType = fontFileType;
            return fontBase;
        }

        public ActionResult LoadFont(string path)
        {
            ActionResult result = Font.Load(this, path);
            if (result.Success)
                LoadedFontFilePath = path;
            return result;
        }

        public ActionResult SaveFont(string path)
        {
            if (!IsLoaded)
                return new ActionResult(false, "No font loaded");
            var result = Font.Save(path);
            if (result.Success)
                LoadedFontFilePath = path;
            return result;
        }

        public void RecreateSheetFromGlyphs(int sheetNumber)
        {
            Font.RecreateSheetFromGlyphs(sheetNumber);
        }

        public void RecreateGlyphsFromSheet(int sheetNumber)
        {
            Font.RecreateGlyphsFromSheet(sheetNumber);
        }

        public static Bitmap? GenerateTransparencyMask(Bitmap? source)
        {
            if (source == null)
                return null;
            int width = source.Width;
            int height = source.Height;

            Bitmap mask = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            bool hasTransparentPixels = false;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c = source.GetPixel(x, y);

                    bool interesting =
                        c.A == 0 &&
                        (c.R > 0 || c.G > 0 || c.B > 0);

                    if (interesting)
                    {
                        // White pixel in mask
                        mask.SetPixel(x, y, Color.FromArgb(255, c.R, c.G, c.B));
                        hasTransparentPixels = true;
                    }
                    else
                    {
                        // transparent pixel in mask
                        mask.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                    }
                }
            }
            if(hasTransparentPixels)
                return mask;
            return null;
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void Dispose()
        {
            if(Font != null)
                Font.Dispose();
        }

        public static Bitmap CreateBitmapFromBytes(byte[] data, int width, int height, PixelFormat pixelFormat)
        {
            Bitmap bmp = new Bitmap(width, height, pixelFormat);
            // Lock the bitmap’s bits
            var rect = new Rectangle(0, 0, width, height);
            var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

            try
            {
                int srcStride = width * 4;          // tightly packed ARGB32 input
                int dstStride = bmpData.Stride;     // GDI+ stride (may be padded)

                IntPtr dstScan0 = bmpData.Scan0;

                for (int y = 0; y < height; y++)
                {
                    // Source offset (tightly packed)
                    int srcOffset = y * srcStride;

                    // Destination offset (stride padded)
                    IntPtr dstRowPtr = IntPtr.Add(dstScan0, y * dstStride);

                    // Copy one row
                    Marshal.Copy(data, srcOffset, dstRowPtr, srcStride);
                }
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }
            return bmp;
        }
    }
}
