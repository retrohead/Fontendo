using System.Windows;
using System.Windows.Controls;

namespace Fontendo.Controls
{
    public partial class EndianessPicker : UserControl
    {
        public EndianessPicker()
        {
            InitializeComponent();
        }

        // DependencyProperty so you can bind it in XAML
        public Endianness.Endian Endianess
        {
            get { return (Endianness.Endian)GetValue(LittleEndianProperty); }
            set 
            {
                SetValue(LittleEndianProperty, value);
                UpdateButton();
            }
        }

        public static readonly DependencyProperty LittleEndianProperty =
            DependencyProperty.Register(
                nameof(Endianess),
                typeof(Endianness.Endian),
                typeof(EndianessPicker),
                new PropertyMetadata(Endianness.Endian.Little, OnEndianChanged));

        private static void OnEndianChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EndianessPicker)d;
            ctrl.UpdateButton();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if(Endianess == Endianness.Endian.Little)
                Endianess = Endianness.Endian.Big;
            else
                Endianess = Endianness.Endian.Little;
        }

        private void UpdateButton()
        {
            ToggleButton.Content = Endianess == Endianness.Endian.Little ? "LE" : "BE";
            ToggleButton.ToolTip = Endianess == Endianness.Endian.Little ? "Little Endian" : "Big Endian";
        }
    }
}
