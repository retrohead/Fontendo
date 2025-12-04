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
            panelFontProperties = new Panel();
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
            tableMainTable.Controls.Add(panelFontProperties, 0, 1);
            tableMainTable.Dock = DockStyle.Top;
            tableMainTable.Location = new Point(0, 0);
            tableMainTable.Name = "tableMainTable";
            tableMainTable.RowCount = 3;
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableMainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableMainTable.Size = new Size(160, 86);
            tableMainTable.TabIndex = 0;
            // 
            // panelFontProperties
            // 
            panelFontProperties.AutoSize = true;
            panelFontProperties.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelFontProperties.BackColor = SystemColors.Control;
            tableMainTable.SetColumnSpan(panelFontProperties, 3);
            panelFontProperties.Dock = DockStyle.Top;
            panelFontProperties.Location = new Point(3, 13);
            panelFontProperties.MinimumSize = new Size(0, 50);
            panelFontProperties.Name = "panelFontProperties";
            panelFontProperties.Size = new Size(154, 50);
            panelFontProperties.TabIndex = 3;
            // 
            // FontEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            Controls.Add(tableMainTable);
            Name = "FontEditor";
            Size = new Size(160, 349);
            tableMainTable.ResumeLayout(false);
            tableMainTable.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel tableMainTable;
        private Panel panelFontProperties;
    }
}
