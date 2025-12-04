using Fontendo.Extensions;
using Fontendo.Extensions.BinaryTools;
using Fontendo.Interfaces;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using static Fontendo.Extensions.FontBase;
using static Fontendo.FontProperties.PropertyList;
using static Fontendo.FontProperties.FontPropertyList;
using static Fontendo.FontProperties.GlyphProperties;

namespace Fontendo.Formats.CTR
{
    public class BCFNT : IFontendoFont, IDisposable
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
        public FontPropertyRegistry Properties { get; set; }

        public BCFNT()
        {
            Properties = new FontPropertyRegistry();
            Properties.AddProperty(FontProperty.Endianness, "Endianness", PropertyValueType.Bool, EditorType.EndiannessPicker);
            Properties.AddProperty(FontProperty.CharEncoding, "Char encoding", PropertyValueType.CharEncoding, EditorType.Label);
            Properties.AddProperty(FontProperty.LineFeed, "Line feed", PropertyValueType.Byte, EditorType.NumberBox, (0, 0xFF));
            Properties.AddProperty(FontProperty.Height, "Height", PropertyValueType.Byte, EditorType.NumberBox, (0, 0xFF));
            Properties.AddProperty(FontProperty.Width, "Width", PropertyValueType.Byte, EditorType.NumberBox, (0, 0xFF));
            Properties.AddProperty(FontProperty.Ascent, "Ascent", PropertyValueType.Byte, EditorType.NumberBox, (0, 0xFF));
            Properties.AddProperty(FontProperty.Baseline, "Baseline", PropertyValueType.Byte, EditorType.NumberBox, (0, 0xFF));
            Properties.AddProperty(FontProperty.Version, "Version", PropertyValueType.UInt32, EditorType.Label, (0, 0xFFFFFFFF));
            Properties.AddProperty(FontProperty.NtrRvlImageFormat, "Image encoding", PropertyValueType.ImageFormat, EditorType.Label);
        }

        public ITextureCodec Codec { get; set; } = TextureCodecFactory.Create(TextureCodecFactory.Platform.CTR);

