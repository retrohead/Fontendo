using Fontendo.Extensions;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fontendo.Controls
{
    public class BrightnessTrackBar : Control
    {
        private TrackBar trackBar;
        private PictureBox brightBar;

        public event EventHandler? BrightnessChanged;

        // Brightness value 0–100
        public int Brightness
        {
            get => trackBar.Value;
            set
            {
                if (trackBar.Value != value)
                {
                    trackBar.Value = Math.Max(trackBar.Minimum, Math.Min(trackBar.Maximum, value));
                    BrightnessChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        // The hue we’re brightening (0–360)
        private int hue = 0;
        public int Hue
        {
            get => hue;
            set
            {
                if (hue != value)
                {
                    hue = Math.Max(0, Math.Min(360, value));
                    RegenerateGradient();
                }
            }
        }

        public BrightnessTrackBar()
        {
            this.Size = new Size(260, 40);

            trackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                TickStyle = TickStyle.None,
                Dock = DockStyle.Top,
                Height = 30,
            };
            trackBar.ValueChanged += (s, e) => BrightnessChanged?.Invoke(this, EventArgs.Empty);

            int margin = 13;
            int brightBarY = 22;

            brightBar = new PictureBox
            {
                Height = 4,
                Left = margin,
                Width = this.Width - (margin * 2),
                Top = brightBarY,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            brightBar.Image = GenerateBrightnessImage(this.Width - (margin * 2), brightBar.Height, hue);

            this.Resize += (s, e) =>
            {
                RegenerateGradient();
                brightBar.Top = brightBarY;
                brightBar.Width = this.Width - (margin * 2);
                trackBar.Height = this.Height;
            };

            Controls.Add(brightBar);
            Controls.Add(trackBar);
        }

        private void RegenerateGradient()
        {
            brightBar.Image?.Dispose();
            brightBar.Image = GenerateBrightnessImage(brightBar.Width, brightBar.Height, hue);
        }

        private Bitmap GenerateBrightnessImage(int width, int height, int hue)
        {
            var bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int x = 0; x < width; x++)
                {
                    double v = x / (double)width; // 0 → 1
                    Color c = ColorHelper.GetColorFromHsb(hue, 1, v);
                    using (Pen pen = new Pen(c))
                    {
                        g.DrawLine(pen, x, 0, x, height);
                    }
                }
            }
            return bmp;
        }
    }
}
