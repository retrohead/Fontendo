using Fontendo.Extensions;
using Fontendo.UI;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Fontendo.Controls
{
    public class SaturationTrackBar : UserControl
    {
        private Slider slider;
        private Rectangle satBar;

        // Event to notify saturation changes
        public event EventHandler? SaturationChanged;

        // Register the dependency property
        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register(
                nameof(Saturation),
                typeof(int),
                typeof(SaturationTrackBar),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSaturationChanged, CoerceSaturation));

        // CLR wrapper
        public int Saturation
        {
            get => (int)GetValue(SaturationProperty);
            set => SetValue(SaturationProperty, value);
        }

        // Called when Saturation changes
        private static void OnSaturationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SaturationTrackBar)d;

            // Update slider value if needed
            if (control.slider != null)
                control.slider.Value = (int)e.NewValue;

            // Raise event
            control.SaturationChanged?.Invoke(control, EventArgs.Empty);
        }

        // Coerce callback ensures value stays within slider bounds
        private static object CoerceSaturation(DependencyObject d, object baseValue)
        {
            var control = (SaturationTrackBar)d;
            var value = (int)baseValue;

            if (control.slider != null)
            {
                return Math.Max((int)control.slider.Minimum, Math.Min((int)control.slider.Maximum, value));
            }

            return value;
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
            slider.ValueChanged += (s, e) => { Saturation = (int)slider.Value; SaturationChanged?.Invoke(this, EventArgs.Empty); };

            satBar = new Rectangle
            {
                Height = 4,
                Margin = new Thickness(5, 0, 5, 0)
            };
            Grid.SetRow(satBar, 1);
            RegenerateGradient();

            grid.Children.Add(slider);
            grid.Children.Add(satBar);

            Content = grid;
        }

        private void RegenerateGradient()
        {
            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0)
            };

            // Build gradient stops across saturation 0 → 1
            for (int i = 0; i <= 10; i++)
            {
                double s = i / 10.0;
                var c = ColorHelper.GetColorFromHsbA(255, hue, s, 1);
                brush.GradientStops.Add(new GradientStop(c, s));
            }

            satBar.Fill = brush;
        }
    }
}
