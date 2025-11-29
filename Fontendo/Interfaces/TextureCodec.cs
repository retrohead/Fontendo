using Fontendo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fontendo.Interfaces
{
    public interface ITextureCodec
    {
        byte[] DecodeTexture(ushort texFmt, BinaryReaderX br, ushort width, ushort height);
        byte[] EncodeTexture(ushort texFmt, byte[] data, ushort width, ushort height);
        byte GeneralTextureType(ushort texFmt);
        byte PlatformTextureType(byte generalFmt);


        public delegate byte[] DecodeFunc(BinaryReaderX br, ushort width, ushort height);

        public class TextureFormatData
        {
            public byte TextureFormatTypeByte;
            public DecodeFunc? DecodeFunction;

            public TextureFormatData(byte textureFormatTypeByte, DecodeFunc decodeFunction)
            {
                TextureFormatTypeByte = textureFormatTypeByte;
                DecodeFunction = decodeFunction;
            }
        }

        // default stub to use for unimplemented formats
        static DecodeFunc NotImplemented(string formatName) =>
                (br, w, h) => throw new NotImplementedException($"Format {formatName} not implemented");
    }
}
