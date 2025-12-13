using Fontendo.Controls;
using Fontendo.Extensions;
using Fontendo.UI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static Fontendo.Controls.CustomWindow;

namespace Fontendo.DockManager
{
    public static class DockHandler
    {
        internal static bool Initialized = false;
        internal const int DragThreshold = 8; // pixels before drag will allow drop
        public class PlaceHolderInfo
        {
            public FrameworkElement ParentControl;
            public Border? Placeholder;
            public Grid? OwnerGrid;
            public int Row;
            public int Col;
            public int RowSpan;
            public int ColSpan;
            public double OrigMinWidth;
            public DockInfo? AttachedDockInfo;
            public bool IsShowing = false;

            public PlaceHolderInfo(FrameworkElement parent)
            {
                ParentControl = parent;
            }
        }

        public class DockInfo
        {
            public Window ParentWindow;
            public DockablePanel DockPanel = null!;
            public UserControl ContentControl = null!;
            public Window? FloatingWindow = null!;
            public string Name = null!;
            public PlaceHolderInfo? PlaceHolderInfo;
            // Track drag state for threshold logic for floating form
            public Point? dragOrigin = null;
            public bool movedEnough = false;
            public bool isMouseDown = false;

            public DockInfo(Window parentWindow, DockablePanel dockPanel, UserControl contentControl, string name)
            {
                ParentWindow = parentWindow;
                DockPanel = dockPanel;
                ContentControl = contentControl;
                Name = name;
            }
        }


        private static readonly Dictionary<UserControl, DockInfo> _dockRegistry = new();
        private static readonly List<PlaceHolderInfo> _placeholderRegistry = new();
        private static readonly List<CustomWindow> _windowRegistry = new();

        public static void Initialize(Window parentWindow)
        {
            Initialized = true;
            parentWindow.Closing += ParentWindow_Closing; // make sure no windows are left open when parent is closed
            parentWindow.Activated += ParentWindow_Activated; // activate all windows we have open when parent is activated
        }

        internal static void ParentWindow_Activated(object? sender, EventArgs e)
        {
            BringAllToFront();
        }

        internal static void ParentWindow_Closing(object? sender, EventArgs e)
        {
            CloseAll();
        }

        public static void Register(Window ParentWindow, DockablePanel dockPanel, UserControl contentControl, string name)
        {
            if (!Initialized) throw new Exception("DockManager is not initialized, make sure to run DockHandler.Initialize() in your parent window");

            if (_dockRegistry.ContainsKey(contentControl)) return; // already registered

            var parent = VisualTreeHelper.GetParent(dockPanel) as FrameworkElement
                         ?? throw new InvalidOperationException("Control must have a parent before registering.");

            dockPanel.InitializePanel(name, contentControl);
            var info = new DockInfo(ParentWindow, dockPanel, contentControl, name);
            // Find the owning Grid (if any)
            Grid? tlp = null;
            FrameworkElement? chain = parent;
            if (chain is Grid t)
                tlp = t;


            var plinfo = new PlaceHolderInfo(parent)
            {
                OwnerGrid = tlp,
                AttachedDockInfo = info,
                IsShowing = true
            };
            // Determine the actual cell host and cell coordinates
            if (tlp != null)
            {
                // dock panel is a table cell
                plinfo.Row = Grid.GetRow(dockPanel);
                plinfo.Col = Grid.GetColumn(dockPanel);
                plinfo.RowSpan = Grid.GetRowSpan(dockPanel);
                plinfo.ColSpan = Grid.GetColumnSpan(dockPanel);
            }
            else
            {
                plinfo.Row = -1;
                plinfo.Col = -1;
                plinfo.RowSpan = -1;
                plinfo.ColSpan = -1;
            }

            info.PlaceHolderInfo = plinfo;
            _dockRegistry[contentControl] = info;
            _placeholderRegistry.Add(plinfo);
        }
        internal static IEnumerable<T> FindLogicalChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) yield break;

