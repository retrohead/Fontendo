using Fontendo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fontendo.Extensions
{
    internal class TextureCodecFactory
    {
        public enum Platform { NTR, RVL, CTR }
        public static ITextureCodec Create(Platform platform) => platform switch
        {
            Platform.RVL => new Fontendo.Codecs.RVL.RVLTextureCodec(),
            Platform.CTR => new Fontendo.Codecs.CTR.CTRTextureCodec(),
            Platform.NTR => new Fontendo.Codecs.NTR.NTRTextureCodec(),
            _ => throw new ArgumentOutOfRangeException(nameof(platform))
        };
    }
}
