using Fontendo.DockManager;
using Fontendo.Extensions;
using Fontendo.UI;
using System.Drawing.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using static Fontendo.Controls.ColorPicker;
using static Fontendo.DockManager.CustomWindowOptions;

namespace Fontendo.Controls
{
    /// <summary>
    /// Interaction logic for DockableWindow.xaml
    /// </summary>
    public partial class CustomWindow : Window
    {
        public static int WindowMarginOffsetBorder = 50;     // outer margin
        WindowHelper windowHelper;
        public bool IsMouseHeld = false;
        public bool IsDragging = false;
        private bool IsDockable = false;
        public Control? AttachedControl = null;
        private CornerGrabber? cornerGrab = null;
        public event EventHandler? CustomWindowReady = null;
        private bool IsScreenDocked = false;

        public enum WindowTypes
        {
            Fixed,
            Resizable,
            Fullscreen,
            DockLeft, 
            DockRight
        }


        public CustomWindowOptions Options;

        public CustomWindow(Window? OwningWindow, CustomWindowOptions options)
        {
            InitializeComponent();
            Owner = OwningWindow;
            Options = options;
            Loaded += OnLoaded;

            PreviewMouseDown += OnPreviewMouseDown;
            PreviewMouseUp += OnPreviewMouseUp;
            windowHelper = new WindowHelper(this); // window help for resizing, shadow click, etc.
        }

        private void VerifyContent(Control content)
        {
            try
            {
                CustomWindowContentBase? customWindowContentBase = content as CustomWindowContentBase;
                if(customWindowContentBase == null || !customWindowContentBase.IsCustomWindowContentInitialized)
                    throw new Exception("DockManager custom window error. Trying to set a content control which is not of type DockManager.CustomWindowContentBase");
            }
            catch
            {
                throw new Exception("DockManager custom window error. Trying to set a content control which is not of type DockManager.CustomWindowContentBase");
            }

            if (double.IsNaN(content.Width) && Options.WindowType == WindowTypes.Fixed) throw new Exception("Make sure to specifically set your content control width in code before attaching to a custom window in fixed mode");
        }

        public void SetDockableContent(Control content)
        {
            VerifyContent(content);
            // scroll viewer for content to sit in
            ScrollViewer sv = new ScrollViewer()
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Background = (Brush)UI_MainWindow.Self.FindResource("WindowBackgroundBrushDark"),
                Style = (Style)UI_MainWindow.Self.FindResource("GalleryScrollViewerStyle")
            };
            sv.Content = content;
            contentArea.Child = sv;
            IsDockable = true;
            ApplyContent(sv);
        }
        public void SetContent(CustomWindowContentBase content, string title)
        {
            Title = title;
            VerifyContent(content);
            ApplyContent(content);
        }
        internal void ApplyContent(Control content)
        {
            if (content == null)
                return;
            AttachedControl = content;
            contentArea.Child = content;

            if (!IsDockable)
            {
                ((CustomWindowContentBase)AttachedControl).Window = this;
                SizeWindowToContent();
            }
        }

