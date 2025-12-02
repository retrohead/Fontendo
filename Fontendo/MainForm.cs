using Fontendo.Extensions;
using Microsoft.VisualBasic.Logging;
using static FileSystem;
using static Fontendo.Extensions.FontBase;

namespace Fontendo
{
    public partial class MainForm : Form
    {
        public static MainForm? Self;
        public FontBase FontendoFont;
        private double windowLeftPercent = 0.5f;
        public SJISConv SJIS;
        private bool debugMode = false;

        public MainForm()
        {// At application startup, choose which types to support
            FileSystem.Initialize(new List<FileType>
            {
                FileType.BinaryCrustFont
            });
            RecentFiles.Initialize();
            SJIS = new SJISConv();

            InitializeComponent();
            Self = this;
            FontendoFont = new FontBase(Platform.CTR);
            splitContainer1_SplitterMoved(this, null);
            colorPickerBgColour.SelectedColor = Properties.Settings.Default.FontBackgroundColor;
        }

        private void btnBrowseFont_Click(object sender, EventArgs e)
        {
            string filename = FileSystem.BrowseForFile("Select a font file");
            if (string.IsNullOrEmpty(filename)) return;
            textFontFilePath.Text = filename;
            ActionResult result = FontendoFont.LoadFont(textFontFilePath.Text);

            if (!result.Success)
            {
                MessageBox.Show($"Font failed to load {result.Message}");
                return;
            }
            RecentFiles.Add(textFontFilePath.Text);
            ListFontSheets();
        }

