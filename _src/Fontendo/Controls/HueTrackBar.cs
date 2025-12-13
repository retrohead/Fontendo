using Fontendo.UI;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Fontendo.Controls
{
    public class HueTrackBar : UserControl
    {
        private Slider slider;
        private Rectangle hueBar;
        public event EventHandler? HueChanged;

        // Register the dependency property
        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register(
                nameof(Hue),
                typeof(int),
                typeof(HueTrackBar),
                new FrameworkPropertyMetadata(
                    0, // default value
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnHueChanged,
                    CoerceHue));

        // CLR wrapper
        public int Hue
        {
            get => (int)GetValue(HueProperty);
            set => SetValue(HueProperty, value);
        }

        // Called when Hue changes
        private static void OnHueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (HueTrackBar)d;
            int newValue = (int)e.NewValue;

            // Update slider if available
            if (control.slider != null)
                control.slider.Value = newValue;

            // Raise event
            control.HueChanged?.Invoke(control, EventArgs.Empty);
        }

        // Clamp the value to slider bounds
        private static object CoerceHue(DependencyObject d, object baseValue)
        {
            var control = (HueTrackBar)d;
            int value = (int)baseValue;

            if (control.slider != null)
            {
                return Math.Max((int)control.slider.Minimum, Math.Min((int)control.slider.Maximum, value));
            }

            return value;
        }

        public HueTrackBar()
        {

            var grid = new Grid();
            grid.VerticalAlignment = VerticalAlignment.Center;
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(22) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(4) });

            slider = new Slider
            {
                Minimum = 0,
                Maximum = 360,
                TickPlacement = System.Windows.Controls.Primitives.TickPlacement.None,
                VerticalAlignment = VerticalAlignment.Stretch,
                Style = (Style)MainWindow.Self.FindResource("SliderStylePointer")
            };
            slider.ValueChanged += (s, e) => { Hue = (int)slider.Value; HueChanged?.Invoke(this, EventArgs.Empty); };

            hueBar = new Rectangle
            {
                Height = 4,
                Margin = new Thickness(5, 0, 5, 0)
            };
            Grid.SetRow(hueBar, 1);
            RegenerateHueGradient();

            grid.Children.Add(slider);
            grid.Children.Add(hueBar);

            Content = grid;
        }

        private void RegenerateHueGradient()
        {
            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0)
            };

            // Build gradient stops across 360 degrees
            for (int h = 0; h <= 360; h += 10)
            {
                var c = FromHsv(h, 1, 1);
                brush.GradientStops.Add(new GradientStop(c, h / 360.0));
            }

            hueBar.Fill = brush;
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
                0 => Color.FromRgb((byte)vi, (byte)t, (byte)p),
                1 => Color.FromRgb((byte)q, (byte)vi, (byte)p),
                2 => Color.FromRgb((byte)p, (byte)vi, (byte)t),
                3 => Color.FromRgb((byte)p, (byte)q, (byte)vi),
                4 => Color.FromRgb((byte)t, (byte)p, (byte)vi),
                _ => Color.FromRgb((byte)vi, (byte)p, (byte)q),
            };
        }
    }
}
