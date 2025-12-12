using Fontendo.DockManager;
using Fontendo.Extensions;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Fontendo.Controls
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : CustomWindowContentBase
    {
        private bool UpdatingColors = false;
        public class ColorChangedEventArgs : EventArgs
        {
            public Color SelectedColor { get; }

            public ColorChangedEventArgs(Color previewColor)
            {
                SelectedColor = previewColor;
            }
        }
        public event EventHandler<ColorChangedEventArgs>? SelectedColorChanged;

        // DependencyProperty for selected colour
        private static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                nameof(SelectedColor),
                typeof(Color),
                typeof(ColorPicker),
                new PropertyMetadata(Colors.Black, OnSelectedColorChanged));

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ColorPicker)d;
            control.previewPanel.Background = new SolidColorBrush(control.SelectedColor);
            if (!control.textHex.IsFocused)
                control.textHex.Text = ColorHelper.ColorToHex(control.SelectedColor, control.EnableAlphaChannel);

            control.SelectedColorChanged?.Invoke(control, new ColorChangedEventArgs(control.SelectedColor));
            control.UpdateHSBControlsFromSelectedColor();
        }

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set
            {
                if (value != SelectedColor)
                {
                    SetValue(SelectedColorProperty, value);
                }
            }
        }

        // DependencyProperty for enabling alpha channel
        public static readonly DependencyProperty EnableAlphaChannelProperty =
            DependencyProperty.Register(
                nameof(EnableAlphaChannel),
                typeof(bool),
                typeof(ColorPicker),
                new PropertyMetadata(true, OnAlphaChannelChanged));

        private static void OnAlphaChannelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue is bool enableAlpha)
            {
                var control = (ColorPicker)d;
                control.sliderAlpha.Visibility = enableAlpha ? Visibility.Visible : Visibility.Collapsed;
                control.numAlpha.Visibility = enableAlpha ? Visibility.Visible : Visibility.Collapsed;
                control.lblAlpha.Visibility = enableAlpha ? Visibility.Visible : Visibility.Collapsed;
                control.textHex.Text = ColorHelper.ColorToHex(control.SelectedColor, enableAlpha);
            } 
        }

        public bool EnableAlphaChannel
        {
            get => (bool)GetValue(EnableAlphaChannelProperty);
            set => SetValue(EnableAlphaChannelProperty, value);
        }

        // DependencyProperty for selected colour
        public static readonly DependencyProperty InitialColorProperty =
            DependencyProperty.Register(
                nameof(InitialColor),
                typeof(Color),
                typeof(ColorPicker),
                new PropertyMetadata(Colors.Black));


        public Color InitialColor
        {
            get => (Color)GetValue(InitialColorProperty);
            set
            {
                if (value != InitialColor)
                {
                    SetValue(InitialColorProperty, value);

                    UpdatingColors = true;
                    {
                        // updating the colours
                        sliderAlpha.Value = EnableAlphaChannel ? InitialColor.A : 255;
                        sliderRed.Value = InitialColor.R;
                        sliderGreen.Value = InitialColor.G;
                        sliderBlue.Value = InitialColor.B;
                        SelectedColor = value;
                    }
                    UpdatingColors = false;
                    Slider_ARGBValueChanged(this, null);
                }
            }
        }


        public ColorPicker()
        {
            InitializeComponent();
        }

        private void UpdateHSBControlsFromSelectedColor()
        {
            UpdatingColors = true;
            {
                sliderHue.Hue = (int)Math.Round(ColorHelper.GetHueFromColor(SelectedColor));
                sliderSat.Hue = sliderHue.Hue;
                sliderBright.Hue = sliderHue.Hue;

                sliderSat.Saturation = (int)Math.Round(ColorHelper.GetSaturationFromColor(SelectedColor) * 100);
                sliderBright.Brightness = (int)Math.Round(ColorHelper.GetBrightnessFromColor(SelectedColor) * 100);

                numHue.Value = sliderHue.Hue;
                numSat.Value = sliderSat.Saturation;
                numBright.Value = sliderBright.Brightness;
            }
            UpdatingColors = false;
        }


        private void UpdateRGBControlsFromSelectedColor()
        {
            UpdatingColors = true;
            {
                sliderAlpha.Value = SelectedColor.A;
                sliderRed.Value = SelectedColor.R;
                sliderGreen.Value = SelectedColor.G;
                sliderBlue.Value = SelectedColor.B;

                numAlpha.Value = (int)sliderAlpha.Value;
                numRed.Value = (int)sliderRed.Value;
                numGreen.Value = (int)sliderGreen.Value;
                numBlue.Value = (int)sliderBlue.Value;
            }
            UpdatingColors = false;
        }

        private void Slider_ARGBValueChanged(object sender, RoutedPropertyChangedEventArgs<double>? e)
        {
            if (UpdatingColors)
                return;

            UpdatingColors = true;
            {
                numAlpha.Value = (long)sliderAlpha.Value;
                numRed.Value = (long)sliderRed.Value;
                numGreen.Value = (long)sliderGreen.Value;
                numBlue.Value = (long)sliderBlue.Value;
                UpdateSelectedColor(Color.FromArgb((byte)sliderAlpha.Value, (byte)sliderRed.Value, (byte)sliderGreen.Value, (byte)sliderBlue.Value));
                UpdateHSBControlsFromSelectedColor();
            }
            UpdatingColors = false;
        }

        private void Slider_HSBValueChanged(object sender, EventArgs? e)
        {
            if (UpdatingColors)
                return;
            UpdatingColors = true;
            {
                // tell the other bars about the hue change
                sliderSat.Hue = sliderHue.Hue;
                sliderBright.Hue = sliderHue.Hue;

                // update the numeric hue boxes
                numHue.Value = sliderHue.Hue;
                numSat.Value = sliderSat.Saturation;
                numBright.Value = sliderBright.Brightness;

                // Convert hue to RGB (full brightness)
                UpdateSelectedColor(GetColorFromSliders());
                UpdateRGBControlsFromSelectedColor();

            }
            UpdatingColors = false;
        }

        private void numUpDown_ARGBValueChanged(object sender, EventArgs e)
        {
            if (UpdatingColors)
                return;
            UpdatingColors = true;
            {
                sliderAlpha.Value = (int)numAlpha.Value;
                sliderRed.Value = (int)numRed.Value;
                sliderGreen.Value = (int)numGreen.Value;
                sliderBlue.Value = (int)numBlue.Value;
                UpdateSelectedColor(Color.FromArgb((byte)sliderAlpha.Value, (byte)sliderRed.Value, (byte)sliderGreen.Value, (byte)sliderBlue.Value));
                UpdateHSBControlsFromSelectedColor();
            }
            UpdatingColors = false;
        }

        private void numUpDown_HSBValueChanged(object sender, EventArgs e)
        {
            if (UpdatingColors)
                return;
            UpdatingColors = true;
            {
                // update the sliders
                sliderHue.Hue = (int)numHue.Value;
                sliderSat.Saturation = (int)numSat.Value;
                sliderBright.Brightness = (int)numBright.Value;

                // tell the other bars about the hue change
                sliderSat.Hue = sliderHue.Hue;
                sliderBright.Hue = sliderHue.Hue;

                UpdateSelectedColor(GetColorFromSliders());
                UpdateRGBControlsFromSelectedColor();
            }
            UpdatingColors = false;
        }

        private void UpdateSelectedColor(Color color, bool updateTextBox = true)
        {
            SelectedColor = Color.FromArgb(color.A, color.R, color.G, color.B);
            previewPanel.Background = new SolidColorBrush(SelectedColor);
            if(updateTextBox)
                textHex.Text = ColorHelper.ColorToHex(SelectedColor, EnableAlphaChannel);
        }

        private Color GetColorFromSliders()
        {
            Color rgbColor = ColorHelper.GetColorFromHsbA(
                (byte)sliderAlpha.Value,
                sliderHue.Hue,
                (double)sliderSat.Saturation / 100,
                (double)sliderBright.Brightness / 100
            );
            return rgbColor;
        }

        private void textHex_TextChanged(object sender, TextChangedEventArgs? e)
        {
            string hex = textHex.Text.Trim();

            // Allow formats like "#FF00AA" or "FF00AA"
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            bool parsed = false;
            {
                if (hex.Length == 6 &&
                    int.TryParse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out int r) &&
                    int.TryParse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out int g) &&
                    int.TryParse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out int b))
                {
                    parsed = true;
                    UpdateSelectedColor(Color.FromRgb((byte)r, (byte)g, (byte)b), false);
                }
            }
            {
                if (hex.Length == 8 &&
                    int.TryParse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out int a) &&
                    int.TryParse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out int r) &&
                    int.TryParse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out int g) &&
                    int.TryParse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber, null, out int b))
                {
                    parsed = true;
                    UpdateSelectedColor(Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b), false);
                }
            }
            if(!parsed)
            {
                // Invalid hex → ignore and show feedback in preview panel
                previewPanel.Background = new SolidColorBrush(Colors.Magenta);
            }
            UpdateRGBControlsFromSelectedColor();
        }

        private void textHex_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                textHex_LostFocus(sender, e);
            }
        }

        private void textHex_LostFocus(object sender, RoutedEventArgs e)
        {
            // Force re-parse when lose focus
            textHex_TextChanged(this, null);
            UpdateSelectedColor(SelectedColor);
        }
    }
}
