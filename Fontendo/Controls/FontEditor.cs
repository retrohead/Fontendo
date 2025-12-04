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
        private float zoomFactor = 1.0f;
        private float maxZoomFactor = 1.0f;
        private Bitmap? currentGlyph;
        private bool userHasSetZoom = false;
        private bool suppressZoomEvent = false;

        public FontEditor()
        {
            InitializeComponent();
        }

        public void ShowFontDetails(IFontendoFont? font)
        {
            if (MainForm.Self == null) return;
            if (font == null)
            {
                ClearFontDetails();
                return;
            }
            BuildPropertiesPanel(panelFontProperties, font);
        }

        public void ClearFontDetails()
        {

        }

        public void BuildPropertiesPanel(Panel panel, IFontendoFont font)
        {
            panel.Controls.Clear();
            //panel.AutoScroll = true;

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = font.Properties.FontPropertyDescriptors.Where(d => d.Value.PreferredControl != EditorType.None).Count() + 2, // +2 for blank row at end and padding at start
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

            foreach (var kvp in font.Properties.FontPropertyDescriptors)
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

                Control? editor = null;
                switch (descriptor.PreferredControl)
                {
                    case EditorType.CodePointPicker:
                        UInt16 val = (UInt16)(font.Properties.FontProperties[kvp.Key] ?? descriptor.ValueRange.Value.Min);
                        editor = new HexNumericUpDown()
                        {
                            Dock = DockStyle.Fill,
                            Minimum = descriptor.ValueRange.Value.Min,
                            Maximum = descriptor.ValueRange.Value.Max,
                            Text = "0x" + val.ToString("X4")
                        };
                        break;

                    case EditorType.TextBox:
                        editor = new TextBox
                        {
                            Dock = DockStyle.Fill,
                            Text = font.Properties.FontProperties[kvp.Key]?.ToString() ?? string.Empty
                        };
                        break;

                    case EditorType.NumberBox:
                        editor = new NumericUpDown
                        {
                            Dock = DockStyle.Fill,
                            Minimum = descriptor.ValueRange.Value.Min,
                            Maximum = descriptor.ValueRange.Value.Max,
                            TextAlign = HorizontalAlignment.Right,
                            Value = Convert.ToDecimal(
                                font.Properties.FontProperties[kvp.Key] ?? descriptor.ValueRange.Value.Min
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
                            Value = Convert.ToInt32(font.Properties.FontProperties[kvp.Key] ?? 0)
                        };
                        break;

                    case EditorType.CheckBox:
                        editor = new CheckBox
                        {
                            Dock = DockStyle.Left,
                            Checked = Convert.ToBoolean(font.Properties.FontProperties[kvp.Key] ?? false)
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
                                font.Properties.SetValue(kvp.Key, dlg.Color.ToArgb());
                            }
                        };
                        break;
                }

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
    }
}
