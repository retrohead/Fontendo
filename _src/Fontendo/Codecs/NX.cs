using Fontendo.Extensions;
using Fontendo.Extensions.BinaryTools;
using Fontendo.Interfaces;
using static Fontendo.Interfaces.ITextureCodec;

namespace Fontendo.Codecs.NX
{
    public class NXTextureCodec : ITextureCodec
    {
        public Dictionary<TextureFormatType, TextureFormatData> TextureFormatFunctions { get; }

        public enum TextureFormatType : int
        {
            None = 0x0,
        }

        public NXTextureCodec()
        {
            TextureFormatFunctions = new Dictionary<TextureFormatType, TextureFormatData>();
        }

        public DecodedTextureType DecodeTexture(ushort texFmt, BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
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

        public DecodedTextureType DecodeBitmap(ushort bpp, BitReader bitr, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }
    }
}