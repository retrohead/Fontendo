using Fontendo.Extensions;
using Fontendo.Interfaces;
using System;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using static Fontendo.Extensions.FontBase;
using static Fontendo.Extensions.PropertyList;
using static Fontendo.Formats.FontProperties;
using static Fontendo.Formats.GlyphProperties;

namespace Fontendo.Formats.CTR
{
    public class BCFNT : IFontendoFont
    {
        private CFNT? CFNT = null;
        private FINF? FINF = null;
        private CGLP? CGLP = null;
        private TGLP? TGLP = null;

        public Point? CellSize;
        public Point? SheetSize;

        private List<CWDH>? CWDH_Headers { get; set; } = null;
        private List<CharWidths>? CharWidths { get; set; } = null;
        private List<CMAP>? CMAP_Headers { get; set; } = null;
        private List<CMAPEntry>? CharMaps { get; set; } = null;
        public Sheets? Sheets { get; set; } = null;
        public List<CharImage>? CharImages { get; set; } = null;
        public List<Glyph>? Glyphs { get; set; } = null;

        public Dictionary<GlyphProperty, FontPropertyListEntryDescriptor> GlyphPropertyDescriptors { get; set; }
            = new Dictionary<GlyphProperty, FontPropertyListEntryDescriptor>
            {
                {
                    GlyphProperty.Index,
                    new FontPropertyListEntryDescriptor(0, "Index", FontPropertyType.UInt16)
                },
                {
                    GlyphProperty.Code,
                    new FontPropertyListEntryDescriptor(1, "Code point", FontPropertyType.UInt16)
                },
                {
                    GlyphProperty.Left,
                    new FontPropertyListEntryDescriptor(2, "Left", FontPropertyType.SByte,(-0x7F, 0x7F))
                },
                {
                    GlyphProperty.GlyphWidth,
                    new FontPropertyListEntryDescriptor(3, "Glyph width", FontPropertyType.Byte, (0x0, 0xFF))
                },
                {
                    GlyphProperty.CharWidth,
                    new FontPropertyListEntryDescriptor(4, "Char width", FontPropertyType.Byte, (0x0, 0xFF))
                },
            };

        public Dictionary<GlyphProperty, PropertyBase> GlyphProperties { get; set; } = new Dictionary<GlyphProperty, PropertyBase>();

        public Dictionary<FontProperty, FontPropertyListEntryDescriptor> FontPropertyDescriptors { get; set; }
            = new Dictionary<FontProperty, FontPropertyListEntryDescriptor>
            {
                {
                    FontProperty.Endianness,
                    new FontPropertyListEntryDescriptor(0, "Endianness", FontPropertyType.Bool)
                },
                {
                    FontProperty.CharEncoding,
                    new FontPropertyListEntryDescriptor(1, "Char encoding", FontPropertyType.CharEncoding)
                },
                {
                    FontProperty.LineFeed,
                    new FontPropertyListEntryDescriptor(2, "Line feed", FontPropertyType.Byte, (0, 0xFF))
                },
                {
                    FontProperty.Height,
                    new FontPropertyListEntryDescriptor(3, "Height", FontPropertyType.Byte, (0, 0xFF))
                },
                {
                    FontProperty.Width,
                    new FontPropertyListEntryDescriptor(4, "Width", FontPropertyType.Byte, (0, 0xFF))
                },
                {
                    FontProperty.Ascent,
                    new FontPropertyListEntryDescriptor(5, "Ascent", FontPropertyType.Byte, (0, 0xFF))
                },
                {
                    FontProperty.Baseline,
                    new FontPropertyListEntryDescriptor(6, "Baseline", FontPropertyType.Byte, (0, 0xFF))
                },
                {
                    FontProperty.Version,
                    new FontPropertyListEntryDescriptor(7, "Version", FontPropertyType.UInt16, (1, 0))
                },
                {
                    FontProperty.NtrRvlImageFormat,
                    new FontPropertyListEntryDescriptor(8, "Image encoding", FontPropertyType.ImageFormat)
                }
            };

        public Dictionary<FontProperty, PropertyBase> FontProperties { get; set; } = new Dictionary<FontProperty, PropertyBase>();

