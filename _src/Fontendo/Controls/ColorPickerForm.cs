using Fontendo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Fontendo.Controls
{
    public partial class ColorPickerForm : Form
    {
        public event EventHandler<ColorPreviewEventArgs>? PreviewColorChanged;

        private Color _selectedcolor;
        public Color SelectedColor
        {
            get
            {
                return _selectedcolor;
            }
            set
            {
                if (value != _selectedcolor)
                {
                    _selectedcolor = value;
                    previewPanel.BackColor = _selectedcolor;
                    textHex.Text = ColorHelper.ColorToHex(SelectedColor);
                    PreviewColorChanged?.Invoke(this, new ColorPreviewEventArgs(_selectedcolor));
                }
            }
        }
        public Color InitialColor { get; set; }

        public ColorPickerForm(Color InitialColor)
        {
            InitializeComponent();

            UpdatingColors = true;
            {
                // updating the colours
                this.InitialColor = InitialColor;
                redBar.Value = InitialColor.R;
                greenBar.Value = InitialColor.G;
                blueBar.Value = InitialColor.B;
            }
            UpdatingColors = false;
            ColourSlider_RGB_ValueChanged(this, EventArgs.Empty);
            ColourSlider_HSB_ValueChanged(this, EventArgs.Empty);
        }

        private static bool UpdatingColors = false;
        private void ColourSlider_RGB_ValueChanged(object sender, EventArgs e)
        {
            if (UpdatingColors)
                return;
            UpdatingColors = true;
            {
                numRed.Value = redBar.Value;
                numericGreen.Value = greenBar.Value;
                numBlue.Value = blueBar.Value;
                SelectedColor = Color.FromArgb(redBar.Value, greenBar.Value, blueBar.Value);

                hueTrackBar1.Hue = (int)Math.Round(ColorHelper.GetHueFromColor(SelectedColor));
                saturationTrackBar1.Saturation = (int)Math.Round(ColorHelper.GetSaturationFromColor(SelectedColor) * 100);
                brightnessTrackBar1.Brightness = (int)Math.Round(ColorHelper.GetBrightnessFromColor(SelectedColor) * 100);

                // update the hsb boxes
                numHue.Value = hueTrackBar1.Hue;
                numSat.Value = saturationTrackBar1.Saturation;
                numBright.Value = brightnessTrackBar1.Brightness;
            }
            UpdatingColors = false;

            // update preview panel
            previewPanel.BackColor = SelectedColor;
        }

        private void ColourSlider_HSB_ValueChanged(object sender, EventArgs e)
        {
            if (UpdatingColors)
                return;
            // tell the other bars about the hue change
            saturationTrackBar1.Hue = hueTrackBar1.Hue;
            brightnessTrackBar1.Hue = hueTrackBar1.Hue;

            // update the numeric hue boxes
            numHue.Value = hueTrackBar1.Hue;
            numSat.Value = saturationTrackBar1.Saturation;
            numBright.Value = brightnessTrackBar1.Brightness;

            // Convert hue to RGB (full brightness)
            Color rgbColor = ColorHelper.GetColorFromHsb(
                hueTrackBar1.Hue,
                (double)saturationTrackBar1.Saturation / 100,
                (double)brightnessTrackBar1.Brightness / 100
            );

            // Update numeric boxes with RGB values
            numRed.Value = rgbColor.R;
            numericGreen.Value = rgbColor.G;
            numBlue.Value = rgbColor.B;
            textHex.Text = ColorHelper.ColorToHex(rgbColor);

            // update preview panel
            SelectedColor = Color.FromArgb(rgbColor.R, rgbColor.G, rgbColor.B);
        }

        private void ColourNumericUpDown_RGB_ValueChanged(object sender, EventArgs e)
        {
            if (UpdatingColors)
                return;
            UpdatingColors = true;
            {
                redBar.Value = (int)numRed.Value;
                greenBar.Value = (int)numericGreen.Value;
                blueBar.Value = (int)numBlue.Value;
                SelectedColor = Color.FromArgb(redBar.Value, greenBar.Value, blueBar.Value);

                hueTrackBar1.Hue = (int)Math.Round(ColorHelper.GetHueFromColor(SelectedColor));
                saturationTrackBar1.Saturation = (int)Math.Round(ColorHelper.GetSaturationFromColor(SelectedColor) * 100);
                brightnessTrackBar1.Brightness = (int)Math.Round(ColorHelper.GetBrightnessFromColor(SelectedColor) * 100);

                // update the hsb boxes
                numHue.Value = hueTrackBar1.Hue;
                numSat.Value = saturationTrackBar1.Saturation;
                numBright.Value = brightnessTrackBar1.Brightness;
            }
            UpdatingColors = false;

            // update preview panel
            previewPanel.BackColor = SelectedColor;
        }

        private void ColourNumericUpDown_HSB_ValueChanged(object sender, EventArgs e)
        {
            if (UpdatingColors)
                return;


            // tell the other bars about the hue change
            saturationTrackBar1.Hue = hueTrackBar1.Hue;
            brightnessTrackBar1.Hue = hueTrackBar1.Hue;

            // update the sliders
            hueTrackBar1.Hue = (int)numHue.Value;
            saturationTrackBar1.Saturation = (int)numSat.Value;
            brightnessTrackBar1.Brightness = (int)numBright.Value;

            // Convert hue to RGB (full brightness)
            Color rgbColor = ColorHelper.GetColorFromHsb(
                hueTrackBar1.Hue,
                (double)saturationTrackBar1.Saturation / 100,
                (double)brightnessTrackBar1.Brightness / 100
            );

            // update the numeric hue boxes
            numHue.Value = hueTrackBar1.Hue;
            numSat.Value = saturationTrackBar1.Saturation;
            numBright.Value = brightnessTrackBar1.Brightness;

            textHex.Text = ColorHelper.ColorToHex(rgbColor);

            // update preview panel
            SelectedColor = Color.FromArgb(rgbColor.R, rgbColor.G, rgbColor.B);
        }

        private void ColorPickerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult == DialogResult.Cancel)
                SelectedColor = InitialColor;
        }

        private void textHex_TextChanged(object sender, EventArgs? e)
        {
            string hex = textHex.Text.Trim();

            // Allow formats like "#FF00AA" or "FF00AA"
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length == 6 &&
                int.TryParse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out int r) &&
                int.TryParse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out int g) &&
                int.TryParse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out int b))
            {
                SelectedColor = Color.FromArgb(r, g, b);
                textHex.Text = ColorHelper.ColorToHex(SelectedColor);
            }
            else
            {
                // Invalid hex → ignore or show feedback
                // e.g. textHex.BackColor = Color.LightPink;
            }
        }

        private void textHex_KeyDown(object sender, KeyEventArgs? e)
        {
            if(e == null || e.KeyCode == Keys.Enter)
            {
                textHex_TextChanged(sender, e);
                textHex.Text = ColorHelper.ColorToHex(SelectedColor);
            }
        }

        private void textHex_Leave(object sender, EventArgs e)
        {
            textHex_KeyDown(sender, null);
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            SelectedColor = previewPanel.BackColor;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SelectedColor = InitialColor;
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
