using Fontendo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fontendo.FontProperties
{
    public class PropertyList
    {
        /// <summary>
        /// Data type of a property value for dynamic GUI and serialization.
        /// Matches the original C++ enum ordering.
        /// </summary>
        public enum PropertyValueType : byte
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
        public enum EditorType
        {
            None,               // No preferred editor
            TextBox,            // Simple text input
            ComboBox,           // Dropdown selection
            Slider,             // Numeric slider
            CheckBox,           // Boolean toggle
            NumberBox,          // Number selection
            ColorPicker,        // Color selection
            CodePointPicker,    // Custom code point picker
            EndiannessPicker,   // Custom endianess picker
            Label               // Read only label
        }
    }
}
