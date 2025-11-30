using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fontendo.Extensions
{
    public class PropertyList
    {
        /// <summary>
        /// Data type of a property value for dynamic GUI and serialization.
        /// Matches the original C++ enum ordering.
        /// </summary>
        public enum PropertyType : byte
        {
            Bool,
            Byte,
            SByte,
            UInt16,
            Int16,
            UInt32,
            Int32,
            UInt64,
            Int64,
            StdString,
            ImageFormat,
            CharEncoding,
            StdPairUInt32UInt32,
            Separator
        }

        /// <summary>
        /// Descriptor shared across properties of the same kind (e.g., glyph entries).
        /// Holds index, name, type info, control hint, and optional numeric range.
        /// </summary>
        public sealed class PropertyListEntryDescriptor
        {
            /// <summary>Property index in a list (avoids passing the index around).</summary>
            public int Index { get; set; }

            /// <summary>Human-readable property name.</summary>
            public string Name { get; set; }

            /// <summary>Underlying data type of the property value.</summary>
            public PropertyType PropType { get; set; }

            /// <summary>
            /// Inclusive numeric range when applicable; ignored for non-numeric types.
            /// </summary>
            public (long Min, long Max) ValueRange { get; set; }

            public PropertyListEntryDescriptor(
                int index,
                string name,
                PropertyType propType,
                (long Min, long Max) valueRange = default)
            {
                Index = index;
                Name = name;
                PropType = propType;
                ValueRange = valueRange;
            }
        }

        /// <summary>
        /// Base class for a property entry. Carries a shared descriptor.
        /// </summary>
        public abstract class PropertyBase : IDisposable
        {
            /// <summary>Shared descriptor describing this property.</summary>
            public PropertyListEntryDescriptor? Descriptor { get; protected set; }

            // If you later attach unmanaged resources, manage them here.
            public virtual void Dispose()
            {
                // No unmanaged state to release; placeholder for future use.
            }
        }


        /// <summary>
        /// Strongly-typed property entry with value payload and shared descriptor.
        /// </summary>
        /// <typeparam name="T">Value type (must match descriptor.PropType semantics).</typeparam>
        public sealed class Property<T> : PropertyBase
        {
            /// <summary>The current value of the property.</summary>
            public T Value { get; set; }

            /// <summary>
            /// Constructs a property using an existing shared descriptor.
            /// </summary>
            public Property(PropertyListEntryDescriptor descriptor, T value)
            {
                Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
                Value = value;
            }

            /// <summary>
            /// Constructs a property and also creates a new descriptor.
            /// Useful when you don't have a shared descriptor prepared.
            /// </summary>
            public Property(
                int index,
                string name,
                T value,
                PropertyType propType,
                (long Min, long Max) valueRange = default)
            {
                Descriptor = new PropertyListEntryDescriptor(index, name, propType, valueRange);
                Value = value;
            }
        }
    }
}
