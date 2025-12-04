namespace Fontendo.Controls
{
    partial class DockablePanel
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
            panelScrollablePanel = new Panel();
            panelGlyphPropertyContainer = new Panel();
            tableWidgetLayout = new TableLayoutPanel();
            panelHeader = new Panel();
            tableHeader = new TableLayoutPanel();
            labelHeader = new Label();
            btnPopOut = new Button();
            panelGlyphPropertyContainer.SuspendLayout();
            tableWidgetLayout.SuspendLayout();
            panelHeader.SuspendLayout();
            tableHeader.SuspendLayout();
            SuspendLayout();
            // 
            // panelScrollablePanel
            // 
            panelScrollablePanel.AutoScroll = true;
            panelScrollablePanel.BackColor = SystemColors.Control;
            panelScrollablePanel.Dock = DockStyle.Fill;
            panelScrollablePanel.Location = new Point(0, 20);
            panelScrollablePanel.Margin = new Padding(0);
            panelScrollablePanel.Name = "panelScrollablePanel";
            panelScrollablePanel.Size = new Size(233, 326);
            panelScrollablePanel.TabIndex = 5;
            // 
            // panelGlyphPropertyContainer
            // 
            panelGlyphPropertyContainer.Controls.Add(tableWidgetLayout);
            panelGlyphPropertyContainer.Dock = DockStyle.Fill;
            panelGlyphPropertyContainer.Location = new Point(0, 0);
            panelGlyphPropertyContainer.Margin = new Padding(0);
            panelGlyphPropertyContainer.Name = "panelGlyphPropertyContainer";
            panelGlyphPropertyContainer.Size = new Size(233, 346);
            panelGlyphPropertyContainer.TabIndex = 4;
            // 
            // tableWidgetLayout
            // 
            tableWidgetLayout.ColumnCount = 1;
            tableWidgetLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableWidgetLayout.Controls.Add(panelScrollablePanel, 0, 1);
            tableWidgetLayout.Controls.Add(panelHeader, 0, 0);
            tableWidgetLayout.Dock = DockStyle.Fill;
            tableWidgetLayout.Location = new Point(0, 0);
            tableWidgetLayout.Margin = new Padding(0);
            tableWidgetLayout.Name = "tableWidgetLayout";
            tableWidgetLayout.RowCount = 2;
            tableWidgetLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableWidgetLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableWidgetLayout.Size = new Size(233, 346);
            tableWidgetLayout.TabIndex = 0;
            // 
            // panelHeader
            // 
            panelHeader.BackColor = SystemColors.ControlDarkDark;
            panelHeader.BorderStyle = BorderStyle.FixedSingle;
            panelHeader.Controls.Add(tableHeader);
            panelHeader.Dock = DockStyle.Fill;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Margin = new Padding(0);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(233, 20);
            panelHeader.TabIndex = 3;
            // 
            // tableHeader
            // 
            tableHeader.BackColor = SystemColors.ControlLight;
            tableHeader.ColumnCount = 2;
            tableHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableHeader.Controls.Add(labelHeader, 0, 0);
            tableHeader.Controls.Add(btnPopOut, 1, 0);
            tableHeader.Dock = DockStyle.Fill;
            tableHeader.Location = new Point(0, 0);
            tableHeader.Name = "tableHeader";
            tableHeader.RowCount = 1;
            tableHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableHeader.Size = new Size(231, 18);
            tableHeader.TabIndex = 0;
            // 
            // labelHeader
            // 
            labelHeader.AutoSize = true;
            labelHeader.ForeColor = SystemColors.ControlText;
            labelHeader.Location = new Point(3, 2);
            labelHeader.Margin = new Padding(3, 2, 3, 0);
            labelHeader.Name = "labelHeader";
            labelHeader.Size = new Size(88, 15);
            labelHeader.TabIndex = 0;
            labelHeader.Text = "Dockable Panel";
            // 
            // btnPopOut
            // 
            btnPopOut.BackColor = SystemColors.ControlLight;
            btnPopOut.Dock = DockStyle.Fill;
            btnPopOut.FlatAppearance.BorderSize = 0;
            btnPopOut.FlatStyle = FlatStyle.Flat;
            btnPopOut.Location = new Point(211, 0);
            btnPopOut.Margin = new Padding(0);
            btnPopOut.Name = "btnPopOut";
            btnPopOut.Size = new Size(20, 18);
            btnPopOut.TabIndex = 1;
            btnPopOut.Text = "^";
            btnPopOut.UseVisualStyleBackColor = false;
            btnPopOut.Click += btnPopOut_Click;
            // 
            // DockablePanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            Controls.Add(panelGlyphPropertyContainer);
            Name = "DockablePanel";
            Size = new Size(233, 346);
            Resize += DockablePanel_Resize;
            panelGlyphPropertyContainer.ResumeLayout(false);
            tableWidgetLayout.ResumeLayout(false);
            panelHeader.ResumeLayout(false);
            tableHeader.ResumeLayout(false);
            tableHeader.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelScrollablePanel;
        private Panel panelGlyphPropertyContainer;
        private TableLayoutPanel tableWidgetLayout;
        private Panel panelHeader;
        private TableLayoutPanel tableHeader;
        private Label labelHeader;
        private Button btnPopOut;
    }
}
