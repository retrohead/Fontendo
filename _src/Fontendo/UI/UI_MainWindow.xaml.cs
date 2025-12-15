using Fontendo.Controls;
using Fontendo.Extensions;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static FileSystemHelper;
using static Fontendo.Extensions.FontBase;
using static Fontendo.Extensions.FontBase.FontSettings;
using Fontendo.DockManager;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Fontendo.Interfaces;

namespace Fontendo.UI
{
    /// <summary>
    /// Interaction logic for UI_MainWindow.xaml
    /// </summary>
    public partial class UI_MainWindow : CustomWindowContentBase, INotifyPropertyChanged
    {
        public static dynamic? loadedTab = null;
        public int RecentItem_Clicked { get; private set; }

        private FontBase? _FontendoFont;
        public FontBase? FontendoFont
        {
            get { return _FontendoFont; }
            set
            {
                if (value != _FontendoFont)
                {
                    _FontendoFont = value;
                    Self.OnPropertyChanged(nameof(FontendoFont));
                }
            }
        }
        public UnicodeNames? UnicodeNames;
        public UI_GlyphEditor GlyphEditor;
        public UI_FontEditor FontEditor = new UI_FontEditor();
        private bool debugMode = false;

        private ObservableCollection<GlyphItem> _GlyphsList = new ObservableCollection<GlyphItem>();
        public ObservableCollection<GlyphItem> GlyphsList
        {
            get { return _GlyphsList; }
            set
            {
                if (_GlyphsList != value)
                {
                    _GlyphsList = value;
                    OnPropertyChanged(nameof(GlyphsList));
                }
            }
        }
        private ObservableCollection<SheetItem> _SheetsList = new ObservableCollection<SheetItem>();
        public ObservableCollection<SheetItem> SheetsList
        {
            get { return _SheetsList; }
            set
            {
                if (_SheetsList != value)
                {
                    _SheetsList = value;
                    OnPropertyChanged(nameof(SheetsList));
                }
            }
        }

        public static UI_MainWindow? Self;
        public class MainFormButtonEnabler : INotifyPropertyChanged
        {
            private readonly FontBase _font;
            private readonly ListView _sheetsList;
            private readonly ListView _glyphList;

            public MainFormButtonEnabler(FontBase font, ListView sheetsList, ListView glyphList)
            {
                _font = font;
                _sheetsList = sheetsList;
                _glyphList = glyphList;

                // Subscribe to font property changes
                _font.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(FontBase.IsLoaded))
                        RaiseAll();
                };

                // Subscribe to selection changes
                _sheetsList.SelectionChanged += (s, e) => RaiseAll();
                _glyphList.SelectionChanged += (s, e) => RaiseAll();
            }

            public bool IsGlyphSelected =>
                _font.IsLoaded && _glyphList.SelectedItems.Count > 0;

            public bool IsSheetSelected =>
                _font.IsLoaded && _sheetsList.SelectedItems.Count > 0;

            private void RaiseAll()
            {
                OnPropertyChanged(nameof(IsGlyphSelected));
                OnPropertyChanged(nameof(IsSheetSelected));
            }

