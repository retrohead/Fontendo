using Fontendo.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fontendo.Controls
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        public event EventHandler? ValueChanged;

        public NumericUpDown()
        {
            InitializeComponent();
            PART_TextBox.Text = Value.ToString();

            UpButton.Click += (s, e) => { Value += Increment; PART_TextBox.Focus(); PART_TextBox.SelectAll(); };
            DownButton.Click += (s, e) => { Value -= Increment; PART_TextBox.Focus(); PART_TextBox.SelectAll(); };
            PART_TextBox.LostFocus += (s, e) => ValidateText();
        }


        private void UpdateTextBox()
        {
            PART_TextBox.Text = DisplayAsHex
                    ? Value.ToString("X" + HexLength)   // hex formatting
                    : Value.ToString();
        }

        private void ValidateText()
        {
            string text = PART_TextBox.Text;

            long newVal;
            if (DisplayAsHex)
            {
                if (long.TryParse(text, System.Globalization.NumberStyles.HexNumber,
                                 System.Globalization.CultureInfo.InvariantCulture, out newVal))
                {
                    Value = Math.Max(Minimum, Math.Min(Maximum, newVal));
                }
            }
            else
            {
                if (long.TryParse(text, out newVal))
                {
                    Value = Math.Max(Minimum, Math.Min(Maximum, newVal));
                }
            }

            PART_TextBox.Text = DisplayAsHex ? Value.ToString("X" + HexLength) : Value.ToString();
            PART_TextBox.SelectAll();
        }

        // Dependency properties
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(long), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(0L, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, CoerveValueCallbal));

        private static object CoerveValueCallbal(DependencyObject d, object baseValue)
        {
            var control = (NumericUpDown)d;
            long value = (long)baseValue;
            return Math.Max(control.Minimum, Math.Min(control.Maximum, value));
        }

        public long Value
        {
            get => (long)GetValue(ValueProperty);
            set
            {
                if (Value != value)
                {
                    SetValue(ValueProperty, value);
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericUpDown)d;
            control.UpdateTextBox();
        }

        public static readonly DependencyProperty DisplayAsHexProperty =
            DependencyProperty.Register(nameof(DisplayAsHex), typeof(bool), typeof(NumericUpDown),
                new PropertyMetadata(false, OnDisplayAsHexChanged));

        public bool DisplayAsHex
        {
            get => (bool)GetValue(DisplayAsHexProperty);
            set => SetValue(DisplayAsHexProperty, value);
        }

        private static void OnDisplayAsHexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericUpDown)d;
            control.UpdateTextBox();
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(long), typeof(NumericUpDown),
                new PropertyMetadata(long.MinValue));

        public long Minimum
        {
            get => (long)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(long), typeof(NumericUpDown),
                new PropertyMetadata(long.MaxValue));

        public long Maximum
        {
            get => (long)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register(nameof(Increment), typeof(long), typeof(NumericUpDown),
                new PropertyMetadata(1L));

        public long Increment
        {
            get => (long)GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }
        public static readonly DependencyProperty HexLengthProperty =
            DependencyProperty.Register(nameof(HexLength), typeof(int), typeof(NumericUpDown),
                new PropertyMetadata(4));

        public int HexLength
        {
            get => (int)GetValue(HexLengthProperty);
            set => SetValue(HexLengthProperty, value);
        }

        private void PART_TextBox_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            if(PART_TextBox.IsFocused)
                return;
            PART_TextBox.Focus();
            PART_TextBox.SelectAll();
            e.Handled = true;
        }

        private void Overlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PART_TextBox.Focus();
            PART_TextBox.SelectAll();
            e.Handled = true;
        }

        private void PART_TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            mainBorder.Background = (Brush)UI_MainWindow.Self.FindResource("ButtonSelectedBrush");
            PART_TextBox.Foreground = (Brush)UI_MainWindow.Self.FindResource("ControlTextActive");
            PART_Label.Foreground = (Brush)UI_MainWindow.Self.FindResource("ControlTextActive");
        }

        private void PART_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            mainBorder.Background = (Brush)UI_MainWindow.Self.FindResource("WindowBackgroundBrushMedium");
            PART_TextBox.Foreground = (Brush)UI_MainWindow.Self.FindResource("ControlTextInactive");
            PART_Label.Foreground = (Brush)UI_MainWindow.Self.FindResource("ControlTextInactive");
        }

        private void Grid_GotFocus(object sender, RoutedEventArgs e)
        {
            PART_TextBox.Focus();
            PART_TextBox.SelectAll();
            e.Handled = true;
        }
    }
}
