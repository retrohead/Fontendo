namespace Fontendo
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            tableLayoutPanel1 = new TableLayoutPanel();
            textFontFilePath = new TextBox();
            btnBrowseFont = new Button();
            menuStrip1 = new MenuStrip();
            splitContainerMain = new SplitContainer();
            splitContainerLeft = new SplitContainer();
            dockablePanelFont = new Fontendo.Controls.DockablePanel();
            panelSheetListBorder = new Panel();
            listViewSheets = new ListView();
            splitContainerRight = new SplitContainer();
            panelCharacterListBorder = new Panel();
            listViewCharacters = new ListView();
            imageListCharacters = new ImageList(components);
            dockablePanelGlyph = new Fontendo.Controls.DockablePanel();
            panel1 = new Panel();
            pictureBox2 = new PictureBox();
            imageListSheets = new ImageList(components);
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerLeft).BeginInit();
            splitContainerLeft.Panel1.SuspendLayout();
            splitContainerLeft.Panel2.SuspendLayout();
            splitContainerLeft.SuspendLayout();
            panelSheetListBorder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerRight).BeginInit();
            splitContainerRight.Panel1.SuspendLayout();
            splitContainerRight.Panel2.SuspendLayout();
            splitContainerRight.SuspendLayout();
            panelCharacterListBorder.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            tableLayoutPanel1.Controls.Add(textFontFilePath, 0, 1);
            tableLayoutPanel1.Controls.Add(btnBrowseFont, 1, 1);
            tableLayoutPanel1.Controls.Add(menuStrip1, 0, 0);
            tableLayoutPanel1.Controls.Add(splitContainerMain, 0, 2);
            tableLayoutPanel1.Controls.Add(panel1, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(898, 497);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // textFontFilePath
            // 
            textFontFilePath.BackColor = SystemColors.ControlLight;
            textFontFilePath.Dock = DockStyle.Fill;
            textFontFilePath.Enabled = false;
            textFontFilePath.Location = new Point(8, 35);
            textFontFilePath.Margin = new Padding(8, 3, 3, 3);
            textFontFilePath.Name = "textFontFilePath";
            textFontFilePath.Size = new Size(787, 23);
            textFontFilePath.TabIndex = 0;
            // 
            // btnBrowseFont
            // 
            btnBrowseFont.Dock = DockStyle.Fill;
            btnBrowseFont.Location = new Point(801, 35);
            btnBrowseFont.Margin = new Padding(3, 3, 8, 3);
            btnBrowseFont.Name = "btnBrowseFont";
            btnBrowseFont.Size = new Size(89, 26);
            btnBrowseFont.TabIndex = 1;
            btnBrowseFont.Text = "...";
            btnBrowseFont.UseVisualStyleBackColor = true;
            btnBrowseFont.Click += btnBrowseFont_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = SystemColors.Menu;
            menuStrip1.Dock = DockStyle.Fill;
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(798, 32);
            menuStrip1.TabIndex = 5;
            menuStrip1.Text = "menuStrip1";
            // 
            // splitContainerMain
            // 
            splitContainerMain.BackColor = SystemColors.Control;
            tableLayoutPanel1.SetColumnSpan(splitContainerMain, 2);
            splitContainerMain.Dock = DockStyle.Fill;
            splitContainerMain.Location = new Point(5, 64);
            splitContainerMain.Margin = new Padding(5, 0, 5, 3);
            splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            splitContainerMain.Panel1.BackColor = SystemColors.Control;
            splitContainerMain.Panel1.Controls.Add(splitContainerLeft);
            splitContainerMain.Panel1MinSize = 150;
            // 
            // splitContainerMain.Panel2
            // 
            splitContainerMain.Panel2.Controls.Add(splitContainerRight);
            splitContainerMain.Panel2MinSize = 300;
            splitContainerMain.Size = new Size(888, 430);
            splitContainerMain.SplitterDistance = 490;
            splitContainerMain.SplitterWidth = 7;
            splitContainerMain.TabIndex = 6;
            // 
            // splitContainerLeft
            // 
            splitContainerLeft.Dock = DockStyle.Fill;
            splitContainerLeft.FixedPanel = FixedPanel.Panel1;
            splitContainerLeft.Location = new Point(0, 0);
            splitContainerLeft.Name = "splitContainerLeft";
            // 
            // splitContainerLeft.Panel1
            // 
            splitContainerLeft.Panel1.Controls.Add(dockablePanelFont);
            splitContainerLeft.Panel1MinSize = 170;
            // 
            // splitContainerLeft.Panel2
            // 
            splitContainerLeft.Panel2.Controls.Add(panelSheetListBorder);
            splitContainerLeft.Size = new Size(490, 430);
            splitContainerLeft.SplitterDistance = 170;
            splitContainerLeft.SplitterWidth = 7;
            splitContainerLeft.TabIndex = 0;
            // 
            // dockablePanelFont
            // 
            dockablePanelFont.AutoScroll = true;
            dockablePanelFont.Dock = DockStyle.Fill;
            dockablePanelFont.HeaderText = "Dockable Panel";
            dockablePanelFont.Location = new Point(0, 0);
            dockablePanelFont.Name = "dockablePanelFont";
            dockablePanelFont.Size = new Size(170, 430);
            dockablePanelFont.TabIndex = 0;
            // 
            // panelSheetListBorder
            // 
            panelSheetListBorder.BorderStyle = BorderStyle.Fixed3D;
            panelSheetListBorder.Controls.Add(listViewSheets);
            panelSheetListBorder.Dock = DockStyle.Fill;
            panelSheetListBorder.Location = new Point(0, 0);
            panelSheetListBorder.Margin = new Padding(0);
            panelSheetListBorder.Name = "panelSheetListBorder";
            panelSheetListBorder.Size = new Size(313, 430);
            panelSheetListBorder.TabIndex = 1;
            // 
            // listViewSheets
            // 
            listViewSheets.BackColor = Color.FromArgb(122, 65, 196);
            listViewSheets.BorderStyle = BorderStyle.None;
            listViewSheets.Dock = DockStyle.Fill;
            listViewSheets.ForeColor = Color.White;
            listViewSheets.Location = new Point(0, 0);
            listViewSheets.Margin = new Padding(0);
            listViewSheets.Name = "listViewSheets";
            listViewSheets.Size = new Size(309, 426);
            listViewSheets.TabIndex = 4;
            listViewSheets.UseCompatibleStateImageBehavior = false;
            listViewSheets.SelectedIndexChanged += listViewSheets_SelectedIndexChanged;
            // 
            // splitContainerRight
            // 
            splitContainerRight.Dock = DockStyle.Fill;
            splitContainerRight.FixedPanel = FixedPanel.Panel2;
            splitContainerRight.Location = new Point(0, 0);
            splitContainerRight.Margin = new Padding(0);
            splitContainerRight.Name = "splitContainerRight";
            // 
            // splitContainerRight.Panel1
            // 
            splitContainerRight.Panel1.Controls.Add(panelCharacterListBorder);
            // 
            // splitContainerRight.Panel2
            // 
            splitContainerRight.Panel2.Controls.Add(dockablePanelGlyph);
            splitContainerRight.Panel2MinSize = 170;
            splitContainerRight.Size = new Size(391, 430);
            splitContainerRight.SplitterDistance = 122;
            splitContainerRight.SplitterWidth = 7;
            splitContainerRight.TabIndex = 0;
            // 
            // panelCharacterListBorder
            // 
            panelCharacterListBorder.BorderStyle = BorderStyle.Fixed3D;
            panelCharacterListBorder.Controls.Add(listViewCharacters);
            panelCharacterListBorder.Dock = DockStyle.Fill;
            panelCharacterListBorder.Location = new Point(0, 0);
            panelCharacterListBorder.Name = "panelCharacterListBorder";
            panelCharacterListBorder.Size = new Size(122, 430);
            panelCharacterListBorder.TabIndex = 0;
            // 
            // listViewCharacters
            // 
            listViewCharacters.BackColor = Color.FromArgb(122, 65, 196);
            listViewCharacters.BorderStyle = BorderStyle.None;
            listViewCharacters.Dock = DockStyle.Fill;
            listViewCharacters.ForeColor = Color.White;
            listViewCharacters.LargeImageList = imageListCharacters;
            listViewCharacters.Location = new Point(0, 0);
            listViewCharacters.Margin = new Padding(0);
            listViewCharacters.Name = "listViewCharacters";
            listViewCharacters.Size = new Size(118, 426);
            listViewCharacters.TabIndex = 7;
            listViewCharacters.UseCompatibleStateImageBehavior = false;
            listViewCharacters.SelectedIndexChanged += listViewCharacters_SelectedIndexChanged;
            // 
            // imageListCharacters
            // 
            imageListCharacters.ColorDepth = ColorDepth.Depth32Bit;
            imageListCharacters.ImageSize = new Size(16, 16);
            imageListCharacters.TransparentColor = Color.Transparent;
            // 
            // dockablePanelGlyph
            // 
            dockablePanelGlyph.AutoScroll = true;
            dockablePanelGlyph.Dock = DockStyle.Fill;
            dockablePanelGlyph.HeaderText = "Dockable Panel";
            dockablePanelGlyph.Location = new Point(0, 0);
            dockablePanelGlyph.Name = "dockablePanelGlyph";
            dockablePanelGlyph.Size = new Size(262, 430);
            dockablePanelGlyph.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Menu;
            panel1.Controls.Add(pictureBox2);
            panel1.Location = new Point(798, 0);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new Size(100, 32);
            panel1.TabIndex = 7;
            // 
            // pictureBox2
            // 
            pictureBox2.Dock = DockStyle.Right;
            pictureBox2.Image = Properties.Resources.Fontendo_tiny;
            pictureBox2.Location = new Point(45, 0);
            pictureBox2.Margin = new Padding(5, 0, 0, 0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(55, 32);
            pictureBox2.TabIndex = 11;
            pictureBox2.TabStop = false;
            // 
            // imageListSheets
            // 
            imageListSheets.ColorDepth = ColorDepth.Depth32Bit;
            imageListSheets.ImageSize = new Size(16, 16);
            imageListSheets.TransparentColor = Color.Transparent;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(898, 497);
            Controls.Add(tableLayoutPanel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(700, 500);
            Name = "MainForm";
            Text = "Fontendo";
            Shown += MainForm_Shown;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            splitContainerLeft.Panel1.ResumeLayout(false);
            splitContainerLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerLeft).EndInit();
            splitContainerLeft.ResumeLayout(false);
            panelSheetListBorder.ResumeLayout(false);
            splitContainerRight.Panel1.ResumeLayout(false);
            splitContainerRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerRight).EndInit();
            splitContainerRight.ResumeLayout(false);
            panelCharacterListBorder.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TextBox textFontFilePath;
        private Button btnBrowseFont;
        private ImageList imageListSheets;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem recentFilesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem noRecentItemsToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private SplitContainer splitContainerMain;
        private ImageList imageListCharacters;
        private SplitContainer splitContainerRight;
        private SplitContainer splitContainerLeft;
        private Panel panelSheetListBorder;
        private Panel panel1;
        private PictureBox pictureBox2;
        private Controls.DockablePanel dockablePanelFont;
        private Controls.DockablePanel dockablePanelGlyph;
        private ListView listViewSheets;
        private Panel panelCharacterListBorder;
        private ListView listViewCharacters;
        private ContextMenuStrip contextMenuGlyph;
        private ToolStripMenuItem replaceGlyphToolStripMenuItem;
        private ToolStripMenuItem exportGlyphToolStripMenuItem;
        private ContextMenuStrip contextMenuSheet;
        private ToolStripMenuItem exportSheetToolStripMenuItem;
        private ToolStripMenuItem replaceSheetToolStripMenuItem;
    }
}
