using Fontendo.Extensions.BinaryTools;
using System.Drawing;
using static Fontendo.Extensions.FontBase;

namespace Fontendo.Interfaces
{
    public interface ITextureCodec
    {
        public class DecodedTextureType
        {
            public byte[] data { get; }
            public byte[]? mask { get; }

            public DecodedTextureType(byte[] data, byte[]? mask)
            {
                this.data = data;
                this.mask = mask;
            }
        }
        public DecodedTextureType DecodeTexture(ushort texFmt, BinaryReaderX br, ushort width, ushort height);

        public DecodedTextureType DecodeBitmap(ushort bpp, BitReader bitr, ushort width, ushort height);

        byte[] EncodeTexture(ushort texFmt, byte[] data, ushort width, ushort height);
        public byte ConvertGeneralTextureTypeToPlatform(ImageFormats generalFmt);
        public ImageFormats ConvertPlatformTextureTypeToGeneral(ushort texFmt);


        public delegate DecodedTextureType DecodeFunc(BinaryReaderX br, ushort width, ushort height);
        public delegate byte[] EncodeFunc(byte[] argbBuf, ushort width, ushort height);

        public class TextureFormatData
        {
            public byte TextureFormatTypeByte;
            public DecodeFunc? DecodeFunction;
            public EncodeFunc? EncodeFunction;

            public TextureFormatData(byte textureFormatTypeByte, DecodeFunc decodeFunction, EncodeFunc encodeFunction)
            {
                TextureFormatTypeByte = textureFormatTypeByte;
                DecodeFunction = decodeFunction;
                EncodeFunction = encodeFunction;
            }
        }

        // default stub to use for unimplemented formats
        static DecodeFunc NotImplemented(string formatName) =>
                (br, w, h) => throw new NotImplementedException($"Format {formatName} not implemented");
    }
}
