using Fontendo.Extensions;
using Fontendo.Interfaces;
using static Fontendo.Interfaces.ITextureCodec;

namespace Fontendo.Codecs.RVL
{
    public class RVLTextureCodec : Fontendo.Interfaces.ITextureCodec
    {
        public Dictionary<TextureFormatType, TextureFormatData> TextureFormatFunctions { get; }

        public enum TextureFormatType : int
        {
            None = 0x0,
        }

        public RVLTextureCodec()
        {
            TextureFormatFunctions = new Dictionary<TextureFormatType, TextureFormatData>();
            /*
            TextureFormats = new Dictionary<byte, DecodeFunc>
            {
                { 0x0, decodeI4 },
                { 0x1, decodeI8 },
                { 0x2, decodeIA4 },
                { 0x3, decodeIA8 },
                { 0x4, decodeRGB565 },
                { 0x5, decodeRGB5A3 },
                { 0x6, decodeRGBA8 },

                // Unimplemented formats wired to a stub
                { 0x8, ITextureCodec.NotImplemented("C4") },
                { 0x9, ITextureCodec.NotImplemented("C8") },
                { 0xA, ITextureCodec.NotImplemented("C14X2") },
                { 0xE, ITextureCodec.NotImplemented("CMPR") }
            };
            */
        }

        private byte[] decodeRGBA8(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] decodeRGB5A3(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] decodeRGB565(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] decodeIA8(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] decodeIA4(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] decodeI8(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        private byte[] decodeI4(BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        public byte[] DecodeTexture(ushort texFmt, BinaryReaderX br, ushort width, ushort height)
        {
            if (texFmt > 0xFF)
                throw new ArgumentOutOfRangeException("Texture format is out of range");
            if (TextureFormatFunctions.TryGetValue((TextureFormatType)texFmt, out var formatdata))
            {
                return formatdata.DecodeFunction(br, width, height);
            }

            throw new ArgumentOutOfRangeException(nameof(texFmt), texFmt, $"Unknown RVL texture format code {texFmt}");

        }

        public byte[] EncodeTexture(ushort texFmt, byte[] data, ushort width, ushort height)
        {
            throw new NotImplementedException();
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