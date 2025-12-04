namespace Fontendo.Controls
{
    partial class FontEditor
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
            btnExportSheet = new Button();
            colorPickerBgColour = new ColorPickerButton();
            groupBox1 = new GroupBox();
            panelFontProperties = new Panel();
            btnReplaceSheet = new Button();
            tableMainTable.SuspendLayout();
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
            tableMainTable.Controls.Add(btnExportSheet, 1, 3);
            tableMainTable.Controls.Add(colorPickerBgColour, 1, 2);
            tableMainTable.Controls.Add(groupBox1, 0, 1);
            tableMainTable.Controls.Add(panelFontProperties, 0, 0);
            tableMainTable.Controls.Add(btnReplaceSheet, 1, 4);
            tableMainTable.Dock = DockStyle.Top;
            tableMainTable.Location = new Point(0, 0);
            tableMainTable.Name = "tableMainTable";
            tableMainTable.RowCount = 7;
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableMainTable.Size = new Size(180, 226);
            tableMainTable.TabIndex = 0;
            // 
            // btnExportSheet
            // 
            btnExportSheet.Dock = DockStyle.Fill;
            btnExportSheet.Location = new Point(33, 101);
            btnExportSheet.Name = "btnExportSheet";
            btnExportSheet.Size = new Size(114, 26);
            btnExportSheet.TabIndex = 10;
            btnExportSheet.Text = "Export Sheet";
            btnExportSheet.UseVisualStyleBackColor = true;
            btnExportSheet.Click += btnExportSheet_Click;
            // 
            // colorPickerBgColour
            // 
            colorPickerBgColour.CircleMargin = 6;
            colorPickerBgColour.CirclePosition = CirclePosition.Left;
            colorPickerBgColour.CircleSize = 8;
            colorPickerBgColour.Dock = DockStyle.Fill;
            colorPickerBgColour.Location = new Point(33, 69);
            colorPickerBgColour.Name = "colorPickerBgColour";
            colorPickerBgColour.SelectedColor = Color.FromArgb(122, 65, 196);
            colorPickerBgColour.Size = new Size(114, 26);
            colorPickerBgColour.TabIndex = 8;
            colorPickerBgColour.Text = "Background Colour";
            colorPickerBgColour.UseVisualStyleBackColor = true;
            colorPickerBgColour.ColorChanged += colorPickerBgColour_ColorChanged;
            colorPickerBgColour.PreviewColorChanged += colorPickerBgColour_PreviewColorChanged;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.ControlLight;
            tableMainTable.SetColumnSpan(groupBox1, 3);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.Location = new Point(3, 59);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(0);
            groupBox1.Size = new Size(174, 2);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            // 
            // panelFontProperties
            // 
            panelFontProperties.AutoSize = true;
            panelFontProperties.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelFontProperties.BackColor = SystemColors.Control;
            tableMainTable.SetColumnSpan(panelFontProperties, 3);
            panelFontProperties.Dock = DockStyle.Top;
            panelFontProperties.Location = new Point(3, 3);
            panelFontProperties.MinimumSize = new Size(0, 50);
            panelFontProperties.Name = "panelFontProperties";
            panelFontProperties.Size = new Size(174, 50);
            panelFontProperties.TabIndex = 3;
            // 
            // btnReplaceSheet
            // 
            btnReplaceSheet.Dock = DockStyle.Fill;
            btnReplaceSheet.Location = new Point(33, 133);
            btnReplaceSheet.Name = "btnReplaceSheet";
            btnReplaceSheet.Size = new Size(114, 26);
            btnReplaceSheet.TabIndex = 9;
            btnReplaceSheet.Text = "Replace Sheet";
            btnReplaceSheet.UseVisualStyleBackColor = true;
            btnReplaceSheet.Click += btnImportSheet_Click;
            // 
            // FontEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            Controls.Add(tableMainTable);
            MinimumSize = new Size(180, 350);
            Name = "FontEditor";
            Size = new Size(180, 350);
            tableMainTable.ResumeLayout(false);
            tableMainTable.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel tableMainTable;
        private Panel panelFontProperties;
        private GroupBox groupBox1;
        private ColorPickerButton colorPickerBgColour;
        private Button btnExportSheet;
        private Button btnReplaceSheet;
    }
}
