using Fontendo.Controls;
using Fontendo.DockManager;
using Fontendo.Extensions;
using Fontendo.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static Fontendo.Extensions.FontBase;
using static Fontendo.FontProperties.PropertyList;
using Color = System.Drawing.Color;

namespace Fontendo.UI
{
    /// <summary>
    /// Interaction logic for FontEditor.xaml
    /// </summary>
    public partial class UI_GlyphEditor : CustomWindowContentBase, INotifyPropertyChanged
    {
        private float zoomFactor = 1.0f;
        private float maxZoomFactor = 1.0f;
        private bool userHasSetZoom = false;
        private bool suppressZoomEvent = false;
        ScaleTransform zoomTransform = new ScaleTransform();
        bool initialized = false;

        private Color glyphBackground = Color.White;
        public Color GlyphBackground
        {
            get => glyphBackground;
            set
            {
                if(glyphBackground != value)
                {
                    glyphBackground = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(GlyphBackground)));
                    updateBackground();
                }
            }
        }

        private BitmapSource? glyphImage;
        public BitmapSource? GlyphImage
        {
            get { return glyphImage; }
            set
            {
                if (glyphImage != value)
                {
                    glyphImage = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(GlyphImage)));
                }
            }
        }

        private BitmapSource? glyphMaskImage;
        public BitmapSource? GlyphMaskImage
        {
            get { return glyphMaskImage; }
            set
            {
                if (glyphMaskImage != value)
                {
                    glyphMaskImage = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(GlyphMaskImage)));
                }
            }
        }

        private void updateBackground()
        {
            try
            {
                if (LoadedGlyph?.Settings.Image != null)
                    borderGlyphImage.Background = new SolidColorBrush(ColorHelper.ToMediaColor(GlyphBackground));
                else
                    borderGlyphImage.Background = (System.Windows.Media.Brush)MainWindow.Self.FindResource("WindowBackgroundBrushMedium");
            }
            catch
            {
                // ignore errors during background update

            }
        }

        Glyph? _loadedGlyph;
        public Glyph? LoadedGlyph 
        {
            get { return _loadedGlyph; }
            set
            {
                if (_loadedGlyph != value)
                {
                    _loadedGlyph = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(LoadedGlyph)));
                }
            }

        }

        public UI_FontEditor FontEditor { get; }

        public UI_GlyphEditor(UI_FontEditor fontEditor)
        {
            FontEditor = fontEditor;
            suppressZoomEvent = true;
            InitializeComponent();
            suppressZoomEvent = false;
            GlyphBackground = ColorHelper.HexToColor(SettingsManager.Settings.FontBackgroundColor);
            
        }


        public void ShowGlyphDetails(Glyph? glyph)
        {
            if (!initialized)
            {
                // Register legacy encodings (including Shift-JIS)
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                UpdateButtonBindings();
                initialized = true;
            }
            if (UI_MainWindow.Self == null) return;
            if (glyph == null)
            {
                ClearGlyphDetails();
                return;
            }
            ShowGlyphImage(glyph);
            LoadedGlyph = glyph;
            panelGlyphProperties.DataContext = LoadedGlyph.Settings;
            BuildPropertiesPanel(panelGlyphProperties, glyph);
            ShowUnicodeDetails();
        }

        private void ShowUnicodeDetails()
        {
            if(LoadedGlyph == null || LoadedGlyph.Settings.Image == null)
            {
                textGlyphSymbol.Text = "";
                lblGlyphName.Text = "";
                return;
            }
            CharEncodings enc = UI_MainWindow.Self.FontendoFont.Settings.CharEncoding;

            UInt16 code = LoadedGlyph.Settings.CodePoint;
            string dchar = "";
            textGlyphSymbol.Text = "";
            switch (enc)
            {
                case CharEncodings.UTF8:
                case CharEncodings.UTF16:
                case CharEncodings.CP1252:
                    break;
                case CharEncodings.ShiftJIS:
                    try
                    {
                        char c = (char)code;
                        code = (ushort)c;
                    }
                    catch
                    {
                        dchar = "";
                    }
                    break;
                case CharEncodings.Num:
                    return;

            }
            UInt16[] chr = { code, 0x0000 };
            // Convert to bytes (2 bytes per UInt16)
            byte[] bytes = new byte[chr.Length * 2];
            Buffer.BlockCopy(chr, 0, bytes, 0, bytes.Length);
            string unicodechar = Encoding.Unicode.GetString(bytes);
            textGlyphSymbol.Text = unicodechar;
            lblGlyphName.Text = UI_MainWindow.Self.UnicodeNames.GetCharNameFromUnicodeCodepoint(code);
        }

        public static BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                // Save the bitmap into a stream
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                // Create a BitmapImage from the stream
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memory;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // make it cross-thread safe

                return bitmapImage;
            }
        }
        private void ShowGlyphImage(Glyph? glyph)
        {
            LoadedGlyph = glyph;
            if (glyph == null)
            {
                ClearGlyphDetails();
                return;
            }

            if (LoadedGlyph?.Settings.Image == null)
            {
                ClearGlyphDetails();
                return;
            }
            try
            {
                if (LoadedGlyph?.Settings.Image.Width <= 0 || LoadedGlyph?.Settings.Image.Height <= 0)
                {
                    ClearGlyphDetails();
                    return;
                }
            } catch
            {
                ClearGlyphDetails();
                return;
            }

            // Compute max zoom so image fits inside PictureBox
            float scaleX = (float)borderGlyphImageBorder.ActualHeight / LoadedGlyph.Settings.Image.Width;
            float scaleY = (float)borderGlyphImageBorder.ActualHeight / LoadedGlyph.Settings.Image.Height;
            maxZoomFactor = Math.Min(scaleX, scaleY);

            // Configure TrackBar range
            suppressZoomEvent = true;
            sliderZoom.Minimum = 100;
            sliderZoom.Maximum = (int)(maxZoomFactor * 100);
            suppressZoomEvent = false;

            // Only set slider to halfway if user has not changed it
            if (!userHasSetZoom)
            {
                int midValue = (int)(sliderZoom.Minimum + sliderZoom.Maximum) / 2;
                suppressZoomEvent = true;
                sliderZoom.Value = midValue;
                suppressZoomEvent = false;
                zoomFactor = midValue / 100f;
            }
            else
            {
                // Respect user’s chosen zoom
                zoomFactor = (float)sliderZoom.Value / 100f;
            }
            GlyphImage = ConvertBitmapToBitmapSource(glyph.Settings.Image);
            GlyphMaskImage = glyph.MaskImage == null ? null : ConvertBitmapToBitmapSource(glyph.MaskImage);
            borderGlyphImage.DataContext = this;

            // Apply zoom factor
            imgGlyphPreview.RenderTransform = zoomTransform;
            imgGlyphPreview.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            imgGlyphPreviewMask.RenderTransform = zoomTransform;
            imgGlyphPreviewMask.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            zoomTransform.ScaleX = zoomFactor;
            zoomTransform.ScaleY = zoomFactor;
            borderGlyphImage.Width = LoadedGlyph.Settings.Image.Width * zoomFactor;
            borderGlyphImage.Height = LoadedGlyph.Settings.Image.Height * zoomFactor;
            updateBackground();

        }
        public void ClearGlyphDetails()
        {
            panelGlyphProperties.Children.Clear();
            LoadedGlyph = null;
            textGlyphSymbol.Text = "";
            lblGlyphName.Text = "";
        }

        public void ExportGlyph()
        {
            if (LoadedGlyph == null) return;

            int digits = (int)Math.Ceiling(Math.Log((double)LoadedGlyph.Settings.CodePoint + 1, 16));
            string fileName = FileSystemHelper.BrowseForSaveFile(FileSystemHelper.FileType.Png, "Export Glyph Image", $"0x{((long)LoadedGlyph.Settings.CodePoint).ToString($"X{digits}")}.png");
            if (fileName == "") return;
            LoadedGlyph.Settings.Image?.Save(fileName);
            MessageBox.Show("Glyph image exported successfully.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnExportGlyph_Click(object sender, RoutedEventArgs e)
        {
            ExportGlyph();
        }

        public void ReplaceGlyph()
        {
            if (LoadedGlyph == null || LoadedGlyph.Settings.Image == null) return;

            UI_MainWindow.GlyphItem? item = UI_MainWindow.Self.GetSelectedCharacterItem();
            if (item == null) return;

            string fileName = FileSystemHelper.BrowseForFile(FileSystemHelper.FileType.Png, "Import Glyph Image");
            if (fileName == "") return;
            Bitmap bmp = new Bitmap(fileName);
            if (bmp.Width != LoadedGlyph.Settings.Image.Width || bmp.Height != LoadedGlyph.Settings.Image.Height)
            {
                MessageBox.Show($"The size of the selected image file ({bmp.Width}x{bmp.Height}) does not match the size of the loaded glyph image ({LoadedGlyph.Settings.Image.Width}x{LoadedGlyph.Settings.Image.Height})", "File Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                bmp.Dispose();
                return;
            }
            if (LoadedGlyph.Settings.Image != null)
                LoadedGlyph.Settings.Image.Dispose();

            int index = UI_MainWindow.Self.FontendoFont!.Settings.Glyphs!.IndexOf(LoadedGlyph);
            Glyph glyph = UI_MainWindow.Self.FontendoFont!.Settings.Glyphs[index];
            glyph.Settings.Image = (Bitmap)bmp.Clone();
            Bitmap? mask = FontBase.GenerateTransparencyMask(glyph.Settings.Image);
            glyph.MaskImage = mask == null ? null : (Bitmap)mask.Clone();
            LoadedGlyph = glyph;
            ShowGlyphDetails(LoadedGlyph);

            item.Image = UI_MainWindow.ConvertBitmap(bmp);
            item.MaskImage = mask == null ? null : UI_MainWindow.ConvertBitmap(mask);

            UI_MainWindow.Self.FontendoFont!.RecreateSheetFromGlyphs(LoadedGlyph.Sheet);

            var sheetitem = UI_MainWindow.Self.SheetsList[LoadedGlyph.Sheet];
            sheetitem.Image = UI_MainWindow.ConvertBitmap(UI_MainWindow.Self.FontendoFont!.Settings.Sheets!.Images[LoadedGlyph.Sheet]);
            sheetitem.Mask = UI_MainWindow.Self.FontendoFont!.Settings.Sheets!.MaskImages[LoadedGlyph.Sheet] == null ? null : UI_MainWindow.ConvertBitmap(UI_MainWindow.Self.FontendoFont!.Settings.Sheets!.MaskImages[LoadedGlyph.Sheet]);

            UI_MainWindow.Self.SheetsList[LoadedGlyph.Sheet] = sheetitem;
            if (mask != null)
            {
                mask.Dispose();
                mask = null;
            }
            bmp.Dispose();
            MessageBox.Show("Glyph image imported successfully.", "Import Successful", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void btnReplaceGlyph_Click(object sender, RoutedEventArgs e)
        {
            ReplaceGlyph();
        }

        public void BuildPropertiesPanel(StackPanel panel, Glyph glyph)
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
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(85) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Padding row at start
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10) });

            int rowIndex = 1;

            foreach (var kvp in glyph.Settings.PropertyDescriptors)
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

                glyph.Settings.GetBindingsForObject(kvp.Key, out var bindings);
                FrameworkElement? editor = CreateControlForEditorType(descriptor.PreferredControl, descriptor, bindings);

                if (editor != null)
                {
                    editor.Margin = new Thickness(0, 5, 0, 0);
                    Grid.SetRow(editor, rowIndex);
                    Grid.SetColumn(editor, 1);
                    grid.Children.Add(editor);

                    // add change events to specific controls
                    if (kvp.Key == FontProperties.GlyphProperties.GlyphProperty.Code)
                    {
                        ((NumericUpDown)editor).ValueChanged += UI_GlyphEditor_Code_ValueChanged;
                    }

                    // Fix row height to 32px
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32) });
                    rowIndex++;
                }
            }

            // Add final blank row that stretches
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            panel.Children.Add(grid);
        }

        private void UI_GlyphEditor_Code_ValueChanged(object? sender, EventArgs e)
        {
            ShowUnicodeDetails();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            try
            {
                PropertyChanged?.Invoke(this, e);
            }
            catch
            {
            }
        }

        private void sliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (suppressZoomEvent) return; // ignore programmatic changes
            userHasSetZoom = true; // only set when user actually moves the slider
            zoomFactor = (float)sliderZoom.Value / 100f;
            zoomTransform.ScaleX = zoomFactor;
            zoomTransform.ScaleY = zoomFactor;
            borderGlyphImage.Width = imgGlyphPreview.ActualWidth * zoomFactor;
            borderGlyphImage.Height = imgGlyphPreview.ActualHeight * zoomFactor;
        }

        public void UpdateButtonBindings()
        {
            btnExportGlyph.SetBinding(Button.IsEnabledProperty,
                new Binding(nameof(UI_MainWindow.Self.ButtonEnabler.IsGlyphSelected)) { Source = UI_MainWindow.Self.ButtonEnabler });

            btnReplaceGlyph.SetBinding(Button.IsEnabledProperty,
                new Binding(nameof(UI_MainWindow.Self.ButtonEnabler.IsGlyphSelected)) { Source = UI_MainWindow.Self.ButtonEnabler });

            sliderZoom.SetBinding(Slider.IsEnabledProperty,
                new Binding(nameof(UI_MainWindow.Self.ButtonEnabler.IsGlyphSelected)) { Source = UI_MainWindow.Self.ButtonEnabler });

        }
    }
}
