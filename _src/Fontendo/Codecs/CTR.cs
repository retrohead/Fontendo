using Fontendo.Extensions.BinaryTools;
using Fontendo.Interfaces;
using System.IO;
using System.Drawing;
using static Fontendo.Extensions.FontBase;
using static Fontendo.Interfaces.ITextureCodec;

namespace Fontendo.Codecs.CTR
{
    public class CTRTextureCodec : ITextureCodec
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
                { TextureFormatType.RGBA8888, new TextureFormatData(0x0, DecodeRGBA8888, EncodeRGBA8888) },
                { TextureFormatType.RGB888,   new TextureFormatData(0x1, DecodeRGB888,   EncodeRGB888)   },
                { TextureFormatType.RGBA5551, new TextureFormatData(0x2, DecodeRGBA5551, EncodeRGBA5551) },
                { TextureFormatType.RGB565,   new TextureFormatData(0x3, DecodeRGB565,   EncodeRGB565)   },
                { TextureFormatType.RGBA4444, new TextureFormatData(0x4, DecodeRGBA4444, EncodeRGBA4444) },
                { TextureFormatType.LA88,     new TextureFormatData(0x5, DecodeLA88,     EncodeLA88)     },
                { TextureFormatType.HL8,      new TextureFormatData(0x6, DecodeHL8,      EncodeHL8)      }, // not used in BCFNT
                { TextureFormatType.L8,       new TextureFormatData(0x7, DecodeL8,       EncodeL8)       }, // not used in BCFNT
                { TextureFormatType.A8,       new TextureFormatData(0x8, DecodeA8,       EncodeA8)       },
                { TextureFormatType.LA44,     new TextureFormatData(0x9, DecodeLA44,     EncodeLA44)     },
                { TextureFormatType.L4,       new TextureFormatData(0xA, DecodeL4,       EncodeL4)       }, // not used in BCFNT
                { TextureFormatType.A4,       new TextureFormatData(0xB, DecodeA4,       EncodeA4)       },
                { TextureFormatType.ETC1,     new TextureFormatData(0xC, DecodeETC1,     EncodeETC1)     }, // not used in BCFNT
                { TextureFormatType.ETC1A4,   new TextureFormatData(0xD, DecodeETC1A4,   EncodeETC1A4)   }  // not used in BCFNT
            };
        }

        #region "Decoders"

        private DecodedTextureType DecodeETC1A4(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private DecodedTextureType DecodeETC1(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        public DecodedTextureType DecodeA4(BinaryReaderX br, ushort width, ushort height)
        {
            byte[] texBuf = new byte[width * height * 4];
            byte[] maskBuf = new byte[width * height * 4];

            for (int tY = 0; tY < height / 8; tY++)
            {
                for (int tX = 0; tX < width / 8; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel += 2)
                    {
                        byte data = br.ReadByte();

                        // -------------------------
                        // FIRST NIBBLE
                        // -------------------------
                        int p0 = tileOrder[pixel];
                        int x0 = tX * 8 + (p0 % 8);
                        int y0 = tY * 8 + (p0 / 8);

                        byte alpha0 = (byte)((data & 0x0F) * 0x11);

                        int offset0 = (y0 * width + x0) * 4;

                        // normal texture
                        texBuf[offset0 + 0] = 0xFF;
                        texBuf[offset0 + 1] = 0xFF;
                        texBuf[offset0 + 2] = 0xFF;
                        texBuf[offset0 + 3] = alpha0;

                        // -------------------------
                        // SECOND NIBBLE
                        // -------------------------
                        int p1 = tileOrder[pixel + 1];
                        int x1 = tX * 8 + (p1 % 8);
                        int y1 = tY * 8 + (p1 / 8);

                        byte alpha1 = (byte)((data >> 4) * 0x11);

                        int offset1 = (y1 * width + x1) * 4;

                        // normal texture
                        texBuf[offset1 + 0] = 0xFF;
                        texBuf[offset1 + 1] = 0xFF;
                        texBuf[offset1 + 2] = 0xFF;
                        texBuf[offset1 + 3] = alpha1;
                    }
                }
            }

            return new DecodedTextureType(texBuf, null);
        }

        private DecodedTextureType DecodeL4(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        public DecodedTextureType DecodeLA44(BinaryReaderX br, ushort width, ushort height)
        {
            byte[] texBuf = new byte[width * height * 4];
            byte[] maskBuf = new byte[width * height * 4];


            int tilesX = (width + 7) / 8;
            int tilesY = (height + 7) / 8;

            for (int tY = 0; tY < tilesY; tY++)
            {
                for (int tX = 0; tX < tilesX; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel++)
                    {
                        int localX = tileOrder[pixel] % 8;
                        int localY = (tileOrder[pixel] - localX) / 8;

                        int x = tX * 8 + localX;
                        int y = tY * 8 + localY;

                        if (x >= width || y >= height)
                        {
                            br.ReadByte();
                            continue;
                        }

                        byte data = br.ReadByte();
                        byte luminance = (byte)((data & 0x0F) * 0x11);
                        byte alpha = (byte)((data >> 4) * 0x11);

                        int offset = (y * width + x) * 4;

                        // -------------------------
                        // NORMAL TEXTURE
                        // -------------------------
                        texBuf[offset + 0] = luminance;
                        texBuf[offset + 1] = luminance;
                        texBuf[offset + 2] = luminance;
                        texBuf[offset + 3] = alpha;

                        // -------------------------
                        // MASK TEXTURE
                        // -------------------------
                        if (alpha == 0 && luminance > 0)
                        {
                            // Make them fully visible in the mask
                            maskBuf[offset + 0] = luminance;
                            maskBuf[offset + 1] = luminance;
                            maskBuf[offset + 2] = luminance;
                            maskBuf[offset + 3] = 0xFF;
                        }
                        else
                        {
                            // Everything else invisible
                            maskBuf[offset + 0] = 0x00;
                            maskBuf[offset + 1] = 0x00;
                            maskBuf[offset + 2] = 0x00;
                            maskBuf[offset + 3] = 0x00;
                        }
                    }
                }
            }

            return new DecodedTextureType(texBuf, maskBuf);
        }

        public DecodedTextureType DecodeA8(BinaryReaderX br, ushort width, ushort height)
        {
            byte[] texBuf = new byte[width * height * 4];
            byte[] maskBuf = new byte[width * height * 4];

            int tilesX = width / 8;
            int tilesY = height / 8;

            for (int tY = 0; tY < tilesY; tY++)
            {
                for (int tX = 0; tX < tilesX; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel++)
                    {
                        int localX = tileOrder[pixel] % 8;
                        int localY = (tileOrder[pixel] - localX) / 8;

                        int x = tX * 8 + localX;
                        int y = tY * 8 + localY;

                        int offset = (y * width + x) * 4;

                        byte alpha = br.ReadByte();

                        // -------------------------
                        // NORMAL TEXTURE
                        // -------------------------
                        texBuf[offset + 0] = 0xFF;
                        texBuf[offset + 1] = 0xFF;
                        texBuf[offset + 2] = 0xFF;
                        texBuf[offset + 3] = alpha;
                    }
                }
            }

            return new DecodedTextureType(texBuf, null);
        }


        private DecodedTextureType DecodeL8(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private DecodedTextureType DecodeHL8(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        public DecodedTextureType DecodeLA88(BinaryReaderX br, ushort width, ushort height)
        {
            byte[] texBuf = new byte[width * height * 4];
            byte[] maskBuf = new byte[width * height * 4];

            int tilesX = width / 8;
            int tilesY = height / 8;

            for (int tY = 0; tY < tilesY; tY++)
            {
                for (int tX = 0; tX < tilesX; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel++)
                    {
                        int localX = tileOrder[pixel] % 8;
                        int localY = (tileOrder[pixel] - localX) / 8;

                        int x = tX * 8 + localX;
                        int y = tY * 8 + localY;

                        int offset = (y * width + x) * 4;

                        byte luminance = br.ReadByte();
                        byte alpha = br.ReadByte();

                        // -------------------------
                        // NORMAL TEXTURE
                        // -------------------------
                        texBuf[offset + 0] = luminance;
                        texBuf[offset + 1] = luminance;
                        texBuf[offset + 2] = luminance;
                        texBuf[offset + 3] = alpha;

                        // -------------------------
                        // MASK TEXTURE
                        // alpha == 0 → visible in mask
                        // else → fully transparent
                        // -------------------------
                        if (alpha == 0 && luminance > 0)
                        {
                            maskBuf[offset + 0] = luminance;
                            maskBuf[offset + 1] = luminance;
                            maskBuf[offset + 2] = luminance;
                            maskBuf[offset + 3] = 0xFF;
                        }
                        else
                        {
                            maskBuf[offset + 0] = 0x00;
                            maskBuf[offset + 1] = 0x00;
                            maskBuf[offset + 2] = 0x00;
                            maskBuf[offset + 3] = 0x00;
                        }
                    }
                }
            }

            return new DecodedTextureType(texBuf, maskBuf);
        }


        private DecodedTextureType DecodeRGBA4444(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        public DecodedTextureType DecodeRGB565(BinaryReaderX br, ushort width, ushort height)
        {
            byte[] argbBuf = new byte[width * height * 4];

            for (int tY = 0; tY < height / 8; tY++)
            {
                for (int tX = 0; tX < width / 8; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel++)
                    {
                        int x = tileOrder[pixel] % 8;
                        int y = (tileOrder[pixel] - x) / 8;
                        int outputOffset = ((tX * 8) + x + ((tY * 8 + y) * width)) * 4;

                        ushort pixelData = (ushort)(br.ReadByte() | (br.ReadByte() << 8));

                        byte red = (byte)((pixelData & 0x1F) << 3);
                        byte green = (byte)(((pixelData >> 5) & 0x3F) << 2);
                        byte blue = (byte)(((pixelData >> 11) & 0x1F) << 3);

                        // Normalize to 8-bit channels
                        argbBuf[outputOffset + 0] = (byte)(blue | (blue >> 5));
                        argbBuf[outputOffset + 1] = (byte)(green | (green >> 6));
                        argbBuf[outputOffset + 2] = (byte)(red | (red >> 5));
                        argbBuf[outputOffset + 3] = 0xFF; // fully opaque
                    }
                }
            }

            return new DecodedTextureType(argbBuf, null);
        }

        public DecodedTextureType DecodeRGBA5551(BinaryReaderX br, ushort width, ushort height)
        {
            byte[] texBuf = new byte[width * height * 4];
            byte[] maskBuf = new byte[width * height * 4];

            for (int tY = 0; tY < height / 8; tY++)
            {
                for (int tX = 0; tX < width / 8; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel++)
                    {
                        int localX = tileOrder[pixel] % 8;
                        int localY = (tileOrder[pixel] - localX) / 8;

                        int x = tX * 8 + localX;
                        int y = tY * 8 + localY;

                        int offset = (y * width + x) * 4;

                        ushort pixelData = (ushort)(br.ReadByte() | (br.ReadByte() << 8));

                        byte red = (byte)(((pixelData >> 1) & 0x1F) << 3);
                        byte green = (byte)(((pixelData >> 6) & 0x1F) << 3);
                        byte blue = (byte)(((pixelData >> 11) & 0x1F) << 3);
                        byte alpha = (byte)((pixelData & 1) * 0xFF);

                        // Normalize 5-bit channels to 8-bit (Ohana trick)
                        byte R = (byte)(red | (red >> 5));
                        byte G = (byte)(green | (green >> 5));
                        byte B = (byte)(blue | (blue >> 5));

                        // -------------------------
                        // NORMAL TEXTURE
                        // -------------------------
                        texBuf[offset + 0] = B;
                        texBuf[offset + 1] = G;
                        texBuf[offset + 2] = R;
                        texBuf[offset + 3] = alpha;

                        // -------------------------
                        // MASK TEXTURE (RGB preserved)
                        // alpha == 0 → keep RGB, alpha=255
                        // alpha != 0 → alpha=0
                        // -------------------------
                        if (alpha == 0 && B + G + R > 0)
                        {
                            maskBuf[offset + 0] = B;
                            maskBuf[offset + 1] = G;
                            maskBuf[offset + 2] = R;
                            maskBuf[offset + 3] = 0xFF;
                        }
                        else
                        {
                            maskBuf[offset + 0] = 0;
                            maskBuf[offset + 1] = 0;
                            maskBuf[offset + 2] = 0;
                            maskBuf[offset + 3] = 0;
                        }
                    }
                }
            }

            return new DecodedTextureType(texBuf, maskBuf);
        }


        public DecodedTextureType DecodeRGB888(BinaryReaderX br, ushort width, ushort height)
        {
            byte[] argbBuf = new byte[width * height * 4];

            for (int tY = 0; tY < height / 8; tY++)
            {
                for (int tX = 0; tX < width / 8; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel++)
                    {
                        int localX = tileOrder[pixel] % 8;
                        int localY = (tileOrder[pixel] - localX) / 8;

                        int x = tX * 8 + localX;
                        int y = tY * 8 + localY;

                        int outputOffset = (y * width + x) * 4;

                        byte red = br.ReadByte();
                        byte green = br.ReadByte();
                        byte blue = br.ReadByte();

                        argbBuf[outputOffset + 0] = blue;
                        argbBuf[outputOffset + 1] = green;
                        argbBuf[outputOffset + 2] = red;

                        // Alpha: transparent if black, opaque otherwise
                        bool isTransparent = (red == 0 && green == 0 && blue == 0);
                        byte alpha = isTransparent ? (byte)0x00 : (byte)0xFF;

                        argbBuf[outputOffset + 3] = alpha;
                    }
                }
            }

            return new DecodedTextureType(argbBuf, null);
        }

        public DecodedTextureType DecodeRGBA8888(BinaryReaderX br, ushort width, ushort height)
        {
            byte[] texBuf = new byte[width * height * 4];
            byte[] maskBuf = new byte[width * height * 4];

            int tilesX = width / 8;
            int tilesY = height / 8;

            for (int tY = 0; tY < tilesY; tY++)
            {
                for (int tX = 0; tX < tilesX; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel++)
                    {
                        int localX = tileOrder[pixel] % 8;
                        int localY = (tileOrder[pixel] - localX) / 8;

                        int x = tX * 8 + localX;
                        int y = tY * 8 + localY;

                        int offset = (y * width + x) * 4;

                        byte alpha = br.ReadByte();
                        byte red = br.ReadByte();
                        byte green = br.ReadByte();
                        byte blue = br.ReadByte();

                        // -------------------------
                        // NORMAL TEXTURE
                        // -------------------------
                        texBuf[offset + 0] = blue;
                        texBuf[offset + 1] = green;
                        texBuf[offset + 2] = red;
                        texBuf[offset + 3] = alpha;

                        // -------------------------
                        // MASK TEXTURE (RGB preserved)
                        // -------------------------
                        if (alpha == 0 && red + green + blue > 0)
                        {
                            maskBuf[offset + 0] = blue;
                            maskBuf[offset + 1] = green;
                            maskBuf[offset + 2] = red;
                            maskBuf[offset + 3] = 0xFF;
                        }
                        else
                        {
                            maskBuf[offset + 0] = 0;
                            maskBuf[offset + 1] = 0;
                            maskBuf[offset + 2] = 0;
                            maskBuf[offset + 3] = 0;
                        }
                    }
                }
            }

            return new DecodedTextureType(texBuf, maskBuf);
        }

        public DecodedTextureType DecodeTexture(ushort texFmt, BinaryReaderX br, ushort width, ushort height)
        {
            if (texFmt > 0xF)
                throw new ArgumentOutOfRangeException("Texture format is out of range for a CTR font");
            if (TextureFormatFunctions.TryGetValue((TextureFormatType)texFmt, out var formatdata))
            {
                return formatdata.DecodeFunction(br, width, height);
            }

            throw new ArgumentOutOfRangeException(nameof(texFmt), texFmt, $"Unknown CTR texture format code {texFmt}");
        }

        #endregion

        #region "Encoders"

        public byte[] EncodeRGB888(byte[] argbBuf, ushort width, ushort height)
        {
            int size = width * height * 3;
            byte[] rgbBuf = new byte[size];
            int i = 0;

            for (int tY = 0; tY < height / 8; tY++)
            {
                for (int tX = 0; tX < width / 8; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel++)
                    {
                        int x = tileOrder[pixel] % 8;
                        int y = (tileOrder[pixel] - x) / 8;
                        int offset = ((tX * 8) + x + ((tY * 8 + y) * width)) * 4;

                        byte blue = argbBuf[offset + 0];
                        byte green = argbBuf[offset + 1];
                        byte red = argbBuf[offset + 2];
                        byte alpha = argbBuf[offset + 3];

                        float floatAlpha = alpha / 255f;

                        rgbBuf[i++] = (byte)Math.Round(red * floatAlpha);
                        rgbBuf[i++] = (byte)Math.Round(green * floatAlpha);
                        rgbBuf[i++] = (byte)Math.Round(blue * floatAlpha);
                    }
                }
            }

            return rgbBuf;
        }

        public byte[] EncodeRGBA8888(byte[] argbBuf, ushort width, ushort height)
        {
            int size = width * height * 4;
            byte[] rgbBuf = new byte[size];
            int i = 0;

            for (int tY = 0; tY < height / 8; tY++)
            {
                for (int tX = 0; tX < width / 8; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel++)
                    {
                        int x = tileOrder[pixel] % 8;
                        int y = (tileOrder[pixel] - x) / 8;
                        int offset = ((tX * 8) + x + ((tY * 8 + y) * width)) * 4;

                        byte blue = argbBuf[offset + 0];
                        byte green = argbBuf[offset + 1];
                        byte red = argbBuf[offset + 2];
                        byte alpha = argbBuf[offset + 3];

                        rgbBuf[i++] = alpha;
                        rgbBuf[i++] = red;
                        rgbBuf[i++] = green;
                        rgbBuf[i++] = blue;
                    }
                }
            }

            return rgbBuf;
        }

        public byte[] EncodeRGBA5551(byte[] argbBuf, ushort width, ushort height)
        {
            int size = width * height * 2;
            byte[] rgbBuf = new byte[size];
            int i = 0;

            // Conversion coefficient: maps 8-bit channel to 5-bit range
            float conv = ((1 << 8) - 1.0f) / ((1 << 5) - 1.0f);

            for (int tY = 0; tY < height / 8; tY++)
            {
                for (int tX = 0; tX < width / 8; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel++)
                    {
                        int x = tileOrder[pixel] % 8;
                        int y = (tileOrder[pixel] - x) / 8;
                        int offset = ((tX * 8) + x + ((tY * 8 + y) * width)) * 4;

                        byte blue = argbBuf[offset + 0];
                        byte green = argbBuf[offset + 1];
                        byte red = argbBuf[offset + 2];
                        byte alpha = argbBuf[offset + 3];

                        // Convert 8-bit channels to 5-bit
                        byte newB = (byte)Math.Round(blue / conv);
                        byte newG = (byte)Math.Round(green / conv);
                        byte newR = (byte)Math.Round(red / conv);
                        byte newA = (byte)(alpha / 255); // 1-bit alpha

                        ushort packed = (ushort)(newA | (newR << 1) | (newG << 6) | (newB << 11));

                        rgbBuf[i++] = (byte)(packed & 0xFF);
                        rgbBuf[i++] = (byte)(packed >> 8);
                    }
                }
            }

            return rgbBuf;
        }

        public byte[] EncodeRGB565(byte[] argbBuf, ushort width, ushort height)
        {
            int size = width * height * 2;
            byte[] rgbBuf = new byte[size];
            int i = 0;

            // Conversion coefficients: map 8-bit channels to 5-bit (R/B) and 6-bit (G)
            float convRB = ((1 << 8) - 1.0f) / ((1 << 5) - 1.0f);
            float convG = ((1 << 8) - 1.0f) / ((1 << 6) - 1.0f);

            for (int tY = 0; tY < height / 8; tY++)
            {
                for (int tX = 0; tX < width / 8; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel++)
                    {
                        int x = tileOrder[pixel] % 8;
                        int y = (tileOrder[pixel] - x) / 8;
                        int offset = ((tX * 8) + x + ((tY * 8 + y) * width)) * 4;

                        byte blue = argbBuf[offset + 0];
                        byte green = argbBuf[offset + 1];
                        byte red = argbBuf[offset + 2];
                        byte alpha = argbBuf[offset + 3];

                        float floatAlpha = alpha / 255f;

                        // Convert 8-bit channels to 5/6-bit with premultiplied alpha
                        byte newB = (byte)Math.Round((blue / convRB) * floatAlpha);
                        byte newG = (byte)Math.Round((green / convG) * floatAlpha);
                        byte newR = (byte)Math.Round((red / convRB) * floatAlpha);

                        ushort packed = (ushort)(newR | (newG << 5) | (newB << 11));

                        rgbBuf[i++] = (byte)(packed & 0xFF);
                        rgbBuf[i++] = (byte)(packed >> 8);
                    }
                }
            }

            return rgbBuf;
        }

        public byte[] EncodeRGBA4444(byte[] argbBuf, ushort width, ushort height)
        {
            int size = width * height * 2;
            byte[] rgbBuf = new byte[size];
            int i = 0;

            // Conversion coefficient: maps 8-bit channel to 4-bit range
            float conv = ((1 << 8) - 1.0f) / ((1 << 4) - 1.0f);

            for (int tY = 0; tY < height / 8; tY++)
            {
                for (int tX = 0; tX < width / 8; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel++)
                    {
                        int x = tileOrder[pixel] % 8;
                        int y = (tileOrder[pixel] - x) / 8;
                        int offset = ((tX * 8) + x + ((tY * 8 + y) * width)) * 4;

                        byte blue = argbBuf[offset + 0];
                        byte green = argbBuf[offset + 1];
                        byte red = argbBuf[offset + 2];
                        byte alpha = argbBuf[offset + 3];

                        // Convert 8-bit channels to 4-bit
                        byte newB = (byte)Math.Round(blue / conv);
                        byte newG = (byte)Math.Round(green / conv);
                        byte newR = (byte)Math.Round(red / conv);
                        byte newA = (byte)Math.Round(alpha / conv);

                        // Pack into RGBA4444: A in lowest 4 bits, then R, G, B
                        ushort packed = (ushort)(newA | (newR << 4) | (newG << 8) | (newB << 12));

                        rgbBuf[i++] = (byte)(packed & 0xFF);
                        rgbBuf[i++] = (byte)(packed >> 8);
                    }
                }
            }

            return rgbBuf;
        }

        public byte[] EncodeLA88(byte[] argbBuf, ushort width, ushort height)
        {
            int size = width * height * 2;
            byte[] rgbBuf = new byte[size];
            int i = 0;

            for (int tY = 0; tY < height / 8; tY++)
            {
                for (int tX = 0; tX < width / 8; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel++)
                    {
                        int x = tileOrder[pixel] % 8;
                        int y = (tileOrder[pixel] - x) / 8;
                        int offset = ((tX * 8) + x + ((tY * 8 + y) * width)) * 4;

                        byte blue = argbBuf[offset + 0];
                        byte green = argbBuf[offset + 1];
                        byte red = argbBuf[offset + 2];
                        byte alpha = argbBuf[offset + 3];

                        // Average RGB to get luminance
                        byte luminance = (byte)Math.Round((blue + green + red) / 3.0);

                        rgbBuf[i++] = luminance;
                        rgbBuf[i++] = alpha;
                    }
                }
            }

            return rgbBuf;
        }

        private byte[] EncodeHL8(byte[] argbBuf, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] EncodeL8(byte[] argbBuf, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        public byte[] EncodeA8(byte[] argbBuf, ushort width, ushort height)
        {
            int size = width * height;
            byte[] rgbBuf = new byte[size];
            int i = 0;

            for (int tY = 0; tY < height / 8; tY++)
            {
                for (int tX = 0; tX < width / 8; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel++)
                    {
                        int x = tileOrder[pixel] % 8;
                        int y = (tileOrder[pixel] - x) / 8;
                        int offset = ((tX * 8) + x + ((tY * 8 + y) * width)) * 4;

                        byte blue = argbBuf[offset + 0];
                        byte green = argbBuf[offset + 1];
                        byte red = argbBuf[offset + 2];
                        byte alpha = argbBuf[offset + 3];

                        // Compute grayscale luminance and premultiply by alpha
                        byte newPixel = (byte)Math.Round(((blue + green + red) / 3.0) * (alpha / 255.0));

                        rgbBuf[i++] = newPixel;
                    }
                }
            }

            return rgbBuf;
        }

        public byte[] EncodeLA44(byte[] argbBuf, ushort width, ushort height)
        {
            int size = width * height;
            byte[] rgbBuf = new byte[size];
            int i = 0;

            // Conversion coefficient: maps 8-bit channel to 4-bit range
            float conv = ((1 << 8) - 1.0f) / ((1 << 4) - 1.0f);

            for (int tY = 0; tY < height / 8; tY++)
            {
                for (int tX = 0; tX < width / 8; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel++)
                    {
                        int x = tileOrder[pixel] % 8;
                        int y = (tileOrder[pixel] - x) / 8;
                        int offset = ((tX * 8) + x + ((tY * 8 + y) * width)) * 4;

                        byte blue = argbBuf[offset + 0];
                        byte green = argbBuf[offset + 1];
                        byte red = argbBuf[offset + 2];
                        byte alpha = argbBuf[offset + 3];

                        // Convert to 4-bit luminance and alpha
                        byte luminance = (byte)Math.Round(((blue + green + red) / 3.0) / conv);
                        byte newAlpha = (byte)Math.Round(alpha / conv);

                        // Pack into single byte: lower nibble = luminance, upper nibble = alpha
                        rgbBuf[i++] = (byte)(luminance | (newAlpha << 4));
                    }
                }
            }

            return rgbBuf;
        }

        private byte[] EncodeL4(byte[] argbBuf, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        public byte[] EncodeA4(byte[] argbBuf, ushort width, ushort height)
        {
            int size = (width * height) / 2;
            byte[] rgbBuf = new byte[size];
            int i = 0;

            // Conversion coefficient: maps 8-bit channel to 4-bit range
            float conv = ((1 << 8) - 1.0f) / ((1 << 4) - 1.0f);

            for (int tY = 0; tY < height / 8; tY++)
            {
                for (int tX = 0; tX < width / 8; tX++)
                {
                    for (int pixel = 0; pixel < 64; pixel += 2)
                    {
                        int x1 = tileOrder[pixel] % 8;
                        int y1 = (tileOrder[pixel] - x1) / 8;
                        int offset1 = ((tX * 8) + x1 + ((tY * 8 + y1) * width)) * 4;

                        byte blue1 = argbBuf[offset1 + 0];
                        byte green1 = argbBuf[offset1 + 1];
                        byte red1 = argbBuf[offset1 + 2];
                        byte alpha1 = argbBuf[offset1 + 3];

                        // Compute grayscale luminance premultiplied by alpha, scaled to 4 bits
                        byte newPixel1 = (byte)Math.Round(((blue1 + green1 + red1) / 3.0) * (alpha1 / 255.0) / conv);

                        int x2 = tileOrder[pixel + 1] % 8;
                        int y2 = (tileOrder[pixel + 1] - x2) / 8;
                        int offset2 = ((tX * 8) + x2 + ((tY * 8 + y2) * width)) * 4;

                        byte blue2 = argbBuf[offset2 + 0];
                        byte green2 = argbBuf[offset2 + 1];
                        byte red2 = argbBuf[offset2 + 2];
                        byte alpha2 = argbBuf[offset2 + 3];

                        byte newPixel2 = (byte)Math.Round(((blue2 + green2 + red2) / 3.0) * (alpha2 / 255.0) / conv);

                        // Pack two 4-bit values into one byte: lower nibble = first pixel, upper nibble = second pixel
                        rgbBuf[i++] = (byte)(newPixel1 | (newPixel2 << 4));
                    }
                }
            }

            return rgbBuf;
        }

        private byte[] EncodeETC1(byte[] argbBuf, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] EncodeETC1A4(byte[] argbBuf, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        public byte[] EncodeTexture(ushort texFmt, byte[] data, ushort width, ushort height)
        {
            if (texFmt > 0xF)
                throw new ArgumentOutOfRangeException("Texture format is out of range for a CTR font");
            if (TextureFormatFunctions.TryGetValue((TextureFormatType)texFmt, out var formatdata))
            {
                return formatdata.EncodeFunction(data, width, height);
            }

            throw new ArgumentOutOfRangeException(nameof(texFmt), texFmt, $"Unknown CTR texture format code {texFmt}");

        }

        #endregion

        public ImageFormats ConvertPlatformTextureTypeToGeneral(ushort texFmt)
        {
            switch((TextureFormatType)texFmt)
            {
                case TextureFormatType.RGBA8888:
                    return ImageFormats.RGBA8;
                case TextureFormatType.RGB888:
                    return ImageFormats.RGB8;
                case TextureFormatType.RGBA5551:
                    return ImageFormats.RGB5A1;
                case TextureFormatType.RGB565:
                    return ImageFormats.RGB565;
                case TextureFormatType.RGBA4444:
                    return ImageFormats.RGBA4; ;
                case TextureFormatType.LA88:
                    return ImageFormats.L8A;
                case TextureFormatType.HL8:
                    return ImageFormats.HL8;
                case TextureFormatType.L8:
                    return ImageFormats.L8;
                case TextureFormatType.A8:
                    return ImageFormats.A8;
                case TextureFormatType.LA44:
                    return ImageFormats.L4A;
                case TextureFormatType.L4:
                    return ImageFormats.L4;
                case TextureFormatType.A4:
                    return ImageFormats.A4;
                case TextureFormatType.ETC1:
                    return ImageFormats.ETC1;
                case TextureFormatType.ETC1A4:
                    return ImageFormats.ETC1A4;
                default:
                    throw new FileFormatException("Unspported CTR tecture format");
            }
        }

        public byte ConvertGeneralTextureTypeToPlatform(ImageFormats generalFmt)
        {
            switch (generalFmt)
            {
                case ImageFormats.RGBA8:
                    return (byte)TextureFormatType.RGBA8888;
                case ImageFormats.RGB8:
                    return (byte)TextureFormatType.RGB888;
                case ImageFormats.RGB5A1:
                    return (byte)TextureFormatType.RGBA5551;
                case ImageFormats.RGB565:
                    return (byte)TextureFormatType.RGB565;
                case ImageFormats.RGBA4:
                    return (byte)TextureFormatType.RGBA4444;
                case ImageFormats.L8A:
                    return (byte)TextureFormatType.LA88;
                case ImageFormats.HL8:
                    return (byte)TextureFormatType.HL8;
                case ImageFormats.L8:
                    return (byte)TextureFormatType.L8;
                case ImageFormats.A8:
                    return (byte)TextureFormatType.A8;
                case ImageFormats.L4A:
                    return (byte)TextureFormatType.LA44;
                case ImageFormats.L4:
                    return (byte)TextureFormatType.L4;
                case ImageFormats.A4:
                    return (byte)TextureFormatType.A4;
                case ImageFormats.ETC1:
                    return (byte)TextureFormatType.ETC1;
                case ImageFormats.ETC1A4:
                    return (byte)TextureFormatType.ETC1A4;
                default:
                    return (byte)TextureFormatType.RGBA8888; //In case it's something unknown, default to RGBA8
            }
        }
    }
}