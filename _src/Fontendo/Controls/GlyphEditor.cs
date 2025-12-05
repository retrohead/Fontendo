using Fontendo.Extensions;
using System.Text;
using System.Windows.Forms;
using static Fontendo.Extensions.FontBase;
using static Fontendo.FontProperties.PropertyList;

namespace Fontendo.Controls
{
    public partial class GlyphEditor : UserControl
    {
        private float zoomFactor = 1.0f;
        private float maxZoomFactor = 1.0f;
        private bool userHasSetZoom = false;
        private bool suppressZoomEvent = false;
        Glyph? LoadedGlyph;


        private Color glyphBackground = Color.White;
        public Color GlyphBackground
        {
            get => glyphBackground;
            set
            {
                glyphBackground = value;
                try
                {
                    if (LoadedGlyph?.Settings.Image == null)
                        pictureBoxGlyph.BackColor = GlyphBackground;
                    else
                        pictureBoxGlyph.Invalidate(); // force redraw with new background
                }
                catch
                {
                    // ignore errors during background update

                }
            }
        }
        bool initialized = false;
        public GlyphEditor()
        {
            InitializeComponent();
        }

        public void ShowGlyphDetails(Glyph? glyph)
        {
            if (!initialized)
            {
                // Register legacy encodings (including Shift-JIS)
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                btnExportGlyph.DataBindings.Add(new Binding(nameof(btnExportGlyph.Enabled), MainForm.Self.ButtonEnabler, nameof(MainForm.Self.ButtonEnabler.IsGlyphSelected), true, DataSourceUpdateMode.Never));
                btnReplaceGlyph.DataBindings.Add(new Binding(nameof(btnReplaceGlyph.Enabled), MainForm.Self.ButtonEnabler, nameof(MainForm.Self.ButtonEnabler.IsGlyphSelected), true, DataSourceUpdateMode.Never));
                trackBarZoom.DataBindings.Add(new Binding(nameof(trackBarZoom.Enabled), MainForm.Self.ButtonEnabler, nameof(MainForm.Self.ButtonEnabler.IsGlyphSelected), true, DataSourceUpdateMode.Never));

                initialized = true;
            }
            if (MainForm.Self == null) return;
            if (glyph == null)
            {
                ClearGlyphDetails();
                return;
            }
            ShowGlyphImage(glyph);
            BuildPropertiesPanel(panelGlyphProperties, glyph);

            CharEncodings enc = MainForm.Self.FontendoFont.Settings.CharEncoding;

            UInt16 code = glyph.Settings.CodePoint;
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
                        //code = MainForm.Self.SJIS.CodeToUTF16(code);
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
            lblGlyphName.Text = MainForm.Self.UnicodeNames.GetCharNameFromUnicodeCodepoint(code);
        }

        private void ShowGlyphImage(Glyph? glyph)
        {
            pictureBoxGlyph.Image = null; // prevent auto-draw
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

            // Compute max zoom so image fits inside PictureBox
            float scaleX = (float)pictureBoxGlyph.Width / LoadedGlyph.Settings.Image.Width;
            float scaleY = (float)pictureBoxGlyph.Height / LoadedGlyph.Settings.Image.Height;
            maxZoomFactor = Math.Min(scaleX, scaleY);

            // Configure TrackBar range
            trackBarZoom.Minimum = 100;
            trackBarZoom.Maximum = (int)(maxZoomFactor * 100);

            // Only set slider to halfway if user has not changed it
            if (!userHasSetZoom)
            {
                int midValue = (trackBarZoom.Minimum + trackBarZoom.Maximum) / 2;
                suppressZoomEvent = true;
                trackBarZoom.Value = midValue;
                suppressZoomEvent = false;
                zoomFactor = midValue / 100f;
            }
            else
            {
                // Respect user’s chosen zoom
                zoomFactor = trackBarZoom.Value / 100f;
            }

            pictureBoxGlyph.Invalidate();
        }

        public void ClearGlyphDetails()
        {
            panelGlyphProperties.Controls.Clear();
            LoadedGlyph = null;
            textGlyphSymbol.Text = "";
            lblGlyphName.Text = "";
        }