        private void ListFontSheets()
        {
            imageListSheets.Images.Clear();
            if (FontendoFont == null) return;
            if (FontendoFont.Font.Sheets == null) return;

            Sheets sheets = FontendoFont.Font.Sheets;
            imageListSheets.ImageSize = new Size(sheets.Width, sheets.Height);
            imageListSheets.ColorDepth = ColorDepth.Depth32Bit;

            // Add all sheets to the ImageList
            foreach (Bitmap sheet in sheets.Items)
            {
                imageListSheets.Images.Add(sheet);
            }

            listViewSheets.LargeImageList = imageListSheets;
            listViewSheets.Items.Clear();

            // Create items with Tag = sheet
            for (int i = 0; i < sheets.Items.Count; i++)
            {
                var item = new ListViewItem($"Sheet {i + 1}", i)
                {
                    ImageIndex = i,
                    Tag = sheets.Items[i] // store the Bitmap in Tag
                };
                listViewSheets.Items.Add(item);
            }
            if (listViewSheets.Items.Count > 0)
                listViewSheets.Items[0].Selected = true;

            splitContainer1.Panel1MinSize = sheets.Width + 50;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnBrowseFont_Click(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {

            ToolStripMenuItem fileItem = (ToolStripMenuItem)sender;
            if (RecentFiles.Items.Count > 0)
            {
                // add the recent items
                recentFilesToolStripMenuItem.DropDownItems.Clear();
                foreach (var item in RecentFiles.Items)
                {
                    ToolStripMenuItem newItem = new ToolStripMenuItem() { Text = FileSystem.ShortenPath(item.FilePath), Tag = item, ToolTipText = item.FilePath };
                    newItem.Click += RecentItem_Click;
                    recentFilesToolStripMenuItem.DropDownItems.Add(newItem);
                }
            }
            else
            {
                // make sure the "No Recent Files" item is in place
                recentFilesToolStripMenuItem.DropDownItems.Clear();
                recentFilesToolStripMenuItem.DropDownItems.Add(noRecentItemsToolStripMenuItem);
            }
        }

        private void RecentItem_Click(object? sender, EventArgs e)
        {
            if (sender == null)
                return;
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            if (item.Tag == null)
                return;
            if (!File.Exists(((RecentFiles.RecentFile)item.Tag).FilePath))
            {
                if (MessageBox.Show(this, "The file no longer exists, do you want to remove it from your recent files list?", "Missing File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    RecentFiles.Items.Remove(((RecentFiles.RecentFile)item.Tag));
                return;
            }

            ActionResult result = FontendoFont.LoadFont(((RecentFiles.RecentFile)item.Tag).FilePath);

            if (!result.Success)
            {
                MessageBox.Show(this, $"Font failed to load {result.Message}", "Font Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            textFontFilePath.Text = ((RecentFiles.RecentFile)item.Tag).FilePath;
            RecentFiles.Add(textFontFilePath.Text);
            ListFontSheets();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!FontendoFont.IsLoaded()) return;

            ActionResult result = FontendoFont.Font.Save(FontendoFont.LoadedFontFilePath);
            if (!result.Success)
            {
                MessageBox.Show(this, $"Font failed to save {result.Message}", "Font Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(this, "Font saved successfully.", "Font Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!FontendoFont.IsLoaded()) return;

            string filepath = FileSystem.BrowseForSaveFile(
                FontendoFont.LoadedFontFileType,
                "Save font file",
                Path.GetFileName(FontendoFont.LoadedFontFilePath)
                );
            if (string.IsNullOrEmpty(filepath)) return;
            ActionResult result = FontendoFont.Font.Save(filepath);
            if (!result.Success)
            {
                MessageBox.Show(this, $"Font failed to save {result.Message}", "Font Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(this, "Font saved successfully.", "Font Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs? e)
        {
            windowLeftPercent = (double)splitContainer1.Panel1.Width / (double)(splitContainer1.Panel1.Width + splitContainer1.Panel2.Width);
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            int totalWidth = splitContainer1.Panel1.Width + splitContainer1.Panel2.Width;
            int newLeftWidth = (int)(totalWidth * windowLeftPercent);
            // Apply the width
            splitContainer1.SplitterDistance = newLeftWidth;
        }

        private void colorPickerBgColour_ColorChanged(object sender, EventArgs e)
        {
            SetBackgroundColour(colorPickerBgColour.SelectedColor, true);
        }

        private void colorPickerBgColour_PreviewColorChanged(object sender, Fontendo.Controls.ColorPreviewEventArgs e)
        {
            SetBackgroundColour(e.PreviewColor, false);
        }

        private void SetBackgroundColour(Color color, bool save)
        {
            listViewSheets.BackColor = color;
            listViewCharacters.BackColor = color;
            glyphEditor1.GlyphBackground = color;
            // Decide font colour based on lumiance
            double luminance = Fontendo.Extensions.ColorHelper.GetLuminance(color);
            listViewSheets.ForeColor = luminance < 0.5 ? Color.White : Color.Black;
            listViewCharacters.ForeColor = listViewSheets.ForeColor;

            // save settings
            if (save)
            {
                Properties.Settings.Default.FontBackgroundColor = color;
                Properties.Settings.Default.Save();
            }
        }

        private void listViewSheets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewSheets.SelectedItems.Count == 0)
                return;

            // Get the sheet index from the selected item
            int sheetIndex = listViewSheets.SelectedItems[0].Index;

            // Clear old items
            listViewCharacters.BeginUpdate();
            listViewCharacters.Items.Clear();
            imageListCharacters.Images.Clear();

            // Get all CharImages belonging to this sheet
            var charsForSheet = FontendoFont.Font.CharImages?.Where(c => c.Sheet.Equals(sheetIndex));
            if (charsForSheet != null)
            {
                foreach (var charImg in charsForSheet)
                {
                    // Add the bitmap to the ImageList
                    imageListCharacters.Images.Add(charImg.Image);

                    // Add a ListViewItem with metadata
                    var item = new ListViewItem
                    {
                        ImageIndex = imageListCharacters.Images.Count - 1,
                        Text = $"Char {charImg.Index}",
                        Tag = charImg // keep reference to the CharImage object
                    };

                    listViewCharacters.Items.Add(item);
                }
            }
            if(listViewCharacters.Items.Count > 0)
                listViewCharacters.Items[0].Selected = true;
            listViewCharacters.EndUpdate();
        }

        private void listViewCharacters_SelectedIndexChanged(object sender, EventArgs e)
        {
            glyphEditor1.ClearGlyphDetails();
            if (listViewCharacters.SelectedItems.Count == 0) return;
            CharImage? img = (CharImage?)listViewCharacters.SelectedItems[0].Tag;
            if (img == null) return;
            int? index = FontendoFont.Font.CharImages?.IndexOf(img);
            if (index == null || index < 0) return;
            var glyph = FontendoFont.Font.Glyphs?.FirstOrDefault(g => g.Index.Equals(index));
            if (index < 0 || glyph == null)
                return;
            glyphEditor1.ShowGlyphDetails(glyph);
        }

        static bool deletelog = true;
        public static void Log(string message)
        {
            if(!Self?.debugMode ?? false)
                return;
            // get application directory and create/open log file
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (deletelog)
            {
                deletelog = false;
                if (File.Exists(Path.Combine(path, "fontendo.log")))
                {
                    File.Delete(Path.Combine(path, "fontendo.log"));
                }
            }
            using (var writer = new StreamWriter(Path.Combine(path, "fontendo.log"), true))
            {
                writer.WriteLine($"[{DateTime.Now}] {message}");
            }
        }
    }
}