            public event PropertyChangedEventHandler? PropertyChanged;
            protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        }

        private int _ItemsLoadingCount = 0;

        public void BeginLoading() { Interlocked.Increment(ref _ItemsLoadingCount); OnPropertyChanged(nameof(OpenButtonEnabled)); }
        public void EndLoading() { Interlocked.Decrement(ref _ItemsLoadingCount); OnPropertyChanged(nameof(OpenButtonEnabled)); }

        public bool OpenButtonEnabled
        {
            get
            {
                return _ItemsLoadingCount == 0;
            }
        }


        private MainFormButtonEnabler? _buttonEnabler;
        public MainFormButtonEnabler? ButtonEnabler
        {
            get { return _buttonEnabler; }
            set { _buttonEnabler = value; OnPropertyChanged(nameof(ButtonEnabler)); }
        }

        public class SheetItem : INotifyPropertyChanged
        {
            public UI_FontEditor FontEditor { get; }

            private string _Label = "";
            public string Label
            {
                get
                {
                    return _Label;
                }
                set
                {
                    if (_Label != value)
                    {
                        _Label = value;
                        OnPropertyChanged(nameof(Label));
                    }
                }
            }
            private BitmapImage? _Image;
            public BitmapImage? Image
            {
                get
                {
                    return _Image;
                }
                set
                {
                    if (_Image != value)
                    {
                        _Image = value;
                        OnPropertyChanged(nameof(Image));
                        OnPropertyChanged(nameof(OriginalWidth));
                    }
                }
            }


            private BitmapImage? _Mask;
            public BitmapImage? Mask
            {
                get
                {
                    return _Mask;
                }
                set
                {
                    if (_Mask != value)
                    {
                        _Mask = value;
                        OnPropertyChanged(nameof(Mask));
                    }
                }
            }

            private object? _Tag;
            public object? Tag
            {
                get
                {
                    return _Tag;
                }
                set
                {
                    if (_Tag != value)
                    {
                        _Tag = value;
                        OnPropertyChanged(nameof(Tag));
                    }
                }
            }

            public double OriginalWidth => Image?.PixelWidth ?? 0;
            public double OriginalHeight => Image?.PixelHeight ?? 0;

            public SheetItem(UI_FontEditor editor)
            {
                FontEditor = editor;
            }
            public event PropertyChangedEventHandler? PropertyChanged;
            protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public class GlyphItem : INotifyPropertyChanged
        {
            public UI_FontEditor FontEditor { get; }

            private string _Label = "";
            public string Label
            {
                get
                {
                    return _Label;
                }
                set
                {
                    if (_Label != value)
                    {
                        _Label = value;
                        OnPropertyChanged(nameof(Label));
                    }
                }
            }
            private BitmapImage? _Image;
            public BitmapImage? Image
            {
                get
                {
                    return _Image;
                }
                set
                {
                    if (_Image != value)
                    {
                        _Image = value;
                        OnPropertyChanged(nameof(Image));
                        OnPropertyChanged(nameof(OriginalWidth));
                    }
                }
            }
            private BitmapImage? _MaskImage;
            public BitmapImage? MaskImage
            {
                get
                {
                    return _MaskImage;
                }
                set
                {
                    if (_MaskImage != value)
                    {
                        _MaskImage = value;
                        OnPropertyChanged(nameof(MaskImage));
                    }
                }
            }
            public GlyphItem(UI_FontEditor editor)
            {
                FontEditor = editor;
            }
            private object? _Tag;
            public object? Tag
            {
                get
                {
                    return _Tag;
                }
                set
                {
                    if (_Tag != value)
                    {
                        _Tag = value;
                        OnPropertyChanged(nameof(Tag));
                    }
                }
            }
            public double OriginalWidth => Image?.PixelWidth ?? 0;
            public double OriginalHeight => Image?.PixelHeight ?? 0;
            public event PropertyChangedEventHandler? PropertyChanged;
            protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public UI_MainWindow()
        {
            GlyphEditor = new UI_GlyphEditor(FontEditor);
            Self = this;
            InitializeComponent();
            try
            {
                // At application startup, choose which file types to support
                FileSystemHelper.Initialize(new List<FileType>
                {
                    FileType.BinaryCrustFont,
                    FileType.NitroFontResource
                });
                UnicodeNames = new UnicodeNames();
                // Get the version from the assembly
                string? fileVersion = Assembly
                        .GetEntryAssembly()?
                        .GetCustomAttribute<AssemblyFileVersionAttribute>()?
                        .Version;

                FontendoFont = new FontBase(Platform.CTR); // create a default font base for now
                ButtonEnabler = new MainFormButtonEnabler(FontendoFont, listViewSheets, listViewCharacters);
                FontEditor.SelectedColor = ColorHelper.ToMediaColor(ColorHelper.HexToColor(SettingsManager.Settings.FontBackgroundColor));
                UI_MainWindow.Self.SetBackgroundColour(ColorHelper.ToDrawingColor(FontEditor.SelectedColor), true);
                FontEditor.ShowFontDetails(null);
                GlyphEditor.ShowGlyphDetails(null);
            } catch
            {
                MessageBox.Show("Error during initialization of main window.");
            }
        }



        public int SelectedSheet
        {
            get
            {
                if (listViewSheets.SelectedItems.Count == 0)
                    return -1;
                return listViewSheets.Items.IndexOf(listViewSheets.SelectedItem);
            }
        }
        private double GetMinimumSize()
        {
            var grid = mainGrid; // your 3-column grid
            var leftCol = grid.ColumnDefinitions[0];
            var middleCol = grid.ColumnDefinitions[2];
            var rightCol = grid.ColumnDefinitions[4];

            // Calculate the minimum width required:
            return leftCol.MinWidth + middleCol.MinWidth + rightCol.MinWidth + 50;
        }
        public void resize(object sender, SizeChangedEventArgs e)
        {
            if (loadedTab != null)
                loadedTab.Content.resize(e.NewSize.Height, e.NewSize.Width);
            RecalculateColumnConstraints();
            double min = GetMinimumSize();
            if (MainWindow.Self.MinWidth != min)
            {
                // Stop shrinking further by snapping back
                MainWindow.Self.MinWidth = min;
            }
        }

        public static bool CanLoseChanges()
        {
            return true;
        }

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            string filename = FileSystemHelper.BrowseForSupportedFile("Select a font file");
            if (string.IsNullOrEmpty(filename)) return;
            LoadFont(filename);
        }

        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!FontendoFont.IsLoaded) return;

            ActionResult result = FontendoFont.SaveFont(FontendoFont.LoadedFontFilePath);
            if (!result.Success)
            {
                MessageBox.Show($"Font failed to save {result.Message}", "Font Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Font saved successfully.", "Font Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MenuItem_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (!FontendoFont.IsLoaded) return;

            string filepath = FileSystemHelper.BrowseForSaveFile(
                FontendoFont.LoadedFontFileType,
                "Save font file",
                Path.GetFileName(FontendoFont.LoadedFontFilePath)
                );
            if (string.IsNullOrEmpty(filepath)) return;
            ActionResult result = FontendoFont.SaveFont(filepath);
            if (!result.Success)
            {
                MessageBox.Show($"Font failed to save {result.Message}", "Font Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                textFontFilePath.Text = FontendoFont.LoadedFontFilePath;
                MessageBox.Show("Font saved successfully.", "Font Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MenuItem_Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void AddNoRecentItemsMenu(System.Windows.Style? style)
        {
            // add a blank item showing no recent files
            menuRecent.Items.Add(new System.Windows.Controls.MenuItem()
            {
                Header = "No Recent Files",
                IsEnabled = false,
                Icon = null,
                Style = style,
                FontSize = 12
            });
        }
        private void RecentItem_Click(object sender, RoutedEventArgs e)
        {
            if (!CanLoseChanges())
                return;
            System.Windows.Controls.MenuItem item = (System.Windows.Controls.MenuItem)sender;
            RecentFilesManager.RecentFile data = (RecentFilesManager.RecentFile)item.Tag;
            if (data == null)
                return;

            if (!File.Exists(data.FilePath))
            {
                if (MessageBox.Show("The selected file no longer exists.\n\n" +
                    data.FilePath + "\n\n" +
                    "Would you like to remove it from your recent files list?", "File no longer exists", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    RecentFilesManager.RemoveRecentFile(data.FilePath);
                }
                return;
            }

            if (!File.Exists(data.FilePath))
            {
                if (MessageBox.Show("The file no longer exists, do you want to remove it from your recent files list?", "Missing File", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    RecentFilesManager.RemoveRecentFile(data.FilePath);
                return;
            }
            LoadFont(data.FilePath);
        }
        public void OpenRecentFile(string fn)
        {
            LoadFont(fn);
        }

        private void upperMenu_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() != typeof(System.Windows.Controls.MenuItem))
                return;
            System.Windows.Controls.MenuItem itemclicked = (System.Windows.Controls.MenuItem)e.OriginalSource;
            if (nameof(upperFileMenu) == itemclicked.Name)
            {
                System.Windows.Style? style = (System.Windows.Style?)MainWindow.Self?.FindResource("MenuItemStyle");
                menuRecent.Items.Clear();
                if (RecentFilesManager.RecentFiles != null)
                {
                    foreach (RecentFilesManager.RecentFile file in RecentFilesManager.RecentFiles)
                    {
                        var tooltip = new ToolTip();
                        tooltip.Background = (SolidColorBrush?)MainWindow.Self?.FindResource("WindowBackgroundBrushMedium");
                        tooltip.Foreground = (SolidColorBrush?)MainWindow.Self?.FindResource("ControlTextInactive");
                        tooltip.Content = file.FilePath;
                        var item = new System.Windows.Controls.MenuItem()
                        {
                            Header = ShortenPath(file.FilePath),
                            Tag = file,
                            Style = style,
                            FontSize = 11,
                            ToolTip = tooltip
                        };
                        item.Click += RecentItem_Click;

                        menuRecent.Items.Add(item);

                    }
                }
                if (menuRecent.Items.Count == 0)
                    AddNoRecentItemsMenu(style);

                upperFileMenu.UpdateLayout();
                menuRecent.UpdateLayout();
            }
        }

        public static string ShortenPath(string path, int maxLength = 50)
        {
            if (string.IsNullOrEmpty(path) || path.Length <= maxLength)
                return path;

            // Split into directory segments
            string[] parts = path.Split(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

            // Always keep first and last segment
            string first = parts[0];
            string last = parts[^1];

            // Build middle segments until we exceed maxLength
            var middle = new List<string>();
            int totalLength = first.Length + last.Length + 5; // 5 for "...\"
            for (int i = 1; i < parts.Length - 1; i++)
            {
                int nextLen = parts[i].Length + 1; // +1 for separator
                if (totalLength + nextLen > maxLength)
                {
                    middle.Add("...");
                    break;
                }
                middle.Add(parts[i]);
                totalLength += nextLen;
            }

            return string.Join(System.IO.Path.DirectorySeparatorChar.ToString(),
                new[] { first }.Concat(middle).Concat(new[] { last }));
        }

        public void SetBackgroundColour(System.Drawing.Color color, bool save)
        {
            // Wrap the Color in a SolidColorBrush for WPF
            var mediaColor = ColorHelper.ToMediaColor(color);
            var backgroundBrush = new SolidColorBrush(mediaColor);

            listViewSheets.Background = backgroundBrush;
            listViewCharacters.Background = backgroundBrush;

            GlyphEditor.GlyphBackground = color;

            // Decide font colour based on luminance
            double luminance = Fontendo.Extensions.ColorHelper.GetLuminance(color);
            var foregroundBrush = luminance < 0.5
                ? System.Windows.Media.Brushes.White
                : System.Windows.Media.Brushes.Black;

            listViewSheets.Foreground = foregroundBrush;
            listViewCharacters.Foreground = foregroundBrush;

            // Save settings
            if (save)
            {
                SettingsManager.Settings.FontBackgroundColor = ColorHelper.ColorToHex(color);
                SettingsManager.Save();
            }
        }

        public void LoadFont(string filename)
        {
            if (FontendoFont == null)
                return;
            textFontFilePath.Text = "";
            FontEditor.ShowFontDetails(null);
            GlyphEditor.ShowGlyphDetails(null);

            FontendoFont.Dispose();
            FontendoFont = FontBase.CreateFontBase(filename);


            if(FontendoFont == null)
            {
                MessageBox.Show($"Font type not supported.", "Font Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            ActionResult result = FontendoFont.LoadFont(filename);

            if (!result.Success)
            {
                MessageBox.Show($"Font failed to load {result.Message}", "Font Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            else if (result.Message != "OK")
            {
                MessageBox.Show($"Font loaded with warnings:\n\n{result.Message}", "Font Warnings Occurred", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            ButtonEnabler = new MainFormButtonEnabler(FontendoFont, listViewSheets, listViewCharacters);
            FontEditor.UpdateButtonBindings();
            GlyphEditor.UpdateButtonBindings();
            textFontFilePath.Text = filename;
            RecentFilesManager.AddRecentFile(textFontFilePath.Text, Path.GetFileName(textFontFilePath.Text));
            ListFontSheets();
            FontEditor.ShowFontDetails(FontendoFont);
        }
        public static BitmapImage? ConvertBitmap(System.Drawing.Bitmap bmp)
        {
            if(bmp == null)
            {
                return null;
            }
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                var img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = ms;
                img.EndInit();
                img.Freeze();
                return img;
            }
        }
        private void ListFontSheets()
        {
            if (FontendoFont == null) return;
            if (FontendoFont.Settings.Sheets == null) return;

            SheetsType sheets = FontendoFont.Settings.Sheets;

            listViewSheets.DataContext = this;
            SheetsList = new ObservableCollection<SheetItem>();
            for (int i = 0; i < sheets.Images.Count; i++)
            {
                int index = i;
                BeginLoading();
                var dispatcher = Application.Current?.Dispatcher;
                // App is closing or dispatcher is gone → skip safely
                if (dispatcher == null || dispatcher.HasShutdownStarted || dispatcher.HasShutdownFinished)
                    return;

                dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        Bitmap bmp = sheets.Images[index];
                        Bitmap? bmp2 = sheets.MaskImages[index];
                        SheetsList.Add(new SheetItem(FontEditor)
                        {
                            Label = $"Sheet {index + 1}",
                            Image = ConvertBitmap(bmp),
                            Mask = bmp2 == null ? null : ConvertBitmap(bmp2),
                            Tag = sheets.Images[index]
                        });
                        if (listViewSheets.Items.Count > 0 && listViewSheets.SelectedIndex == -1)
                        {
                            listViewSheets.SelectedIndex = 0;
                            listViewSheets.ScrollIntoView(listViewSheets.Items[0]);
                        }
                    }
                    finally
                    {
                        EndLoading();
                    }
                }, DispatcherPriority.Background);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Self == null) throw new Exception("Main window not intialized properly");
            DockHandler.Register(MainWindow.Self.Window, dockablePanelGlyph, GlyphEditor, "Glyph Properties");
            DockHandler.Register(MainWindow.Self.Window, dockablePanelFont, FontEditor, "Font Properties");
        }
        private void sideSplittersDragDelta(object sender, DragDeltaEventArgs e)
        {
            RecalculateColumnConstraints();
        }

        private void RecalculateColumnConstraints()
        {
            var grid = mainGrid; // your 3-column grid

            var leftCol = grid.ColumnDefinitions[0];
            var middleCol = grid.ColumnDefinitions[2];
            var rightCol = grid.ColumnDefinitions[4];

            // Clamp both sides so middle never shrinks below MinWidth
            leftCol.MaxWidth = rightCol.ActualWidth + middleCol.MinWidth;
            rightCol.MaxWidth = leftCol.ActualWidth + middleCol.MinWidth;
        }

        private void MenuItem_Options_Click(object sender, RoutedEventArgs e)
        {
            dynamic pop = new popUpOptions(MainWindow.Self, this);
            popUps.loadPopUp(MainWindow.Self, "Theme Settings", "theme.png", ref pop, true);
        }

        private async void listViewSheets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listViewSheets.SelectedIndex < 0)
                return;

            int sheetIndex = listViewSheets.SelectedIndex;
            GlyphsList = new ObservableCollection<GlyphItem>();
            listViewCharacters.DataContext = this;
            await Task.Run(() =>
            {
                // Get all CharImages belonging to this sheet
                var glyphsForSheet = FontendoFont.Settings.Glyphs?
                    .Where(g => g.Sheet == sheetIndex);

                if (glyphsForSheet == null)
                    return;

                foreach (var glyph in glyphsForSheet)
                {
                    BeginLoading();
                    var dispatcher = Application.Current?.Dispatcher;
                    // App is closing or dispatcher is gone → skip safely
                    if (dispatcher == null || dispatcher.HasShutdownStarted || dispatcher.HasShutdownFinished)
                        return;

                    dispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            GlyphsList.Add(new GlyphItem(FontEditor)
                            {
                                Label = $"Char {glyph.Index}",
                                Image = ConvertBitmap(glyph.Settings.Image),
                                MaskImage = glyph.MaskImage == null ? null : ConvertBitmap(glyph.MaskImage),
                                Tag = glyph
                            });

                            if (listViewCharacters.Items.Count > 0 && listViewCharacters.SelectedIndex == -1)
                            {
                                listViewCharacters.SelectedIndex = 0;
                                listViewCharacters.ScrollIntoView(listViewCharacters.Items[0]);
                            }
                        }
                        finally
                        {
                            EndLoading();
                        }
                    }, DispatcherPriority.Background);
                }
            });
        }

        private void listViewCharacters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GlyphEditor.ClearGlyphDetails();
            if (listViewCharacters.SelectedItems.Count == 0) return;
            GlyphItem? glyph = (GlyphItem?)listViewCharacters.SelectedItems[0];
            if (glyph == null) return;
            Glyph? glyphdata = (Glyph?)glyph.Tag;
            if (glyphdata == null) return;
            GlyphEditor.ShowGlyphDetails(glyphdata);
        }

        public GlyphItem? GetSelectedCharacterItem()
        {
            return (GlyphItem?)listViewCharacters.SelectedItem;
        }
        public SheetItem? GetSelectedSheetItem()
        {
            return (SheetItem?)listViewSheets.SelectedItem;
        }

        private void ExportSheet_Click(object sender, RoutedEventArgs e)
        {
            FontEditor.ExportSheet();
        }

        private void ReplaceSheet_Click(object sender, RoutedEventArgs e)
        {
            FontEditor.ReplaceSheet();
        }
        private void ExportGlyph_Click(object sender, RoutedEventArgs e)
        {
            GlyphEditor.ExportGlyph();
        }

        private void ReplaceGlyph_Click(object sender, RoutedEventArgs e)
        {
            GlyphEditor.ReplaceGlyph();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
