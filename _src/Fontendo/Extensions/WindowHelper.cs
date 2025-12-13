using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace Fontendo.Extensions
{
    internal class WindowHelper
    {

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetAncestor(IntPtr hwnd, uint gaFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags);


        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left, Top, Right, Bottom; }

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        private const int WM_GETMINMAXINFO = 0x0024;
        private const int WM_NCHITTEST = 0x0084;
        private const int HTCLIENT = 1;
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;
        private const uint GA_ROOT = 2;
        private const uint GW_HWNDNEXT = 2;
        private const int SW_RESTORE = 9;
        private static readonly IntPtr HWND_TOP = new IntPtr(0);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;

        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int WS_DISABLED = 0x08000000;
        public const int WS_VISIBLE = 0x10000000;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_NOACTIVATE = 0x00000080;

        private const int ResizeBorderThickness = 8;   // active resize grip size
        private int WindowMarginOffset = 50;

        Window activeWindow;

        public WindowHelper(Window activeWindow)
        {
            this.activeWindow = activeWindow;
        }

        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_NCHITTEST)
            {
                // mouse position in screen coords
                int x = (short)(lParam.ToInt32() & 0xFFFF);
                int y = (short)(lParam.ToInt32() >> 16);
                Point mouse = activeWindow.PointFromScreen(new Point(x, y));

                double width = activeWindow.ActualWidth;
                double height = activeWindow.ActualHeight;

                // inner content rect (exclude 50px shadow band)
                double innerLeft = WindowMarginOffset;
                double innerTop = WindowMarginOffset;
                double innerRight = width - WindowMarginOffset;
                double innerBottom = height - WindowMarginOffset;

                handled = true;

                // 1) In the shadow band? Treat as client (no resize).
                bool inShadowBand =
                    mouse.X < innerLeft ||
                    mouse.X > innerRight ||
                    mouse.Y < innerTop ||
                    mouse.Y > innerBottom;

                if (inShadowBand)
                    return (IntPtr)HTCLIENT;

                // 2) Inside inner rect: check edges against inner borders
                bool nearLeft = mouse.X <= innerLeft + ResizeBorderThickness;
                bool nearRight = mouse.X >= innerRight - ResizeBorderThickness;
                bool nearTop = mouse.Y <= innerTop + ResizeBorderThickness;
                bool nearBottom = mouse.Y >= innerBottom - ResizeBorderThickness;

                // Corners first
                if (nearTop && nearLeft) return (IntPtr)HTTOPLEFT;
                if (nearTop && nearRight) return (IntPtr)HTTOPRIGHT;
                if (nearBottom && nearLeft) return (IntPtr)HTBOTTOMLEFT;
                if (nearBottom && nearRight) return (IntPtr)HTBOTTOMRIGHT;

                // Edges
                if (nearTop) return (IntPtr)HTTOP;
                if (nearBottom) return (IntPtr)HTBOTTOM;
                if (nearLeft) return (IntPtr)HTLEFT;
                if (nearRight) return (IntPtr)HTRIGHT;

                // 3) Otherwise: regular client area
                handled = true;
                return (IntPtr)HTCLIENT;
            }
            if (msg == WM_GETMINMAXINFO)
            {
                MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

                // enforce minimum size including your shadow margin
                int minheight = 40;
                int minwidth = 40;
                if ((int)activeWindow.MinHeight > minheight)
                    minheight = (int)activeWindow.MinHeight;
                if ((int)activeWindow.MinWidth > minwidth)
                    minwidth = (int)activeWindow.MinWidth;
                mmi.ptMinTrackSize.X = minwidth + (WindowMarginOffset * 2);
                mmi.ptMinTrackSize.Y = minheight + (WindowMarginOffset * 2);

                Marshal.StructureToPtr(mmi, lParam, true);
                handled = true;
            }

            return IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
        public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
        public static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
                return GetWindowLongPtr64(hWnd, nIndex);
            else
                return GetWindowLongPtr32(hWnd, nIndex);
        }
        public static int GetWindowLong(IntPtr hWnd, int nIndex)
        {
            return (int)GetWindowLongPtr(hWnd, nIndex);
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
        }


        public bool IsRealWindow(IntPtr hWnd)
        {
            // ✅ Skip invisible windows
            if (!IsWindowVisible(hWnd))
            {
                return false;
            }

            // ✅ Skip tool windows (floating helpers, overlays, GPU windows, etc.)
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            if ((exStyle & WS_EX_TOOLWINDOW) != 0)
                return false;

            // ✅ Skip windows that cannot be activated
            if ((exStyle & WS_EX_NOACTIVATE) != 0)
                return false;

            // ✅ Skip disabled windows
            int style = GetWindowLong(hWnd, GWL_STYLE);
            if ((style & WS_DISABLED) != 0)
                return false;

            return true;
        }

        public void ActivateUnderlyingWindow(Window activeWindow, Point screen)
        {
            POINT pt;
            pt.X = (int)screen.X;
            pt.Y = (int)screen.Y;

            IntPtr myHandle = new WindowInteropHelper(activeWindow).Handle;

            // Start with the window at the point
            IntPtr hWnd = WindowFromPoint(pt);

            // Walk down the Z-order until we find a window under the point that isn't us
            while (hWnd != IntPtr.Zero)
            {
                if (hWnd != myHandle && IsRealWindow(hWnd))
                {
                    string title = GetWindowTitle(hWnd);
                    if (GetWindowRect(hWnd, out RECT rect))
                    {

                        // ignore hidden helper windows
                        if (!string.Equals(title, "Hidden Window", StringComparison.OrdinalIgnoreCase))
                        {
                            if (pt.X >= rect.Left && pt.X <= rect.Right &&
                                pt.Y >= rect.Top && pt.Y <= rect.Bottom)
                            {
                                //ShowWindow(hWnd, SW_RESTORE);
                                BringWindowToTop(hWnd);
                                SetForegroundWindow(hWnd);
                                SetActiveWindow(hWnd);
                                SetFocus(hWnd);
                                return;
                            }
                        }
                    }
                }

                // step to next window in Z-order
                hWnd = GetWindow(hWnd, GW_HWNDNEXT);
            }
        }
        private static string GetWindowTitle(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            if (length == 0) return string.Empty;

            var sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        public static void BringToFront(nint handle)
        {
            WindowHelper.SetWindowPos(handle, HWND_TOP, 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }


        [DllImport("user32.dll")]
        static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        public static Rect GetWorkingArea(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var monitor = MonitorFromWindow(hwnd, 2); // MONITOR_DEFAULTTONEAREST

            var info = new MONITORINFO();
            info.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            GetMonitorInfo(monitor, ref info);

            // Convert Win32 pixel rect → WPF device-independent rect
            var source = HwndSource.FromHwnd(hwnd);
            if (source != null)
            {
                var transform = source.CompositionTarget.TransformFromDevice;

                var topLeft = transform.Transform(new Point(info.rcWork.Left, info.rcWork.Top));
                var bottomRight = transform.Transform(new Point(info.rcWork.Right, info.rcWork.Bottom));

                return new Rect(topLeft, bottomRight);
            }

            // fallback (rare)
            return new Rect(
                info.rcWork.Left,
                info.rcWork.Top,
                info.rcWork.Right - info.rcWork.Left,
                info.rcWork.Bottom - info.rcWork.Top);
        }

        public static void HideWindow(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            int exStyle = (int)GetWindowLongPtr(hwnd, GWL_EXSTYLE);

            exStyle |= WS_EX_TOOLWINDOW; // hide from Z-order and Alt-Tab
            SetWindowLongPtr(hwnd, GWL_EXSTYLE, (IntPtr)exStyle);
        }
    }
}
