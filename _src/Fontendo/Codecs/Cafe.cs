using Fontendo.Extensions;
using Fontendo.Extensions.BinaryTools;
using Fontendo.Interfaces;
using System.Drawing;
using static Fontendo.Interfaces.ITextureCodec;

namespace Fontendo.Codecs.CAFE
{
    public class CAFETextureCodec : ITextureCodec
    {
        public Dictionary<TextureFormatType, TextureFormatData> TextureFormatFunctions { get; }

        public enum TextureFormatType : byte
        {
            None = 0x0,
        }

        public CAFETextureCodec()
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


        // NTR ONLY
        public DecodedTextureType DecodeBitmap(ushort bpp, BitReader bitr, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        public void EncodeBitmap(Bitmap image, Span<byte> span, byte ntrBpp, int x, int y)
        {
            throw new NotImplementedException();
        }
    }
}