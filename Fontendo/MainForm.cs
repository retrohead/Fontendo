using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Fontendo
{
    public partial class MainForm : Form
    {
        BCFNT BCFNT = new BCFNT();
        public static MainForm? Self;

        public MainForm()
        {
            InitializeComponent();
            Self = this;
        }

        /// <summary>
        /// Opens a file browser dialog with a custom filter and title.
        /// </summary>
        /// <param name="filter">File filter string (e.g. "CIA files (*.cia)|*.cia").</param>
        /// <param name="title">Dialog title (e.g. "Select a CIA file").</param>
        /// <returns>Full path of the selected file, or empty string if cancelled.</returns>
        public static string BrowseForFile(string filter = "All files (*.*)|*.*", string title = "Select a file")
        {
            string filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = filter;
                openFileDialog.Title = title;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog(MainForm.Self) == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                }
            }

            return filePath;
        }

        private void LoadFont(string path)
        {
            ActionResult result = BCFNT.Load(textBox1.Text);
            if (!result.Success)
            {
                MessageBox.Show($"Failed {result.Message}");
                return;
            }
            imageList1.Images.Clear();
            imageList1.ImageSize = new Size(BCFNT.TGLP.sheetWidth, BCFNT.TGLP.sheetHeight);
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            foreach (Bitmap sheet in BCFNT.Sheets)
            {
                imageList1.Images.Add(sheet);
            }
            // Example: bind to a ListView
            listView1.LargeImageList = imageList1;
            for (int i = 0; i < imageList1.Images.Count; i++)
            {
                listView1.Items.Add(new ListViewItem($"Sheet {i}", i) { ImageIndex = i });
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            LoadFont(textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filename = BrowseForFile("BCFNT (*.bcfnt)|*.bcfnt", "Select a font file");
            if(string.IsNullOrEmpty(filename)) return;
            textBox1.Text = filename;
            LoadFont(textBox1.Text);
        }
    }
}
