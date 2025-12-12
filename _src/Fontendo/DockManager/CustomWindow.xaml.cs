using Fontendo.DockManager;
using Fontendo.Extensions;
using Fontendo.UI;
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
        public Control? AttachedControl = null;

        public event EventHandler? CustomWindowReady = null;

        public enum WindowTypes
        {
            Fixed,
            Resizable,
            Fullscreen
        }


        public CustomWindowOptions Options;

        // DependencyProperty for fullscreen host flag
        internal static readonly DependencyProperty IsFullscreenHostProperty =
            DependencyProperty.Register(
                nameof(IsFullscreenHost),
                typeof(bool),
                typeof(CustomWindow),
                new PropertyMetadata(false));

        internal bool IsFullscreenHost
        {
            get => (bool)GetValue(IsFullscreenHostProperty);
            set => SetValue(IsFullscreenHostProperty, value);
        }


        internal CustomWindow CustomWindowFullScreen(CustomWindow parent)
        {
            if (parent.Owner == null) throw new Exception("OwningWindow is null on window, cannot create a full screen instance");
            CustomWindow win = DockHandler.CreateCustomWindow(parent, new CustomWindowOptions() { WindowType = WindowTypes.Fullscreen, ShowGripperWhenResizable = parent.Options.ShowGripperWhenResizable });
            win.IsFullscreenHost = true;
            win.WindowMarginOffset = WindowMarginOffsetBorder;
            win.NormalWindowSize = new Rect(parent.Left, parent.Top, parent.ActualWidth, parent.ActualHeight);
            win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            win.ResizeMode = ResizeMode.NoResize;
            win.ShowInTaskbar = parent.ShowInTaskbar;
            win.Title = parent.Title;
            win.WindowState = WindowState.Maximized;
            win.IsDockable = parent.IsDockable;
            return win;
        }
        public CustomWindow(Window OwningWindow, CustomWindowOptions options)
        {
            InitializeComponent();
            Owner = OwningWindow;
            Options = options;
            Loaded += OnLoaded;
            WindowMarginOffset = Options.WindowType == WindowTypes.Fullscreen ? 0 : WindowMarginOffsetBorder;
            CreateShadow(Options.WindowType == WindowTypes.Fullscreen);

            if (Options.WindowType == WindowTypes.Fullscreen)
            {
                var workingArea = WindowHelper.GetWorkingArea(this);
                MaxWidth = workingArea.Width;
                MaxHeight = workingArea.Height;

                Left = workingArea.Left;
                Top = workingArea.Top;

                customTitleBar.ChangeToFullscreenCloseButton();
            }
            PreviewMouseDown += OnPreviewMouseDown;
            PreviewMouseUp += OnPreviewMouseUp;
            windowHelper = new WindowHelper(this); // window help for resizing, shadow click, etc.
            if (Options.WindowType == WindowTypes.Resizable && Options.ShowGripperWhenResizable)
            {
                // add a corner grabber
                CornerGrabber c = new CornerGrabber()
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 1, 1),
                    Height = 17
                };
                Grid.SetRow(c, 2);
                mainGrid.Children.Add(c);
            }
            else if (Options.WindowType == WindowTypes.Fixed)
            {
                // do not show resize buttons for fixed window
                customTitleBar.ShowResizeButtons = false;
            }
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
                Background = (Brush)UI_MainWindowContent.Self.FindResource("WindowBackgroundBrushDark"),
                Style = (Style)UI_MainWindowContent.Self.FindResource("GalleryScrollViewerStyle")
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
            DarkTitleBar.Apply(this);
            Theme.applyThemeColors(this, Theme.getThemeColorsFromWindowResources(Owner));
            CustomWindowReady?.Invoke(this, EventArgs.Empty);
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

            Point p = e.GetPosition(this);
            double width = ActualWidth;
            double height = ActualHeight;

            // shadow band check
            bool inShadowBand =
                p.X < WindowMarginOffset ||
                p.X > width - WindowMarginOffset ||
                p.Y < WindowMarginOffset ||
                p.Y > height - WindowMarginOffset;

            if (inShadowBand && Options.WindowType != WindowTypes.Fullscreen)
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
                    pl.Placeholder.Background = (Brush)UI_MainWindow.Self.FindResource("WindowBackgroundBrushMedium");

                    DockHandler.SetRedockPlaceholder(attachedControl, pl);
                    Close(); // redocks
                    return;
                }
            }
        }

        private void CreateShadow(bool fullScreen)
        {
            mainBorder.Margin = fullScreen ? new Thickness(-1) : new Thickness(WindowMarginOffset);
            mainBorder.Background = (Brush)UI_MainWindow.Self.FindResource("WindowBackgroundBrushDark");
            mainBorder.BorderBrush = (Brush)UI_MainWindow.Self.FindResource("ControlBorder");
            mainBorder.CornerRadius = new CornerRadius(fullScreen ? 0 : 10);
            mainBorder.BorderThickness = new Thickness(1);
            if (!fullScreen)
            {
                mainBorder.Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = WindowMarginOffset,
                    ShadowDepth = 0,
                    Opacity = 0.7
                };
            }
            Content = mainBorder;
            mainBorder.Child = mainGrid;
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
                        DockHandler.Redock((UserControl)sv.Content);
                    }
                    if(IsFullscreenHost)
                        Owner.Close();
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
            customTitleBar.Foreground = (Brush)UI_MainWindow.Self.FindResource("ControlTextInactive");
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            if(activeTextBrush == null)
                activeTextBrush = customTitleBar.Foreground;
            customTitleBar.Foreground = activeTextBrush;
        }
        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (Options.WindowType != WindowTypes.Resizable) // dont fire on full screen
                return IntPtr.Zero;
            return windowHelper.WndProc(hwnd, msg, wParam, lParam, ref handled);
        }
    }
}
