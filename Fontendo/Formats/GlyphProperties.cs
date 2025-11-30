using Fontendo.Extensions;
using Fontendo.Interfaces;
using static Fontendo.Extensions.PropertyList;

namespace Fontendo.Formats
{
    public class GlyphProperties
    {
        /// <summary>
        /// Glyph property indexes for fonts.
        /// </summary>
        public enum GlyphProperty : int
        {
            Index,
            Code,
            Left,
            GlyphWidth,
            CharWidth
        }

        /// <summary>
        /// Gets a typed property's value from the list at the given index.
        /// </summary>
        public static T? GetGlyphPropertyValue<T>(Glyph g, GlyphProperty property)
        {
            var prop = g.Properties?.GetValueOrDefault(property);
            return prop is Property<T> pt ? pt.Value : default;
        }

        /// <summary>
        /// Sets a typed property's value in the list at the given index.
        /// </summary>
        public static void SetGlyphPropertyValue<T>(Glyph g, GlyphProperty property, dynamic value)
        {
            var prop = g.Properties?.GetValueOrDefault(property);
            if (prop is Property<T> pt)
                pt.Value = value;
        }
    }
}
