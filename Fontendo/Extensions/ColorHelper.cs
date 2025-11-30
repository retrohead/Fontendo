using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fontendo.Extensions
{
    internal class ColorHelper
    {
        public static double GetHueFromColor(Color c)
        {
            // Normalize RGB to 0–1
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double hue = 0;

            if (delta == 0)
            {
                hue = 0; // undefined hue → treat as 0
            }
            else if (max == r)
            {
                hue = 60 * (((g - b) / delta) % 6);
            }
            else if (max == g)
            {
                hue = 60 * (((b - r) / delta) + 2);
            }
            else if (max == b)
            {
                hue = 60 * (((r - g) / delta) + 4);
            }

            if (hue < 0) hue += 360;
            return hue;
        }

        public static double GetSaturationFromColor(Color c)
        {
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            if (max == 0) return 0; // avoid division by zero
            return delta / max;     // saturation in [0,1]
        }

        public static double GetBrightnessFromColor(Color c)
        {
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            return max; // brightness/value in [0,1]
        }

        public static Color GetColorFromHsb(double h, double s, double b)
        {
            int hi = Convert.ToInt32(Math.Floor(h / 60)) % 6;
            double f = h / 60 - Math.Floor(h / 60);

            b *= 255;
            int bi = (int)b;
            int p = (int)(bi * (1 - s));
            int q = (int)(bi * (1 - f * s));
            int t = (int)(bi * (1 - (1 - f) * s));

            return hi switch
            {
                0 => Color.FromArgb(bi, t, p),
                1 => Color.FromArgb(q, bi, p),
                2 => Color.FromArgb(p, bi, t),
                3 => Color.FromArgb(p, q, bi),
                4 => Color.FromArgb(t, p, bi),
                _ => Color.FromArgb(bi, p, q),
            };
        }
        public static string ColorToHex(Color c)
        {
            return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }

        public static double GetLuminance(Color c)
        {
            // Normalize to 0–1
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            // Apply gamma correction
            r = (r <= 0.03928) ? r / 12.92 : Math.Pow((r + 0.055) / 1.055, 2.4);
            g = (g <= 0.03928) ? g / 12.92 : Math.Pow((g + 0.055) / 1.055, 2.4);
            b = (b <= 0.03928) ? b / 12.92 : Math.Pow((b + 0.055) / 1.055, 2.4);

            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }
    }
}