        public ITextureCodec Codec { get; set; } = TextureCodecFactory.Create(TextureCodecFactory.Platform.CTR);

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
                        CharMaps.AddRange(cmap.Entries);
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
                        var props = new Dictionary<GlyphProperty, PropertyBase>
                        {
                            {
                                GlyphProperty.Index,
                                new Property<ushort>(GlyphPropertyDescriptors[GlyphProperty.Index], 0)
                            },
                            {
                                GlyphProperty.Code,
                                new Property<ushort>(GlyphPropertyDescriptors[GlyphProperty.Code], CharMaps[i].Code)
                            },
                            {
                                GlyphProperty.Left,
                                new Property<sbyte>(GlyphPropertyDescriptors[GlyphProperty.Left], CharWidths[i].Left)
                            },
                            {
                                GlyphProperty.GlyphWidth,
                                new Property<byte>(GlyphPropertyDescriptors[GlyphProperty.GlyphWidth], CharWidths[i].GlyphWidth)
                            },
                            {
                                GlyphProperty.CharWidth,
                                new Property<byte>(GlyphPropertyDescriptors[GlyphProperty.CharWidth], CharWidths[i].CharWidth)
                            }
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
                    GlyphPropertyDescriptors[GlyphProperty.Code].ValueRange = range;

                    if (CharMaps.Count() != CharWidths.Count())
                        return new ActionResult(false, "character maps count does not match character widths count");

                    // fill in some other stuff
                    CellSize = new Point(TGLP.CellWidth, TGLP.CellHeight);
                    SheetSize = new Point(TGLP.SheetWidth, TGLP.SheetHeight);

                    FontPropertyDescriptors[FontProperty.Endianness].ValueRange = range;

                    FontProperties = new Dictionary<FontProperty, PropertyBase>
                    {
                        {
                            FontProperty.Endianness,
                            new Property<bool>(FontPropertyDescriptors[FontProperty.Endianness], br.GetEndianness() == Endianness.Endian.Little)
                        },
                        {
                            FontProperty.CharEncoding,
                            new Property<CharEncodings>(FontPropertyDescriptors[FontProperty.CharEncoding], (CharEncodings)FINF.Encoding)
                        },
                        {
                            FontProperty.LineFeed,
                            new Property<CharEncodings>(FontPropertyDescriptors[FontProperty.LineFeed], (CharEncodings)FINF.LineFeed)
                        },
                        {
                            FontProperty.Height,
                            new Property<CharEncodings>(FontPropertyDescriptors[FontProperty.Height], (CharEncodings)FINF.Height)
                        },
                        {
                            FontProperty.Width,
                            new Property<CharEncodings>(FontPropertyDescriptors[FontProperty.Width], (CharEncodings)FINF.Width)
                        },
                        {
                            FontProperty.Ascent,
                            new Property<CharEncodings>(FontPropertyDescriptors[FontProperty.Ascent], (CharEncodings)FINF.Ascent)
                        },
                        {
                            FontProperty.Baseline,
                            new Property<CharEncodings>(FontPropertyDescriptors[FontProperty.Baseline], (CharEncodings)TGLP.BaselinePos)
                        },
                        {
                            FontProperty.Version,
                            new Property<CharEncodings>(FontPropertyDescriptors[FontProperty.Version], (CharEncodings)CFNT.Version)
                        },
                        {
                            FontProperty.NtrRvlImageFormat,
                            new Property<ImageFormats>(FontPropertyDescriptors[FontProperty.CharEncoding], Codec.ConvertPlatformTextureTypeToGeneral(TGLP.SheetFormat))
                        }
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
                    if (Glyphs != null)
                        Glyphs.Clear();
                }
                br.Close();
                br.Dispose();
            }
            return new ActionResult(true, "OK");
        }

