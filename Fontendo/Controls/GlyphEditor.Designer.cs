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
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            pictureBox1 = new PictureBox();
            trackBarZoom = new TrackBar();
            panelGlyphPropertyContainer = new Panel();
            tableLayoutPanel3 = new TableLayoutPanel();
            panelGlyphPropertiesScrollablePanel = new Panel();
            panelGlyphProperties = new Panel();
            panelGlyphHeader = new Panel();
            tableLayoutPanel4 = new TableLayoutPanel();
            label1 = new Label();
            button1 = new Button();
            tableLayoutPanel5 = new TableLayoutPanel();
            lblGlyphSymbol = new Label();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarZoom).BeginInit();
            panelGlyphPropertyContainer.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            panelGlyphPropertiesScrollablePanel.SuspendLayout();
            panelGlyphHeader.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 0);
            tableLayoutPanel1.Controls.Add(panelGlyphPropertyContainer, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel5, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 182F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(412, 294);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel2.Controls.Add(pictureBox1, 0, 0);
            tableLayoutPanel2.Controls.Add(trackBarZoom, 0, 1);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(262, 0);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel2.Size = new Size(150, 182);
            tableLayoutPanel2.TabIndex = 1;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.FromArgb(122, 65, 196);
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Margin = new Padding(0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(150, 150);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Paint += pictureBox1_Paint;
            // 
            // trackBarZoom
            // 
            trackBarZoom.Dock = DockStyle.Fill;
            trackBarZoom.Location = new Point(3, 153);
            trackBarZoom.Maximum = 100;
            trackBarZoom.Name = "trackBarZoom";
            trackBarZoom.Size = new Size(144, 26);
            trackBarZoom.TabIndex = 1;
            trackBarZoom.TickStyle = TickStyle.TopLeft;
            trackBarZoom.ValueChanged += trackBarZoom_ValueChanged;
            // 
            // panelGlyphPropertyContainer
            // 
            tableLayoutPanel1.SetColumnSpan(panelGlyphPropertyContainer, 2);
            panelGlyphPropertyContainer.Controls.Add(tableLayoutPanel3);
            panelGlyphPropertyContainer.Dock = DockStyle.Fill;
            panelGlyphPropertyContainer.Location = new Point(0, 182);
            panelGlyphPropertyContainer.Margin = new Padding(0);
            panelGlyphPropertyContainer.Name = "panelGlyphPropertyContainer";
            panelGlyphPropertyContainer.Size = new Size(412, 112);
            panelGlyphPropertyContainer.TabIndex = 3;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Controls.Add(panelGlyphPropertiesScrollablePanel, 0, 1);
            tableLayoutPanel3.Controls.Add(panelGlyphHeader, 0, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(0, 0);
            tableLayoutPanel3.Margin = new Padding(0);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(412, 112);
            tableLayoutPanel3.TabIndex = 0;
            // 
            // panelGlyphPropertiesScrollablePanel
            // 
            panelGlyphPropertiesScrollablePanel.BackColor = SystemColors.Control;
            panelGlyphPropertiesScrollablePanel.Controls.Add(panelGlyphProperties);
            panelGlyphPropertiesScrollablePanel.Dock = DockStyle.Fill;
            panelGlyphPropertiesScrollablePanel.Location = new Point(0, 20);
            panelGlyphPropertiesScrollablePanel.Margin = new Padding(0);
            panelGlyphPropertiesScrollablePanel.Name = "panelGlyphPropertiesScrollablePanel";
            panelGlyphPropertiesScrollablePanel.Size = new Size(412, 92);
            panelGlyphPropertiesScrollablePanel.TabIndex = 5;
            panelGlyphPropertiesScrollablePanel.Resize += panelGlyphPropertiesScrollablePanel_Resize;
            // 
            // panelGlyphProperties
            // 
            panelGlyphProperties.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelGlyphProperties.BackColor = SystemColors.Control;
            panelGlyphProperties.Dock = DockStyle.Top;
            panelGlyphProperties.Location = new Point(0, 0);
            panelGlyphProperties.Name = "panelGlyphProperties";
            panelGlyphProperties.Size = new Size(412, 90);
            panelGlyphProperties.TabIndex = 3;
            // 
            // panelGlyphHeader
            // 
            panelGlyphHeader.BackColor = SystemColors.ControlDarkDark;
            panelGlyphHeader.BorderStyle = BorderStyle.FixedSingle;
            panelGlyphHeader.Controls.Add(tableLayoutPanel4);
            panelGlyphHeader.Dock = DockStyle.Fill;
            panelGlyphHeader.Location = new Point(0, 0);
            panelGlyphHeader.Margin = new Padding(0);
            panelGlyphHeader.Name = "panelGlyphHeader";
            panelGlyphHeader.Size = new Size(412, 20);
            panelGlyphHeader.TabIndex = 3;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.BackColor = SystemColors.ControlLight;
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel4.Controls.Add(label1, 0, 0);
            tableLayoutPanel4.Controls.Add(button1, 1, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(0, 0);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.Size = new Size(410, 18);
            tableLayoutPanel4.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ControlText;
            label1.Location = new Point(3, 2);
            label1.Margin = new Padding(3, 2, 3, 0);
            label1.Name = "label1";
            label1.Size = new Size(94, 15);
            label1.TabIndex = 0;
            label1.Text = "Glyph Properties";
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ControlLight;
            button1.Dock = DockStyle.Fill;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(390, 0);
            button1.Margin = new Padding(0);
            button1.Name = "button1";
            button1.Size = new Size(20, 18);
            button1.TabIndex = 1;
            button1.Text = "^";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Controls.Add(lblGlyphSymbol, 1, 0);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(3, 3);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 2;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Size = new Size(256, 176);
            tableLayoutPanel5.TabIndex = 4;
            // 
            // lblGlyphSymbol
            // 
            lblGlyphSymbol.AutoSize = true;
            lblGlyphSymbol.BorderStyle = BorderStyle.Fixed3D;
            lblGlyphSymbol.Dock = DockStyle.Fill;
            lblGlyphSymbol.Font = new Font("Segoe UI", 36F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblGlyphSymbol.Location = new Point(131, 7);
            lblGlyphSymbol.Margin = new Padding(3, 7, 3, 0);
            lblGlyphSymbol.Name = "lblGlyphSymbol";
            lblGlyphSymbol.Size = new Size(122, 81);
            lblGlyphSymbol.TabIndex = 0;
            lblGlyphSymbol.Text = "A";
            lblGlyphSymbol.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // GlyphEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "GlyphEditor";
            Size = new Size(412, 294);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarZoom).EndInit();
            panelGlyphPropertyContainer.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            panelGlyphPropertiesScrollablePanel.ResumeLayout(false);
            panelGlyphHeader.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private PictureBox pictureBox1;
        private TableLayoutPanel tableLayoutPanel2;
        private TrackBar trackBarZoom;
        private Panel panelGlyphPropertyContainer;
        private TableLayoutPanel tableLayoutPanel3;
        private Panel panelGlyphHeader;
        private TableLayoutPanel tableLayoutPanel4;
        private Label label1;
        private Button button1;
        private Panel panelGlyphPropertiesScrollablePanel;
        private Panel panelGlyphProperties;
        private TableLayoutPanel tableLayoutPanel5;
        private Label lblGlyphSymbol;
    }
}
