namespace Fontendo.Controls
{
    partial class GlyphEditor
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableMainTable = new TableLayoutPanel();
            textGlyphSymbol = new TextBox();
            groupBox2 = new GroupBox();
            tableGlyphImage = new TableLayoutPanel();
            pictureBoxGlyph = new PictureBox();
            trackBarZoom = new TrackBar();
            panelGlyphProperties = new Panel();
            btnExportGlyph = new Button();
            btnReplaceGlyph = new Button();
            lblGlyphName = new Label();
            tableMainTable.SuspendLayout();
            tableGlyphImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxGlyph).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarZoom).BeginInit();
            SuspendLayout();
            // 
            // tableMainTable
            // 
            tableMainTable.AutoSize = true;
            tableMainTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableMainTable.ColumnCount = 3;
            tableMainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableMainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            tableMainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableMainTable.Controls.Add(textGlyphSymbol, 1, 3);
            tableMainTable.Controls.Add(groupBox2, 0, 6);
            tableMainTable.Controls.Add(tableGlyphImage, 1, 1);
            tableMainTable.Controls.Add(panelGlyphProperties, 0, 5);
            tableMainTable.Controls.Add(btnExportGlyph, 1, 7);
            tableMainTable.Controls.Add(btnReplaceGlyph, 1, 8);
            tableMainTable.Controls.Add(lblGlyphName, 0, 4);
            tableMainTable.Dock = DockStyle.Top;
            tableMainTable.Location = new Point(0, 0);
            tableMainTable.Name = "tableMainTable";
            tableMainTable.RowCount = 9;
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 142F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            tableMainTable.RowStyles.Add(new RowStyle());
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableMainTable.Size = new Size(160, 372);
            tableMainTable.TabIndex = 0;
            // 
            // textGlyphSymbol
            // 
            textGlyphSymbol.BackColor = SystemColors.ControlLight;
            textGlyphSymbol.Font = new Font("Segoe UI", 20F);
            textGlyphSymbol.Location = new Point(55, 165);
            textGlyphSymbol.Margin = new Padding(35, 3, 3, 3);
            textGlyphSymbol.Multiline = true;
            textGlyphSymbol.Name = "textGlyphSymbol";
            textGlyphSymbol.ReadOnly = true;
            textGlyphSymbol.Size = new Size(50, 50);
            textGlyphSymbol.TabIndex = 9;
            textGlyphSymbol.TextAlign = HorizontalAlignment.Center;
            // 
            // groupBox2
            // 
            groupBox2.BackColor = SystemColors.ControlLight;
            tableMainTable.SetColumnSpan(groupBox2, 3);
            groupBox2.Dock = DockStyle.Top;
            groupBox2.Location = new Point(3, 301);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(0);
            groupBox2.Size = new Size(154, 2);
            groupBox2.TabIndex = 7;
            groupBox2.TabStop = false;
            // 
            // tableGlyphImage
            // 
            tableGlyphImage.ColumnCount = 1;
            tableGlyphImage.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableGlyphImage.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableGlyphImage.Controls.Add(pictureBoxGlyph, 0, 0);
            tableGlyphImage.Controls.Add(trackBarZoom, 0, 1);
            tableGlyphImage.Dock = DockStyle.Fill;
            tableGlyphImage.Location = new Point(20, 10);
            tableGlyphImage.Margin = new Padding(0);
            tableGlyphImage.Name = "tableGlyphImage";
            tableGlyphImage.RowCount = 2;
            tableGlyphImage.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableGlyphImage.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableGlyphImage.Size = new Size(120, 142);
            tableGlyphImage.TabIndex = 1;
            // 
            // pictureBoxGlyph
            // 
            pictureBoxGlyph.BackColor = SystemColors.ControlLight;
            pictureBoxGlyph.BorderStyle = BorderStyle.Fixed3D;
            pictureBoxGlyph.Location = new Point(0, 0);
            pictureBoxGlyph.Margin = new Padding(0);
            pictureBoxGlyph.Name = "pictureBoxGlyph";
            pictureBoxGlyph.Size = new Size(120, 110);
            pictureBoxGlyph.TabIndex = 0;
            pictureBoxGlyph.TabStop = false;
            pictureBoxGlyph.Paint += pictureBox1_Paint;
            // 
            // trackBarZoom
            // 
            trackBarZoom.Dock = DockStyle.Fill;
            trackBarZoom.Location = new Point(3, 113);
            trackBarZoom.Maximum = 100;
            trackBarZoom.Name = "trackBarZoom";
            trackBarZoom.Size = new Size(114, 26);
            trackBarZoom.TabIndex = 1;
            trackBarZoom.TickStyle = TickStyle.TopLeft;
            trackBarZoom.ValueChanged += trackBarZoom_ValueChanged;
            // 
            // panelGlyphProperties
            // 
            panelGlyphProperties.AutoSize = true;
            panelGlyphProperties.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelGlyphProperties.BackColor = SystemColors.Control;
            tableMainTable.SetColumnSpan(panelGlyphProperties, 3);
            panelGlyphProperties.Dock = DockStyle.Top;
            panelGlyphProperties.Location = new Point(3, 245);
            panelGlyphProperties.MinimumSize = new Size(0, 50);
            panelGlyphProperties.Name = "panelGlyphProperties";
            panelGlyphProperties.Size = new Size(154, 50);
            panelGlyphProperties.TabIndex = 3;
            // 
            // btnExportGlyph
            // 
            btnExportGlyph.Dock = DockStyle.Fill;
            btnExportGlyph.Location = new Point(23, 311);
            btnExportGlyph.Name = "btnExportGlyph";
            btnExportGlyph.Size = new Size(114, 26);
            btnExportGlyph.TabIndex = 4;
            btnExportGlyph.Text = "Export Glyph";
            btnExportGlyph.UseVisualStyleBackColor = true;
            btnExportGlyph.Click += btnExportGlyph_Click;
            // 
            // btnReplaceGlyph
            // 
            btnReplaceGlyph.Dock = DockStyle.Fill;
            btnReplaceGlyph.Location = new Point(23, 343);
            btnReplaceGlyph.Name = "btnReplaceGlyph";
            btnReplaceGlyph.Size = new Size(114, 26);
            btnReplaceGlyph.TabIndex = 5;
            btnReplaceGlyph.Text = "Replace Glyph";
            btnReplaceGlyph.UseVisualStyleBackColor = true;
            btnReplaceGlyph.Click += btnReplaceGlyph_Click;
            // 
            // lblGlyphName
            // 
            lblGlyphName.AutoSize = true;
            tableMainTable.SetColumnSpan(lblGlyphName, 3);
            lblGlyphName.Dock = DockStyle.Fill;
            lblGlyphName.Location = new Point(3, 222);
            lblGlyphName.MinimumSize = new Size(114, 20);
            lblGlyphName.Name = "lblGlyphName";
            lblGlyphName.Size = new Size(154, 20);
            lblGlyphName.TabIndex = 0;
            lblGlyphName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // GlyphEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            Controls.Add(tableMainTable);
            Name = "GlyphEditor";
            Size = new Size(160, 406);
            tableMainTable.ResumeLayout(false);
            tableMainTable.PerformLayout();
            tableGlyphImage.ResumeLayout(false);
            tableGlyphImage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxGlyph).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarZoom).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel tableMainTable;
        private PictureBox pictureBoxGlyph;
        private TableLayoutPanel tableGlyphImage;
        private TrackBar trackBarZoom;
        private Panel panelGlyphProperties;
        private Button btnExportGlyph;
        private Button btnReplaceGlyph;
        private GroupBox groupBox2;
        private Label lblGlyphName;
        private TextBox textGlyphSymbol;
    }
}
