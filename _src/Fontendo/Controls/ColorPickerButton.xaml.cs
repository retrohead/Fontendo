using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Fontendo.DockManager;
using Fontendo.Extensions;
using Fontendo.UI;
using static Fontendo.Controls.ColorPicker;

namespace Fontendo.Controls
{
    /// <summary>
    /// Interaction logic for ColorPickerButton.xaml
    /// </summary>
    public partial class ColorPickerButton : UserControl
    {
        public event EventHandler<ColorChangedEventArgs>? SelectedColorChanged;
        public event EventHandler<ColorChangedEventArgs>? PreviewSelectedColorChanged;
        private CustomWindow? window = null;

        // DependencyProperty for selected colour
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                nameof(SelectedColor),
                typeof(Color),
                typeof(ColorPickerButton),
                new PropertyMetadata(Colors.Black, OnSelectedColorChanged));

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ColorPickerButton)d;
            var newColor = (Color)e.NewValue;

            if (control.previewColor != null)
            {
                control.previewColor.Background = new SolidColorBrush(newColor);
            }

            control.SelectedColorChanged?.Invoke(control, new ColorChangedEventArgs(newColor));
        }

        public static readonly DependencyProperty IsPressedProperty =
    DependencyProperty.Register(
        nameof(IsPressed),
        typeof(bool),
        typeof(ColorPickerButton),
        new PropertyMetadata(false));

        public bool IsPressed
        {
            get => (bool)GetValue(IsPressedProperty);
            set => SetValue(IsPressedProperty, value);
        }

        public static readonly DependencyProperty IsPickerFocusedProperty =
    DependencyProperty.Register(
        nameof(IsPickerFocused),
        typeof(bool),
        typeof(ColorPickerButton),
        new PropertyMetadata(false));

        public bool IsPickerFocused
        {
            get => (bool)GetValue(IsPickerFocusedProperty);
            set => SetValue(IsPickerFocusedProperty, value);
        }

        // DependencyProperty for enabling alpha channel
        public static readonly DependencyProperty EnableAlphaChannelProperty =
            DependencyProperty.Register(
                nameof(EnableAlphaChannel),
                typeof(bool),
                typeof(ColorPickerButton),
                new PropertyMetadata(true));

        public bool EnableAlphaChannel
        {
            get => (bool)GetValue(EnableAlphaChannelProperty);
            set => SetValue(EnableAlphaChannelProperty, value);
        }


        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public void SetSelectedColor(Color color)
        {
            SelectedColor = color;
        }
        public void SetSelectedColor(System.Drawing.Color color)
        {
            SelectedColor = ColorHelper.ToMediaColor(color);
        }

        public ColorPickerButton()
        {
            InitializeComponent();
        }

        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsPressed = true;
            ((Border)Content).Focus();
            window = DockHandler.CreateCustomWindow(MainWindow.Self.Window, new CustomWindowOptions() { WindowType = CustomWindow.WindowTypes.Fixed });
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            UI_ColorPicker picker = new UI_ColorPicker(SelectedColor, EnableAlphaChannel) { Width = 260, Height = EnableAlphaChannel ? 432 : 400 };
            picker.PreviewSelectedColorChanged += Picker_PreviewSelectedColorChanged;
            window.SetContent(picker, "Fontendo Colour Picker");
            window.ShowDialog();
            SetSelectedColor(picker.SelectedColor);
            IsPressed = false;
        }

        private void Picker_PreviewSelectedColorChanged(object? sender, ColorPicker.ColorChangedEventArgs e)
        {
            PreviewSelectedColorChanged?.Invoke(this, e);

            if (previewColor != null)
                previewColor.Background = new SolidColorBrush(e.SelectedColor);
        }

        private void Border_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                Border_PreviewMouseDown(sender, null!);
        }
    }
}
