using Fontendo.Extensions;
using Fontendo.Interfaces;
using System;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using static Fontendo.Extensions.FontBase;
using static Fontendo.Extensions.PropertyList;
using static Fontendo.Formats.FontProperties;

namespace Fontendo.Formats.CTR
{
    public class BCFNT : IFontendoFont
    {
        private CFNT? CFNT = null;
        private FINF? FINF = null;
        private CGLP? CGLP = null;
        private TGLP? TGLP = null;

        private List<CWDH>? CWDH_Headers = null;
        private List<CharWidths>? CharWidths = null;
        private List<CMAP>? CMAP_Headers = null;
        private List<CMAPEntry>? CharMaps = null;
        private Sheets? Sheets;
        private List<CharImage>? CharImages = null;
        private List<Glyph>? Glyphs = null;

        public List<PropertyListEntryDescriptor> GlyphProperty = new List<PropertyListEntryDescriptor>()
        {
            new PropertyListEntryDescriptor((int)GlyphProp.Index, "Index", PropertyType.UInt16),
            new PropertyListEntryDescriptor((int)GlyphProp.Code, "Code point", PropertyType.UInt16),
            new PropertyListEntryDescriptor((int)GlyphProp.Left, "Left", PropertyType.SByte, (-0x7F, 0x7F)),
            new PropertyListEntryDescriptor((int)GlyphProp.GlyphWidth, "Glyph width", PropertyType.Byte, (0x0, 0xFF)),
            new PropertyListEntryDescriptor((int)GlyphProp.CharWidth, "Char width", PropertyType.Byte, (0x0, 0xFF))
        };

        public List<PropertyListEntryDescriptor> FontProperties = new List<PropertyListEntryDescriptor>();



        private ITextureCodec Codec = TextureCodecFactory.Create(TextureCodecFactory.Platform.CTR);


        public Point? CellSize;
        public Point? SheetSize;

        public Sheets GetSheets()
        {
            if (Sheets == null)
                Sheets = new Sheets(0, 0);
            return Sheets;
        }
        public List<CharImage> GetCharImages(int? sheet = null)
        {
            if (CharImages == null)
                CharImages = new List<CharImage>();
            if (sheet == null)
                return CharImages;
            else
                return CharImages.Where(c => c.Sheet.Equals(sheet)).ToList();
        }

        public List<Glyph> GetGlyphDetails(int? glyphid = null)
        {
            if (Glyphs == null)
                Glyphs = new List<Glyph>();
            if(glyphid == null)
                return Glyphs;
            else
                return Glyphs.Where(g => g.Index.Equals(glyphid)).ToList();
        }

