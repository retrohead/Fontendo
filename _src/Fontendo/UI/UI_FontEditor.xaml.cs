using Fontendo.Controls;
using Fontendo.DockManager;
using Fontendo.Extensions;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static Fontendo.FontProperties.PropertyList;
using System.Drawing;

namespace Fontendo.UI
{
    /// <summary>
    /// Interaction logic for FontEditor.xaml
    /// </summary>
    public partial class UI_FontEditor : CustomWindowContentBase, INotifyPropertyChanged
    {
        private System.Windows.Media.Color _selectedColor;
        public System.Windows.Media.Color SelectedColor 
        {
            get => _selectedColor;
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedColor)));
                }
            }
        }
        private FontBase? LoadedFont;
        bool initialized = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public UI_FontEditor()
        {
            DataContext = this;
            InitializeComponent();
            SelectedColor = ColorHelper.ToMediaColor(SettingsManager.Settings.FontBackgroundColor);
        }



        public void ShowFontDetails(FontBase? font)
        {
            if (!initialized)
            {
                // Register legacy encodings (including Shift-JIS)
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                btnExportSheet.SetBinding(Button.IsEnabledProperty,
                    new Binding(nameof(UI_MainWindow.Self.ButtonEnabler.IsSheetSelected)) { Source = UI_MainWindow.Self.ButtonEnabler });

                btnReplaceSheet.SetBinding(Button.IsEnabledProperty,
                    new Binding(nameof(UI_MainWindow.Self.ButtonEnabler.IsSheetSelected)) { Source = UI_MainWindow.Self.ButtonEnabler });
                initialized = true;
            }
            if (UI_MainWindow.Self == null) return;
            LoadedFont = font;
            if (font == null)
            {
                ClearFontDetails();
                return;
            }
            BuildPropertiesPanel(panelFontProperties, font);
        }
        public void ClearFontDetails()
        {
            panelFontProperties.Children.Clear();
        }

        private void btnExportSheet_Click(object sender, RoutedEventArgs e)
        {
            ExportSheet();
        }

        public void ExportSheet()
        {
            int index = UI_MainWindow.Self.SelectedSheet;
            if (index < 0 || LoadedFont == null || LoadedFont.Settings.Sheets == null)
                return;
            string fileName = FileSystemHelper.BrowseForSaveFile(FileSystemHelper.FileType.Png, "Export Sheet Image", $"Sheet {index + 1}.png");
            if (fileName == "") return;
            LoadedFont.Settings.Sheets.Images[index].Save(fileName);
            MessageBox.Show("Sheet image exported successfully.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        public void ReplaceSheet()
        {
            int index = UI_MainWindow.Self.SelectedSheet;
            if (index < 0 || LoadedFont == null || LoadedFont.Settings.Sheets == null) return;

            string fileName = FileSystemHelper.BrowseForFile(FileSystemHelper.FileType.Png, "Import Sheet Image");
            if (fileName == "") return;
            Bitmap bmp = new Bitmap(fileName);
            if (bmp.Width != LoadedFont.Settings.Sheets.Width || bmp.Height != LoadedFont.Settings.Sheets.Height)
            {
                MessageBox.Show($"The size of the selected image file ({bmp.Width}x{bmp.Height}) does not match the size of the loaded sheet ({LoadedFont.Settings.Sheets.Width}x{LoadedFont.Settings.Sheets.Height})", "File Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                bmp.Dispose();
                return;
            }
            if (LoadedFont.Settings.Sheets.Images[index] != null)
                LoadedFont.Settings.Sheets.Images[index].Dispose();
            LoadedFont.Settings.Sheets.Images[index] = (Bitmap)bmp.Clone();
            UI_MainWindow.SheetItem? item = UI_MainWindow.Self.GetSelectedSheetItem();
            if (item == null) throw new Exception("Failed to get the selected sheet item");
            item.Image = UI_MainWindow.ConvertBitmap(LoadedFont.Settings.Sheets.Images[index]);
            item.Tag = LoadedFont.Settings.Sheets.Images[index];
            bmp.Dispose();
            MessageBox.Show("Sheet image imported successfully.", "Import Successful", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        public void BuildPropertiesPanel(StackPanel panel, FontBase font)
        {
            panel.Children.Clear();

            // Create a Grid to mimic TableLayoutPanel
            var grid = new Grid
            {
                Margin = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top
            };

            // Define two columns: first fixed 100px, second fills remaining
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Padding row at start
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10) });

            int rowIndex = 1;

            foreach (var kvp in font.Settings.PropertyDescriptors)
            {
                var descriptor = kvp.Value;

                if (descriptor.PreferredControl != EditorType.None)
                {
                    var label = new TextBlock
                    {
                        Text = descriptor.Name,
                        Margin = new Thickness(3, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 12, // adjust as needed
                        Foreground = (System.Windows.Media.Brush)MainWindow.Self.FindResource("LabelTextBrush")
                    };

                    Grid.SetRow(label, rowIndex);
                    Grid.SetColumn(label, 0);
                    grid.Children.Add(label);
                }

                font.Settings.GetBindingsForObject(kvp.Key, out var bindings);
                FrameworkElement? editor = CreateControlForEditorType(descriptor.PreferredControl, descriptor, bindings);

                if (editor != null)
                {
                    editor.Margin = new Thickness(0, 5, 0, 0);
                    Grid.SetRow(editor, rowIndex);
                    Grid.SetColumn(editor, 1);
                    grid.Children.Add(editor);

                    // Fix row height to 32px
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32) });
                    rowIndex++;
                }
            }

            // Add final blank row that stretches
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            panel.Children.Add(grid);
        }

        public void btnReplaceSheet_Click(object sender, RoutedEventArgs e)
        {
            ReplaceSheet();
        }

        private void ColorPickerButton_SelectedColorChanged(object sender, ColorPicker.ColorChangedEventArgs e)
        {
            SelectedColor = e.SelectedColor;
            if (UI_MainWindow.Self == null)
                return;
            UI_MainWindow.Self.SetBackgroundColour(ColorHelper.ToDrawingColor(e.SelectedColor), true);
        }

        private void colorPicker_PreviewSelectedColorChanged(object sender, ColorPicker.ColorChangedEventArgs e)
        {
            UI_MainWindow.Self.SetBackgroundColour(ColorHelper.ToDrawingColor(e.SelectedColor), false);
        }
    }
}
