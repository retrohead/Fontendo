using Fontendo.Extensions;
using Fontendo.Interfaces;
using static Fontendo.Interfaces.ITextureCodec;

namespace Fontendo.Codecs.NX
{
    public class NXTextureCodec : Fontendo.Interfaces.ITextureCodec
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