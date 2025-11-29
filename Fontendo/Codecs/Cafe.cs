using Fontendo.Extensions;
using Fontendo.Interfaces;
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


        public byte[] DecodeTexture(ushort texFmt, BinaryReaderX br, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        public byte[] EncodeTexture(ushort texFmt, byte[] data, ushort width, ushort height)
        {
            throw new NotImplementedException();
        }

        public byte GetTextureType(ushort texFmt)
        {
            throw new NotImplementedException();
        }
    }
}