        public void BuildPropertiesPanel(Panel panel, Glyph glyph)
        {
            foreach (Control c in panel.Controls)
                c.DataBindings.Clear();
            panel.Controls.Clear();
            //panel.AutoScroll = true;

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = glyph.Settings.PropertyDescriptors.Where(d => d.Value.PreferredControl != EditorType.None).Count() + 2, // +2 for blank row at end and padding at start
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = false
            };
            // padding at start
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));

            // Column styles: first fixed 100px, second fills remaining
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            int rowIndex = 1;

            foreach (var kvp in glyph.Settings.PropertyDescriptors)
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
                Control? editor;
                glyph.Settings.GetBindingForObject(kvp.Key, out var binding);
                editor = CreateControlForEditorType(kvp.Value.PreferredControl, descriptor, new List<Binding> { binding });

                if (editor != null)
                {
                    editor.Padding = new Padding(0, 7, 0, 0);
                    table.Controls.Add(editor, 1, rowIndex);

                    // Fix row height to 32px
                    table.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
                    rowIndex++;
                }
            }

            // Add final blank row that stretches
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            table.Dock = DockStyle.Top;
            table.AutoSize = true;

            panel.Controls.Add(table);
        }

        private void trackBarZoom_ValueChanged(object? sender, EventArgs? e)
        {
            if (suppressZoomEvent) return; // ignore programmatic changes
            userHasSetZoom = true; // only set when user actually moves the slider
            zoomFactor = trackBarZoom.Value / 100f;
            pictureBoxGlyph.Invalidate();
        }

        private void pictureBox1_Paint(object? sender, PaintEventArgs? e)
        {
            if (LoadedGlyph?.Settings.Image == null)
            {
                using var brush = new SolidBrush(SystemColors.ControlLight);
                e.Graphics.FillRectangle(brush, pictureBoxGlyph.ClientRectangle);
                return;
            }
            float scaledWidth;
            try
            {
                scaledWidth = LoadedGlyph.Settings.Image.Width * zoomFactor;
            }
            catch
            {
                return;
            }


            float scaledHeight = LoadedGlyph.Settings.Image.Height * zoomFactor;

            float centerX = pictureBoxGlyph.ClientSize.Width / 2f;
            float centerY = pictureBoxGlyph.ClientSize.Height / 2f;

            float drawX = centerX - scaledWidth / 2f;
            float drawY = centerY - scaledHeight / 2f;

            // Fill full background
            using var bgBrush = new SolidBrush(SystemColors.ControlLight);
            e.Graphics.FillRectangle(bgBrush, pictureBoxGlyph.ClientRectangle);

            // Fill glyph background
            using var glyphBrush = new SolidBrush(glyphBackground);
            e.Graphics.FillRectangle(glyphBrush, drawX, drawY, scaledWidth, scaledHeight);

            // Draw glyph
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(LoadedGlyph.Settings.Image, drawX, drawY, scaledWidth, scaledHeight);
        }

        public void btnExportGlyph_Click(object sender, EventArgs e)
        {
            if (LoadedGlyph == null) return;

            int digits = (int)Math.Ceiling(Math.Log((double)LoadedGlyph.Settings.CodePoint + 1, 16));
            string fileName = FileSystemHelper.BrowseForSaveFile(FileSystemHelper.FileType.Png, "Export Glyph Image", $"0x{((long)LoadedGlyph.Settings.CodePoint).ToString($"X{digits}")}.png");
            if (fileName == "") return;
            LoadedGlyph.Settings.Image?.Save(fileName);
            MessageBox.Show("Glyph image exported successfully.", "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void btnReplaceGlyph_Click(object sender, EventArgs e)
        {
            if (LoadedGlyph == null || LoadedGlyph.Settings.Image == null) return;
            string fileName = FileSystemHelper.BrowseForFile(FileSystemHelper.FileType.Png, "Import Glyph Image");
            if (fileName == "") return;
            Bitmap bmp = new Bitmap(fileName);
            if (bmp.Width != LoadedGlyph.Settings.Image.Width || bmp.Height != LoadedGlyph.Settings.Image.Height)
            {
                MessageBox.Show("The selected image file does not match the size of the loaded glyph image", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (LoadedGlyph.Settings.Image != null)
                LoadedGlyph.Settings.Image.Dispose();
            LoadedGlyph.Settings.Image = bmp;
            ShowGlyphDetails(LoadedGlyph);
            MainForm.Self.UpdateListViewImagesFromGlyphs();
            MessageBox.Show("Glyph image imported successfully.", "Import Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
