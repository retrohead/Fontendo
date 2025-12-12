using Fontendo.Controls;
using Fontendo.DockManager;
using Fontendo.Extensions;
using Fontendo.UI;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fontendo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UI_MainWindow mainWindow;
        public MainWindow()
        {
            InitializeComponent();
            DockHandler.Initialize(this);
            WindowState = WindowState.Minimized;
            Visibility = Visibility.Collapsed;
            ShowInTaskbar = false;
            mainWindow = new UI_MainWindow();
            Theme.initTheme(this);
            Theme.applyCustomTheme(Properties.Settings.Default.SelectedTheme, Properties.Settings.Default.ThemeColours);
            Theme.applyTheme(mainWindow);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CustomWindow win = DockHandler.CreateCustomWindow(this, new CustomWindowOptions() { WindowType = CustomWindow.WindowTypes.Resizable, ShowGripperWhenResizable = false });
            win.Closed += (s, args) => this.Close();
            win.ApplyContent(mainWindow);
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.Show();
            DockHandler.ApplyThemeColorsToOpenWindows(Theme.getThemeColorsFromWindowResources(mainWindow));
        }
    }
}