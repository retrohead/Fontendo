using Fontendo.Extensions;
using System.Windows.Forms;
using static Fontendo.FontProperties.PropertyList;

namespace Fontendo.Controls
{
    public partial class GlyphEditor : UserControl
    {
        private float zoomFactor = 1.0f;
        private float maxZoomFactor = 1.0f;
        private Bitmap? currentGlyph;
        private bool userHasSetZoom = false;
        private bool suppressZoomEvent = false;

        private Color glyphBackground = Color.White;
        public Color GlyphBackground
        {
            get => glyphBackground;
            set
            {
                glyphBackground = value;
                if (currentGlyph == null)
                    pictureBox1.BackColor = GlyphBackground;
                else
                    pictureBox1.Invalidate(); // force redraw with new background
            }
        }
        public GlyphEditor()
        {
            InitializeComponent();
            DockManager.Register(panelGlyphPropertyContainer);
        }

        public void ShowGlyphDetails(Glyph? glyph)
        {
            if (MainForm.Self == null) return;
            if (glyph == null)
            {
                ClearGlyphDetails();
                return;
            }
            ShowGlyphImage(glyph);
            BuildPropertiesPanel(panelGlyphProperties, glyph);
        }

        private void ShowGlyphImage(Glyph? glyph)
        {
            pictureBox1.Image = null; // prevent auto-draw

            if (glyph == null)
            {
                ClearGlyphDetails();
                return;
            }

            currentGlyph = glyph.Pixmap;
            if (currentGlyph == null)
            {
                ClearGlyphDetails();
                return;
            }

            // Compute max zoom so image fits inside PictureBox
            float scaleX = (float)pictureBox1.Width / currentGlyph.Width;
            float scaleY = (float)pictureBox1.Height / currentGlyph.Height;
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

            pictureBox1.Invalidate();
        }

        public void ClearGlyphDetails()
        {

        }

        public void BuildPropertiesPanel(Panel panel, Glyph glyph)
        {
            panel.Controls.Clear();
            panel.AutoScroll = true;

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = glyph.Properties.GlyphPropertyDescriptors.Where(d => d.Value.PreferredControl != EditorType.None).Count() + 1, // +1 for blank row
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = false
            };

            // Column styles: first fixed 100px, second fills remaining
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            int rowIndex = 0;

            foreach (var kvp in glyph.Properties.GlyphPropertyDescriptors)
            {
                var descriptor = kvp.Value;

                if (descriptor.PreferredControl != EditorType.None)
                {
                    var label = new Label
                    {
                        Text = descriptor.Name,
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Margin = Padding.Empty
                    };
                    table.Controls.Add(label, 0, rowIndex);
                }

                Control? editor = null;

                switch (descriptor.PreferredControl)
                {
                    case EditorType.CodePointPicker:
                        editor = new HexNumericUpDown
                        {
                            Dock = DockStyle.Fill,
                            Minimum = descriptor.ValueRange.Value.Min,
                            Maximum = descriptor.ValueRange.Value.Max,
                            Value = Convert.ToDecimal(
                                glyph.Properties.GlyphPropertyValues[kvp.Key] ?? descriptor.ValueRange.Value.Min
                            )
                        };
                        break;

                    case EditorType.TextBox:
                        editor = new TextBox
                        {
                            Dock = DockStyle.Fill,
                            Text = glyph.Properties.GlyphPropertyValues[kvp.Key]?.ToString() ?? string.Empty
                        };
                        break;

                    case EditorType.NumberBox:
                        editor = new NumericUpDown
                        {
                            Dock = DockStyle.Fill,
                            Minimum = descriptor.ValueRange.Value.Min,
                            Maximum = descriptor.ValueRange.Value.Max,
                            Value = Convert.ToDecimal(
                                glyph.Properties.GlyphPropertyValues[kvp.Key] ?? descriptor.ValueRange.Value.Min
                            )
                        };
                        break;

                    case EditorType.ComboBox:
                        editor = new ComboBox
                        {
                            Dock = DockStyle.Fill,
                            DropDownStyle = ComboBoxStyle.DropDownList
                        };
                        // TODO: populate items based on descriptor metadata
                        break;

                    case EditorType.Slider:
                        editor = new TrackBar
                        {
                            Dock = DockStyle.Fill,
                            Minimum = (int)(descriptor.ValueRange?.Min ?? 0),
                            Maximum = (int)(descriptor.ValueRange?.Max ?? 100),
                            Value = Convert.ToInt32(glyph.Properties.GlyphPropertyValues[kvp.Key] ?? 0)
                        };
                        break;

                    case EditorType.CheckBox:
                        editor = new CheckBox
                        {
                            Dock = DockStyle.Left,
                            Checked = Convert.ToBoolean(glyph.Properties.GlyphPropertyValues[kvp.Key] ?? false)
                        };
                        break;

                    case EditorType.ColorPicker:
                        editor = new Button
                        {
                            Dock = DockStyle.Left,
                            Text = "Pick Color"
                        };
                        editor.Click += (s, e) =>
                        {
                            using var dlg = new ColorDialog();
                            if (dlg.ShowDialog() == DialogResult.OK)
                            {
                                glyph.Properties.SetValue(kvp.Key, dlg.Color.ToArgb());
                            }
                        };
                        break;
                }

                if (editor != null)
                {
                    editor.Margin = Padding.Empty;
                    table.Controls.Add(editor, 1, rowIndex);

                    // Fix row height to 32px
                    table.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
                    rowIndex++;
                }
            }

            // Add final blank row that stretches
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            panel.Controls.Add(table);
        }




        private void trackBarZoom_ValueChanged(object? sender, EventArgs? e)
        {
            if (suppressZoomEvent) return; // ignore programmatic changes
            userHasSetZoom = true; // only set when user actually moves the slider
            zoomFactor = trackBarZoom.Value / 100f;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object? sender, PaintEventArgs? e)
        {
            if (currentGlyph == null)
            {
                using var brush = new SolidBrush(SystemColors.ControlLight);
                e.Graphics.FillRectangle(brush, pictureBox1.ClientRectangle);
                return;
            }
            float scaledWidth;
            try
            {
                scaledWidth = currentGlyph.Width * zoomFactor;
            }
            catch
            {
                return;
            }


            float scaledHeight = currentGlyph.Height * zoomFactor;

            float centerX = pictureBox1.ClientSize.Width / 2f;
            float centerY = pictureBox1.ClientSize.Height / 2f;

            float drawX = centerX - scaledWidth / 2f;
            float drawY = centerY - scaledHeight / 2f;

            // Fill full background
            using var bgBrush = new SolidBrush(SystemColors.ControlLight);
            e.Graphics.FillRectangle(bgBrush, pictureBox1.ClientRectangle);

            // Fill glyph background
            using var glyphBrush = new SolidBrush(glyphBackground);
            e.Graphics.FillRectangle(glyphBrush, drawX, drawY, scaledWidth, scaledHeight);

            // Draw glyph
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(currentGlyph, drawX, drawY, scaledWidth, scaledHeight);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DockManager.PopOut(panelGlyphPropertyContainer);
        }
    }
}
