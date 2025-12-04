using Fontendo.Extensions;
using Fontendo.Interfaces;
using System.Text;
using System.Windows.Forms;
using static Fontendo.Extensions.FontBase;
using static Fontendo.FontProperties.PropertyList;

namespace Fontendo.Controls
{
    public partial class FontEditor : UserControl
    {
        private FontBase? LoadedFont;
        public Color SelectedColor
        {
            get => colorPickerBgColour.SelectedColor;
            set => colorPickerBgColour.SelectedColor = value;
        }

        public FontEditor()
        {
            InitializeComponent();
        }

        bool initialized = false;
        public void ShowFontDetails(FontBase? font)
        {
            if (!initialized)
            {
                // Register legacy encodings (including Shift-JIS)
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                btnExportSheet.DataBindings.Add(new Binding(nameof(btnExportSheet.Enabled), MainForm.Self.ButtonEnabler, nameof(MainForm.Self.ButtonEnabler.IsSheetSelected), true, DataSourceUpdateMode.Never));
                btnReplaceSheet.DataBindings.Add(new Binding(nameof(btnReplaceSheet.Enabled), MainForm.Self.ButtonEnabler, nameof(MainForm.Self.ButtonEnabler.IsSheetSelected), true, DataSourceUpdateMode.Never));

                initialized = true;
            }
            if (MainForm.Self == null) return;
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
            panelFontProperties.Controls.Clear();
        }

        public void BuildPropertiesPanel(Panel panel, FontBase font)
        {
            panel.Controls.Clear();
            //panel.AutoScroll = true;

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = font.Settings.PropertyDescriptors.Where(d => d.Value.PreferredControl != EditorType.None).Count() + 2, // +2 for blank row at end and padding at start
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = false
            };
            // padding at start
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

            // Column styles: first fixed 100px, second fills remaining
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            int rowIndex = 1;

            foreach (var kvp in font.Settings.PropertyDescriptors)
            {
                var descriptor = kvp.Value;

                if (descriptor.PreferredControl != EditorType.None)
                {
                    var label = new Label
                    {
                        Text = descriptor.Name,
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Margin = new Padding(3, 0, 0, 0)
                    };
                    label.Font = new Font(label.Font.FontFamily, 8);
                    table.Controls.Add(label, 0, rowIndex);
                }

                font.Settings.GetBindingForObject(kvp.Key, out var bindings);
                Control? editor = CreateControlForEditorType(descriptor.PreferredControl, descriptor, bindings);

                if (editor != null)
                {
                    editor.Padding = new Padding(0, 7, 0, 0);
                    table.Controls.Add(editor, 1, rowIndex);

                    // Fix row height to 32px
                    table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    rowIndex++;
                }
            }

            // Add final blank row that stretches
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            table.Dock = DockStyle.Top;
            table.AutoSize = true;

            panel.Controls.Add(table);
        }


        private void colorPickerBgColour_ColorChanged(object sender, EventArgs e)
        {
            MainForm.Self.SetBackgroundColour(colorPickerBgColour.SelectedColor, true);
        }


        private void colorPickerBgColour_PreviewColorChanged(object sender, Fontendo.Controls.ColorPreviewEventArgs e)
        {
            MainForm.Self.SetBackgroundColour(e.PreviewColor, false);
        }

        public void btnImportSheet_Click(object sender, EventArgs e)
        {
            int index = MainForm.Self.SelectedSheet;
            if (index < 0 || LoadedFont == null || LoadedFont.Settings.Sheets == null) return;

            string fileName = FileSystemHelper.BrowseForFile(FileSystemHelper.FileType.Png, "Import Sheet Image");
            if (fileName == "") return;
            Bitmap bmp = new Bitmap(fileName);
            if (bmp.Width != LoadedFont.Settings.Sheets.Width || bmp.Height != LoadedFont.Settings.Sheets.Height)
            {
                MessageBox.Show("The selected image file does not match the size of the loaded sheet", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (LoadedFont.Settings.Sheets.Images[index] != null)
                LoadedFont.Settings.Sheets.Images[index].Dispose();
            LoadedFont.Settings.Sheets.Images[index] = bmp;
            MainForm.Self.UpdateListViewImagesFromSheets();
            MessageBox.Show("Sheet image imported successfully.", "Import Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void btnExportSheet_Click(object sender, EventArgs e)
        {
            int index = MainForm.Self.SelectedSheet;
            if (index < 0 || LoadedFont == null || LoadedFont.Settings.Sheets == null)
                return;
            string fileName = FileSystemHelper.BrowseForSaveFile(FileSystemHelper.FileType.Png, "Export Sheet Image", $"Sheet {index + 1}.png");
            if (fileName == "") return;
            LoadedFont.Settings.Sheets.Images[index].Save(fileName);
            MessageBox.Show("Sheet image exported successfully.", "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