        public ActionResult Save(string filename)
        {
            try
            {
                // Prepare glyphs for export: sort by code point, set indices, track max char width
                var encodedGlyphs = new List<Glyph>(Glyphs);
                encodedGlyphs.Sort((a, b) =>
                {
                    // Assuming helper reads value from glyph props
                    var codeA = GetGlyphPropertyValue<ushort>(a, GlyphProperty.Code);
                    var codeB = GetGlyphPropertyValue<ushort>(b, GlyphProperty.Code);
                    return codeA.CompareTo(codeB);
                });

                //Set glyph indexes, while also looking for the widest glyph
                byte maxCharWidth = 0;
                ushort index = 0;
                foreach (var g in encodedGlyphs)
                {
                    SetGlyphPropertyValue<UInt16>(g, GlyphProperty.Index, index);
                    var cw = GetGlyphPropertyValue<byte>(g, GlyphProperty.CharWidth);
                    if (cw > maxCharWidth) maxCharWidth = cw;
                    index++;
                }

                // Recalculate sheet geometry (equivalent to recalculateSheetInfo)

                RecalculateSheetInfo(0);

                // Stitch glyph bitmaps into sheet images
                var cellsPerRow = (uint)(SheetSize.Value.X / (CellSize.Value.X + 1));
                var cellsPerColumn = (uint)(SheetSize.Value.Y / (CellSize.Value.Y + 1));
                var cellsPerSheet = cellsPerRow * cellsPerColumn;
                var numSheets = (uint)Math.Ceiling((double)encodedGlyphs.Count / cellsPerSheet);

                var sheetImgs = new List<Bitmap>(new Bitmap[numSheets]);
                foreach (var g in encodedGlyphs)
                {
                    var i = GetGlyphPropertyValue<ushort>(g, GlyphProperty.Index);
                    var currentSheet = i / (cellsPerRow * cellsPerColumn);
                    var i2 = i - (currentSheet * cellsPerRow * cellsPerColumn);
                    var currentRow = i2 / cellsPerRow;
                    var currentColumn = i2 - (currentRow * cellsPerRow);
                    var startX = (int)(currentColumn * (CellSize.Value.X + 1));
                    var startY = (int)(currentRow * (CellSize.Value.Y + 1));

                    // Create target sheet bitmap if missing
                    if (sheetImgs[(int)currentSheet] == null)
                    {
                        var bmp = new Bitmap(SheetSize.Value.X, SheetSize.Value.Y, PixelFormat.Format32bppArgb);
                        using (var gfx = Graphics.FromImage(bmp)) gfx.Clear(Color.Transparent);
                        sheetImgs[(int)currentSheet] = bmp;
                    }

                    // Draw glyph image onto the sheet
                    using (var gfx = Graphics.FromImage(sheetImgs[(int)currentSheet]))
                    {
                        gfx.DrawImage(g.Pixmap, new Rectangle(startX, startY, g.Pixmap.Width, g.Pixmap.Height));
                    }
                }

                // Encode sheets to platform texture format
                // Image format taken from FontProperties (NtrRvlImageFormat), converted to platform
                var generalFmt = GetFontPropertyValue<ImageFormats>(this, FontProperty.NtrRvlImageFormat);
                var platformFmt = Codec.ConvertGeneralTextureTypeToPlatform(generalFmt);
                var encodedSheets = new List<byte[]>(sheetImgs.Count);
                for (int iSheet = 0; iSheet < sheetImgs.Count; iSheet++)
                {
                    var bmp = sheetImgs[iSheet];
                    // Extract raw ARGB bytes
                    var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                    var bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
                    try
                    {
                        var length = bmpData.Stride * bmp.Height;
                        var argb = new byte[length];
                        System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, argb, 0, length);

                        var encoded = Codec.EncodeTexture(platformFmt, argb, (ushort)bmp.Size.Width, (ushort)bmp.Size.Height);
                        encodedSheets.Add(encoded);
                    }
                    finally
                    {
                        bmp.UnlockBits(bmpData);
                    }
                }

                // Build width entries (CWDH) from encodedGlyphs
                List<CharWidths> widthEntries = CTR.CharWidths.CreateWidthEntries(encodedGlyphs);

                //// Build CMAP entries: Direct, Table, Scan
                List<(Glyph, Glyph)> directEntries = CMAP.CreateDirectEntries(encodedGlyphs);
                //var tableEntries = CMAP.CreateTableEntries(encodedGlyphs);
                //var scanEntries = CMAP.CreateScanEntries(encodedGlyphs);

                //// Create headers from current property values
                //// FINF expects many fields from FontProperties and current state
                //var finfLineFeed = GetFontPropertyValue<byte>(this, FontProperty.LineFeed);
                //var finfCharEnc = (byte)GetFontPropertyValue<CharEncodings>(this, FontProperty.CharEncoding);
                //var finfHeight = GetFontPropertyValue<byte>(this, FontProperty.Height);
                //var finfWidth = GetFontPropertyValue<byte>(this, FontProperty.Width);
                //var finfAscent = GetFontPropertyValue<byte>(this, FontProperty.Ascent);
                //var finfBaseline = GetFontPropertyValue<byte>(this, FontProperty.Baseline);
                //var cfntVersion = GetFontPropertyValue<ushort>(this, FontProperty.Version);
                //var endiannessLittle = GetFontPropertyValue<bool>(this, FontProperty.Endianness);

                //// Construct CFNT, FINF, TGLP blocks (matching signatures and constants from your parsers)
                //var cfnt = new CFNT() { Version = cfntVersion }; // If your CFNT has ctor overload, adjust accordingly
                //var finf = new FINF()
                //{
                //    HeaderSize = 0x20,
                //    FontType = 0x1,
                //    LineFeed = finfLineFeed,
                //    AltCharIndex = 0,                  // matches original hardcoded
                //    WidthEntryTemplate = widthEntries[0].Clone(), // same as original: template from first glyph
                //    Encoding = finfCharEnc,
                //    Height = finfHeight,
                //    Width = finfWidth,
                //    Ascent = finfAscent
                //};

                //var tglp = new TGLP()
                //{
                //    CellWidth = CellSize.x,
                //    CellHeight = CellSize.Y,
                //    BaselinePos = finfBaseline,
                //    MaxCharWidth = maxCharWidth,
                //    SheetDataSize = encodedSheets[0].Length,
                //    SheetCount = (ushort)encodedSheets.Count,
                //    SheetFormat = platformFmt,
                //    CellsPerRow = (ushort)cellsPerRow,
                //    CellsPerColumn = (ushort)cellsPerColumn,
                //    SheetWidth = (ushort)SheetSize.Value.X,
                //    SheetHeight = (ushort)SheetSize.Value.Y
                //};

                //// Create CWDH headers container
                //var cwdhHeaders = new List<CWDH> { new CWDH(widthEntries) };

                //// Create CMAP headers
                //var cmapHeaders = new List<CMAP>();
                //foreach (var pair in directEntries)
                //    cmapHeaders.Add(new CMAP(pair));
                //foreach (var table in tableEntries)
                //    cmapHeaders.Add(new CMAP(type: 0x1, table));
                //foreach (var scan in scanEntries)
                //    cmapHeaders.Add(new CMAP(type: 0x2, scan));

                //// Delete old file if exists
                //if (File.Exists(filename)) File.Delete(filename);

                //// Write with correct endianness
                //using (var bw = new BinaryWriterX(File.OpenWrite(filename), littleEndian: endiannessLittle))
                //{
                //    var linker = new BlockLinker();

                //    cfnt.Serialize(bw, linker);
                //    finf.Serialize(bw, linker);
                //    tglp.Serialize(bw, linker, encodedSheets, align: 0x80); // CTR alignment

                //    linker.AddLookupValue("ptrWidth", bw.BaseStream.Position);
                //    foreach (var header in cwdhHeaders)
                //        header.Serialize(bw, linker);

                //    linker.AddLookupValue("ptrMap", bw.BaseStream.Position + 0x8U);
                //    foreach (var cmap in cmapHeaders)
                //        cmap.Serialize(bw, linker);

                //    linker.AddLookupValue("fileSize", bw.BaseStream.Position);
                //    linker.MakeBlockLink(bw);
                //}

                //// Cleanup bitmaps
                //foreach (var bmp in sheetImgs)
                //    bmp?.Dispose();

                return new ActionResult(true, "OK");
            }
            catch (Exception ex)
            {
                // Best-effort cleanup
                return new ActionResult(false, ex.Message);
            }
        }

        public void RecalculateSheetInfo(int targetPixelCount)
        {
            // Total pixels required for all glyphs (including +1 spacing in both directions)
            int requiredGlyphPixels = (CellSize.Value.X + 1) * (CellSize.Value.Y + 1) * Glyphs.Count;

            // Track the best (smallest) excess pixel count found
            int bestExcessPixels = int.MaxValue;

            // Try candidate sheet sizes from 32 up to 1024, doubling each time
            for (int candidateWidth = 32; candidateWidth <= 1024; candidateWidth *= 2)
            {
                int usableWidth = candidateWidth - 2; // subtract border
                if (usableWidth < CellSize.Value.X) continue;

                for (int candidateHeight = 32; candidateHeight <= 1024; candidateHeight *= 2)
                {
                    // If targetPixelCount is nonzero, only consider exact matches
                    if (targetPixelCount != 0 && candidateWidth * candidateHeight != targetPixelCount)
                        continue;

                    int usableHeight = candidateHeight - 2;
                    if (usableHeight < CellSize.Value.Y) continue;

                    // How many glyphs fit per sheet
                    int cellsPerRow = (usableWidth + 1) / (CellSize.Value.X + 1);
                    int cellsPerColumn = (usableHeight + 1) / (CellSize.Value.Y + 1);
                    int cellsPerSheet = cellsPerRow * cellsPerColumn;

                    // Number of sheets needed
                    int sheetCount = (int)Math.Ceiling(Glyphs.Count / (double)cellsPerSheet);

                    // Excess pixels = total pixels allocated - required glyph pixels
                    int excessPixels = usableWidth * usableHeight * sheetCount - requiredGlyphPixels;

                    if (excessPixels < bestExcessPixels)
                    {
                        bestExcessPixels = excessPixels;
                        SheetSize = new Point(candidateWidth, candidateHeight);
                    }
                }
            }

            if (bestExcessPixels == int.MaxValue)
                throw new InvalidOperationException("Invalid sheet pixel configuration: no suitable sheet size found.");
        }

    }
}