            foreach (var child in LogicalTreeHelper.GetChildren(parent))
            {
                if (child is DependencyObject dep)
                {
                    if (child is T t)
                        yield return t;

                    foreach (var sub in FindLogicalChildren<T>(dep))
                        yield return sub;
                }
            }
        }
        internal static void EnsurePlaceholder(DockInfo info)
        {
            if (info.PlaceHolderInfo == null || info.PlaceHolderInfo.Placeholder != null)
                return; // already exists

            info.PlaceHolderInfo.Placeholder = new Border
            {
                Background = (Brush)UI_MainWindow.Self.FindResource("WindowBackgroundBrushMedium"),
                BorderThickness = new Thickness(1),
                BorderBrush = (Brush)UI_MainWindow.Self.FindResource("ControlBorder")
            }; 

            if (info.PlaceHolderInfo.OwnerGrid != null)
            {
                var grid = info.PlaceHolderInfo.OwnerGrid;
                Grid.SetRow(info.PlaceHolderInfo.Placeholder, info.PlaceHolderInfo.Row);
                Grid.SetColumn(info.PlaceHolderInfo.Placeholder, info.PlaceHolderInfo.Col);
                Grid.SetRowSpan(info.PlaceHolderInfo.Placeholder, info.PlaceHolderInfo.RowSpan);
                Grid.SetColumnSpan(info.PlaceHolderInfo.Placeholder, info.PlaceHolderInfo.ColSpan);
                grid.Children.Add(info.PlaceHolderInfo.Placeholder);

                // 1. capture current column width
                var splitGrid = (Grid)info.PlaceHolderInfo.OwnerGrid.Parent;
                int destcol = Grid.GetColumn(info.PlaceHolderInfo.OwnerGrid);
                double currentWidth = splitGrid.ColumnDefinitions[destcol].ActualWidth;

                // 2. store min width and set column to Auto
                info.PlaceHolderInfo.OrigMinWidth = splitGrid.ColumnDefinitions[destcol].MinWidth;
                splitGrid.ColumnDefinitions[destcol].Width = GridLength.Auto;
                splitGrid.ColumnDefinitions[destcol].MinWidth = 0;

                // 3. if column is the final column, also set the max with on previous column
                if(destcol == splitGrid.ColumnDefinitions.Count() - 1)
                    splitGrid.ColumnDefinitions[destcol - 2].MaxWidth = double.PositiveInfinity;

                AnimateBorderClose(info.PlaceHolderInfo.Placeholder, currentWidth);
            }
            else
            {
                throw new Exception("Placeholder creation for non-grid parents is not implemented");
            }
        }

        /// <summary>
        /// Animates a border to shrink its min width down to 0, 
        /// if fromWidth is not sent the current border width will be uses
        /// </summary>
        /// <param name="placeholder"></param>
        /// <param name="fromWidth"></param>
        internal static void AnimateBorderClose(Border placeholder, double fromWidth = -1)
        {
            if (fromWidth == -1)
                fromWidth = placeholder.ActualWidth;
            // 4. animate border width down to zero
            var anim = new DoubleAnimation
            {
                From = fromWidth,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.25),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            placeholder.BeginAnimation(FrameworkElement.WidthProperty, anim);
        }

        /// <summary>
        /// Animates a border to grow its min width upto the set size, 
        /// </summary>
        /// <param name="placeholder"></param>
        /// <param name="fromWidth"></param>
        internal static void AnimateBorderOpen(Border placeholder, double toWidth)
        {
            if (toWidth == -1)
                toWidth = placeholder.ActualWidth;
            // 4. animate border width down to zero
            var anim = new DoubleAnimation
            {
                From = placeholder.ActualWidth,
                To = toWidth,
                Duration = TimeSpan.FromSeconds(0.25),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            placeholder.BeginAnimation(FrameworkElement.WidthProperty, anim);
        }

        internal static Rect GetScreenRect(FrameworkElement element)
        {
            if (element == null || !element.IsVisible)
                return Rect.Empty;

            // Top-left relative to the window
            var topLeft = element.PointToScreen(new Point(0, 0));

            // Bottom-right relative to the window
            var bottomRight = element.PointToScreen(
                new Point(element.ActualWidth, element.ActualHeight));

            return new Rect(topLeft, bottomRight);
        }

        public static CustomWindow CreateCustomWindow(Window? OwningWindow, CustomWindowOptions options)
        {
            CustomWindow customWindow = new CustomWindow(OwningWindow, options);
            customWindow.Closing += (s, e) =>
            {
                _windowRegistry.Remove(customWindow);
            };
            _windowRegistry.Add(customWindow);
            return _windowRegistry.Last();
        }
        public static void PopOut(UserControl control)
        {
            if (!_dockRegistry.TryGetValue(control, out var info)) return;
            if (info.FloatingWindow != null) return;
            if (info.PlaceHolderInfo == null) return;

            EnsurePlaceholder(info);

            DockablePanel dockablePanel = info.DockPanel;
            // Remove control from its immediate parent (wrapper or table)
            int minHeight = 300;
            ((Panel)info.PlaceHolderInfo.ParentControl).Children.Remove(dockablePanel);
            info.DockPanel.panelScrollablePanel.Content = null;
            var placeholderRect = GetScreenRect(info.PlaceHolderInfo.ParentControl);
            var floatWin = CreateCustomWindow(info.ParentWindow, new CustomWindowOptions() { WindowType = WindowTypes.Resizable });
            floatWin.Title = info.Name;
            floatWin.Width = placeholderRect.Width + 50;
            floatWin.Height = placeholderRect.Height < minHeight ? minHeight : placeholderRect.Height + 50;
            floatWin.Left = placeholderRect.Left;
            floatWin.Top = placeholderRect.Top;
            floatWin.SetDockableContent(control);
            // Apply intended offset
            const int offset = 16;
            floatWin.Left = floatWin.Left + offset;
            floatWin.Top = floatWin.Top + offset;

            floatWin.Closed += (s, e) =>
            {
                Redock(control);
            };
            info.FloatingWindow = floatWin;

            // Highlight placeholders while moving
            floatWin.LocationChanged += FloatingWindow_LocationChanged;
            floatWin.Show();
            floatWin.Focus();
        }

        internal static void FloatingWindow_LocationChanged(object? sender, EventArgs e)
        {
            if (sender is not Window floatForm) return;
            if (floatForm.Content == null || floatForm.ActualWidth == 0) return;
            Border? bd = floatForm.Content as Border;
            Grid grid = (Grid)bd.Child;
            Border innerborder = (Border)grid.Children[1];
            ScrollViewer sv = (ScrollViewer)innerborder.Child;
            if (sv == null) throw new Exception("Dockable window scroll viewer not found");
            if (!_dockRegistry.TryGetValue((UserControl)sv.Content, out var info)) return;
            if (info.FloatingWindow == null) return;
            if (info.PlaceHolderInfo == null) return;
            if (((CustomWindow)floatForm).IsMouseHeld == true)
            {
                // Only consider moves while mouse is down (actual drag), not when the owner form moves
                if (!((CustomWindow)floatForm).IsDragging)
                {
                    ((CustomWindow)floatForm).IsDragging = true;
                    info.movedEnough = false;
                    info.dragOrigin = new Point(floatForm.Left, floatForm.Top);
                }
                else if (!info.movedEnough)
                {
                    double dx = floatForm.Left - info.dragOrigin.Value.X;
                    double dy = floatForm.Top - info.dragOrigin.Value.Y;
                    if (dx * dx + dy * dy >= DragThreshold * DragThreshold) // Euclidean threshold
                        info.movedEnough = true;
                }
            } else
            {
                info.movedEnough = false;
                ((CustomWindow)floatForm).IsDragging = false;
                info.dragOrigin = null;
            }

            PlaceHolderInfo? pl = GetUnderlyingPlaceholder((CustomWindow)info.FloatingWindow, info.ContentControl);
            // only highlight placeholders after threshold to reduce jitter
            foreach (var kvp in _placeholderRegistry)
            {
                if (kvp.Placeholder == null) continue;

                if (kvp == pl || kvp.Placeholder.ActualWidth < 2)
                {
                    kvp.Placeholder.Background = (Brush)UI_MainWindowContent.Self.FindResource("ButtonMouseOverBrush");
                    kvp.Placeholder.BorderBrush = (Brush)UI_MainWindowContent.Self.FindResource("ButtonSelectedBrush");
                    AnimateBorderOpen(kvp.Placeholder, kvp.OrigMinWidth);
                }
                else
                {
                    kvp.Placeholder.Background = (Brush)UI_MainWindowContent.Self.FindResource("WindowBackgroundBrushMedium");
                    kvp.Placeholder.BorderBrush = (Brush)UI_MainWindowContent.Self.FindResource("ButtonMouseOverBrush");
                    AnimateBorderClose(kvp.Placeholder);
                }
            }
        }

        public static PlaceHolderInfo? GetUnderlyingPlaceholder(CustomWindow win, UserControl control)
        {
            foreach (var kvp in _placeholderRegistry)
            {
                if (kvp.Placeholder == null) continue;

                // Convert placeholder bounds to screen coordinates
                var topLeft = kvp.Placeholder.PointToScreen(new Point(0, 0));
                var bottomRight = kvp.Placeholder.PointToScreen(
                    new Point(kvp.Placeholder.ActualWidth == 0 ? 10 : kvp.Placeholder.ActualWidth, kvp.Placeholder.ActualHeight));
                var rect = new Rect(topLeft, bottomRight);
                if (win.ActualWidth == 0)
                    continue;
                var floatRect = 
                    new Rect(
                        win.Left + CustomWindow.WindowMarginOffsetBorder, 
                        win.Top + CustomWindow.WindowMarginOffsetBorder, 
                        win.ActualWidth - CustomWindow.WindowMarginOffsetBorder * 2, 
                        win.ActualHeight - CustomWindow.WindowMarginOffsetBorder * 2
                        );

                _dockRegistry.TryGetValue(control, out var info);
                if (info == null) throw new Exception("Could not find info on window control");
                if (info.movedEnough && rect.IntersectsWith(floatRect))
                {
                    return kvp;

                }
            }
            return null;
        }

        /// <summary>
        /// Redocks into the current PlaceholderInfo attached to the control.
        /// If a control is already attached to the placeholder, a new home is found for it
        /// </summary>
        /// <param name="control"></param>
        /// <exception cref="Exception"></exception>
        public static void Redock(UserControl control)
        {
            if (!_dockRegistry.TryGetValue(control, out var info)) return;
            if (info.FloatingWindow == null) return;
            if (info.PlaceHolderInfo == null) return;

            info.FloatingWindow.Content = null;
            info.DockPanel.Reattach(control);
            info.FloatingWindow = null;

            var placeholderparent = info.PlaceHolderInfo.ParentControl as Panel;
            if (info.PlaceHolderInfo.Placeholder != null)
            {
                placeholderparent?.Children.Remove(info.PlaceHolderInfo.Placeholder);
                info.PlaceHolderInfo.Placeholder = null;
            }
            if (info.PlaceHolderInfo.OwnerGrid != null)
            {
                Grid.SetColumn(info.DockPanel, info.PlaceHolderInfo.Col);
                Grid.SetRow(info.DockPanel, info.PlaceHolderInfo.Row);
                Grid.SetColumnSpan(info.DockPanel, info.PlaceHolderInfo.ColSpan);
                Grid.SetRowSpan(info.DockPanel, info.PlaceHolderInfo.RowSpan);
                if (!info.PlaceHolderInfo.OwnerGrid.Children.Contains(info.DockPanel))
                    info.PlaceHolderInfo.OwnerGrid.Children.Add(info.DockPanel);

                if (info.PlaceHolderInfo.Placeholder != null && info.PlaceHolderInfo.OwnerGrid.Children.Contains(info.PlaceHolderInfo.Placeholder))
                {
                    info.PlaceHolderInfo.OwnerGrid.Children.Remove(info.PlaceHolderInfo.Placeholder);
                    info.PlaceHolderInfo.Placeholder = null; // 👈 reset so EnsurePlaceholder can recreate later
                }

                if (!placeholderparent.Children.Contains(info.DockPanel))
                    placeholderparent.Children.Add(info.DockPanel);
                // reset min width on the column

            }
            else
            {
                throw new Exception("Redocking to non-grid parents is not implemented");
            }

            // store the current attached dock info so we can give the current placeholder owner a home
            DockInfo? orphan = info.PlaceHolderInfo.AttachedDockInfo;
            if (orphan != null && orphan != info)
            {
                // find the orhpan a home
                var freeSlot = _placeholderRegistry.FirstOrDefault(p => p.AttachedDockInfo == null);
                if (freeSlot == null) throw new Exception("Could not find a home for an orphan dock");
                orphan.PlaceHolderInfo = freeSlot;
                freeSlot.AttachedDockInfo = orphan;
            }

            info.PlaceHolderInfo.AttachedDockInfo = info;

            // reset the column min width
            var splitGrid = (Grid)info.PlaceHolderInfo.OwnerGrid.Parent;
            int destcol = Grid.GetColumn(info.PlaceHolderInfo.OwnerGrid);
            splitGrid.ColumnDefinitions[destcol].MinWidth = info.PlaceHolderInfo.OrigMinWidth;
        }

        internal static void SetRedockPlaceholder(UserControl attachedControl, PlaceHolderInfo pl)
        {
            if (!_dockRegistry.TryGetValue(attachedControl, out var info)) return;
            info.PlaceHolderInfo.AttachedDockInfo = null; // disconnect from the current placeholder
            info.PlaceHolderInfo = pl; // attach to the new placeholder
            // DO NOT update dock info, this is handled during redock
        }

        internal static void CloseAll()
        {
            foreach (var info in _dockRegistry.Values)
            {
                if(info.FloatingWindow != null)
                    ((CustomWindow)info.FloatingWindow).CloseRequest();
            }
        }

        internal static void BringAllToFront()
        {
            if (UI_MainWindow.Self.Window == null)
                return;
            var mainHandle = new WindowInteropHelper(UI_MainWindow.Self.Window).Handle;

            WindowHelper.BringToFront(mainHandle);
            int index = 0;
            foreach (var info in _dockRegistry.Values)
            {
                var win = info.FloatingWindow;
                if (win == null) continue;

                var handle = new WindowInteropHelper(win).Handle;

                // simulate z-index: mainForm is baseline, panels are above it in order
                // we can’t literally assign a numeric ZIndex to HWNDs, but we can reorder them
                // by repeatedly calling SetWindowPos with HWND_TOP
                WindowHelper.BringToFront(handle);

                index++;
            }
        }

        public static void ApplyThemeColorsToOpenWindows(Theme.ThemeColorsType themeColors)
        {
            foreach (var win in _windowRegistry)
            {
                Theme.applyThemeColors(win, themeColors);
            }
        }
    }
}
