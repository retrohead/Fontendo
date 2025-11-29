
using Fontendo.Extensions;
using Fontendo.Interfaces;
using System;
using System.Drawing.Imaging;

public class BCFNT
{
    public CFNT? CFNT = null;
    public FINF? FINF = null;
    public CGLP? CGLP = null;
    public TGLP? TGLP = null;

    public List<CWDH>? CWDH_Headers = null;
    public List<CharWidths>? CharWidths = null;
    public List<CMAP>? CMAP_Headers = null;
    public List<CMAPEntry>? CharMaps = null;
    public List<Bitmap>? Sheets = null;

    public ActionResult Load(string filename)
    {
        ActionResult result;

        CFNT = new CFNT();
        FINF = new FINF();
        using (var br = new BinaryReaderX(File.OpenRead(filename)))
        {
            br.SetEndianness();
            br.BaseStream.Position = 0;

            // CFNT Header
            result = CFNT.Parse(br);
            if (!result.Success)
                return new ActionResult(false, result.Message);

            // FINF Header
            br.BaseStream.Position = CFNT.headerSize;
            result = FINF.Parse(br);
            if (!result.Success)
                return new ActionResult(false, result.Message);

            // Glyph Header
            br.BaseStream.Position = FINF.ptrGlyph - 0x8U;
            if (FINF.fontType == 0x0)  //FONT_TYPE_GLYPH, use CGLP
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

            } else if (FINF.fontType == 0x1)
            {
                TGLP = new TGLP();
                result = TGLP.Parse(br);
                if (!result.Success)
                    return new ActionResult(false, result.Message);
            } else
            {
                return new ActionResult(false, $"Unknown font type 0x{FINF.fontType.ToString("X")}");
            }
            // CWDH data
            UInt32 nextPtr = FINF.ptrWidth - 0x8;
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
            nextPtr = FINF.ptrMap;
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
                nextPtr = cmap.ptrNext;
                CMAP_Headers.Add(cmap); //Append the CMAP header to the list
            }

            // Decoding textures
            br.BaseStream.Position = TGLP.sheetPtr;
            Sheets = new List<Bitmap>();

            ITextureCodec codec = TextureCodecFactory.Create(TextureCodecFactory.Platform.CTR);
            for (ushort i = 0; i < TGLP.sheetCount; i++)
            {
                byte[] sheetRaw = codec.DecodeTexture(TGLP.sheetFormat, br, TGLP.sheetWidth, TGLP.sheetHeight);

                Bitmap sheet = new Bitmap(TGLP.sheetWidth, TGLP.sheetHeight, PixelFormat.Format32bppArgb);

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

                Sheets.Add(sheet);
            }

        }
        return new ActionResult(true, "OK");
    }
}