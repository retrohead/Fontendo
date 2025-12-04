using Fontendo.Controls;
using Fontendo.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            Separator,
            Image
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


        /// <summary>
        /// Strongly-typed glyph property entry with value payload and shared descriptor.
        /// </summary>
        public class PropertyValue<T> : INotifyPropertyChanged
        {

            private T? _value;
            public T? Value
            {
                get => _value;
                set
                {
                    if (!Equals(_value, value))
                    {
                        _value = value;
                        OnPropertyChanged(nameof(Value));
                    }
                }
            }

            public PropertyValue(T value)
            {
                Value = value;
            }


            public event PropertyChangedEventHandler? PropertyChanged;
            protected void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static Control? CreateControlForEditorType(EditorType editorType,dynamic? descriptor, Binding? binding)
        {
            Control? editor = null;
            switch (editorType)
            {
                case EditorType.None:
                    break;
                case EditorType.EndiannessPicker:
                    editor = new EndianessPicker
                    {
                        Dock = DockStyle.Fill,
                        DataBindings = { binding }
                    };
                    break;
                case EditorType.CodePointPicker:
                    editor = new HexNumericUpDown
                    {
                        Dock = DockStyle.Fill,
                        Minimum = descriptor.ValueRange.Item1,
                        Maximum = descriptor.ValueRange.Item2,
                        TextAlign = HorizontalAlignment.Center,
                        DataBindings = { binding }
                    };
                    break;

                case EditorType.TextBox:
                    editor = new TextBox 
                    {
                        Dock = DockStyle.Fill,
                        DataBindings = { binding }
                    };
                    break;

                case EditorType.NumberBox:
                    editor = new NumericUpDown
                    {
                        Dock = DockStyle.Fill,
                        Minimum = descriptor.ValueRange.Item1,
                        Maximum = descriptor.ValueRange.Item2,
                        TextAlign = HorizontalAlignment.Center,
                        DataBindings = { binding }
                    };
                    break;

                case EditorType.ComboBox:
                    editor = new ComboBox
                    {
                        Dock = DockStyle.Fill,
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        DataBindings = { binding }
                    };
                    // TODO: populate items based on descriptor metadata
                    break;

                case EditorType.Slider:
                    editor = new TrackBar
                    {
                        Dock = DockStyle.Fill,
                        Minimum = (int)(descriptor.ValueRange?.Min ?? 0),
                        Maximum = (int)(descriptor.ValueRange?.Max ?? 100),
                        DataBindings = { binding }
                    };
                    break;

                case EditorType.CheckBox:
                    editor = new CheckBox
                    {
                        Dock = DockStyle.Left,
                        DataBindings = { binding }
                    };
                    break;

                case EditorType.ColorPicker:
                    editor = new ColorPickerButton
                    {
                        Dock = DockStyle.Left,
                        Text = "Pick Color",
                        DataBindings = { binding }
                    };
                    break;
                case EditorType.Label:
                    editor = new Label
                    {
                        Dock = DockStyle.Left,
                        DataBindings = { binding }
                    };
                    break;
                default:
                    throw new Exception($"Editor type {editorType} not handled");
            }
            return editor;
        }
    
        public static string? GetBindingMemberForControl(EditorType editorType)
        {
            return editorType switch
            {
                EditorType.None => null,
                EditorType.EndiannessPicker => "Endian",
                EditorType.Label => "Text",
                EditorType.TextBox => "Text",
                EditorType.CodePointPicker => "HexValue",
                EditorType.NumberBox => "Value",
                EditorType.ComboBox => "SelectedItem",
                EditorType.Slider => "Value",
                EditorType.CheckBox => "Checked",
                EditorType.ColorPicker => "SelectedColor",
                _ => null
            };
        }
    }
}
