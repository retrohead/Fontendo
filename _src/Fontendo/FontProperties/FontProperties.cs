using Newtonsoft.Json.Linq;
using System.ComponentModel;
using static Fontendo.FontProperties.PropertyList;

namespace Fontendo.FontProperties
{
    public class FontPropertyList
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
        /// Descriptor shared across properties of the same kind (e.g., glyph entries).
        /// Holds index, name, type info, control hint, and optional numeric range.
        /// </summary>
        public sealed class FontPropertyDescriptor
        {
            /// <summary>
            /// Property index in a list (avoids passing the index around).
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// Human-readable property name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Underlying data type of the property value.
            /// </summary>
            public PropertyValueType PropType { get; set; }

            /// <summary>
            /// Inclusive numeric range when applicable; ignored for non-numeric types.
            /// </summary>
            public (long Min, long Max)? ValueRange { get; set; }

            /// <summary>
            /// Optional preferred control type for editing this property.
            /// Can be EditorType.None if no editing is intended.
            /// </summary>
            public EditorType PreferredControl { get; set; }

            public FontPropertyDescriptor(
                int index,
                string name,
                PropertyValueType propType,
                EditorType preferredControl,
                (long Min, long Max)? valueRange = null
                )
            {
                Index = index;
                Name = name;
                PropType = propType;
                ValueRange = valueRange;
                PreferredControl = preferredControl;
            }

            /// <summary>
            /// Update the inclusive numeric range for this descriptor.
            /// </summary>
            public void UpdateValueRange((long, long) range)
            {
                if (range.Item1 > range.Item2)
                    throw new ArgumentException("Min cannot be greater than Max.");

                ValueRange = range;
            }
        }

        public class FontPropertyRegistry
        {
            // Shared descriptors for each property type
            public Dictionary<FontProperty, FontPropertyDescriptor> FontPropertyDescriptors { get; }
                = new();

            // Actual property values keyed by property type
            public Dictionary<FontProperty, object> FontProperties { get; }
                = new();

            /// <summary>
            /// Add a property descriptor and its initial value to the registry.
            /// </summary>
            public void AddProperty(FontProperty propType,
                                    string name,
                                    PropertyValueType valueType,
                                    EditorType preferredControl,
                                    (long Min, long Max)? range = null)
            {
                var descriptor = new FontPropertyDescriptor(
                    index: (int)propType,
                    name: name,
                    propType: valueType,
                    preferredControl: preferredControl,
                    valueRange: range);

                FontPropertyDescriptors[propType] = descriptor;
            }

            /// <summary>
            /// Get a strongly-typed property value.
            /// </summary>
            public T? GetValue<T>(FontProperty propType)
            {
                if (FontProperties[propType].GetType() == typeof(T))
                    return (T)FontProperties[propType];

                // Handle numeric widening explicitly
                if (typeof(T) == typeof(int) && FontProperties[propType] is byte b)
                    return (T)(object)(int)b;
                if (typeof(T) == typeof(int?) && FontProperties[propType] is byte b2)
                    return (T)(object)(int?)b2;

                if (FontProperties[propType] is T tValue)
                    return tValue;

                return (T)Convert.ChangeType(FontProperties[propType], typeof(T));
            }

            /// <summary>
            /// Update a property value.
            /// </summary>
            public void SetValue(FontProperty propType, object value)
            {
                if (FontPropertyDescriptors.TryGetValue(propType, out var desc))
                {
                    // Optional: validate against range
                    if (desc.ValueRange.HasValue && value is IConvertible)
                    {
                        long val = Convert.ToInt64(value);
                        if (val < desc.ValueRange.Value.Min || val > desc.ValueRange.Value.Max)
                            throw new ArgumentOutOfRangeException(nameof(value), $"Value {val} is outside of range {desc.ValueRange.Value.Min}-{desc.ValueRange.Value.Max} for font property {desc.Name}");
                    }
                }

                FontProperties[propType] = value;
            }
            public void UpdateValueRange(FontProperty prop, (long, long) range)
            {
                if (!FontPropertyDescriptors.TryGetValue(prop, out var desc))
                    throw new KeyNotFoundException($"Descriptor for {prop} not found.");

                if (range.Item1 > range.Item2)
                    throw new ArgumentException("Min cannot be greater than Max.");

                desc.UpdateValueRange(range);
            }
        }

    }
}
