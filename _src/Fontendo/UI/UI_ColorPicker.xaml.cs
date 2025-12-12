using Fontendo.DockManager;
using Fontendo.Extensions;
using System.Drawing;
using System.Runtime.CompilerServices;
using static Fontendo.Controls.ColorPicker;

namespace Fontendo.UI
{
    /// <summary>
    /// Interaction logic for UI_ColorPickerFrame.xaml
    /// </summary>
    public partial class UI_ColorPicker : CustomWindowContentBase
    {

        public event EventHandler<ColorChangedEventArgs>? PreviewSelectedColorChanged;

        public Color SelectedColor
        {
            get => ColorHelper.ToDrawingColor(ColorPickerControl.SelectedColor);
        }

        public UI_ColorPicker(System.Windows.Media.Color SelectedColor, bool EnableAlphaChannel)
        {
            InitializeComponent();
            ColorPickerControl.InitialColor = SelectedColor;
            ColorPickerControl.EnableAlphaChannel = EnableAlphaChannel;
        }
        public UI_ColorPicker(Color SelectedColor, bool EnableAlphaChannel)
        {
            InitializeComponent();
            ColorPickerControl.InitialColor = ColorHelper.ToMediaColor(SelectedColor);
            ColorPickerControl.EnableAlphaChannel = EnableAlphaChannel;
        }

        private void btnConfirm_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
            ColorPickerControl.SelectedColor = ColorPickerControl.InitialColor;
            Close();
        }

        private void ColorPickerControl_SelectedColorChanged(object sender, Controls.ColorPicker.ColorChangedEventArgs e)
        {
            PreviewSelectedColorChanged?.Invoke(this, e);
        }
    }
}
