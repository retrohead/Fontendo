using Fontendo.Controls;
using Fontendo.DockManager;
using Fontendo.Extensions;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Fontendo
{
    /// <summary>
    /// Interaction logic for HiddenWindow.xaml
    /// </summary>
    public partial class HiddenWindow : Window
    {
        MainWindow? mainWindow;
        CustomWindow? customWindow;
        public HiddenWindow()
        {
            InitializeComponent();
            WindowStyle = WindowStyle.None;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            base.OnContentRendered(e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            this.Hide(); // hide immediately

            CustomWindow.WindowTypes winType = CustomWindow.WindowTypes.Resizable;
            customWindow = DockHandler.CreateCustomWindow(null, new CustomWindowOptions() { WindowType = winType, ShowGripperWhenResizable = false });
            customWindow.Loaded += CustomWindow_Loaded;
            customWindow.Closed += (s, ev) => { try { this.Close(); } catch { /* may already be closed */ } };
            DockHandler.Initialize(customWindow);
            mainWindow = new MainWindow();

            customWindow.ApplyContent(mainWindow);
            customWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            customWindow.Show();
        }

        private void CustomWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Theme.initTheme(customWindow!);
            Theme.applyCustomTheme(SettingsManager.Settings.SelectedTheme, SettingsManager.Settings.ThemeColours);
            Theme.applyTheme(mainWindow!);
            DockHandler.ApplyThemeColorsToOpenWindows(Theme.getThemeColorsFromWindowResources(mainWindow!));
        }
    }
}
