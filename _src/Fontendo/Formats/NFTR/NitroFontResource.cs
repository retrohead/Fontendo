using Fontendo.Extensions;
using Fontendo.Extensions.BinaryTools;
using Fontendo.Formats.CTR;
using Fontendo.Interfaces;
using System;
using System.Drawing;
using System.IO;
using static Fontendo.Extensions.FontBase;
using static Fontendo.Extensions.FontBase.FontSettings;
using static Fontendo.FontProperties.FontPropertyList;
using static Fontendo.FontProperties.GlyphProperties;
using static Fontendo.FontProperties.PropertyList;
using static Fontendo.Interfaces.ITextureCodec;

namespace Fontendo.Formats
{
    internal class NitroFontResource : IFontendoFont, IDisposable
    {
        private NFTR? NFTR = null;
        private FINF? FINF = null;
        private CGLP? CGLP = null;

        public Point? CellSize;
        private List<CWDH>? CWDH_Headers { get; set; } = null;
        private List<CharWidths>? CharWidths { get; set; } = null;
        private List<CMAP>? CMAP_Headers { get; set; } = null;
        private List<CMAPEntry>? CharMaps { get; set; } = null;
        public SheetsType? Sheets { get; set; } = null;
        public List<Glyph>? Glyphs { get; set; } = null;
        public List<CharImageType>? CharImages { get; set; } = null;
        public FontBase FontBase { get; }
        public ITextureCodec Codec { get; set; } = TextureCodecFactory.Create(TextureCodecFactory.Platform.NTR);

        public NitroFontResource(FontBase FontBase) 
        {
            this.FontBase = FontBase;
        }

        private bool IsPocketMonstersFont()
        {
            if(CGLP == null)
                return false;
            double pixelsPerByte = 8.0 / CGLP.BytesPerPixel;
            double expectedCellSize = Math.Ceiling(CGLP.CellWidth * CGLP.CellHeight / pixelsPerByte);
            return CGLP.CellSize != expectedCellSize;
        }

