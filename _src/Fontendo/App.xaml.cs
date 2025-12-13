using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Fontendo.Controls;
using Fontendo.DockManager;
using Fontendo.UI;

namespace Fontendo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string decimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        public static string groupSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
        public static string dateFormat = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        public static string fileToOpen = "";
        public App()
        {            // Handle UI thread exceptions
            this.DispatcherUnhandledException += GlobalDispatcherExceptionHandler;

            // Handle non-UI thread exceptions
            AppDomain.CurrentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
            // Handle missing drivers
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;

        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            string[] excludes = {
                "System.Private.DataContractSerialization.resources.dll",
                "System.Private.Xml.resources.dll",
                "PresentationFramework.Aero2.resources.dll",
                "System.ComponentModel.TypeConverter.resources.dll",
                "PresentationCore.resources.dll",
                "PresentationFramework.resources.dll",
                "Microsoft.VisualStudio.DesignTools.WpfTap.resources.dll",
                "SharepointConnector.resources.dll",
                "Microsoft.IdentityModel.Clients.ActiveDirectory.dll",
                "System.Web.Http.dll",
                "System.Web.Services.dll",
                "System.ServiceModel.dll",
                "System.Net.Requests.resources.dll",
                "Fontendo.resources.dll",
                "Microsoft.SharePoint.Client.Runtime.resources.dll",
                "System.Net.Requests.resources.dll",
                "SharePointPnP.IdentityModel.Extensions.dll",
                "Microsoft.AnalysisServices.Tabular.resources.dll",
                "Microsoft.AnalysisServices.Runtime.Core.resources.dll",
                "Microsoft.VisualStudio.dll",
                "System.Net.Sockets.resources.dll",
                "ICSharpCode.AvalonEdit.resources.dll",
                "Xceed.Wpf.Toolkit.resources.dll",
                "Microsoft.CSharp.resources.dll",
                "Microsoft.Data.SqlClient.resources.dll",
                "CefSharp.Wpf.resources.dll",
                "System.Xaml.resources.dll",
                "WindowsBase.resources.dll",
                "System.Private.Uri.resources.dll",
                "System.Linq.resources.dll",
                "Microsoft.AnalysisServices.Core.resources.dll",
                "System.Data.Odbc.resources.dll",
                "TOMWrapper.resources.dll",
                "System.Data.Common.resources.dll",
                "System.Data.Odbc.resources.dll",
                "System.Net.Security.resources.dll",
                "System.Net.Http.resources.dll",
                "System.Formats.Nrbf.dll",
                "System.Reflection.Metadata.dll"
            };

            string finalname = args.Name.Substring(0, args.Name.IndexOf(',')) + ".dll";

            try
            {
                // Gets the main assembly
                Assembly parentAssembly = Assembly.GetExecutingAssembly();

                // Search the resources for our DLL and get the first match
                string[]? resourcesList = parentAssembly.GetManifestResourceNames();
                string? ourResourceName = resourcesList.FirstOrDefault(name => name.EndsWith(finalname));

                if (!string.IsNullOrWhiteSpace(ourResourceName))
                {
                    // Get a stream representing our resource then load it as bytes
                    using (Stream? stream = parentAssembly.GetManifestResourceStream(ourResourceName))
                    {
                        if (stream == null || stream.Length == 0)
                        {
                            if (!excludes.Contains(finalname))
                            {
                                CustomMessageBox.Show("Failed to resolve assembly " + finalname);
                            }
                            return null;
                        }
                        byte[] block = new byte[stream.Length];
                        stream.Read(block, 0, block.Length);
                        stream.Close();
                        return Assembly.Load(block);
                    }
                }
                else
                {
                    if (!excludes.Contains(finalname))
                    {
                        CustomMessageBox.Show("Failed to resolve assembly " + finalname);
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                if (!excludes.Contains(finalname))
                {
                    CustomMessageBox.Show("Failed to load assembly " + finalname + Environment.NewLine + Environment.NewLine + ex.Message);
                }
                return null;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (e.Args.Length > 0)
            {
                string filePath = e.Args[0];
                if (File.Exists(filePath))
                    fileToOpen = filePath;
            }
        }

        private void GlobalDispatcherExceptionHandler(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ShowFatalErrorMessage(e.Exception);
            e.Handled = true; // Prevent the app from closing
        }

        private void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            ShowFatalErrorMessage(e.ExceptionObject as Exception);
        }
        public static class CustomMessageBox
        {
            public static MessageBoxResult Show(string message, string title = "Message", MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None, MessageBoxResult defaultResult = MessageBoxResult.None)
            {
                return Application.Current.Dispatcher.Invoke(() =>
                {
                    // Get the active window to ensure the MessageBox stays on top
                    Window topWindow = Application.Current.MainWindow;

                    // Show the message box and return the result
                    return MessageBox.Show(topWindow, message, title, buttons, icon, defaultResult);
                });
            }
        }
        private void ShowFatalErrorMessage(Exception? ex)
        {
            if (ex != null)
            {
                App.CustomMessageBox.Show($"A fatal error occurred:\n\n{ex.Message}\n\n" +
                                "Please screenshot this error and send it to the developer.",
                                "Fatal Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        public static void Thumb_DragDelta(object sender, DragCompletedEventArgs e)
        {
            GridViewColumnHeader header = (GridViewColumnHeader)((Thumb)sender).TemplatedParent;
            if ((header.Column.ActualWidth < 30))
                header.Column.Width = 30;
        }

        public static string ConvertSecureStringToString(SecureString? secureString)
        {
            if (secureString == null)
                return "";
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(ptr) ?? string.Empty;
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ptr); // Ensure memory cleanup
            }
        }
        public static SecureString ConvertStringToSecureString(string str)
        {
            SecureString securePwd = new SecureString();
            foreach (char c in str) // Replace with actual password input
            {
                securePwd.AppendChar(c);
            }
            return securePwd;
        }
        public static T? DeepCopy<T>(T? obj)
        {
            string json = JsonSerializer.Serialize(obj);
            return JsonSerializer.Deserialize<T>(json);
        }

        CustomWindow customWindow;
        UI_MainWindow mainWindow;
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            CustomWindow.WindowTypes winType = CustomWindow.WindowTypes.Resizable;
            if (Fontendo.Properties.Settings.Default.FullSize)
                winType = CustomWindow.WindowTypes.Fullscreen;
            customWindow = DockHandler.CreateCustomWindow(null, new CustomWindowOptions() { WindowType = winType, ShowGripperWhenResizable = false });
            customWindow.Loaded += CustomWindow_Loaded;
            //InitializeComponent();
            DockHandler.Initialize(customWindow);
            mainWindow = new UI_MainWindow();

            customWindow.ApplyContent(mainWindow);
            customWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            customWindow.Show();
        }

        private void CustomWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Theme.initTheme(customWindow);
            Theme.applyCustomTheme(Fontendo.Properties.Settings.Default.SelectedTheme, Fontendo.Properties.Settings.Default.ThemeColours);
            Theme.applyTheme(mainWindow);
            DockHandler.ApplyThemeColorsToOpenWindows(Theme.getThemeColorsFromWindowResources(mainWindow));
        }
    }
    
    #region Convertors
    public class HasChildrenToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            if (((MenuItem)value).Items.Count > 0)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
    public class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = int.Parse(parameter.ToString() ?? "");
            List<listViewColumnDataType> arr = (List<listViewColumnDataType>)value;
            if (index > -1 && index < arr.Count)
            {
                return arr[index]?.data?.ToString() ?? string.Empty;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack is not implemented.");
        }
    }

    public class ListToNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is int index && value is Array array && index < array.Length)
            {
                var data = array.GetValue(index)?.ToString()?.Trim();
                if (data == "N/A")
                {
                    return data;
                }
                return long.TryParse(data, out var number) ? number.ToString("N0", culture) : "0";
            }
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                strValue = strValue.Replace(",", string.Empty);
                return long.TryParse(strValue, out var number) ? number : 0;
            }
            return 0;
        }
    }

    public class ListToDecimalNumberBrushConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is int index && value is Array array && index < array.Length)
            {
                var data = array.GetValue(index)?.ToString()?.Trim();
                if (data == "N/A")
                {
                    return data;
                }
                if (double.TryParse(data, out var number))
                {
                    return number < 0
                        ? Application.Current.MainWindow.FindResource("ControlTextNegative")
                        : Application.Current.MainWindow.FindResource("ControlTextPositive");
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack is not implemented.");
        }
    }

    public class ListToCMPercentageBrushConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is int index && value is Array array && index < array.Length)
            {
                var data = array.GetValue(index)?.ToString()?.Trim();
                if (data == "N/A")
                {
                    return data;
                }
                if (double.TryParse(data, out var number))
                {
                    return number < 0.3
                        ? Application.Current.MainWindow.FindResource("ControlTextNegative")
                        : Application.Current.MainWindow.FindResource("ControlTextPositive");
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack is not implemented.");
        }
    }

    public class ListToDecimalNumberConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is int index && value is Array array && index < array.Length)
            {
                var data = array.GetValue(index)?.ToString()?.Trim();
                if (data == "N/A")
                {
                    return data;
                }
                data = data?.Replace(App.groupSeparator, "");
                if (string.IsNullOrEmpty(data))
                {
                    data = "0";
                }
                return double.TryParse(data, out var number)
                    ? number.ToString("N2", culture)
                    : "0.00";
            }
            return "0.00";
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                strValue = strValue.Replace(App.groupSeparator, "");
                if (string.IsNullOrEmpty(strValue))
                {
                    strValue = "0";
                }
                return double.TryParse(strValue, out var number) ? number : 0;
            }
            return 0;
        }
    }

    public class ListToLongDecimalNumberConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is int index && value is Array array && index < array.Length)
            {
                var data = array.GetValue(index)?.ToString()?.Trim();
                if (data == "N/A")
                {
                    return data;
                }
                data = data?.Replace(App.groupSeparator, "");
                return decimal.TryParse(data, out var number)
                    ? number.ToString("N5", culture)
                    : "0.00000";
            }
            return "0.00000";
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                strValue = strValue.Replace(App.groupSeparator, "");
                if (string.IsNullOrEmpty(strValue))
                {
                    strValue = "0";
                }
                return decimal.TryParse(strValue, out var number) ? number : 0;
            }
            return 0;
        }
    }

    public class ListToPercentageNumberConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is int index && value is Array array && index < array.Length)
            {
                var data = array.GetValue(index)?.ToString()?.Trim();
                if (string.IsNullOrEmpty(data))
                {
                    return "00" + App.decimalSeparator + "00%";
                }
                return double.TryParse(data, out var number)
                    ? number.ToString("P2", culture)
                    : "00" + App.decimalSeparator + "00%";
            }
            return "00" + App.decimalSeparator + "00%";
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                strValue = strValue.Replace(App.groupSeparator, "")
                                   .Replace("%", "");
                if (string.IsNullOrEmpty(strValue))
                {
                    strValue = "0" + App.decimalSeparator + "00";
                }
                return double.TryParse(strValue, out var number) ? number / 100 : 0;
            }
            return 0;
        }
    }

    public class ValueToWholeNumberConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return "0";
            }
            try
            {
                return long.TryParse(value.ToString()?.Trim(), out var number)
                    ? number.ToString("N0", culture)
                    : "0";
            }
            catch
            {
                return "0";
            }
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                strValue = strValue.Replace(App.groupSeparator, "");
                if (string.IsNullOrEmpty(strValue))
                {
                    strValue = "0";
                }
                return long.TryParse(strValue, out var number) ? number : 0;
            }
            return 0;
        }
    }

    public class ValueToWholeNumberConverterNoComma : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Only allow conversion if the correct TargetType is passed in.
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return "0";
            }
            try
            {
                return long.TryParse(value.ToString()?.Trim(), out var number) ? number.ToString("0", culture) : "0";
            }
            catch
            {
                return "0";
            }
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not used, so throw exception if this method is called.
            var newval = value?.ToString()?.Trim() ?? string.Empty;
            newval = newval.Replace(" ", "");
            if (string.IsNullOrEmpty(newval))
            {
                newval = "0";
            }
            try
            {
                return long.TryParse(newval, out var number) ? number : 0;
            }
            catch
            {
                return 0;
            }
        }
    }

    public class ValueToDecimalNumberConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Only allow conversion if the correct TargetType is passed in.
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return "0" + App.decimalSeparator + "00";
            }
            try
            {
                return double.TryParse(value.ToString()?.Trim(), out var number)
                    ? number.ToString("N2", culture)
                    : "0" + App.decimalSeparator + "00";
            }
            catch
            {
                return "0" + App.decimalSeparator + "00";
            }
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not used, so throw exception if this method is called.
            var newval = value?.ToString()?.Trim() ?? string.Empty;
            newval = newval.Replace(App.groupSeparator, "");
            if (string.IsNullOrEmpty(newval))
            {
                newval = "0";
            }
            return double.TryParse(newval, out var number) ? number : 0;
        }
    }

    public class ValueToLongDecimalNumberConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            // Only allow conversion if the correct TargetType is passed in.
            string newVal;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return "0" + App.decimalSeparator + "0000";
            }
            string val = value.ToString() ?? "";
            var decimalSeparator = App.decimalSeparator;
            if (val.Contains(decimalSeparator))
            {
                var splitval = value.ToString()?.Split(new[] { decimalSeparator }, StringSplitOptions.None);

                newVal = long.TryParse(splitval?[0], out var wholePart)
                    ? wholePart.ToString("N0", culture)
                    : "0";

                newVal += decimalSeparator + splitval?[1];
            }
            else
            {
                newVal = decimal.TryParse(value.ToString()?.Trim(), out var number)
                    ? number.ToString("N5", culture)
                    : "0" + decimalSeparator + "00000";
            }
            return newVal;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not used, so throw exception if this method is called.
            var newval = value?.ToString()?.Trim() ?? string.Empty;
            newval = newval.Replace(App.groupSeparator, "")
                           .Replace("$", "")
                           .Replace("£", "")
                           .Replace("€", "")
                           .Replace(" ", "");

            if (string.IsNullOrEmpty(newval))
            {
                newval = "0" + App.decimalSeparator + "00";
            }

            return decimal.TryParse(newval, out var number) ? number : 0;
        }
    }

    public class ValueToPercentageConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Only allow conversion if the correct TargetType is passed in.
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return "0" + App.decimalSeparator + "00%";
            }

            return double.TryParse(value.ToString()?.Trim(), out var number)
                ? number.ToString("P2", culture)
                : "0" + App.decimalSeparator + "00%";
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not used, so throw exception if this method is called.
            var newval = value?.ToString()?.Trim() ?? string.Empty;
            newval = newval.Replace(App.groupSeparator, "")
                           .Replace("%", "");

            if (string.IsNullOrEmpty(newval))
            {
                newval = "0";
            }

            return double.TryParse(newval, out var number) ? number / 100 : 0;
        }
    }

    public class ValueToTimeConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Only allow conversion if the correct TargetType is passed in.
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return "00:00:00";
            }
            try
            {
                if (DateTime.TryParse(value.ToString(), out var dateValue))
                {
                    return $"{dateValue.Hour:00}:{dateValue.Minute:00}:{dateValue.Second:00}";
                }
                else
                {
                    return "00:00:00";
                }
            }
            catch
            {
                return "00:00:00";
            }
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not used, so throw exception if this method is called.
            var newval = value?.ToString()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(newval))
            {
                newval = "00:00:00";
            }
            try
            {
                return DateTime.TryParse(newval, out var dateValue) ? dateValue : "00:00:00";
            }
            catch
            {
                return "00:00:00";
            }
        }
    }

    public class TreeViewIconSelectorConverter : IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is treeViewItemDataType data && values[1] is bool isExpanded)
            {
                return isExpanded ? (data.icon_expanded == null ? data.icon : data.icon_expanded) : data.icon;
            }
            return null; // Fallback in case of errors
        }


        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ThemeHelper
    {
        public static readonly DependencyProperty IsThemedProperty =
    DependencyProperty.RegisterAttached("IsThemed", typeof(bool), typeof(ThemeHelper),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));

        public static void SetIsThemed(DependencyObject element, bool value)
        {
            element.SetValue(IsThemedProperty, value);
        }

        public static bool GetIsThemed(DependencyObject? element)
        {
            if (element == null)
                return false;
            var isThemed = element.GetValue(IsThemedProperty);
            if (isThemed == DependencyProperty.UnsetValue)
                return false;
            return (bool)element.GetValue(IsThemedProperty);
        }
    }


    public class IconImageSourceConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MenuItem? m = value as MenuItem;
            string menuText = m?.Header?.ToString() ?? "";
            bool ItemIsThemed = ThemeHelper.GetIsThemed(m);
            if (m?.Icon == null)
                return null;
            Image? image = m.Icon == null ? null : (Image)m.Icon;
            if (!ItemIsThemed)
                return image == null ? null : image.Source;
            if (m.Icon is Image iconImage && iconImage.Source is BitmapSource bitmapSource)
            {
                return bitmapSource; // Return the ImageSource
            }
            return null; // If Icon is not an image
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class SecureStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SecureString secureString)
            {
                return ConvertSecureStringToString(secureString); // Convert securely
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                return ConvertStringToSecureString(strValue); // Convert back securely
            }
            return new SecureString();
        }

        private static string ConvertSecureStringToString(SecureString secureString)
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(ptr) ?? string.Empty;
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ptr); // Cleanup memory
            }
        }

        private static SecureString ConvertStringToSecureString(string input)
        {
            SecureString secureString = new SecureString();
            foreach (char c in input)
            {
                secureString.AppendChar(c);
            }
            return secureString;
        }
    }
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }
    public class ChevronFlipConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isExpanded = (bool)value;
            Geometry data;
            if (isExpanded)
                data = Geometry.Parse("M 10,0 L 2,8 L 10,16"); // Flipped chevron
            else
                data = Geometry.Parse("M 2,0 L 10,8 L 2,16"); // Default
            return data;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
    public class BoolToZIndexConverter_RelationshipBox : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int zindex = (value is bool b && b) ? 4 : 0;
            return zindex;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolToZIndexConverter_RelationshipLine : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int zindex = (value is bool b && b) ? 1 : -3;
            return zindex;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolToZIndexConverter_RelationshipLabel : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int zindex = (value is bool b && b) ? 2 : -1;
            return zindex;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolToZIndexConverter_RelationshipLabelCenter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int zindex = (value is bool b && b) ? 3 : -2;
            return zindex;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion



    public class HexValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long l)
                return $"0x{l:X}";
            if (value is int i)
                return $"0x{i:X8}";
            if (value is UInt32 u32)
                return $"0x{u32:X8}";
            if (value is UInt16 u16)
                return $"0x{u16:X4}";
            if (value is byte b)
                return $"0x{b:X2}";
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value?.ToString()?.Trim().Replace("0x", "") ?? "";
            if (long.TryParse(str, NumberStyles.HexNumber, culture, out var parsed))
            {
                if (targetType == typeof(short)) return (short)parsed;
                if (targetType == typeof(ushort)) return (ushort)parsed;
                if (targetType == typeof(int)) return (int)parsed;
                if (targetType == typeof(uint)) return (uint)parsed;
                if (targetType == typeof(long)) return parsed;
                return parsed;
            }
            return 0;
        }
    }

}
