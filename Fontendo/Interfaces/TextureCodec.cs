using Fontendo.Extensions;

namespace Fontendo.Interfaces
{
    public interface ITextureCodec
    {
        byte[] DecodeTexture(ushort texFmt, BinaryReaderX br, ushort width, ushort height);
        byte[] EncodeTexture(ushort texFmt, byte[] data, ushort width, ushort height);
        byte GetTextureType(ushort texFmt);


        public delegate byte[] DecodeFunc(BinaryReaderX br, ushort width, ushort height);
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
