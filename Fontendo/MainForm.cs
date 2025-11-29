using Fontendo.Extensions;
using System.IO;
using System.Windows.Forms;
using static FileSystem;
using static Fontendo.Extensions.FontBase;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Fontendo
{
    public partial class MainForm : Form
    {
        public static MainForm? Self;
        private FontBase FontendoFont;
        private double windowLeftPercent = 0.5f;

        public MainForm()
        {// At application startup, choose which types to support
            FileSystem.Initialize(new List<FileType>
            {
                FileType.BinaryCrustFont
            });
            RecentFiles.Initialize();
            InitializeComponent();
            Self = this;
            FontendoFont = new FontBase(Platform.CTR);
            splitContainer1_SplitterMoved(this, null);
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
            imageList1.Images.Clear();
            if (FontendoFont == null) return;
            Sheets sheets = FontendoFont.Font.GetSheets();
            imageList1.ImageSize = new Size(sheets.Width, sheets.Height);
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            foreach (Bitmap sheet in sheets.Items)
            {
                imageList1.Images.Add(sheet);
            }
            listView1.LargeImageList = imageList1;
            listView1.Items.Clear();
            for (int i = 0; i < imageList1.Images.Count; i++)
            {
                listView1.Items.Add(new ListViewItem($"Sheet {i + 1}", i) { ImageIndex = i });
            }
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
                if (MessageBox.Show("The file no longer exists, do you want to remove it from your recent files list?", "Missing File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    RecentFiles.Items.Remove(((RecentFiles.RecentFile)item.Tag));
                return;
            }

            ActionResult result = FontendoFont.LoadFont(((RecentFiles.RecentFile)item.Tag).FilePath);

            if (!result.Success)
            {
                MessageBox.Show($"Font failed to load {result.Message}");
                return;
            }
            textFontFilePath.Text = ((RecentFiles.RecentFile)item.Tag).FilePath;
            RecentFiles.Add(textFontFilePath.Text);
            ListFontSheets();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!FontendoFont.IsLoaded()) return;

            FontendoFont.Font.Save(FontendoFont.LoadedFontFilePath);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!FontendoFont.IsLoaded()) return;

            string filepath = FileSystem.BrowseForSaveFile(
                FontendoFont.LoadedFontFileType,
                "Save font file",
                Path.GetFileName(FontendoFont.LoadedFontFilePath)
                );
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs? e)
        {
            windowLeftPercent = splitContainer1.Panel1.Width / (splitContainer1.Panel1.Width + splitContainer1.Panel2.Width);
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            int totalWidth = splitContainer1.Panel1.Width + splitContainer1.Panel2.Width;
            int newLeftWidth = (int)(totalWidth * windowLeftPercent);
            // Apply the width
            splitContainer1.SplitterDistance = newLeftWidth;
        }
    }
}
