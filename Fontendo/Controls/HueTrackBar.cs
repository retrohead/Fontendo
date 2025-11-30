using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fontendo.Controls
{
    public class HueTrackBar : Control
    {
        private TrackBar trackBar;
        private PictureBox hueBar;

        public event EventHandler? HueChanged;

        public int Hue
        {
            get => trackBar.Value;
            set
            {
                if (trackBar.Value != value)
                {
                    trackBar.Value = Math.Max(trackBar.Minimum, Math.Min(trackBar.Maximum, value));
                    HueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public HueTrackBar()
        {
            this.Size = new Size(260, 40);
            // TrackBar
            trackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 360,
                TickStyle = TickStyle.None,
                Dock = DockStyle.Top,
                Height = 30,
            };
            trackBar.ValueChanged += (s, e) => HueChanged?.Invoke(this, EventArgs.Empty);

            int margin = 13;
            int hueBarY = 22;

            // PictureBox for hue gradient
            hueBar = new PictureBox
            {
                Dock = DockStyle.Bottom,
                Height = 4,
                Left = margin,
                Width = this.Width - (margin * 2),
                Top = hueBarY, // place near bottom
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            hueBar.Image = GenerateHueImage(this.Width - (margin * 2), hueBar.Height);

            // Resize handler to regenerate gradient
            this.Resize += (s, e) =>
            {
                hueBar.Image?.Dispose();
                hueBar.Image = GenerateHueImage(this.Width - (margin * 2), hueBar.Height);
                hueBar.Top = hueBarY;
                hueBar.Width = this.Width - (margin * 2);
                trackBar.Height = this.Height;
            };

            Controls.Add(hueBar);
            Controls.Add(trackBar);
        }

        private Bitmap GenerateHueImage(int width, int height)
        {
            var bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int x = 0; x < width; x++)
                {
                    int h = (int)((x / (float)width) * 360);
                    Color c = FromHsv(h, 1, 1);
                    using (Pen pen = new Pen(c))
                    {
                        g.DrawLine(pen, x, 0, x, height);
                    }
                }
            }
            return bmp;
        }

        private static Color FromHsv(double h, double s, double v)
        {
            int hi = Convert.ToInt32(Math.Floor(h / 60)) % 6;
            double f = h / 60 - Math.Floor(h / 60);

            v *= 255;
            int vi = (int)v;
            int p = (int)(v * (1 - s));
            int q = (int)(v * (1 - f * s));
            int t = (int)(v * (1 - (1 - f) * s));

            return hi switch
            {
                0 => Color.FromArgb(vi, t, p),
                1 => Color.FromArgb(q, vi, p),
                2 => Color.FromArgb(p, vi, t),
                3 => Color.FromArgb(p, q, vi),
                4 => Color.FromArgb(t, p, vi),
                _ => Color.FromArgb(vi, p, q),
            };
        }
    }
}
