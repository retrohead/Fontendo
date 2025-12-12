using Fontendo.Extensions;

namespace Fontendo.Controls
{
    /// <summary>
    /// Interaction logic for DockableWindow.xaml
    /// </summary>
    public partial class CustomWindow : Window
    {
        private CustomWindow? _fullScreenWin;
        private UIElement? _movedContent;
        public static int WindowMarginOffsetBorder = 50;     // your outer margin
        private int WindowMarginOffset = WindowMarginOffsetBorder;
        public bool CloseRequested = false;
        WindowHelper windowHelper;
        public bool IsMouseHeld = false;
        public bool IsDragging = false;
        private bool IsDockable = false;
        private Rect NormalWindowSize;

        // DependencyProperty for fullscreen host flag
        public static readonly DependencyProperty IsFullscreenHostProperty =
            DependencyProperty.Register(
                nameof(IsFullscreenHost),
                typeof(bool),
                typeof(CustomWindow),
                new PropertyMetadata(false));

        public bool IsFullscreenHost
        {
            get => (bool)GetValue(IsFullscreenHostProperty);
            set => SetValue(IsFullscreenHostProperty, value);
        }

        private CustomWindow CustomWindowFullScreen(CustomWindow parent)
        {
            CustomWindow win = new CustomWindow(true)
            {
                IsFullscreenHost = true,
                WindowMarginOffset = WindowMarginOffsetBorder,
                NormalWindowSize = new Rect(parent.Left, parent.Top,parent.ActualWidth, parent.ActualHeight),
                Owner = this,
                WindowStartupLocation = System.Windows.WindowStartupLocation.Manual,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = true,
                Title = parent.Title,
                WindowState = System.Windows.WindowState.Maximized,
                IsDockable = parent.IsDockable
            };
            return win;
        }
        public CustomWindow(bool fullscreen)
        {
            IsFullscreenHost = fullscreen;
            InitializeComponent();
            Loaded += OnLoaded;
            WindowMarginOffset = fullscreen ? 0 : WindowMarginOffsetBorder;
            CreateShadow(IsFullscreenHost);
            PreviewMouseDown += OnPreviewMouseDown;
            PreviewMouseUp += OnPreviewMouseUp;
            windowHelper = new WindowHelper(this); // window help for resizing, shadow click, etc.
        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsMouseHeld = false;
            if (IsDragging)
            {
                ScrollViewer sv = (ScrollViewer)contentArea.Child;
                UserControl attachedControl = (UserControl)sv.Content;
                var pl = DockManager.GetUnderlyingPlaceholder(this, attachedControl);
                if (pl != null)
                {
                    pl.Placeholder.Background = (Brush)MainWindow.Self.FindResource("WindowBackgroundBrushMedium");

                    DockManager.SetRedockPlaceholder(attachedControl, pl);
                    Close(); // redocks
                }
            }
        }

        public void SetDockableContent(Control content)
        {
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
            SetContent(sv);
        }
        public void SetContent(Control content)
        {
            contentArea.Child = content;
        }
        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            Theme.loadTheme(this, "Theme_00.xaml");
            Theme.loadTheme(this, "Theme_Templates.xaml");
            Theme.applyTheme(this);
            DarkTitleBar.Apply(this);
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseHeld = true;
            // checking to see if shadow band was clicked
            base.OnMouseDown(e);

            Point p = e.GetPosition(this);
            double width = ActualWidth;
            double height = ActualHeight;

            // shadow band check
            bool inShadowBand =
                p.X < WindowMarginOffset ||
                p.X > width - WindowMarginOffset ||
                p.Y < WindowMarginOffset ||
                p.Y > height - WindowMarginOffset;

            if (inShadowBand)
            {
                // convert to screen coords
                Point screen = PointToScreen(p);
                windowHelper.ActivateUnderlyingWindow(this, screen);
            }
        }

        private void CreateShadow(bool fullScreen)
        {
            Border border = new Border();
            border.Margin = new Thickness(fullScreen ? -1 : WindowMarginOffset);
            border.Background = (Brush)MainWindow.Self.FindResource("WindowBackgroundBrushDark");
            border.BorderBrush = (Brush)MainWindow.Self.FindResource("ControlBorder");
            border.CornerRadius = new CornerRadius(fullScreen ? 0 : 10);
            border.BorderThickness = new Thickness(1);
            if (!fullScreen)
            {
                border.Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = WindowMarginOffset,
                    ShadowDepth = 0,
                    Opacity = 0.7
                };
            } else
            {
                cornerIndicator.Visibility = Visibility.Collapsed;
            }
            Content = border;
            border.Child = mainGrid;
        }

        public void GoFullScreenSwap()
        {
            // Already in fullscreen? Do nothing.
            if (_fullScreenWin != null) return;

            // Nothing to move? Do nothing.
            _movedContent = contentArea.Child;
            if (_movedContent == null) return;

            // Detach from this window.
            contentArea.Child = null;
            // Create a new host window
            var fs = CustomWindowFullScreen(this);
            // Hide original and show fullscreen
            this.Hide();
            fs.Show();
            fs.Activate();

            // Keep DataContext if you rely on it
            fs.DataContext = this.DataContext;

            // Move the UI into the fullscreen window
            fs.contentArea.Child = _movedContent;

            // add state changed event so we know if size is set back to normal size

            fs.StateChanged += (s, e) =>
            {
                if(fs.WindowState == System.Windows.WindowState.Normal)
                {
                    ReturnFromFullScreenSwap();
                }
            };

            _fullScreenWin = fs;

        }

        public void CloseRequest()
        {
            CloseRequested = true;
            ReturnFromFullScreenSwap(CloseRequested);
        }

        public void ReturnFromFullScreenSwap(bool closing = false)
        {
            if (_fullScreenWin == null)
            {
                if (closing)
                {
                    this.Close();
                    if (IsDockable)
                    {
                        ScrollViewer sv = (ScrollViewer)contentArea.Child;
                        DockManager.Redock((UserControl)sv.Content);
                    }
                }
                return;
            }
            if (_fullScreenWin.contentArea.Child == null) return; // already redocked
            // Extract content from fullscreen window
            var content = _fullScreenWin.contentArea.Child;
            _fullScreenWin.contentArea.Child = null;

            // Unhook events
            _fullScreenWin.StateChanged -= (s, e) => { };

            // Move content back
            contentArea.Child = content;

            // Restore original window visuals
            WindowState = System.Windows.WindowState.Normal;
            ResizeMode = ResizeMode.CanResize;

            // Show original window again
            Width = _fullScreenWin.NormalWindowSize.Width;
            Height = _fullScreenWin.NormalWindowSize.Height;
            Left = _fullScreenWin.NormalWindowSize.X;
            Top = _fullScreenWin.NormalWindowSize.Y;
            // Close fullscreen window if still alive
            if (_fullScreenWin != null && _fullScreenWin.IsVisible)
            {
                _fullScreenWin.Close();
            }
            _fullScreenWin = null;
            this.Show();
            this.Activate();
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
            customTitleBar.Foreground = (Brush)MainWindow.Self.FindResource("ControlBorder");
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            if(activeTextBrush == null)
                activeTextBrush = customTitleBar.Foreground;
            customTitleBar.Foreground = activeTextBrush;
        }
        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return windowHelper.WndProc(hwnd, msg, wParam, lParam, ref handled);
        }
    }
}
