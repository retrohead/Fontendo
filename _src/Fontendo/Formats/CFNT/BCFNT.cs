using Fontendo.Extensions;
using Fontendo.Extensions.BinaryTools;
using Fontendo.Interfaces;
using System.Drawing.Imaging;
using static Fontendo.Extensions.FontBase;
using static Fontendo.FontProperties.FontPropertyList;
using static Fontendo.FontProperties.GlyphProperties;
using static Fontendo.Extensions.FontBase.FontSettings;
using System.Drawing;
using System.IO;
using Fontendo.UI;
using static Fontendo.Interfaces.ITextureCodec;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.Drawing.Drawing2D;

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
        public SheetsType? Sheets { get; set; } = null;
        public List<Glyph>? Glyphs { get; set; } = null;
        public List<CharImageType>? CharImages { get; set; } = null;
        public FontBase FontBase { get; }
        public ITextureCodec Codec { get; set; } = TextureCodecFactory.Create(TextureCodecFactory.Platform.CTR);

        public BCFNT(FontBase FontBase)
        {
            this.FontBase = FontBase;
        }


        public ActionResult Load(FontBase fontbase, string filename)
        {
            ActionResult result;
            MainWindow.Log($"------------------------------------------");
            MainWindow.Log($"Loading file: {filename}");


            string fixedCorrupt = "";
            using (var br = new BinaryReaderX(File.OpenRead(filename)))
            {
                CFNT = new CFNT(br);
                FINF = new FINF(br);
                try
                {
                    br.SetEndianness();
                    br.BaseStream.Position = 0;

                    // CFNT Header
                    MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} CTR Start");
                    result = CFNT.Parse();
                    if (!result.Success)
                        return new ActionResult(false, result.Message);
                    MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} CTR End");

                    // FINF Header
                    br.BaseStream.Position = CFNT.HeaderSize;
                    MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} FINF Start");
                    result = FINF.Parse();
                    if (!result.Success)
                        return new ActionResult(false, result.Message);
                    MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} FINF End");

                    // Glyph Header
                    br.BaseStream.Position = FINF.PtrGlyph - 0x8U;

                    if (FINF.FontType == 0x0)  //FONT_TYPE_GLYPH, use CGLP
                    {
                        //CGLP is a section that served the same purpose as TGLP on the NTR/DS.
                        //All later font formats such as RFNT, CFNT and FFNT have this CGLP leftover
                        //both in the font converter and the SDK headers...maybe they planned it but
                        //it got replaced with TGLP and monochrome image formats it supports

                        MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} CGLP Start");
                        CGLP = new CGLP(br, true);
                        result = CGLP.Parse();
                        MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} CGLP End");
                        if (!result.Success)
                            return new ActionResult(false, result.Message);
                        //TODO: Find out if RVL supported CGLP fonts or not, if it did then implement it here
                        return new ActionResult(false, "You've found a CGLP RVL font! Aborting because it is not known how to handle the file...");

                    }
                    else if (FINF.FontType == 0x1)
                    {
                        MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} TGLP Start");
                        TGLP = new TGLP(br);
                        result = TGLP.Parse();
                        if (!result.Success)
                            return new ActionResult(false, result.Message);
                        MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} TGLP End");
                    }
                    else
                    {
                        return new ActionResult(false, $"Unknown font type 0x{FINF.FontType.ToString("X")}");
                    }

                    // CWDH data
                    UInt32 nextPtr = FINF.PtrWidth - 0x8;
                    MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} CWDH Start");
                    CWDH_Headers = new List<CWDH>();
                    CharWidths = new List<CharWidths>();
                    int count = 0;
                    while (nextPtr != 0x0)
                    {
                        br.BaseStream.Position = nextPtr;
                        MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} CWDH Entry {count} Start");
                        CWDH cwdh = new CWDH(br); //Read the header and the entries
                        result = cwdh.Parse();
                        MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} CWDH Entry {count} End");
                        if (!result.Success)
                            return new ActionResult(false, result.Message);
                        CharWidths.AddRange(cwdh.Entries);
                        nextPtr = cwdh.PtrNext; //Set the pointer to the next section
                        CWDH_Headers.Add(cwdh); //Append the header
                        count++;
                    }
                    MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} CWDH End");

                    // CMAP data
                    nextPtr = FINF.PtrMap;
                    CMAP_Headers = new List<CMAP>();
                    CharMaps = new List<CMAPEntry>();
                    count = 0;
                    MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} CMAPs Start");
                    while (nextPtr != 0x0)
                    {
                        br.BaseStream.Position = nextPtr - 0x8; //This is somewhat of a bodge, but it work nonetheless

                        MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} CMAP {count} Start");
                        CMAP cmap = new CMAP(br);
                        result = cmap.Parse();
                        MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} CMAP {count} End");
                        if (!result.Success)
                            return new ActionResult(false, result.Message);
                        CharMaps.AddRange(cmap.Entries);
                        nextPtr = cmap.PtrNext;
                        CMAP_Headers.Add(cmap); //Append the CMAP header to the list
                        count++;
                    }
                    MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} CMAP End");

                    // Decoding textures
                    br.BaseStream.Position = TGLP.SheetPtr;
                    if (Sheets?.Images != null)
                    {
                        foreach (var bmp in Sheets.Images)
                        {
                            bmp.Dispose();
                        }
                        Sheets.Images.Clear();
                    }
                    Sheets = new SheetsType(TGLP.SheetWidth, TGLP.SheetHeight);
                    Sheets.Images = new List<Bitmap>();
                    MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} Sheets start");
                    for (ushort i = 0; i < TGLP.SheetCount; i++)
                    {
                        int decodeWidth = (TGLP.SheetWidth + 7) & ~7;  // round up to multiple of 8
                        int decodeHeight = (TGLP.SheetHeight + 7) & ~7;  // round up to multiple of 8
                        DecodedTextureType sheetRaw = Codec.DecodeTexture(TGLP.SheetFormat, br, TGLP.SheetWidth, TGLP.SheetHeight);
                        Sheets.Images.Add(CreateBitmapFromBytes(sheetRaw.data, TGLP.SheetWidth, TGLP.SheetHeight, PixelFormat.Format32bppArgb));

                        if (sheetRaw.mask != null)
                        { 
                            Sheets.MaskImages.Add(CreateBitmapFromBytes(sheetRaw.mask, TGLP.SheetWidth, TGLP.SheetHeight, PixelFormat.Format32bppArgb));
                        } else
                        {
                            Sheets.MaskImages.Add(null);
                        }

                    }
                    MainWindow.Log($"0x{br.BaseStream.Position.ToString("X8")} Sheets End");

                    //Pull sheets apart into individual glyph images
                    if (CharImages != null)
                    {
                        foreach (var img in CharImages)
                        {
                            img.Image.Dispose();
                        }
                        CharImages.Clear();
                    }
                    CharImages = new List<CharImageType>();

                    List<Bitmap?> maskImages = new List<Bitmap?>();
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
                        Bitmap bmp = Sheets.Images[currentSheet].Clone(rect, Sheets.Images[currentSheet].PixelFormat);
                        Bitmap? maskbmp = null;
                        if(Sheets.MaskImages[currentSheet] != null)
                            maskbmp = Sheets.MaskImages[currentSheet]!.Clone(rect, Sheets.MaskImages[currentSheet]!.PixelFormat);
                        maskImages.Add(maskbmp);
                        CharImageType newImg = new CharImageType(i, currentSheet, bmp);
                        // Append Bitmap to list
                        CharImages.Add(newImg);
                    }
                    // sort CMAP entries
                    CharMaps.Sort((a, b) => a.Index.CompareTo(b.Index));

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
                        glyph.MaskImage = maskImages[i];
                        Glyphs.Add(glyph);
                    }
                    // try to fix corrupted fonts with too many char widths from NintyFont
                    if (CharMaps.Count() == Glyphs.Count() && CharWidths.Count() > CharMaps.Count())
                    {
                        int excess = CharWidths.Count() - CharMaps.Count();
                        while (CharWidths.Count() > CharMaps.Count())
                            CharWidths.RemoveAt(0);
                        fixedCorrupt = $"{excess} corrupted char widths were found and removed from the font so you can hopefully recover it";
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


                    FontBase.Settings.Endianness = br.GetEndianness();
                    FontBase.Settings.CharEncoding = (CharEncodings)FINF.Encoding;
                    FontBase.Settings.LineFeed = FINF.LineFeed;
                    FontBase.Settings.Height = FINF.Height;
                    FontBase.Settings.Width = FINF.Width;
                    FontBase.Settings.Ascent = FINF.Ascent;
                    FontBase.Settings.Baseline = TGLP.BaselinePos;
                    FontBase.Settings.Version = CFNT.Version;
                    FontBase.Settings.NtrRvlImageFormat = (ImageFormats)Codec.ConvertPlatformTextureTypeToGeneral(TGLP.SheetFormat);
                    FontBase.Settings.Sheets = Sheets;
                    FontBase.Settings.Glyphs = Glyphs;
                }
                catch (Exception e)
                {
                    Dispose();
                    return new ActionResult(false, e.Message);
                }
                br.Close();
                br.Dispose();
            }
            return new ActionResult(true, fixedCorrupt == "" ? "OK" : fixedCorrupt);
        }

        /// <summary>
        /// DO NOT FORGET TO DISPOSE THE BITMAPS LATER
        /// creates a new image for each sheet and draws all glyphs onto their respective sheets
        /// </summary>
        private List<Bitmap> CreateAllSheets()
        {
            List<Bitmap> sheetImgs = new List<Bitmap>();

            // Create empty sheets
            for (var i = 0; i < TGLP.SheetCount; i++)
            {
                var bmp = new Bitmap(
                    SheetSize.Value.X,
                    SheetSize.Value.Y,
                    PixelFormat.Format32bppArgb);

                using (var gfx = Graphics.FromImage(bmp))
                    gfx.Clear(Color.Transparent);

                sheetImgs.Add(bmp);
            }

            // Fill sheets with glyphs
            foreach (var g in Glyphs)
            {
                int index = g.Settings.Index;

                int currentSheet = index / (TGLP.CellsPerRow * TGLP.CellsPerColumn);
                if (currentSheet >= sheetImgs.Count)
                    throw new Exception("calculated sheet index exceeds created sheets count");

                int i2 = index - (currentSheet * TGLP.CellsPerRow * TGLP.CellsPerColumn);
                int currentRow = i2 / TGLP.CellsPerRow;
                int currentColumn = i2 - (currentRow * TGLP.CellsPerRow);

                int startX = (int)(currentColumn * (CellSize.Value.X + 1));
                int startY = (int)(currentRow * (CellSize.Value.Y + 1));

                // Exact ARGB copy — preserves 00FFFFFF perfectly
                BlitGlyphExact(
                    sheetImgs[currentSheet],
                    g.Settings.Image,
                    startX,
                    startY);
            }

            return sheetImgs;
        }


        private static void BlitGlyphExact(
            Bitmap sheet,
            Bitmap glyph,
            int destX,
            int destY)
        {
            if (sheet.PixelFormat != PixelFormat.Format32bppArgb ||
                glyph.PixelFormat != PixelFormat.Format32bppArgb)
                throw new InvalidOperationException("Expected Format32bppArgb for exact blit.");

            var rectSheet = new Rectangle(0, 0, sheet.Width, sheet.Height);
            var rectGlyph = new Rectangle(0, 0, glyph.Width, glyph.Height);

            var sheetData = sheet.LockBits(rectSheet, ImageLockMode.WriteOnly, sheet.PixelFormat);
            var glyphData = glyph.LockBits(rectGlyph, ImageLockMode.ReadOnly, glyph.PixelFormat);

            try
            {
                int bytesPerPixel = 4;

                for (int y = 0; y < glyph.Height; y++)
                {
                    int srcOffset = y * glyphData.Stride;
                    int dstOffset = (destY + y) * sheetData.Stride + destX * bytesPerPixel;

                    // Copy one row from glyph to sheet
                    byte[] row = new byte[glyph.Width * bytesPerPixel];
                    System.Runtime.InteropServices.Marshal.Copy(
                        glyphData.Scan0 + srcOffset,
                        row,
                        0,
                        row.Length);

                    System.Runtime.InteropServices.Marshal.Copy(
                        row,
                        0,
                        sheetData.Scan0 + dstOffset,
                        row.Length);
                }
            }
            finally
            {
                sheet.UnlockBits(sheetData);
                glyph.UnlockBits(glyphData);
            }
        }

        public void RecreateSheetFromGlyphs(int i)
        {
            var bmp = new Bitmap(SheetSize.Value.X, SheetSize.Value.Y, PixelFormat.Format32bppArgb);
            // Clear to transparent first
            using (var gfx = Graphics.FromImage(bmp))
            {
                gfx.Clear(Color.Transparent);
            }
            foreach (var g in Glyphs)
            {
                var index = g.Settings.Index;
                var currentSheet = index / (TGLP.CellsPerRow * TGLP.CellsPerColumn);
                if (currentSheet != i) continue;
                var i2 = index - (currentSheet * TGLP.CellsPerRow * TGLP.CellsPerColumn);
                var currentRow = i2 / TGLP.CellsPerRow;
                var currentColumn = i2 - (currentRow * TGLP.CellsPerRow);
                var startX = (int)(currentColumn * (CellSize.Value.X + 1));
                var startY = (int)(currentRow * (CellSize.Value.Y + 1));
                // Draw glyph image onto the sheet
                BlitGlyphExact(
                    bmp,
                    g.Settings.Image,
                    startX,
                    startY);
            }
            // Replace old sheet image
            Sheets!.Images[i].Dispose();
            Sheets.Images[i] = bmp;

            if (Sheets!.MaskImages[i] != null)
            {
                Sheets.MaskImages[i]!.Dispose();
            }
            Bitmap? mask = FontBase.GenerateTransparencyMask(bmp);
            Sheets.MaskImages[i] = mask == null ? null : mask;
        }


        public void RecreateGlyphsFromSheet(int sheetNum)
        {
            var charImages = CharImages.FindAll(c => c.Sheet.Equals(sheetNum));
            for (int i = 0; i < charImages.Count(); i++) //Get glyph count from charMaps, cause there's no better way to do that
            {
                //Do some math to figure out where we should start reading the pixels
                int i2 = CharImages.IndexOf(charImages[i]) - (sheetNum * TGLP.CellsPerRow * TGLP.CellsPerColumn);
                int currentRow = i2 / TGLP.CellsPerRow;
                int currentColumn = i2 - (currentRow * TGLP.CellsPerRow);
                int startX = currentColumn * (TGLP.CellWidth + 1);
                int startY = currentRow * (TGLP.CellHeight + 1);
                // Copy pixels from the sheet into a new Bitmap

                Rectangle rect = new Rectangle(startX, startY, TGLP.CellWidth + 1, TGLP.CellHeight + 1);
                Bitmap bmp = Sheets.Images[sheetNum].Clone(rect, Sheets.Images[sheetNum].PixelFormat);
                // replace the bmp
                charImages[i].Image.Dispose();
                charImages[i].Image = bmp;
                if (Glyphs![charImages[i].Index].Settings.Image != null)
                    Glyphs[charImages[i].Index].Settings.Image.Dispose();
                if (Glyphs[charImages[i].Index].MaskImage != null)
                    Glyphs[charImages[i].Index].MaskImage!.Dispose();
                Glyphs[charImages[i].Index].Settings.Image = bmp;

                Bitmap? mask = FontBase.GenerateTransparencyMask(Glyphs[charImages[i].Index].Settings.Image);
                Glyphs[charImages[i].Index].MaskImage = mask == null ? null : mask;
            }
        }

        public ActionResult Save(string filename)
        {
            MainWindow.Log($"------------------------------------------");
            MainWindow.Log($"Saving file: {filename}");
            try
            {
                if (CharMaps.Count() != CharWidths.Count())
                    return new ActionResult(false, "character maps count does not match character widths count");

                // Prepare glyphs for export: sort by code point, set indices, track max char width
                var encodedGlyphs = new LinkedList<Glyph>(Glyphs.OrderBy((a) =>
                {
                    return a.Settings.CodePoint;
                }).ToList());

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

                List<Bitmap> sheetImgs = CreateAllSheets();


                // Encode sheets to platform texture format
                // Image format taken from FontProperties (NtrRvlImageFormat), converted to platform
                var generalFmt = FontBase.Settings.NtrRvlImageFormat;
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
                List<(Glyph First, Glyph Last)> directEntries = CMAP.CreateDirectEntries(ref encodedGlyphs);
                List<List<CMAPEntry>> tableEntries = CMAP.CreateTableEntries(ref encodedGlyphs);
                List<List<CMAPEntry>> scanEntries = CMAP.CreateScanEntries(encodedGlyphs);

                // Create headers from current property values
                // FINF expects many fields from FontProperties and current state
                var finfLineFeed = FontBase.Settings.LineFeed;
                var finfCharEnc = FontBase.Settings.CharEncoding;
                var finfHeight = FontBase.Settings.Height;
                var finfWidth = FontBase.Settings.Width;
                var finfAscent = FontBase.Settings.Ascent;
                var finfBaseline = FontBase.Settings.Baseline;
                var cfntVersion = FontBase.Settings.Version;
                var endiannessLittle = FontBase.Settings.Endianness;

                // Construct CFNT, FINF, TGLP blocks (matching signatures and constants from your parsers)
                var cfnt = new CFNT(); // If your CFNT has ctor overload, adjust accordingly
                var finf = new FINF(
                    0x20,
                    0x1,
                    finfLineFeed,
                    0x00 /*Hardcoded altCharIndex to 0*/,
                    widthEntries[0],       // same as original: template from first glyph
                    ((byte)finfCharEnc),
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
                    (ushort)TGLP.CellsPerRow,
                    (ushort)TGLP.CellsPerColumn,
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
                using (var bw = new BinaryWriterX(filename, endiannessLittle == Endianness.Endian.Little))
                {
                    var linker = new BlockLinker();

                    MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} CTR Start");
                    cfnt.Serialize(bw, linker);
                    MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} CTR End");

                    MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} FINF Start");
                    finf.Serialize(bw, linker);
                    MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} FINF End");

                    MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} TGLP Start");
                    tglp.Serialize(bw, linker, encodedSheets, align: 0x80); // CTR alignment

                    MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} CWHD Start");
                    linker.AddLookupValue(FontPointerType.ptrWidth, bw.BaseStream.Position);
                    int count = 0;
                    foreach (var header in cwdhHeaders)
                    {
                        MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} CWHD {count} Start");
                        header.Serialize(bw, linker);
                        MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} CWHD {count} End");
                        count++;
                    }
                    MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} CWHD End");

                    MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} CMAP Start");
                    linker.AddLookupValue(FontPointerType.ptrMap, bw.BaseStream.Position + 0x8U);
                    count = 0;
                    foreach (var cmap in cmapHeaders)
                    {
                        MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} CMAP {count} Start");
                        cmap.Serialize(bw, linker);
                        MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} CMAP {count} End");
                        count++;
                    }
                    MainWindow.Log($"0x{bw.BaseStream.Position.ToString("X8")} CMAP End");
                    linker.AddLookupValue(FontPointerType.fileSize, bw.BaseStream.Position);

                    linker.MakeBlockLink(bw);
                }

                //// Cleanup bitmaps
                foreach (var bmp in sheetImgs)
                    bmp?.Dispose();

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
                    if(bmp != null)
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