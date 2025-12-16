using Fontendo.Extensions;
using Fontendo.Extensions.BinaryTools;
using Fontendo.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using static Fontendo.Interfaces.ITextureCodec;

namespace Fontendo.Codecs.NTR
{
    public class NTRTextureCodec : ITextureCodec
    {
        public Dictionary<TextureFormatType, TextureFormatData> TextureFormatFunctions { get; }

        public enum TextureFormatType : int
        {
            None = 0x0,
        }

        public NTRTextureCodec()
        {
            TextureFormatFunctions = new Dictionary<TextureFormatType, TextureFormatData>();
        }

        public DecodedTextureType DecodeTexture(ushort bpp, BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }
        public DecodedTextureType DecodeBitmap(ushort bpp, BitReader bitr, ushort width, ushort height)
        {
            byte[] imgBuf = new byte[width * height * 4];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte px = bitr.ReadBitsNormalizedBackwards(bpp);

                    int offset = ((y * width + x) * 4);
                    imgBuf[offset + 0] = px;
                    imgBuf[offset + 1] = px;
                    imgBuf[offset + 2] = px;
                    imgBuf[offset + 3] = (px == 0) ? (byte)0x00 : (byte)0xFF;
                }
            }

            return new DecodedTextureType(imgBuf, null);
        }


        public byte[] EncodeTexture(ushort texFmt, byte[] data, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private static byte FlipBitOrder(byte bits, int bpp)
        {
            byte result = 0;
            for (int i = 0; i < 8; i++)
            {
                result |= (byte)(((bits & (1 << i)) >> i) << (7 - i));
            }
            return (byte)(result >> (8 - bpp));
        }

        public void EncodeBitmap(Bitmap image, Span<byte> span, byte bpp, int width, int height)
        {
            if (span.Length == 0)
                return;

            // Expected encoded size; if this doesn't match, caller logic is wrong.
            int expectedBytes = (width * height * bpp) / 8;
            if (span.Length < expectedBytes)
                throw new ArgumentException("Span too small for encoded bitmap", nameof(span));

            var rect = new Rectangle(0, 0, width, height);
            var data = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                int stride = data.Stride;
                int bitpos = 0;
                double conv = (((1 << 8) - 1.0) / ((1 << bpp) - 1.0));

                int dstIndex = 0;
                byte[] rowBuffer = new byte[width * 4];

                for (int y = 0; y < height; y++)
                {
                    IntPtr srcPtr = data.Scan0 + y * stride;
                    Marshal.Copy(srcPtr, rowBuffer, 0, rowBuffer.Length);

                    for (int x = 0; x < width; x++)
                    {
                        int offset = x * 4;

                        byte blue = rowBuffer[offset + 0];
                        byte green = rowBuffer[offset + 1];
                        byte red = rowBuffer[offset + 2];
                        byte alpha = rowBuffer[offset + 3];

                        double floatpixel =
                            (((red + green + blue) / 3.0) * (alpha / 255.0)) / conv;

                        byte bytepx = FlipBitOrder((byte)Math.Round(floatpixel), bpp);

                        int shift = (8 - bpp) - bitpos;

                        // First write
                        if (dstIndex >= span.Length)
                            throw new IndexOutOfRangeException("EncodeBitmap: dstIndex out of range");

                        if (shift >= 0)
                            span[dstIndex] |= (byte)(bytepx << shift);
                        else
                            span[dstIndex] |= (byte)(bytepx >> -shift);

                        bitpos += bpp;

                        if (bitpos >= 8)
                        {
                            bitpos %= 8;
                            dstIndex++;

                            if (bitpos != 0)
                            {
                                if (dstIndex >= span.Length)
                                    throw new IndexOutOfRangeException("EncodeBitmap: dstIndex out of range (carry write)");

                                // Same logic as C++: *dst |= bytepx << (8 - (8 - bitpos));
                                span[dstIndex] |= (byte)(bytepx << bitpos);
                            }
                        }
                    }
                }
            }
            finally
            {
                image.UnlockBits(data);
            }
        }



        public static void EncodeBitmap(byte[] argb, byte[] dst, int bpp, ushort width, ushort height)
        {
            int bitpos = 0;
            double conv = ((1 << 8) - 1.0) / ((1 << bpp) - 1.0);

            int dstIndex = 0; // index into dst array

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int offset = ((y * width + x) * 4);
                    byte red = argb[offset + 0];
                    byte green = argb[offset + 1];
                    byte blue = argb[offset + 2];
                    byte alpha = argb[offset + 3];

                    double floatpixel = (((red + green + blue) / 3.0) * (alpha / 255.0)) / conv;
                    byte bytepx = FlipBitOrder((byte)Math.Round(floatpixel), bpp);

                    int shift = (8 - bpp) - bitpos;
                    if (shift >= 0)
                    {
                        dst[dstIndex] |= (byte)(bytepx << shift);
                    }
                    else
                    {
                        dst[dstIndex] |= (byte)(bytepx >> Math.Abs(shift));
                    }

                    bitpos += bpp;
                    if (bitpos >= 8)
                    {
                        bitpos %= 8;
                        dstIndex++;
                        if (bitpos != 0)
                        {
                            dst[dstIndex] |= (byte)(bytepx << (8 - (8 - bitpos)));
                        }
                    }
                }
            }
        }

        public byte ConvertGeneralTextureTypeToPlatform(FontBase.ImageFormats generalFmt)
        {
            throw new NotImplementedException();
        }

        public FontBase.ImageFormats ConvertPlatformTextureTypeToGeneral(ushort texFmt)
        {
            throw new NotImplementedException();
        }
    }
}