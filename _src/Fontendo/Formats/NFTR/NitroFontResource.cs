using Fontendo.Extensions;
using Fontendo.Extensions.BinaryTools;
using Fontendo.Formats.CTR;
using Fontendo.Interfaces;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
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
                    BitReader bitReader = new BitReader(br);
                    for (int i = 0; i < CharMaps.Count(); i++)
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
                        bitReader.Update();
                        DecodedTextureType tex = Codec.DecodeBitmap(CGLP.BytesPerPixel, bitReader, CGLP.CellWidth, CGLP.CellHeight);
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

                    // Stitch glyphs together into small sheets for easier loading in UI, max 128 x 128
                    int sheetWidth = 128 - (128 % CGLP.CellWidth);
                    int sheetHeight = 128 - (128 % CGLP.CellHeight);

                    int cellsPerRow = sheetWidth / CGLP.CellWidth;
                    int cellsPerCol = sheetHeight / CGLP.CellHeight;
                    int cellsPerSheet = cellsPerRow * cellsPerCol;

                    // Create the first sheet
                    Bitmap sheetBitmap = new Bitmap(sheetWidth, sheetHeight);
                    using (var gfx = Graphics.FromImage(sheetBitmap))
                        gfx.Clear(Color.Transparent);

                    int glyphIndexInSheet = 0;
                    Sheets = new SheetsType(sheetWidth, sheetHeight);
                    for (int i = 0; i < Glyphs.Count; i++)
                    {
                        // If the current sheet is full, finalize it and start a new one
                        if (glyphIndexInSheet >= cellsPerSheet)
                        {
                            Sheets.Images.Add(sheetBitmap);
                            Sheets.MaskImages.Add(null);

                            sheetBitmap = new Bitmap(sheetWidth, sheetHeight);
                            using (var gfx = Graphics.FromImage(sheetBitmap))
                                gfx.Clear(Color.Transparent);

                            glyphIndexInSheet = 0;
                        }

                        Glyph glyph = Glyphs[i];
                        glyph.Sheet = Sheets.Images.Count(); // set the sheet index

                        int x = (glyphIndexInSheet % cellsPerRow) * CGLP.CellWidth;
                        int y = (glyphIndexInSheet / cellsPerRow) * CGLP.CellHeight;

                        BlitBitmapExact(sheetBitmap, glyph.Settings.Image, x, y);

                        glyphIndexInSheet++;
                    }

                    // Add the final sheet
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

                    if (NFTR.Version <= 0x1000)
                    {
                        // hide some controls for older versions
                        FontBase.Settings.UpdatePreferredControl(FontProperty.Height, EditorType.None);
                        FontBase.Settings.UpdatePreferredControl(FontProperty.Width, EditorType.None);
                        FontBase.Settings.UpdatePreferredControl(FontProperty.Ascent, EditorType.None);
                    }

                    FontBase.Settings.NtrGameFreak = isPocketMonstersFont;
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

        public void RecreateGlyphsFromSheet(int sheetNum)
        {
            int sheetWidth = 128 - (128 % CGLP!.CellWidth);
            int sheetHeight = 128 - (128 % CGLP.CellHeight);

            int cellsPerRow = sheetWidth / CGLP.CellWidth;
            int cellsPerCol = sheetHeight / CGLP.CellHeight;

            // Get glyphs for this sheet IN ORDER
            var sheetGlyphs = Glyphs!
                .Where(g => g.Sheet == sheetNum)
                .OrderBy(g => g.Index)   // ensures stable ordering
                .ToList();

            for (int index = 0; index < sheetGlyphs.Count; index++)
            {
                int x = (index % cellsPerRow) * CGLP.CellWidth;
                int y = (index / cellsPerRow) * CGLP.CellHeight;

                Rectangle rect = new Rectangle(x, y, CGLP.CellWidth, CGLP.CellHeight);

                Bitmap bmp = Sheets!.Images[sheetNum].Clone(rect, Sheets.Images[sheetNum].PixelFormat);

                // Replace glyph image
                var glyph = sheetGlyphs[index];
                glyph.Settings.Image?.Dispose();
                glyph.Settings.Image = bmp;

                // Update CharImages too
                var ci = CharImages!.First(c => c.Index == glyph.Index);
                ci.Image?.Dispose();
                ci.Image = bmp;
            }
        }


        public void RecreateSheetFromGlyphs(int i)
        {
            int sheetWidth = 128 - (128 % CGLP.CellWidth);
            int sheetHeight = 128 - (128 % CGLP.CellHeight);

            int cellsPerRow = sheetWidth / CGLP.CellWidth;
            int cellsPerCol = sheetHeight / CGLP.CellHeight;
            int cellsPerSheet = cellsPerRow * cellsPerCol;
            var bmp = new Bitmap(sheetWidth, sheetHeight, PixelFormat.Format32bppArgb);
            // Clear to transparent first
            using (var gfx = Graphics.FromImage(bmp))
            {
                gfx.Clear(Color.Transparent);
            }
            var sheetGlyphs = Glyphs!.FindAll(g => g.Sheet == i);
            for (int index = 0; index < sheetGlyphs.Count; index++)
            {

                int x = (index % cellsPerRow) * CGLP.CellWidth;
                int y = (index / cellsPerRow) * CGLP.CellHeight;

                BlitBitmapExact(bmp, sheetGlyphs[index].Settings.Image, x, y);
            }
            // Replace old sheet image
            Sheets!.Images[i].Dispose();
            Sheets.Images[i] = bmp;
        }

        public ActionResult Save(string filename)
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
            bool isGF = FontBase.Settings.NtrGameFreak;

            double bits = CellSize!.Value.X * CellSize.Value.Y * FontBase.Settings.NtrBpp;
            int cellBytes = (int)Math.Ceiling(bits / 8);

            if (isGF == true)
                cellBytes += 3;

            List<CharWidths> widthEntries = Formats.CharWidths.CreateWidthEntries(encodedGlyphs);

            //Encode the images
            List<byte[]> sheetImgs = new List<byte[]>();
            foreach (var g in encodedGlyphs)
            {
                byte[] cell = new byte[cellBytes];
                sheetImgs.Add(cell);
                if(!isGF)
                {
                    Codec.EncodeBitmap(g.Settings.Image, cell, FontBase.Settings.NtrBpp, g.Settings.Image.Width, g.Settings.Image.Height);
                } else
                {
                    cell[0] = (byte)g.Settings.Left;
                    cell[1] = (byte)g.Settings.GlyphWidth;
                    cell[2] = (byte)g.Settings.CharWidth;
                    Codec.EncodeBitmap(g.Settings.Image, cell.AsSpan(3), FontBase.Settings.NtrBpp, g.Settings.Image.Width, g.Settings.Image.Height);
                }
            }

            // Build CMAP entries: Direct, Table, Scan
            List<(Glyph First, Glyph Last)> directEntries = CMAP.CreateDirectEntries(ref encodedGlyphs);
            List<List<CMAPEntry>> tableEntries = CMAP.CreateTableEntries(ref encodedGlyphs);
            List<List<CMAPEntry>> scanEntries = CMAP.CreateScanEntries(encodedGlyphs);


            var nftr = new NFTR(version: (ushort)FontBase.Settings.Version);
            uint finfSize = 0x1C;
            if (FontBase.Settings.Width != 0xFF || FontBase.Settings.Height != 0xFF || FontBase.Settings.Ascent != 0xFF)
                finfSize = 0x20;

            var finf = new FINF(
                finfSize,
                0x0,
                FontBase.Settings.LineFeed,
                (ushort)(FINF == null ? 0x00 : FINF.DefaultCharIndex),
                FINF == null ? widthEntries[0] : FINF.DefaultWidths!,
                (byte)FontBase.Settings.CharEncoding,
                FontBase.Settings.Height,
                FontBase.Settings.Width,
                FontBase.Settings.Ascent
                );

            var cglp = new CGLP(
                (byte)CellSize.Value.X,
                (byte)CellSize.Value.Y,
                (ushort)cellBytes,
                (sbyte)FontBase.Settings.Baseline,
                maxCharWidth,
                FontBase.Settings.NtrBpp,
                FontBase.Settings.NtrRotation
                );

            List<CWDH> cwdhHeaders = new List<CWDH> { new CWDH(widthEntries) };

            var cmapHeaders = new List<CMAP>();
            for (var i = 0; i < directEntries.Count(); i++)
                cmapHeaders.Add(new CMAP(directEntries[i]));
            for (var i = 0; i < tableEntries.Count(); i++)
                cmapHeaders.Add(new CMAP(0x1, tableEntries[i]));
            for (var i = 0; i < scanEntries.Count(); i++)
                cmapHeaders.Add(new CMAP(0x2, scanEntries[i]));

            // sort headers TODO: not sure if needed of it will break anything
            cmapHeaders = cmapHeaders.OrderBy(h => h.MappingMethod)
                        .ThenBy(h => h.CodeBegin)
                        .ToList();

            // Delete old file if exists
            if (File.Exists(filename)) File.Delete(filename);

            // Write with correct endianness
            using (var bw = new BinaryWriterX(filename, FontBase.Settings.Endianness == Endianness.Endian.Little))
            {
                var linker = new BlockLinker();
                nftr.Serialize(bw, linker);
                finf.Serialize(bw, linker);
                cglp.Serialize(bw, linker, sheetImgs);
                linker.AddLookupValue(FontPointerType.ptrWidth, (uint)bw.BaseStream.Position);
                foreach (var cwdh in cwdhHeaders)
                {
                    cwdh.Serialize(bw, linker);
                }
                linker.AddLookupValue(FontPointerType.ptrMap, bw.BaseStream.Position + 0x8);
                foreach (var cmap in cmapHeaders)
                {
                    cmap.Serialize(bw, linker);
                }
                linker.AddLookupValue(FontPointerType.fileSize, bw.BaseStream.Position);
                linker.MakeBlockLink(bw);
            }

            // Nothing to cleanup :)
            return new ActionResult(true, "OK");
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
