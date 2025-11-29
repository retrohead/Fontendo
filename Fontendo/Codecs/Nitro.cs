using Fontendo.Extensions;
using static Fontendo.Interfaces.ITextureCodec;

namespace Fontendo.Codecs.NTR
{
    public class NTRTextureCodec : Fontendo.Interfaces.ITextureCodec
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

        public byte[] DecodeTexture(ushort texFmt, BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }


        public byte[] EncodeTexture(ushort texFmt, byte[] data, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        public byte GeneralTextureType(ushort texFmt)
        {
            throw new NotImplementedException();
        }

        public byte PlatformTextureType(byte generalFmt)
        {
            throw new NotImplementedException();
        }


        public static byte[] DecodeBitmap(int bpp, BitReader br, ushort width, ushort height)
        {
            byte[] imgBuf = new byte[width * height * 4];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte px = br.ReadBitsNormalizedBackwards(bpp);

                    int offset = ((y * width + x) * 4);
                    imgBuf[offset + 0] = px;
                    imgBuf[offset + 1] = px;
                    imgBuf[offset + 2] = px;
                    imgBuf[offset + 3] = (px == 0) ? (byte)0x00 : (byte)0xFF;
                }
            }

            return imgBuf;
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
    }
}