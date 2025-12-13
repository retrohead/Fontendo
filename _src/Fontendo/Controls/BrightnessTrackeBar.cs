using Fontendo.Extensions;
using Fontendo.UI;
using System;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Fontendo.Controls
{
    public class BrightnessTrackBar : UserControl
    {
        private Slider slider;
        private Rectangle gradientBar;

        public event EventHandler? BrightnessChanged;
        
        // Register the dependency property
        public static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register(
                nameof(Brightness),
                typeof(int),
                typeof(BrightnessTrackBar),
                new FrameworkPropertyMetadata(
                    0, // default value
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnBrightnessChanged,
                    CoerceBrightness));

        // CLR wrapper
        public int Brightness
        {
            get => (int)GetValue(BrightnessProperty);
            set => SetValue(BrightnessProperty, value);
        }

        // Called when Brightness changes
        private static void OnBrightnessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (BrightnessTrackBar)d;
            int newValue = (int)e.NewValue;

            // Update slider if available
            if (control.slider != null)
                control.slider.Value = newValue;

            // Raise event
            control.BrightnessChanged?.Invoke(control, EventArgs.Empty);
        }

        // Clamp the value to slider bounds
        private static object CoerceBrightness(DependencyObject d, object baseValue)
        {
            var control = (BrightnessTrackBar)d;
            int value = (int)baseValue;

            if (control.slider != null)
            {
                return Math.Max((int)control.slider.Minimum, Math.Min((int)control.slider.Maximum, value));
            }

            return value;
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
            var grid = new Grid();
            grid.VerticalAlignment = VerticalAlignment.Center;
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(22) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(4) });

            slider = new Slider
            {
                Minimum = 0,
                Maximum = 100,
                TickPlacement = System.Windows.Controls.Primitives.TickPlacement.None,
                VerticalAlignment = VerticalAlignment.Stretch,
                Style = (Style)MainWindow.Self.FindResource("SliderStylePointer")
            };
            slider.ValueChanged += (s, e) => { Brightness = (int)slider.Value; BrightnessChanged?.Invoke(this, EventArgs.Empty); };

            gradientBar = new Rectangle
            {
                Height = 4,
                Margin = new Thickness(5, 0, 5, 0)
            };
            Grid.SetRow(gradientBar, 1);
            RegenerateGradient();

            grid.Children.Add(slider);
            grid.Children.Add(gradientBar);

            Content = grid;
        }

        private void RegenerateGradient()
        {
            gradientBar.Fill = GenerateBrightnessBrush(hue);
        }

        private Brush GenerateBrightnessBrush(int hue)
        {
            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0)
            };

            // Build gradient stops from 0 → 1 brightness
            for (int i = 0; i <= 10; i++)
            {
                double v = i / 10.0;
                var c = ColorHelper.GetColorFromHsbA(255, hue, 1, v); // full alpha channel
                brush.GradientStops.Add(new GradientStop(
                    Color.FromArgb(c.A, c.R, c.G, c.B), v));
            }

            return brush;
        }
    }
}
