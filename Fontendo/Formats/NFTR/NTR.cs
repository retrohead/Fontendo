using Fontendo.Extensions;

namespace Fontendo.Formats.NFTR
{
    public class NTR
    {
        /// <summary>
        /// Glyph property indexes for NFTR/CFNT fonts.
        /// </summary>
        public enum NitroGlyphProp : int
        {
            Index = 0,
            Code = 1,
            Left = 2,
            GlyphWidth = 3,
            CharWidth = 4
        }
        /// <summary>
        /// Font property indexes for NFTR/CFNT fonts.
        /// Shared + NTR-specific.
        /// </summary>
        public enum NitroFontProp : int
        {
            // Shared
            Endianness = 0,
            CharEncoding = 1,
            Linefeed = 2,
            Height = 3,
            Width = 4,
            Ascent = 5,
            Baseline = 6,
            Version = 7,

            // NTR-specific
            NtrBpp = 8,
            NtrVertical = 9,
            NtrRotation = 10,
            NtrGameFreak = 11
        }

        /// <summary>
        /// Gets a typed property's value from the list at the given index.
        /// </summary>
        private static T GetValue<T>(List<PropertyList.PropertyBase> props, int index)
        {
            return ((PropertyList.Property<T>)props[index]).Value;
        }

        /// <summary>
        /// Sets a typed property's value in the list at the given index.
        /// </summary>
        private static void SetValue<T>(List<PropertyList.PropertyBase> props, int index, T value)
        {
            ((PropertyList.Property<T>)props[index]).Value = value;
        }

        // ---- NITRO GLYPH ----

        /// <summary>Gets the glyph index.</summary>
        public static ushort GetGlyphIndex(List<PropertyList.PropertyBase> props)
            => GetValue<ushort>(props, (int)NitroGlyphProp.Index);

        /// <summary>Gets the glyph code point.</summary>
        public static ushort GetGlyphCode(List<PropertyList.PropertyBase> props)
            => GetValue<ushort>(props, (int)NitroGlyphProp.Code);

        /// <summary>Gets the glyph left bearing.</summary>
        public static sbyte GetGlyphLeft(List<PropertyList.PropertyBase> props)
            => GetValue<sbyte>(props, (int)NitroGlyphProp.Left);

        /// <summary>Gets the glyph bitmap width.</summary>
        public static byte GetGlyphGlyphWidth(List<PropertyList.PropertyBase> props)
            => GetValue<byte>(props, (int)NitroGlyphProp.GlyphWidth);

        /// <summary>Gets the advance (character) width.</summary>
        public static byte GetGlyphCharWidth(List<PropertyList.PropertyBase> props)
            => GetValue<byte>(props, (int)NitroGlyphProp.CharWidth);

        /// <summary>Sets the glyph index.</summary>
        public static void SetGlyphIndex(List<PropertyList.PropertyBase> props, ushort val)
            => SetValue<ushort>(props, (int)NitroGlyphProp.Index, val);

        /// <summary>Sets the glyph code point.</summary>
        public static void SetGlyphCode(List<PropertyList.PropertyBase> props, ushort val)
            => SetValue<ushort>(props, (int)NitroGlyphProp.Code, val);

        /// <summary>Sets the glyph left bearing.</summary>
        public static void SetGlyphLeft(List<PropertyList.PropertyBase> props, sbyte val)
            => SetValue<sbyte>(props, (int)NitroGlyphProp.Left, val);

        /// <summary>Sets the glyph bitmap width.</summary>
        public static void SetGlyphGlyphWidth(List<PropertyList.PropertyBase> props, byte val)
            => SetValue<byte>(props, (int)NitroGlyphProp.GlyphWidth, val);

        /// <summary>Sets the advance (character) width.</summary>
        public static void SetGlyphCharWidth(List<PropertyList.PropertyBase> props, byte val)
            => SetValue<byte>(props, (int)NitroGlyphProp.CharWidth, val);

        // ---- NITRO FONT ----

        /// <summary>Gets font endianness.</summary>
        public static bool GetFontEndianness(List<PropertyList.PropertyBase> props)
            => GetValue<bool>(props, (int)NitroFontProp.Endianness);

        /// <summary>Gets font character encoding.</summary>
        public static byte GetFontCharEncoding(List<PropertyList.PropertyBase> props)
            => GetValue<byte>(props, (int)NitroFontProp.CharEncoding);