        private void AttachedControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (AttachedControl == null) return;
            if (AttachedControl.Width == 0 || double.IsNaN(AttachedControl.Width) ) return; // keep firing until we get a width
            AttachedControl.SizeChanged -= AttachedControl_Loaded;
            SizeWindowToContent();
        }

        private void SizeWindowToContent()
        {
            if (AttachedControl == null) return;
            // set width to the content
            int WindowMarginOffset = WindowMarginOffsetBorder;
            if (WindowState == WindowState.Maximized)
                WindowMarginOffset = 0;


            Width = AttachedControl.Width + (WindowMarginOffset * 2) + 2;
            Height = AttachedControl.Height + (WindowMarginOffset * 2) + customTitleBar.Height + 2;
            if (Options.WindowType == WindowTypes.Fixed)
            {
                // lock max and min
                MinWidth = Width;
                MaxWidth = Width;
                MinHeight = Height;
                MaxHeight = Height;
            } else
            {
                MinWidth = AttachedControl.MinWidth + (WindowMarginOffset * 2) + 2;
                MaxWidth = AttachedControl.MaxWidth + (WindowMarginOffset * 2) + 2;
                MinHeight = AttachedControl.MinHeight + (WindowMarginOffset * 2) + customTitleBar.Height + 2;
                MaxHeight = AttachedControl.MaxHeight + (WindowMarginOffset * 2) + customTitleBar.Height + 2;
            }
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            Theme.loadTheme(this, "Theme_00.xaml");
            Theme.loadTheme(this, "Theme_Templates.xaml");
            Theme.applyTheme(this);
            Theme.applyThemeColors(this, Theme.getThemeColorsFromWindowResources(Owner));
            CustomWindowReady?.Invoke(this, EventArgs.Empty);
            AdjustBordersForWindowSize(Options.WindowType);
            if(Options.WindowType == WindowTypes.Fullscreen)
                WindowState = WindowState.Maximized;
        }

        public void SetThemeColors(Theme.ThemeColorsType colors)
        {
            Theme.applyThemeColors(this, colors);
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (AttachedControl == null)
                return;
            IsMouseHeld = true;

            if (WindowState == WindowState.Maximized)
                return;

            Point p = e.GetPosition(this);
            double width = ActualWidth;
            double height = ActualHeight;

            // shadow band check
            bool inShadowBand =
                p.X < WindowMarginOffsetBorder ||
                p.X > width - WindowMarginOffsetBorder ||
                p.Y < WindowMarginOffsetBorder ||
                p.Y > height - WindowMarginOffsetBorder;

            if (inShadowBand)
            {
                // convert to screen coords
                Point screen = PointToScreen(p);
                windowHelper.ActivateUnderlyingWindow(this, screen);
            }
        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsMouseHeld = false;
            if (IsDragging)
            {
                ScrollViewer sv = (ScrollViewer)contentArea.Child;
                UserControl attachedControl = (UserControl)sv.Content;
                var pl = DockHandler.GetUnderlyingPlaceholder(this, attachedControl);
                if (pl != null)
                {
                    pl.Placeholder.Background = (Brush)MainWindow.Self.FindResource("WindowBackgroundBrushMedium");

                    DockHandler.SetRedockPlaceholder(attachedControl, pl);
                    Close(); // redocks
                    return;
                }
            }
        }

        public void AdjustBordersForWindowSize(WindowTypes windowType)
        {
            mainBorder.Background = (Brush)MainWindow.Self.FindResource("WindowBackgroundBrushDark");
            mainBorder.BorderBrush = (Brush)MainWindow.Self.FindResource("ControlBorder");
            mainBorder.BorderThickness = new Thickness(1);
            if (windowType == WindowTypes.Fullscreen)
            {
                mainBorder.Margin = new Thickness(5, 5, 0, 0);
                mainBorder.Effect = null;
                mainBorder.CornerRadius = new CornerRadius(0);
                var workingArea = WindowHelper.GetWorkingArea(this);
                MaxWidth = workingArea.Width + 10;
                MaxHeight = workingArea.Height + 10;

                customTitleBar.btnClose.Style = MainWindow.Self.FindResource("TitleBarCloseButtonStyleFullScreen") as Style;
                if (cornerGrab != null)
                    cornerGrab.Visibility = Visibility.Collapsed;
            }
            else if (windowType == WindowTypes.DockRight)
            {
                mainBorder.Margin = new Thickness(0, -1, -1, -1);
                mainBorder.Effect = null;
                mainBorder.CornerRadius = new CornerRadius(0);
                var workingArea = WindowHelper.GetWorkingArea(this);

                customTitleBar.btnClose.Style = MainWindow.Self.FindResource("TitleBarCloseButtonStyleFullScreen") as Style;
                if (cornerGrab != null)
                    cornerGrab.Visibility = Visibility.Collapsed;
            }
            else if (windowType == WindowTypes.DockLeft)
            {
                mainBorder.Margin = new Thickness(-1, -1, 0, -2);
                mainBorder.Effect = null;
                mainBorder.CornerRadius = new CornerRadius(0);
                var workingArea = WindowHelper.GetWorkingArea(this);

                customTitleBar.btnClose.Style = MainWindow.Self.FindResource("TitleBarCloseButtonStyleFullScreen") as Style;
                if (cornerGrab != null)
                    cornerGrab.Visibility = Visibility.Collapsed;
            }
            else
            {
                mainBorder.Margin = new Thickness(WindowMarginOffsetBorder);
                mainBorder.CornerRadius = new CornerRadius(10);
                mainBorder.Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = WindowMarginOffsetBorder,
                    ShadowDepth = 0,
                    Opacity = 0.7
                };
                customTitleBar.btnClose.Style = MainWindow.Self.FindResource("TitleBarCloseButtonStyle") as Style;
            }
            if (windowType == WindowTypes.Resizable && Options.ShowGripperWhenResizable)
            {
                // add a corner grabber
                if (cornerGrab == null)
                {
                    cornerGrab = new CornerGrabber()
                    {
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new Thickness(0, 0, 1, 1),
                        Height = 17
                    };
                    Grid.SetRow(cornerGrab, 2);
                    mainGrid.Children.Add(cornerGrab);
                }
                cornerGrab.Visibility = Visibility.Visible;
            }
            else if (windowType == WindowTypes.Fixed)
            {
                // do not show resize buttons for fixed window
                customTitleBar.ShowResizeButtons = false;
            }
            Options.WindowType = windowType;
        }

        public void CloseRequest()
        {
            this.Close();
            if (IsDockable)
            {
                ScrollViewer sv = (ScrollViewer)contentArea.Child;
                DockHandler.Redock((UserControl)sv.Content);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            hwndSource.AddHook(WndProc);
        }

        Brush? activeTextBrush = null;
        private void Window_Deactivated(object sender, EventArgs e)
        {
            activeTextBrush = customTitleBar.Foreground;
            customTitleBar.Foreground = (Brush)MainWindow.Self.FindResource("ControlTextInactive");
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            if(activeTextBrush == null)
                activeTextBrush = customTitleBar.Foreground;
            customTitleBar.Foreground = activeTextBrush;
        }
        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (WindowState == WindowState.Maximized || Options.WindowType == WindowTypes.DockLeft || Options.WindowType == WindowTypes.DockRight) // dont fire on full screen
                return IntPtr.Zero;
            return windowHelper.WndProc(hwnd, msg, wParam, lParam, ref handled);
        }

        Point? locationBeforeFullScreen = null;
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                locationBeforeFullScreen = new Point(Left, Top);
                AdjustBordersForWindowSize(CustomWindow.WindowTypes.Fullscreen);
            }
            else
            {
                AdjustBordersForWindowSize(CustomWindow.WindowTypes.Resizable);
                if(locationBeforeFullScreen != null)
                {
                    Left = locationBeforeFullScreen.Value.X;
                    Top = locationBeforeFullScreen.Value.Y;
                }
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var screen = WindowHelper.GetWorkingArea(this);
            if(e.NewSize.Height == screen.Height && e.NewSize.Width == screen.Width / 2)
            {
                // handling dock request
                if (Left == 0 || Left == -screen.Width)
                {
                    AdjustBordersForWindowSize(WindowTypes.DockLeft);
                    IsScreenDocked = true;
                }
                else if( (Left == screen.Width / 2) || (Left == -(screen.Width / 2)))
                {
                    AdjustBordersForWindowSize(WindowTypes.DockRight);
                    IsScreenDocked = true;
                }
            }
            if(IsScreenDocked)
            {
                if (e.NewSize.Height != screen.Height && e.NewSize.Width != screen.Width / 2)
                {
                    // undock
                    AdjustBordersForWindowSize(WindowTypes.Resizable);
                    IsScreenDocked = false;
                }
            }
        }
    }
}
