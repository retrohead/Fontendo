using Fontendo.Extensions;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fontendo.Controls
{
    public class SaturationTrackBar : Control
    {
        private TrackBar trackBar;
        private PictureBox satBar;

        public event EventHandler? SaturationChanged;

        // Saturation value 0–100
        public int Saturation
        {
            get => trackBar.Value;
            set
            {
                if (trackBar.Value != value)
                {
                    trackBar.Value = Math.Max(trackBar.Minimum, Math.Min(trackBar.Maximum, value));
                    SaturationChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        // The hue we’re saturating (0–360)
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

        public SaturationTrackBar()
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
            trackBar.ValueChanged += (s, e) => SaturationChanged?.Invoke(this, EventArgs.Empty);

            int margin = 13;
            int satBarY = 22;

            satBar = new PictureBox
            {
                Height = 4,
                Left = margin,
                Width = this.Width - (margin * 2),
                Top = satBarY,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            satBar.Image = GenerateSaturationImage(this.Width - (margin * 2), satBar.Height, hue);

            this.Resize += (s, e) =>
            {
                RegenerateGradient();
                satBar.Top = satBarY;
                satBar.Width = this.Width - (margin * 2);
                trackBar.Height = this.Height;
            };

            Controls.Add(satBar);
            Controls.Add(trackBar);
        }

        private void RegenerateGradient()
        {
            satBar.Image?.Dispose();
            satBar.Image = GenerateSaturationImage(satBar.Width, satBar.Height, hue);
        }

        private Bitmap GenerateSaturationImage(int width, int height, int hue)
        {
            var bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int x = 0; x < width; x++)
                {
                    double s = x / (double)width; // 0 → 1
                    Color c = ColorHelper.GetColorFromHsb(hue, s, 1);
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