        public ActionResult Load(FontBase fontbase, string filename)
        {
            ActionResult result;
            MainWindow.Log($"------------------------------------------");
            MainWindow.Log($"Loading file: {filename}");


            string fixedCorrupt = "";
            using (var br = new BinaryReaderX(File.OpenRead(filename)))
            {
                FINF = new FINF(br);
                try
                {
                    br.SetEndianness();
                    long fileSize = br.BaseStream.Length;
                    br.BaseStream.Position = 0;

                    //Read NFTR header
                    NFTR = new NFTR(br);
                    result = NFTR.Parse(br);
                    if (!result.Success)
                        return new ActionResult(false, $"Failed parsing NFTR header\n\n{result.Message}");
                    //Read FINF block
                    br.BaseStream.Position = NFTR.PtrInfo;
                    result = FINF.Parse();
                    if (!result.Success)
                        return new ActionResult(false, $"Failed parsing FINF block\n\n{result.Message}");
                    //Read glyph header
                    br.BaseStream.Position = FINF.PtrGlyph - 0x8;
                    if (FINF.FontType == 0x0) //FONT_TYPE_GLYPH, use CGLP
                    {
                        CGLP = new CGLP(br, NFTR.Version > 0x1000);
                        result = CGLP.Parse();
                        if (!result.Success)
                            return new ActionResult(false, $"Failed parsing CGLP block\n\n{result.Message}");
                    }
                    else if (FINF.FontType == 0x1)
                    {
                        return new ActionResult(false, "You've found a TGLP NTR font! Aborting because it is not known how to handle the file...");
                    }
                    else
                    {
                        return new ActionResult(false, $"Unknown font type 0x{FINF.FontType.ToString("X2")}");
                    }
                    // Read CWDH data
                    long nextPtr = FINF.PtrWidth - 0x8;
                    CWDH_Headers = new List<CWDH>();
                    CharWidths = new List<CharWidths>();
                    while (IsValidBlockOffset(nextPtr, 0x43574448U, fileSize, br))
                    {
                        CWDH cwdh = new CWDH(br);
                        result = cwdh.Parse();
                        if (!result.Success)
                            return new ActionResult(false, $"Failed parsing CWDH block at offset 0x{nextPtr.ToString("X8")}\n\n{result.Message}");
                        CWDH_Headers.Add(cwdh);
                        CharWidths.AddRange(cwdh.Entries);
                        nextPtr = cwdh.PtrNext - 0x8;
                    }
                    // Read CMAP data
                    nextPtr = FINF.PtrMap - 0x8;
                    CMAP_Headers = new List<CMAP>();
                    CharMaps = new List<CMAPEntry>();
                    while (IsValidBlockOffset(nextPtr, 0x434D4150U, fileSize, br))
                    {
                        CMAP cmap = new CMAP(br);
                        result = cmap.Parse();
                        if (!result.Success)
                            return new ActionResult(false, $"Failed parsing CMAP block at offset 0x{nextPtr.ToString("X8")}\n\n{result.Message}");
                        CMAP_Headers.Add(cmap);
                        CharMaps.AddRange(cmap.Entries);
                        nextPtr = cmap.PtrNext - 0x8;
                    }
                    // Decoding Images
                    bool isPocketMonstersFont = IsPocketMonstersFont();
                    if (isPocketMonstersFont)
                    {
                        //Check for the font being from Pocket Monsters BW/B2W2 which need additional measures
                        CharWidths.Clear();
                        CWDH_Headers.Clear();
                    }
                    if (CharImages != null)
                    {
                        foreach (var img in CharImages)
                        {
                            img.Image.Dispose();
                        }
                        CharImages.Clear();
                    }
                    CharImages = new List<CharImageType>();
                    for (int i = 0; i < CharMaps.Count; i++)
                    {
                        long offset = CGLP.BitmapPtr + (CGLP.CellSize * i);
                        br.BaseStream.Position = offset;
                        if (isPocketMonstersFont)
                        {
                            CharWidths cw = new CharWidths(br);
                            result = cw.Parse();
                            if (!result.Success)
                                return new ActionResult(false, $"Failed parsing CWDH charwidth at index {i}\n\n{result.Message}");
                            CharWidths.Add(cw);
                        }
                        DecodedTextureType tex = Codec.DecodeTexture(CGLP.BytesPerPixel, br, CGLP.CellWidth, CGLP.CellHeight);
                        CharImages.Add(
                            new CharImageType(
                                i, 
                                0, // applying all to one large sheet 
                                FontBase.CreateBitmapFromBytes(
                                    tex.data, 
                                    CGLP.CellWidth, 
                                    CGLP.CellHeight, 
                                    System.Drawing.Imaging.PixelFormat.Format32bppArgb
                                )
                            )
                        );
                    }
                    // sort CMAP entries
                    CharMaps.Sort((a, b) => a.Index.CompareTo(b.Index));
                    // Merge any entries which define a glyph with the same index, some fonts have duped entries across different CMAPs
                    for (int i=0;i < CharMaps.Count - 1; i++)
                    {
                        if (CharMaps[i].Code == CharMaps[i + 1].Code)
                        {
                            CharMaps.RemoveAt(i + 1);
                            i--;
                        }
                    }
                    // try to fix corrupted fonts with too many char widths from NintyFont
                    if (CharMaps.Count() != CharWidths.Count())
                    {
                        if(FINF.DefaultWidths == null)
                            throw new Exception("Font has mismatched char widths and maps, but no default width to use for fixing");
                        while (CharWidths.Count() < CharMaps.Count())
                            CharWidths.Add(FINF.DefaultWidths);
                    }

                    // create glyphs
                    Glyphs = new List<Glyph>();
                    for (int i = 0; i < CharMaps.Count; i++)
                    {
                        Glyph glyph = new Glyph(CharImages[i].Sheet, i, CharMaps[i].Index);
                        glyph.Settings.Index = i;
                        glyph.Settings.CodePoint = CharMaps[i].Code;
                        glyph.Settings.Left = CharWidths[i].Left;
                        glyph.Settings.GlyphWidth = CharWidths[i].GlyphWidth;
                        glyph.Settings.CharWidth = CharWidths[i].CharWidth;
                        glyph.Settings.Image = CharImages[i].Image;
                        Glyphs.Add(glyph);
                    }

                    // Stitch glyphs together into one large sheet
                    Bitmap sheetBitmap = new Bitmap(FINF.Width, FINF.Height);
                    using (Graphics g = Graphics.FromImage(sheetBitmap))
                    {
                        g.Clear(Color.Transparent);
                        for (int i = 0; i < Glyphs.Count; i++)
                        {
                            Glyph glyph = Glyphs[i];
                            int x = glyph.Settings.Index % (FINF.Width / CGLP.CellWidth) * CGLP.CellWidth;
                            int y = glyph.Settings.Index / (FINF.Width / CGLP.CellWidth) * CGLP.CellHeight;
                            g.DrawImage(glyph.Settings.Image, x, y, CGLP.CellWidth, CGLP.CellHeight);
                        }
                    }
                    Sheets = new SheetsType(FINF.Width, FINF.Height);
                    Sheets.Images.Add(sheetBitmap);
                    Sheets.MaskImages.Add(null);

                    // Set the glyph code point bounds
                    (long Min, long Max) range;

                    switch (FINF.Encoding)
                    {
                        case (int)CharEncodings.Num:
                        case (int)CharEncodings.UTF16:
                        case (int)CharEncodings.ShiftJIS:
                            range = (0x0, 0xFFFE);
                            break;

                        case (int)CharEncodings.UTF8:
                        case (int)CharEncodings.CP1252:
                            range = (0x0, 0xFF);
                            break;

                        default:
                            return new ActionResult(false, $"unknown NTFR glyph encoding {FINF.Encoding}");
                    }
                    // Assign the range to the descriptors
                    foreach (var g in Glyphs)
                        g.Settings.UpdateValueRange(GlyphProperty.Code, range);

                    // fill in some other stuff
                    CellSize = new Point(CGLP.CellWidth, CGLP.CellHeight);


                    FontBase.Settings.Endianness = br.GetEndianness();
                    FontBase.Settings.CharEncoding = (CharEncodings)FINF.Encoding;
                    FontBase.Settings.LineFeed = FINF.LineFeed;
                    FontBase.Settings.Height = FINF.Height;
                    FontBase.Settings.Width = FINF.Width;
                    FontBase.Settings.Ascent = FINF.Ascent;
                    FontBase.Settings.Baseline = (byte)CGLP.BaselinePos;
                    FontBase.Settings.Version = NFTR.Version;
                    // ntr specific
                    FontBase.Settings.NtrBpp = CGLP.BytesPerPixel;
                    FontBase.Settings.NtrVertical = CGLP.Flags;
                    FontBase.Settings.NtrRotation = CGLP.Flags;

                    FontBase.Settings.Sheets = Sheets;
                    FontBase.Settings.Glyphs = Glyphs;

                    if(NFTR.Version <= 0x1000)
                    {
                        // hide some controls for older versions
                        FontBase.Settings.UpdatePreferredControl(FontProperty.Height, EditorType.None);
                        FontBase.Settings.UpdatePreferredControl(FontProperty.Width, EditorType.None);
                        FontBase.Settings.UpdatePreferredControl(FontProperty.Ascent, EditorType.None);
                    }

                    if (isPocketMonstersFont)
                    {
                        FontBase.Settings.AddProperty(FontProperty.NtrGameFreak, "PM BW/B2W2 font", PropertyValueType.Bool, EditorType.None);
                        FontBase.Settings.NtrGameFreak = true;
                    }
                }
                catch (Exception e)
                {
                    Dispose();
                    return new ActionResult(false, e.Message);
                }
                br.Close();
                br.Dispose();
            }

            return new ActionResult(true, "OK");
        }

