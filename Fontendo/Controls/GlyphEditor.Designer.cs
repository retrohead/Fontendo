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
            tableGlyphImage = new TableLayoutPanel();
            pictureBoxGlyph = new PictureBox();
            trackBarZoom = new TrackBar();
            lblGlyphSymbol = new Label();
            panelGlyphProperties = new Panel();
            btnExport = new Button();
            btnImport = new Button();
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
            tableMainTable.Controls.Add(tableGlyphImage, 1, 1);
            tableMainTable.Controls.Add(lblGlyphSymbol, 1, 2);
            tableMainTable.Controls.Add(panelGlyphProperties, 0, 3);
            tableMainTable.Controls.Add(btnExport, 1, 4);
            tableMainTable.Controls.Add(btnImport, 1, 5);
            tableMainTable.Dock = DockStyle.Top;
            tableMainTable.Location = new Point(0, 0);
            tableMainTable.Name = "tableMainTable";
            tableMainTable.RowCount = 6;
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 142F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableMainTable.Size = new Size(160, 332);
            tableMainTable.TabIndex = 0;
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
            // lblGlyphSymbol
            // 
            lblGlyphSymbol.BackColor = SystemColors.ControlLight;
            lblGlyphSymbol.BorderStyle = BorderStyle.Fixed3D;
            lblGlyphSymbol.Dock = DockStyle.Right;
            lblGlyphSymbol.Font = new Font("Segoe UI", 25F);
            lblGlyphSymbol.Location = new Point(55, 157);
            lblGlyphSymbol.Margin = new Padding(0, 5, 35, 5);
            lblGlyphSymbol.Name = "lblGlyphSymbol";
            lblGlyphSymbol.Padding = new Padding(5, 0, 0, 10);
            lblGlyphSymbol.Size = new Size(50, 50);
            lblGlyphSymbol.TabIndex = 0;
            lblGlyphSymbol.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelGlyphProperties
            // 
            panelGlyphProperties.AutoSize = true;
            panelGlyphProperties.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelGlyphProperties.BackColor = SystemColors.Control;
            tableMainTable.SetColumnSpan(panelGlyphProperties, 3);
            panelGlyphProperties.Dock = DockStyle.Top;
            panelGlyphProperties.Location = new Point(3, 215);
            panelGlyphProperties.MinimumSize = new Size(0, 50);
            panelGlyphProperties.Name = "panelGlyphProperties";
            panelGlyphProperties.Size = new Size(154, 50);
            panelGlyphProperties.TabIndex = 3;
            // 
            // btnExport
            // 
            btnExport.Dock = DockStyle.Fill;
            btnExport.Location = new Point(23, 271);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(114, 26);
            btnExport.TabIndex = 4;
            btnExport.Text = "Export";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += btnExport_Click;
            // 
            // btnImport
            // 
            btnImport.Dock = DockStyle.Fill;
            btnImport.Location = new Point(23, 303);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(114, 26);
            btnImport.TabIndex = 5;
            btnImport.Text = "Import";
            btnImport.UseVisualStyleBackColor = true;
            btnImport.Click += btnImport_Click;
            // 
            // GlyphEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            Controls.Add(tableMainTable);
            Name = "GlyphEditor";
            Size = new Size(160, 364);
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
        private Label lblGlyphSymbol;
        private Button btnExport;
        private Button btnImport;
    }
}
