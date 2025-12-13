using Fontendo.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fontendo.Controls
{
    /// <summary>
    /// Interaction logic for CustomTitleBar.xaml
    /// </summary>
    public partial class CustomTitleBar : UserControl
    {
        // DependencyProperty to hold a reference to the parent Window
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(Icon),
                typeof(ImageSource),
                typeof(CustomTitleBar),
                new PropertyMetadata(null));

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        // DependencyProperty to hold a reference to the parent Window
        public static readonly DependencyProperty TargetWindowProperty =
            DependencyProperty.Register(
                nameof(TargetWindow),
                typeof(CustomWindow),
                typeof(CustomTitleBar),
                new PropertyMetadata(null));

        public CustomWindow TargetWindow
        {
            get => (CustomWindow)GetValue(TargetWindowProperty);
            set => SetValue(TargetWindowProperty, value);
        }


        // DependencyProperty to hold a reference to the parent Window
        public static readonly DependencyProperty ShowResizeButtonsProperty =
            DependencyProperty.Register(
                nameof(ShowResizeButtons),
                typeof(bool),
                typeof(CustomTitleBar),
                new PropertyMetadata(null));

        public bool ShowResizeButtons
        {
            get => (bool)GetValue(ShowResizeButtonsProperty);
            set
            {
                SetValue(ShowResizeButtonsProperty, value);
                if (value)
                {
                    btnMin.Visibility = Visibility.Visible;
                    btnMax.Visibility = Visibility.Visible;
                }
                else
                {
                    btnMin.Visibility = Visibility.Collapsed;
                    btnMax.Visibility = Visibility.Collapsed;
                }
            }
        }

        public CustomTitleBar()
        {
            InitializeComponent();
            MouseLeftButtonDown += TitleBar_MouseLeftButtonDown;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var win = Window.GetWindow(this);
            if (e.ClickCount == 2)
            {
                ToggleMaximize(win);
            }
            else
            {
                win?.DragMove();
            }
        }

        private void Minimize_MouseUp(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }

        private void MaximizeRestore_MouseUp(object sender, RoutedEventArgs e)
        {
            var win = Window.GetWindow(this);
            ToggleMaximize(win);
        }

        private void Close_MouseUp(object sender, RoutedEventArgs e)
        {
            ((CustomWindow)Window.GetWindow(this)).CloseRequest();
        }

        private void ToggleMaximize(Window win)
        {
            if (win == null) return;

            if (win.WindowState == WindowState.Maximized)
            {
                win.WindowState = WindowState.Normal;
            }
            else
            {
                win.WindowState = WindowState.Maximized;
            }
        }
    }

}