        public ActionResult Load(string filename)
        {
            ActionResult result;

            CFNT = new CFNT();
            FINF = new FINF();
            using (var br = new BinaryReaderX(File.OpenRead(filename)))
            {
                try
                {
                    br.SetEndianness();
                    br.BaseStream.Position = 0;

                    // CFNT Header
                    result = CFNT.Parse(br);
                    if (!result.Success)
                        return new ActionResult(false, result.Message);

                    // FINF Header
                    br.BaseStream.Position = CFNT.HeaderSize;
                    result = FINF.Parse(br);
                    if (!result.Success)
                        return new ActionResult(false, result.Message);

                    // Glyph Header
                    br.BaseStream.Position = FINF.PtrGlyph - 0x8U;
                    if (FINF.FontType == 0x0)  //FONT_TYPE_GLYPH, use CGLP
                    {
                        //CGLP is a section that served the same purpose as TGLP on the NTR/DS.
                        //All later font formats such as RFNT, CFNT and FFNT have this CGLP leftover
                        //both in the font converter and the SDK headers...maybe they planned it but
                        //it got replaced with TGLP and monochrome image formats it supports
                        CGLP = new CGLP();
                        result = CGLP.Parse(br);
                        if (!result.Success)
                            return new ActionResult(false, result.Message);
                        //TODO: Find out if RVL supported CGLP fonts or not, if it did then implement it here
                        return new ActionResult(false, "You've found a CGLP RVL font! Aborting because it is not known how to handle the file...");

                    }
                    else if (FINF.FontType == 0x1)
                    {
                        TGLP = new TGLP();
                        result = TGLP.Parse(br);
                        if (!result.Success)
                            return new ActionResult(false, result.Message);
                    }
                    else
                    {
                        return new ActionResult(false, $"Unknown font type 0x{FINF.FontType.ToString("X")}");
                    }
                    // CWDH data
                    UInt32 nextPtr = FINF.PtrWidth - 0x8;
                    CWDH_Headers = new List<CWDH>();
                    CharWidths = new List<CharWidths>();
                    while (nextPtr != 0x0)
                    {
                        br.BaseStream.Position = nextPtr;
                        CWDH cwdh = new CWDH(); //Read the header and the entries
                        result = cwdh.Parse(br);
                        if (!result.Success)
                            return new ActionResult(false, result.Message);
                        CharWidths.AddRange(cwdh.Entries);
                        nextPtr = cwdh.PtrNext; //Set the pointer to the next section
                        CWDH_Headers.Add(cwdh); //Append the header
                    }

                    // CMAP data
                    nextPtr = FINF.PtrMap;
                    CMAP_Headers = new List<CMAP>();
                    CharMaps = new List<CMAPEntry>();
                    while (nextPtr != 0x0)
                    {
                        br.BaseStream.Position = nextPtr - 0x8; //This is somewhat of a bodge, but it work nonetheless
                        CMAP cmap = new CMAP();
                        result = cmap.Parse(br);
                        if (!result.Success)
                            return new ActionResult(false, result.Message);
                        CharMaps.AddRange(cmap.entries);
                        nextPtr = cmap.PtrNext;
                        CMAP_Headers.Add(cmap); //Append the CMAP header to the list
                    }

                    // Decoding textures
                    br.BaseStream.Position = TGLP.SheetPtr;
                    if (Sheets?.Items != null)
                    {
                        foreach (var bmp in Sheets.Items)
                        {
                            bmp.Dispose();
                        }
                        Sheets.Items.Clear();
                    }
                    Sheets = new Sheets(TGLP.SheetWidth, TGLP.SheetHeight);
                    Sheets.Items = new List<Bitmap>();
                    for (ushort i = 0; i < TGLP.SheetCount; i++)
                    {
                        byte[] sheetRaw = Codec.DecodeTexture(TGLP.SheetFormat, br, TGLP.SheetWidth, TGLP.SheetHeight);

                        Bitmap sheet = new Bitmap(TGLP.SheetWidth, TGLP.SheetHeight, PixelFormat.Format32bppArgb);

                        // Lock the bitmap’s bits
                        var rect = new Rectangle(0, 0, sheet.Width, sheet.Height);
                        var bmpData = sheet.LockBits(rect, ImageLockMode.WriteOnly, sheet.PixelFormat);

                        try
                        {
                            // Copy raw ARGB data into the bitmap
                            System.Runtime.InteropServices.Marshal.Copy(sheetRaw, 0, bmpData.Scan0, sheetRaw.Length);
                        }
                        finally
                        {
                            sheet.UnlockBits(bmpData);
                        }

                        Sheets.Items.Add(sheet);
                    }

                    //Pull sheets apart into individual glyph images
                    if (CharImages != null)
                    {
                        foreach (var img in CharImages)
                        {
                            img.Image.Dispose();
                        }
                        CharImages.Clear();
                    }
                    CharImages = new List<CharImage>();
                    Glyphs = new List<Glyph>();
                    for (int i = 0; i < CharMaps.Count(); i++) //Get glyph count from charMaps, cause there's no better way to do that
                    {
                        //Do some math to figure out where we should start reading the pixels
                        int currentSheet = i / (TGLP.CellsPerRow * TGLP.CellsPerColumn);
                        int i2 = i - (currentSheet * TGLP.CellsPerRow * TGLP.CellsPerColumn);
                        int currentRow = i2 / TGLP.CellsPerRow;
                        int currentColumn = i2 - (currentRow * TGLP.CellsPerRow);
                        int startX = currentColumn * (TGLP.CellWidth + 1);
                        int startY = currentRow * (TGLP.CellHeight + 1);
                        // Copy pixels from the sheet into a new Bitmap

                        Rectangle rect = new Rectangle(startX, startY, TGLP.CellWidth + 1, TGLP.CellHeight + 1);
                        Bitmap bmp = Sheets.Items[currentSheet].Clone(rect, Sheets.Items[currentSheet].PixelFormat);
                        CharImage newImg = new CharImage(i, currentSheet, bmp);
                        // Append Bitmap to list
                        CharImages.Add(newImg);
                        // add glyph to list
                        var props = new List<PropertyBase>
                        {
                            new Property<ushort>(GlyphProperty[(int)GlyphProp.Index], 0),
                            new Property<ushort>(GlyphProperty[(int)GlyphProp.Code], CharMaps[i].Code),
                            new Property<sbyte>(GlyphProperty[(int)GlyphProp.Left], CharWidths[i].Left),
                            new Property<byte>(GlyphProperty[(int)GlyphProp.GlyphWidth], CharWidths[i].GlyphWidth),
                            new Property<byte>(GlyphProperty[(int)GlyphProp.CharWidth], CharWidths[i].CharWidth)
                        };
                        Glyphs.Add(new Glyph(i, props, CharImages[CharImages.Count() - 1].Image));
                    }
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
                            return new ActionResult(false, $"unknown BCFNT glyph encoding {FINF.Encoding}");
                    }

                    // Assign the range to the descriptor
                    GlyphProperty[(int)GlyphProp.Code].ValueRange = range;

                    if (CharMaps.Count() != CharWidths.Count())
                        return new ActionResult(false, "character maps count does not match character widths count");

                    // fill in some other stuff
                    CellSize = new Point(TGLP.CellWidth, TGLP.CellHeight);
                    SheetSize = new Point(TGLP.SheetWidth, TGLP.SheetHeight);

                    FontProperties = new List<PropertyListEntryDescriptor>
                    {
                        // Shared with NTR
                        new PropertyListEntryDescriptor((int)FontProperty.Endianness, "Endianness", PropertyType.Bool),
                        new PropertyListEntryDescriptor((int)FontProperty.CharEncoding, "Char encoding", PropertyType.CharEncoding),
                        new PropertyListEntryDescriptor((int)FontProperty.LineFeed, "Line feed", PropertyType.Byte, (0, 0xFF)), // aka Leading
                        new PropertyListEntryDescriptor((int)FontProperty.Height, "Height", PropertyType.Byte, (0, 0xFF)),
                        new PropertyListEntryDescriptor((int)FontProperty.Width, "Width", PropertyType.Byte, (0, 0xFF)),
                        new PropertyListEntryDescriptor((int)FontProperty.Ascent, "Ascent", PropertyType.Byte, (0, 0xFF)),
                        new PropertyListEntryDescriptor((int)FontProperty.Baseline, "Baseline", PropertyType.Byte, (0, 0xFF)), // aka Descending?
                        new PropertyListEntryDescriptor((int)FontProperty.Version, "Version", PropertyType.UInt16, (1, 0)), // range.first = 1 so shows as hex string

                        // RVL-only
                        new PropertyListEntryDescriptor((int)FontProperty.NtrRvlImageFormat, "Image encoding", PropertyType.ImageFormat)
                    };

                }
                catch (Exception e)
                {
                    if (CharImages != null)
                    {
                        foreach (var img in CharImages)
                            img.Image.Dispose();
                        CharImages.Clear();
                    }
                    if(Glyphs != null)
                        Glyphs.Clear();
                }
                br.Close();
                br.Dispose();
            }
            return new ActionResult(true, "OK");
        }

        public ActionResult Save(string filename)
        {

            return new ActionResult(true, "Saved OK");
        }
    }
}