using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Win32;

namespace Fontendo.Extensions
{
    public static class DarkTitleBar
    {
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        /// <summary>
        /// Checks if Windows is in dark mode for apps.
        /// </summary>
        public static bool IsSystemDarkMode()
        {
            object? regVal = Registry.GetValue(
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                "AppsUseLightTheme",
                null);

            // If value == 0 → dark mode, if 1 → light mode
            return regVal is int i && i == 0;
        }

        /// <summary>
        /// Applies dark title bar if system dark mode is enabled.
        /// </summary>
        public static void Apply(Window window)
        {
            if (window == null) return;
            if (!IsSystemDarkMode()) return; // only apply if dark mode is active

            var hwnd = new WindowInteropHelper(window).Handle;
            if (Environment.OSVersion.Version.Major >= 10)
            {
                int useDark = 1;
                DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDark, sizeof(int));
            }
        }
    }
}
