using System;
using System.Collections.Generic;
using static Fontendo.FontProperties.PropertyList;

namespace Fontendo.FontProperties
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
            CharWidth,
            Image,
            MaskImage
        }

        /// <summary>
        /// Descriptor for a glyph property: index, name, type, default, range.
        /// </summary>
        public sealed class GlyphPropertyDescriptor
        {
            public int Index { get; set; }
            public string Name { get; set; }
            public PropertyList.PropertyValueType PropType { get; set; }
            public (long Min, long Max)? ValueRange { get; set; }

            /// <summary>
            /// Optional preferred control type for editing this property.
            /// Can be EditorType.None if no editing is intended.
            /// </summary>
            public EditorType PreferredControl { get; set; }

            public GlyphPropertyDescriptor(
                int index,
                string name,
                PropertyList.PropertyValueType propType,
                EditorType preferredControl = EditorType.None,
                (long Min, long Max)? valueRange = null)
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

        /// <summary>
        /// Strongly-typed glyph property entry with value payload and shared descriptor.
        /// </summary>
        public class GlyphPropertyEntry<T>
        {
            public GlyphPropertyDescriptor Descriptor { get; protected set; }
            public T Value { get; set; }

            public GlyphPropertyEntry(GlyphPropertyDescriptor descriptor, T value)
            {
                Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
                Value = value;
            }

            public GlyphPropertyEntry(
                int index,
                string name,
                T value,
                EditorType preferredControl,
                PropertyValueType valueType,
                (long Min, long Max)? valueRange = null)
            {
                Descriptor = new GlyphPropertyDescriptor(index, name, valueType, preferredControl, valueRange);
                Value = value;
            }


        }


        /// <summary>
        /// Registry holding descriptors and values for all glyph properties.
        /// </summary>
        public class GlyphPropertyRegistry
        {
            public Dictionary<GlyphProperty, GlyphPropertyDescriptor> GlyphPropertyDescriptors { get; }
                = new();

            public Dictionary<GlyphProperty, object> GlyphPropertyValues { get; }
                = new();

            public void AddProperty(GlyphProperty prop,
                                    string name,
                                    PropertyValueType valueType,
                                    EditorType preferredControl,
                                    (long Min, long Max)? range = null)
            {
                var descriptor = new GlyphPropertyDescriptor(
                    index: (int)prop,
                    name: name,
                    propType: valueType,
                    preferredControl: preferredControl,
                    valueRange: range);

                GlyphPropertyDescriptors[prop] = descriptor;
            }

            public T GetValue<T>(GlyphProperty prop)
            {
                if (GlyphPropertyValues[prop].GetType() == typeof(T))
                    return (T)GlyphPropertyValues[prop];
                return (T)Convert.ChangeType(GlyphPropertyValues[prop], typeof(T));
            }

            public void SetValue(GlyphProperty prop, object value)
            {
                if (GlyphPropertyDescriptors.TryGetValue(prop, out var desc))
                {
                    if (desc.ValueRange.HasValue && value is IConvertible)
                    {
                        long val = Convert.ToInt64(value);
                        if (val < desc.ValueRange.Value.Min || val > desc.ValueRange.Value.Max)
                            throw new ArgumentOutOfRangeException(nameof(value),
                                $"Value {val} is outside of range {desc.ValueRange.Value.Min}-{desc.ValueRange.Value.Max}");
                    }
                }
                GlyphPropertyValues[prop] = value;
            }
            public void UpdateValueRange(GlyphProperty prop, (long, long) range)
            {
                if (!GlyphPropertyDescriptors.TryGetValue(prop, out var desc))
                    throw new KeyNotFoundException($"Descriptor for {prop} not found.");

                if (range.Item1 > range.Item2)
                    throw new ArgumentException("Min cannot be greater than Max.");

                desc.UpdateValueRange(range);
            }
        }
    }
}
