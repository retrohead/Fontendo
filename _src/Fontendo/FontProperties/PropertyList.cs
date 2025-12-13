using Fontendo.Controls;
using Fontendo.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using Xceed.Wpf.Toolkit;
using Fontendo.UI;
using System.Windows.Media;

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

        public static FrameworkElement? CreateControlForEditorType(EditorType editorType, dynamic? descriptor, List<Binding>? bindings)
        {
            FrameworkElement? editor = null;

            switch (editorType)
            {
                case EditorType.None:
                    break;

                case EditorType.EndiannessPicker:
                    editor = new EndianessPicker
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 75, Height = 25, Margin = new Thickness(0,0,0,0), Endianess = Endianness.Endian.Little
                    };
                    break;

                case EditorType.CodePointPicker:
                    editor = new NumericUpDown
                    {
                        Minimum = descriptor.ValueRange.Item1,
                        Maximum = descriptor.ValueRange.Item2,
                        HorizontalContentAlignment = HorizontalAlignment.Stretch,
                        DisplayAsHex = true,
                        Background = (Brush)MainWindow.Self.FindResource("WindowBackgroundBrushMedium")
                    };
                    break;

                case EditorType.TextBox:
                    editor = new TextBox
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };
                    break;

                case EditorType.NumberBox:
                    // If you’re using Extended WPF Toolkit’s IntegerUpDown
                    var numBox = new NumericUpDown
                    {
                        Minimum = (int)descriptor.ValueRange.Item1,
                        Maximum = (int)descriptor.ValueRange.Item2,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Foreground = (Brush)MainWindow.Self.FindResource("ControlTextActive"),
                        Background = (Brush)MainWindow.Self.FindResource("WindowBackgroundBrushMedium")
                    };
                    editor = numBox;
                    break;

                case EditorType.ComboBox:
                    editor = new ComboBox
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        IsEditable = false
                    };
                    // TODO: populate items based on descriptor metadata
                    break;

                case EditorType.Slider:
                    editor = new Slider
                    {
                        Minimum = (int)(descriptor.ValueRange?.Min ?? 0),
                        Maximum = (int)(descriptor.ValueRange?.Max ?? 100),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Style = (Style)MainWindow.Self.FindResource("SliderStyle")
                    };
                    break;

                case EditorType.CheckBox:
                    editor = new CheckBox
                    {
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    break;

                case EditorType.ColorPicker:
                    //editor = new ColorPickerButton
                    //{
                    //    Content = "Pick Color",
                    //    HorizontalAlignment = HorizontalAlignment.Left
                    //};
                    break;

                case EditorType.Label:
                    editor = new Label
                    {
                        Margin = new Thickness(0, 0, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Foreground = (Brush)MainWindow.Self.FindResource("ControlTextInactive")
                    };
                    break;

                default:
                    throw new Exception($"Editor type {editorType} not handled");
            }

            if (editor != null && bindings != null)
            {
                foreach (var binding in bindings)
                {
                    // In WPF you bind to a DependencyProperty, not DataBindings
                    if (editor is TextBox tb)
                        tb.SetBinding(TextBox.TextProperty, binding);
                    else if (editor is Slider sl)
                        sl.SetBinding(Slider.ValueProperty, binding);
                    else if (editor is CheckBox cb)
                        cb.SetBinding(CheckBox.IsCheckedProperty, binding);
                    else if (editor is Label lbl)
                        lbl.SetBinding(Label.ContentProperty, binding);
                    else if (editor is ComboBox combo)
                        combo.SetBinding(ComboBox.SelectedItemProperty, binding);
                    else if (editor is NumericUpDown num)
                        num.SetBinding(NumericUpDown.ValueProperty, binding);
                    else if (editor is EndianessPicker endian)
                        endian.SetBinding(EndianessPicker.LittleEndianProperty, binding);
                    else
                        // add other cases as needed
                        throw new Exception($"{editor.GetType()} not handled");
                }
            }
            return editor;
        }

        public static string? GetBindingMemberForControl(EditorType editorType)
        {
            return editorType switch
            {
                EditorType.None => null,
                EditorType.EndiannessPicker => "Endianess",
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