        public ActionResult Load(string filename)
        {
            ActionResult result;
            MainForm.Log($"------------------------------------------");
            MainForm.Log($"Loading file: {filename}");

            using (var br = new BinaryReaderX(File.OpenRead(filename)))
            {
                CFNT = new CFNT(br);
                FINF = new FINF(br);
                try
                {
                    br.SetEndianness();
                    br.BaseStream.Position = 0;

                    // CFNT Header
                    MainForm.Log($"CTR Start: {br.BaseStream.Position}");
                    result = CFNT.Parse();
                    if (!result.Success)
                        return new ActionResult(false, result.Message);
                    MainForm.Log($"CTR End: {br.BaseStream.Position}");

                    // FINF Header
                    br.BaseStream.Position = CFNT.HeaderSize;
                    MainForm.Log($"FINF Start: {br.BaseStream.Position}");
                    result = FINF.Parse();
                    if (!result.Success)
                        return new ActionResult(false, result.Message);
                    MainForm.Log($"FINF End: {br.BaseStream.Position}");

                    // Glyph Header
                    br.BaseStream.Position = FINF.PtrGlyph - 0x8U;

                    if (FINF.FontType == 0x0)  //FONT_TYPE_GLYPH, use CGLP
                    {
                        //CGLP is a section that served the same purpose as TGLP on the NTR/DS.
                        //All later font formats such as RFNT, CFNT and FFNT have this CGLP leftover
                        //both in the font converter and the SDK headers...maybe they planned it but
                        //it got replaced with TGLP and monochrome image formats it supports

                        MainForm.Log($"CGLP Start: {br.BaseStream.Position}");
                        CGLP = new CGLP(br, true);
                        result = CGLP.Parse();
                        MainForm.Log($"CGLP End: {br.BaseStream.Position}");
                        if (!result.Success)
                            return new ActionResult(false, result.Message);
                        //TODO: Find out if RVL supported CGLP fonts or not, if it did then implement it here
                        return new ActionResult(false, "You've found a CGLP RVL font! Aborting because it is not known how to handle the file...");

                    }
                    else if (FINF.FontType == 0x1)
                    {
                        MainForm.Log($"TGLP Start: {br.BaseStream.Position}");
                        TGLP = new TGLP(br);
                        result = TGLP.Parse();
                        if (!result.Success)
                            return new ActionResult(false, result.Message);
                        MainForm.Log($"TGLP End: {br.BaseStream.Position}");
                    }
                    else
                    {
                        return new ActionResult(false, $"Unknown font type 0x{FINF.FontType.ToString("X")}");
                    }

                    // CWDH data
                    UInt32 nextPtr = FINF.PtrWidth - 0x8;
                    MainForm.Log($"CWDH Start: {br.BaseStream.Position}");
                    CWDH_Headers = new List<CWDH>();
                    CharWidths = new List<CharWidths>();
                    int count = 0;
                    while (nextPtr != 0x0)
                    {
                        br.BaseStream.Position = nextPtr;
                        MainForm.Log($"CWDH {count} Start: {br.BaseStream.Position}");
                        CWDH cwdh = new CWDH(br); //Read the header and the entries
                        result = cwdh.Parse();
                        MainForm.Log($"CWDH {count} End: {br.BaseStream.Position}");
                        if (!result.Success)
                            return new ActionResult(false, result.Message);
                        CharWidths.AddRange(cwdh.Entries);
                        nextPtr = cwdh.PtrNext; //Set the pointer to the next section
                        CWDH_Headers.Add(cwdh); //Append the header
                        count++;
                    }
                    MainForm.Log($"CWDH End: {br.BaseStream.Position}");

                    // CMAP data
                    nextPtr = FINF.PtrMap;
                    CMAP_Headers = new List<CMAP>();
                    CharMaps = new List<CMAPEntry>();
                    MainForm.Log($"CMAP Start: {br.BaseStream.Position} (but go back 0x8 ?)");
                    count = 0;
                    while (nextPtr != 0x0)
                    {
                        br.BaseStream.Position = nextPtr - 0x8; //This is somewhat of a bodge, but it work nonetheless

                        MainForm.Log($"CMAP {count} Start: {br.BaseStream.Position}");
                        CMAP cmap = new CMAP(br);
                        result = cmap.Parse();
                        MainForm.Log($"CMAP {count} End: {br.BaseStream.Position}");
                        if (!result.Success)
                            return new ActionResult(false, result.Message);
                        CharMaps.AddRange(cmap.Entries);
                        nextPtr = cmap.PtrNext;
                        CMAP_Headers.Add(cmap); //Append the CMAP header to the list
                        count++;
                    }
                    MainForm.Log($"CMAP End: {br.BaseStream.Position}");

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
                    MainForm.Log($"Sheets start: {br.BaseStream.Position}");
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
                    MainForm.Log($"Sheets End: {br.BaseStream.Position}");

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
                    }
                    // sort CMAP entries
                    CharMaps.Sort((a, b) => a.Index.CompareTo(b.Index));


                    if (CharMaps.Count() != CharWidths.Count())
                        return new ActionResult(false, $"character maps count '{CharMaps.Count()}' does not match character widths count '{CharWidths.Count()}'");


                    // put all glyphs into data
                    Glyphs = new List<Glyph>();
                    for (int i = 0; i < CharMaps.Count(); i++)
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

                    if (CharMaps.Count() != Glyphs.Count())
                        return new ActionResult(false, "character maps count does not match glyph count");
                    if (CharWidths.Count() != Glyphs.Count())
                        return new ActionResult(false, "character widths count does not match glyph count");

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

                    // Assign the range to the descriptors
                    foreach(var g in Glyphs)
                        g.Settings.UpdateValueRange(GlyphProperty.Code, range);

                    // fill in some other stuff
                    CellSize = new Point(TGLP.CellWidth, TGLP.CellHeight);
                    SheetSize = new Point(TGLP.SheetWidth, TGLP.SheetHeight);

                    Properties.SetValue(FontProperty.Endianness, br.GetEndianness() == Endianness.Endian.Little);
                    Properties.SetValue(FontProperty.CharEncoding, (CharEncodings)FINF.Encoding);
                    Properties.SetValue(FontProperty.LineFeed, FINF.LineFeed);
                    Properties.SetValue(FontProperty.Height, FINF.Height);
                    Properties.SetValue(FontProperty.Width, FINF.Width);
                    Properties.SetValue(FontProperty.Ascent, FINF.Ascent);
                    Properties.SetValue(FontProperty.Baseline, TGLP.BaselinePos);
                    Properties.SetValue(FontProperty.Version, CFNT.Version);
                    Properties.SetValue(FontProperty.NtrRvlImageFormat, (ImageFormats)Codec.ConvertPlatformTextureTypeToGeneral(TGLP.SheetFormat));

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
                    return new ActionResult(false, e.Message);
                }
                br.Close();
                br.Dispose();
            }
            return new ActionResult(true, "OK");
        }

        public ActionResult Save(string filename)
        {
            MainForm.Log($"------------------------------------------");
            MainForm.Log($"Saving file: {filename}");
            try
            {
                if (CharMaps.Count() != CharWidths.Count())
                    return new ActionResult(false, "character maps count does not match character widths count");

                // Prepare glyphs for export: sort by code point, set indices, track max char width
                var encodedGlyphs = new List<Glyph>(Glyphs);
                encodedGlyphs.Sort((a, b) =>
                {
                    // Assuming helper reads value from glyph props
                    var codeA = a.Settings.GetValue<UInt16>(GlyphProperty.Code);
                    var codeB = b.Settings.GetValue<UInt16>(GlyphProperty.Code);
                    return codeA.CompareTo(codeB);
                });

                //Set glyph indexes, while also looking for the widest glyph
                byte maxCharWidth = 0;
                ushort index = 0;
                foreach (var g in encodedGlyphs)
                {
                    g.Settings.Index = index;
                    var cw = g.Settings.GetValue<byte>(GlyphProperty.CharWidth);
                    if (cw > maxCharWidth) maxCharWidth = cw;
                    index++;
                }

                // Recalculate sheet geometry (equivalent to recalculateSheetInfo)

                RecalculateSheetInfo();

                // Stitch glyph bitmaps into sheet images
                var cellsPerRow = (uint)(SheetSize.Value.X / (CellSize.Value.X + 1));
                var cellsPerColumn = (uint)(SheetSize.Value.Y / (CellSize.Value.Y + 1));
                var cellsPerSheet = cellsPerRow * cellsPerColumn;
                var numSheets = (uint)Math.Ceiling((double)encodedGlyphs.Count / cellsPerSheet);

                // create empty sheet bmps for writing on
                List<Bitmap> sheetImgs = new List<Bitmap>();
                for (var i = 0; i < numSheets; i++)
                {
                    var bmp = new Bitmap(SheetSize.Value.X, SheetSize.Value.Y, PixelFormat.Format32bppArgb);
                    using (var gfx = Graphics.FromImage(bmp)) gfx.Clear(Color.Transparent);
                    sheetImgs.Add(bmp);
                }

                foreach (var g in Glyphs)
                {
                    var i = g.Settings.GetValue<ushort>(GlyphProperty.Index);
                    var currentSheet = i / (cellsPerRow * cellsPerColumn);
                    var i2 = i - (currentSheet * cellsPerRow * cellsPerColumn);
                    var currentRow = i2 / cellsPerRow;
                    var currentColumn = i2 - (currentRow * cellsPerRow);
                    var startX = (int)(currentColumn * (CellSize.Value.X + 1));
                    var startY = (int)(currentRow * (CellSize.Value.Y + 1));
                    if(currentSheet >= sheetImgs.Count)
                    {
                        throw new Exception("calculated sheet index exceeds created sheets count");
                    }

                    // Draw glyph image onto the sheet
                    using (var gfx = Graphics.FromImage(sheetImgs[(int)currentSheet]))
                    {
                        gfx.DrawImage(g.Settings.Image, new Rectangle(startX, startY, g.Settings.Image.Width, g.Settings.Image.Height));
                    }
                }
                sheetImgs[0].Save("C:\\Users\\kebud\\source\\repos\\Izuto\\sample_files\\sheet0.png");
                // Encode sheets to platform texture format
                // Image format taken from FontProperties (NtrRvlImageFormat), converted to platform
                var generalFmt = Properties.GetValue<ImageFormats>(FontProperty.NtrRvlImageFormat);
                var platformFmt = Codec.ConvertGeneralTextureTypeToPlatform(generalFmt);
                var encodedSheets = new List<byte[]>(sheetImgs.Count());
                for (int iSheet = 0; iSheet < sheetImgs.Count(); iSheet++)
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

                // Build CMAP entries: Direct, Table, Scan
                List<(Glyph First, Glyph Last)> directEntries = CMAP.CreateDirectEntries(encodedGlyphs);
                List<List<CMAPEntry>> tableEntries = CMAP.CreateTableEntries(encodedGlyphs);
                List<List<CMAPEntry>> scanEntries = CMAP.CreateScanEntries(encodedGlyphs);

                // Create headers from current property values
                // FINF expects many fields from FontProperties and current state
                var finfLineFeed = Properties.GetValue<byte>(FontProperty.LineFeed);
                var finfCharEnc = Properties.GetValue<byte>(FontProperty.CharEncoding);
                var finfHeight = Properties.GetValue<byte>(FontProperty.Height);
                var finfWidth = Properties.GetValue<byte>(FontProperty.Width);
                var finfAscent = Properties.GetValue<byte>(FontProperty.Ascent);
                var finfBaseline = Properties.GetValue<byte>(FontProperty.Baseline);
                var cfntVersion = Properties.GetValue<UInt32>(FontProperty.Version);
                var endiannessLittle = Properties.GetValue<bool>(FontProperty.Endianness);

                // Construct CFNT, FINF, TGLP blocks (matching signatures and constants from your parsers)
                var cfnt = new CFNT(); // If your CFNT has ctor overload, adjust accordingly
                var finf = new FINF(
                    0x20,
                    0x1,
                    finfLineFeed,
                    0x1F,                  // matches original hardcoded
                    widthEntries[0],       // same as original: template from first glyph
                    finfCharEnc,
                    finfHeight,
                    finfWidth,
                    finfAscent,
                    0x464E4946U
                    );

                var tglp = new TGLP(
                    (byte)CellSize.Value.X,
                    (byte)CellSize.Value.Y,
                    finfBaseline,
                    maxCharWidth,
                    (uint)encodedSheets[0].Length,
                    (ushort)encodedSheets.Count,
                    platformFmt,
                    (ushort)cellsPerRow,
                    (ushort)cellsPerColumn,
                    (ushort)SheetSize.Value.X,
                    (ushort)SheetSize.Value.Y,
                    0x504C4754U
                );
                if (CharMaps.Count() != widthEntries.Count())
                    return new ActionResult(false, "character maps count does not match character widths count");
                // Create CWDH headers container
                List<CWDH> cwdhHeaders = new List<CWDH> { new CWDH(widthEntries, 0x48445743U) };
                
                // Create CMAP
                var cmapHeaders = new List<CMAP>();
                for(var i = 0; i < directEntries.Count(); i++)
                    cmapHeaders.Add(new CMAP(directEntries[i], 0x50414D43U));
                for (var i = 0; i < tableEntries.Count(); i++)
                    cmapHeaders.Add(new CMAP(0x1, tableEntries[i], 0x50414D43U));
                for (var i = 0; i < scanEntries.Count(); i++)
                    cmapHeaders.Add(new CMAP(0x2, scanEntries[i], 0x50414D43U));
                // sort headers
                cmapHeaders = cmapHeaders.OrderBy(h => h.MappingMethod)
                            .ThenBy(h => h.CodeBegin)
                            .ToList();
                // Delete old file if exists
                if (File.Exists(filename)) File.Delete(filename);

                // Write with correct endianness
                using (var bw = new BinaryWriterX(filename, endiannessLittle))
                {
                    var linker = new BlockLinker();

                    MainForm.Log($"CTR Start: {bw.BaseStream.Position}");
                    cfnt.Serialize(bw, linker);
                    MainForm.Log($"CTR End: {bw.BaseStream.Position}");

                    MainForm.Log($"FINF Start: {bw.BaseStream.Position}");
                    finf.Serialize(bw, linker);
                    MainForm.Log($"FINF End: {bw.BaseStream.Position}");

                    MainForm.Log($"TGLP Start: {bw.BaseStream.Position}");
                    tglp.Serialize(bw, linker, encodedSheets, align: 0x80); // CTR alignment

                    MainForm.Log($"CWHD Start: {bw.BaseStream.Position}");
                    linker.AddLookupValue("ptrWidth", bw.BaseStream.Position);
                    int count = 0;
                    foreach (var header in cwdhHeaders)
                    {
                        MainForm.Log($"CWHD {count} Start: {bw.BaseStream.Position}");
                        header.Serialize(bw, linker);
                        MainForm.Log($"CWHD {count} End: {bw.BaseStream.Position}");
                        count++;
                    }
                    MainForm.Log($"CWHD End: {bw.BaseStream.Position}");

                    MainForm.Log($"CMAP Start: {bw.BaseStream.Position}");
                    linker.AddLookupValue("ptrMap", bw.BaseStream.Position + 0x8U);
                    count = 0;
                    foreach (var cmap in cmapHeaders)
                    {
                        MainForm.Log($"CMAP {count} Start: {bw.BaseStream.Position}");
                        cmap.Serialize(bw, linker);
                        MainForm.Log($"CMAP {count} End: {bw.BaseStream.Position}");
                        count++;
                    }
                    MainForm.Log($"CMAP End: {bw.BaseStream.Position}");

                    linker.MakeBlockLink(bw);
                }

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

        public void RecalculateSheetInfo(int targetPixelCount = 0)
        {
            // Total pixels required for all glyphs (including +1 spacing in both directions)
            int requiredGlyphPixels = (CellSize.Value.X + 1) * (CellSize.Value.Y + 1) * Glyphs.Count();

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
            if (Sheets != null && Sheets.Items != null)
            {
                foreach (var bmp in Sheets.Items)
                {
                    bmp.Dispose();
                }
                Sheets.Items.Clear();
            }
            if (Glyphs != null)
            {
                foreach (var glyph in Glyphs)
                {
                    if(glyph.Settings.Image != null)
                        glyph.Settings.Image.Dispose();
                }
                Glyphs.Clear();
            }
        }
    }
}