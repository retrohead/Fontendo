using Fontendo.Extensions;
using Fontendo.Interfaces;
using System.Text;
using static Fontendo.Interfaces.ITextureCodec;

namespace Fontendo.Codecs.CTR
{
    public class CTRTextureCodec : Fontendo.Interfaces.ITextureCodec
    {
        public Dictionary<TextureFormatType, TextureFormatData> TextureFormatFunctions { get; }

        public enum TextureFormatType : int
        {
            RGBA8888 = 0x0,
            RGB888 = 0x1,
            RGBA5551 = 0x2,
            RGB565 = 0x3,
            RGBA4444 = 0x4,
            LA88 = 0x5,
            HL8 = 0x6,
            L8 = 0x7,
            A8 = 0x8,
            LA44 = 0x9,
            L4 = 0xA,
            A4 = 0xB,
            ETC1 = 0xC,
            ETC1A4 = 0xD
        }
        int[] tileOrder = { 0, 1, 8, 9, 2, 3, 10, 11, 16, 17, 24, 25, 18, 19, 26, 27, 4, 5, 12, 13, 6, 7, 14, 15, 20, 21, 28, 29, 22, 23, 30, 31, 32, 33, 40, 41, 34, 35, 42, 43, 48, 49, 56, 57, 50, 51, 58, 59, 36, 37, 44, 45, 38, 39, 46, 47, 52, 53, 60, 61, 54, 55, 62, 63 };


        public CTRTextureCodec()
        {
            TextureFormatFunctions = new Dictionary<TextureFormatType, TextureFormatData>
                {
                    { TextureFormatType.RGBA8888, new TextureFormatData(0x0, decodeRGBA8888) },
                    { TextureFormatType.RGB888,   new TextureFormatData(0x1, decodeRGB888) },   // RGB888
                    { TextureFormatType.RGBA5551, new TextureFormatData(0x2, decodeRGBA5551) }, // RGBA5551
                    { TextureFormatType.RGB565,   new TextureFormatData(0x3, decodeRGB565) },   // RGB565
                    { TextureFormatType.RGBA4444, new TextureFormatData(0x4, decodeRGBA4444) }, // RGBA4444
                    { TextureFormatType.LA88,     new TextureFormatData(0x5, decodeLA88) },     // LA88
                    { TextureFormatType.HL8,      new TextureFormatData(0x6, decodeHL8) },      // HL8
                    { TextureFormatType.L8,       new TextureFormatData(0x7, decodeL8) },       // L8
                    { TextureFormatType.A8,       new TextureFormatData(0x8, decodeA8) },       // A8
                    { TextureFormatType.LA44,     new TextureFormatData(0x9, decodeLA44) },     // LA44
                    { TextureFormatType.L4,       new TextureFormatData(0xA, decodeL4) },       // L4
                    { TextureFormatType.A4,       new TextureFormatData(0xB, DecodeA4) },       // A4
                    { TextureFormatType.ETC1,     new TextureFormatData(0xC, decodeETC1) },     // ETC1
                    { TextureFormatType.ETC1A4,   new TextureFormatData(0xD, decodeETC1A4) }    // ETC1A4
                };
        }

        private byte[] decodeETC1A4(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] decodeETC1(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }
        public byte[] DecodeA4(BinaryReaderX br, ushort width, ushort height)
        {
            byte[] argbBuf = new byte[width * height * 4];

            for (int tY = 0; tY < height / 8; tY++)
            {
                for (int tX = 0; tX < width / 8; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel += 2)
                    {
                        int x = tileOrder[pixel] % 8;
                        int y = (tileOrder[pixel] - x) / 8;
                        int outputOffset = ((tX * 8) + x + ((tY * 8 + y) * width)) * 4;

                        byte data = br.ReadByte();

                        // First nibble (low 4 bits)
                        byte newpixel = (byte)((data & 0x0F) * 0x11);
                        argbBuf[outputOffset + 0] = 0xFF;
                        argbBuf[outputOffset + 1] = 0xFF;
                        argbBuf[outputOffset + 2] = 0xFF;
                        argbBuf[outputOffset + 3] = newpixel;

                        // Second nibble (high 4 bits)
                        newpixel = (byte)((data >> 4) * 0x11);
                        argbBuf[outputOffset + 4] = 0xFF;
                        argbBuf[outputOffset + 5] = 0xFF;
                        argbBuf[outputOffset + 6] = 0xFF;
                        argbBuf[outputOffset + 7] = newpixel;
                    }
                }
            }

            return argbBuf;
        }


        private byte[] decodeL4(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] decodeLA44(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] decodeA8(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] decodeL8(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] decodeHL8(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] decodeLA88(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] decodeRGBA4444(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] decodeRGB565(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] decodeRGBA5551(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] decodeRGB888(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] decodeRGBA8888(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        public byte[] DecodeTexture(ushort texFmt, BinaryReaderX br, ushort width, ushort height)
        {
            if (texFmt > 0xF)
                throw new ArgumentOutOfRangeException("Texture format is out of range for a CTR font");
            if (TextureFormatFunctions.TryGetValue((TextureFormatType)texFmt, out var formatdata))
            {
                return formatdata.DecodeFunction(br, width, height);
            }

            throw new ArgumentOutOfRangeException(nameof(texFmt), texFmt, $"Unknown CTR texture format code {texFmt}");
        }

        public byte[] EncodeTexture(ushort texFmt, byte[] data, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }


        byte ITextureCodec.GeneralTextureType(ushort texFmt)
        {
            if (texFmt > 0xF)
                throw new ArgumentOutOfRangeException("Texture format is out of range for a CTR font");
            if (TextureFormatFunctions.TryGetValue((TextureFormatType)texFmt, out var formatdata))
                return formatdata.TextureFormatTypeByte;

            throw new ArgumentOutOfRangeException(nameof(texFmt), texFmt, "Unsupported CTR texture format!");
        }

        public byte PlatformTextureType(byte generalFmt)
        {
            throw new NotImplementedException();
        }
    }
}