        /// <summary>Gets font linefeed.</summary>
        public static byte GetFontLinefeed(List<PropertyList.PropertyBase> props)
            => GetValue<byte>(props, (int)NitroFontProp.Linefeed);

        /// <summary>Gets font height.</summary>
        public static byte GetFontHeight(List<PropertyList.PropertyBase> props)
            => GetValue<byte>(props, (int)NitroFontProp.Height);

        /// <summary>Gets font width.</summary>
        public static byte GetFontWidth(List<PropertyList.PropertyBase> props)
            => GetValue<byte>(props, (int)NitroFontProp.Width);

        /// <summary>Gets font ascent.</summary>
        public static byte GetFontAscent(List<PropertyList.PropertyBase> props)
            => GetValue<byte>(props, (int)NitroFontProp.Ascent);

        /// <summary>Gets font baseline.</summary>
        public static byte GetFontBaseline(List<PropertyList.PropertyBase> props)
            => GetValue<byte>(props, (int)NitroFontProp.Baseline);

        /// <summary>Gets font version.</summary>
        public static ushort GetFontVersion(List<PropertyList.PropertyBase> props)
            => GetValue<ushort>(props, (int)NitroFontProp.Version);

        /// <summary>Sets font endianness.</summary>
        public static void SetFontEndianness(List<PropertyList.PropertyBase> props, bool val)
            => SetValue<bool>(props, (int)NitroFontProp.Endianness, val);

        /// <summary>Sets font character encoding.</summary>
        public static void SetFontCharEncoding(List<PropertyList.PropertyBase> props, byte val)
            => SetValue<byte>(props, (int)NitroFontProp.CharEncoding, val);

        /// <summary>Sets font linefeed.</summary>
        public static void SetFontLinefeed(List<PropertyList.PropertyBase> props, byte val)
            => SetValue<byte>(props, (int)NitroFontProp.Linefeed, val);

        /// <summary>Sets font height.</summary>
        public static void SetFontHeight(List<PropertyList.PropertyBase> props, byte val)
            => SetValue<byte>(props, (int)NitroFontProp.Height, val);

        /// <summary>Sets font width.</summary>
        public static void SetFontWidth(List<PropertyList.PropertyBase> props, byte val)
            => SetValue<byte>(props, (int)NitroFontProp.Width, val);

        /// <summary>Sets font ascent.</summary>
        public static void SetFontAscent(List<PropertyList.PropertyBase> props, byte val)
            => SetValue<byte>(props, (int)NitroFontProp.Ascent, val);

        /// <summary>Sets font baseline.</summary>
        public static void SetFontBaseline(List<PropertyList.PropertyBase> props, byte val)
            => SetValue<byte>(props, (int)NitroFontProp.Baseline, val);

        /// <summary>Sets font version.</summary>
        public static void SetFontVersion(List<PropertyList.PropertyBase> props, ushort val)
            => SetValue<ushort>(props, (int)NitroFontProp.Version, val);

        // ---- NTR-only ----

        /// <summary>Gets NTR font bits-per-pixel (BPP).</summary>
        public static byte GetNtrFontBpp(List<PropertyList.PropertyBase> props)
            => GetValue<byte>(props, (int)NitroFontProp.NtrBpp);

        /// <summary>Gets NTR vertical layout flag.</summary>
        public static bool GetNtrFontVertical(List<PropertyList.PropertyBase> props)
            => GetValue<bool>(props, (int)NitroFontProp.NtrVertical);

        /// <summary>Gets NTR rotation value.</summary>
        public static byte GetNtrFontRotation(List<PropertyList.PropertyBase> props)
            => GetValue<byte>(props, (int)NitroFontProp.NtrRotation);

        /// <summary>Sets NTR font bits-per-pixel (BPP).</summary>
        public static void SetNtrFontBpp(List<PropertyList.PropertyBase> props, byte val)
            => SetValue<byte>(props, (int)NitroFontProp.NtrBpp, val);

        /// <summary>Sets NTR vertical layout flag.</summary>
        public static void SetNtrFontVertical(List<PropertyList.PropertyBase> props, bool val)
            => SetValue<bool>(props, (int)NitroFontProp.NtrVertical, val);

        /// <summary>Sets NTR rotation value.</summary>
        public static void SetNtrFontRotation(List<PropertyList.PropertyBase> props, byte val)
            => SetValue<byte>(props, (int)NitroFontProp.NtrRotation, val);
    }
}