        public void RecreateGlyphsFromSheet(int i)
        {
            throw new NotImplementedException();
        }

        public void RecreateSheetFromGlyphs(int i)
        {
            throw new NotImplementedException();
        }

        public ActionResult Save(string filename)
        {
            throw new NotImplementedException();
        }

        bool IsValidBlockOffset(long offset, uint blockMagic, long fileLength, BinaryReader br)
        {
            // PM fonts sometimes contain weird offsets, so validate carefully.
            if (offset == 0)
                return false;

            // Must have at least 4 bytes available for the magic
            if (offset > fileLength - 4)
                return false;

            try
            {
                br.BaseStream.Position = offset;
                uint magic = br.ReadUInt32();
                return magic == blockMagic;
            }
            finally
            {
                // Restore original position
                br.BaseStream.Position = offset;
            }
        }


        public void Dispose()
        {
            if (CharImages != null)
            {
                foreach (var img in CharImages)
                {
                    img.Image.Dispose();
                }
                CharImages.Clear();
            }
            if (Sheets != null && Sheets.Images != null)
            {
                foreach (var bmp in Sheets.Images)
                {
                    bmp.Dispose();
                }
                Sheets.Images.Clear();
            }
            if (Sheets != null && Sheets.MaskImages != null)
            {
                foreach (var bmp in Sheets.MaskImages)
                {
                    if (bmp != null)
                        bmp.Dispose();
                }
                Sheets.MaskImages.Clear();
            }
            if (Glyphs != null)
            {
                foreach (var glyph in Glyphs)
                {
                    if (glyph.Settings.Image != null)
                        glyph.Settings.Image.Dispose();
                    if (glyph.MaskImage != null)
                        glyph.MaskImage.Dispose();
                }
                Glyphs.Clear();
            }
        }
    }
}
