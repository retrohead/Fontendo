using Fontendo.Extensions;
using Fontendo.Interfaces;
using static Fontendo.Extensions.PropertyList;

namespace Fontendo.Formats
{
    public class FontProperties
    {
        /// <summary>
        /// Font property indexes.
        /// </summary>
        public enum FontProperty : int
        {
            // Shared
            Endianness,
            CharEncoding,
            LineFeed,
            Height,
            Width,
            Ascent,
            Baseline,
            Version,

            // NTR-specific
            NtrBpp,
            NtrVertical,
            NtrRotation,
            NtrGameFreak,

            // RVL only
            NtrRvlImageFormat
        }

        /// <summary>
        /// Gets a typed property's value from the list at the given index.
        /// </summary>
        public static T? GetFontPropertyValue<T>(IFontendoFont f, FontProperty property)
        {
            var propdescriptor = f.FontProperties.GetValueOrDefault(property);
            var prop = propdescriptor as Property<T>;
            if (prop == null)
                return default;
            return prop.Value;
        }

        /// <summary>
        /// Sets a typed property's value in the list at the given index.
        /// </summary>
        public static void SetFontPropertyValue<T>(IFontendoFont f, FontProperty property, dynamic value)
        {
            var prop = f.FontProperties.GetValueOrDefault(property);
            if (prop is Property<T> pt)
                pt.Value = value;
        }
    }
}
