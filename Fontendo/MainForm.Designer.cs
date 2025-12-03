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
            fileToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            recentFilesToolStripMenuItem = new ToolStripMenuItem();
            noRecentItemsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            splitContainer1 = new SplitContainer();
            tableLayoutPanel3 = new TableLayoutPanel();
            listViewSheets = new ListView();
            colorPickerBgColour = new Fontendo.Controls.ColorPickerButton();
            splitContainer2 = new SplitContainer();
            tableLayoutPanel2 = new TableLayoutPanel();
            listViewCharacters = new ListView();
            imageListCharacters = new ImageList(components);
            tableLayoutPanel4 = new TableLayoutPanel();
            pictureBox2 = new PictureBox();
            glyphEditor1 = new Fontendo.Controls.GlyphEditor();
            imageListSheets = new ImageList(components);
            tableLayoutPanel1.SuspendLayout();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
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
            tableLayoutPanel1.Controls.Add(splitContainer1, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(898, 461);
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
            tableLayoutPanel1.SetColumnSpan(menuStrip1, 2);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(898, 24);
            menuStrip1.TabIndex = 5;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripSeparator1, recentFilesToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            fileToolStripMenuItem.DropDownOpening += fileToolStripMenuItem_DropDownOpening;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(136, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(136, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(136, 22);
            saveAsToolStripMenuItem.Text = "Save As...";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(133, 6);
            // 
            // recentFilesToolStripMenuItem
            // 
            recentFilesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { noRecentItemsToolStripMenuItem });
            recentFilesToolStripMenuItem.Name = "recentFilesToolStripMenuItem";
            recentFilesToolStripMenuItem.Size = new Size(136, 22);
            recentFilesToolStripMenuItem.Text = "Recent Files";
            // 
            // noRecentItemsToolStripMenuItem
            // 
            noRecentItemsToolStripMenuItem.Enabled = false;
            noRecentItemsToolStripMenuItem.Name = "noRecentItemsToolStripMenuItem";
            noRecentItemsToolStripMenuItem.Size = new Size(161, 22);
            noRecentItemsToolStripMenuItem.Text = "No Recent Items";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(133, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(136, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.BackColor = SystemColors.Control;
            tableLayoutPanel1.SetColumnSpan(splitContainer1, 2);
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(5, 64);
            splitContainer1.Margin = new Padding(5, 0, 5, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.BackColor = SystemColors.Control;
            splitContainer1.Panel1.Controls.Add(tableLayoutPanel3);
            splitContainer1.Panel1MinSize = 150;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Panel2MinSize = 300;
            splitContainer1.Size = new Size(888, 397);
            splitContainer1.SplitterDistance = 283;
            splitContainer1.SplitterWidth = 7;
            splitContainer1.TabIndex = 6;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Controls.Add(listViewSheets, 0, 0);
            tableLayoutPanel3.Controls.Add(colorPickerBgColour, 0, 1);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(0, 0);
            tableLayoutPanel3.Margin = new Padding(0);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel3.Size = new Size(283, 397);
            tableLayoutPanel3.TabIndex = 0;
            // 
            // listViewSheets
            // 
            listViewSheets.BackColor = Color.FromArgb(122, 65, 196);
            tableLayoutPanel3.SetColumnSpan(listViewSheets, 2);
            listViewSheets.Dock = DockStyle.Fill;
            listViewSheets.ForeColor = Color.White;
            listViewSheets.Location = new Point(0, 0);
            listViewSheets.Margin = new Padding(0);
            listViewSheets.Name = "listViewSheets";
            listViewSheets.Size = new Size(283, 365);
            listViewSheets.TabIndex = 3;
            listViewSheets.UseCompatibleStateImageBehavior = false;
            listViewSheets.SelectedIndexChanged += listViewSheets_SelectedIndexChanged;
            // 
            // colorPickerBgColour
            // 
            colorPickerBgColour.CircleMargin = 10;
            colorPickerBgColour.CirclePosition = Fontendo.Controls.CirclePosition.Left;
            colorPickerBgColour.CircleSize = 20;
            colorPickerBgColour.Location = new Point(3, 368);
            colorPickerBgColour.Name = "colorPickerBgColour";
            colorPickerBgColour.SelectedColor = Color.FromArgb(122, 65, 196);
            colorPickerBgColour.Size = new Size(144, 26);
            colorPickerBgColour.TabIndex = 5;
            colorPickerBgColour.Text = "Background Colour";
            colorPickerBgColour.UseVisualStyleBackColor = true;
            colorPickerBgColour.ColorChanged += colorPickerBgColour_ColorChanged;
            colorPickerBgColour.PreviewColorChanged += colorPickerBgColour_PreviewColorChanged;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.FixedPanel = FixedPanel.Panel2;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Margin = new Padding(0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(tableLayoutPanel2);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(tableLayoutPanel4);
            splitContainer2.Panel2MinSize = 157;
            splitContainer2.Size = new Size(598, 397);
            splitContainer2.SplitterDistance = 305;
            splitContainer2.SplitterWidth = 7;
            splitContainer2.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.BackColor = SystemColors.Control;
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(listViewCharacters, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(0, 0);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel2.Size = new Size(305, 397);
            tableLayoutPanel2.TabIndex = 1;
            // 
            // listViewCharacters
            // 
            listViewCharacters.BackColor = Color.FromArgb(122, 65, 196);
            listViewCharacters.Dock = DockStyle.Fill;
            listViewCharacters.ForeColor = Color.White;
            listViewCharacters.LargeImageList = imageListCharacters;
            listViewCharacters.Location = new Point(0, 0);
            listViewCharacters.Margin = new Padding(0);
            listViewCharacters.Name = "listViewCharacters";
            listViewCharacters.Size = new Size(305, 365);
            listViewCharacters.TabIndex = 5;
            listViewCharacters.UseCompatibleStateImageBehavior = false;
            listViewCharacters.SelectedIndexChanged += listViewCharacters_SelectedIndexChanged;
            // 
            // imageListCharacters
            // 
            imageListCharacters.ColorDepth = ColorDepth.Depth32Bit;
            imageListCharacters.ImageSize = new Size(16, 16);
            imageListCharacters.TransparentColor = Color.Transparent;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            tableLayoutPanel4.Controls.Add(pictureBox2, 1, 1);
            tableLayoutPanel4.Controls.Add(glyphEditor1, 0, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(0, 0);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 2;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel4.Size = new Size(286, 397);
            tableLayoutPanel4.TabIndex = 0;
            // 
            // pictureBox2
            // 
            pictureBox2.Dock = DockStyle.Fill;
            pictureBox2.Image = Properties.Resources.Fontendo_tiny;
            pictureBox2.Location = new Point(231, 365);
            pictureBox2.Margin = new Padding(5, 0, 0, 0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(55, 32);
            pictureBox2.TabIndex = 10;
            pictureBox2.TabStop = false;
            // 
            // glyphEditor1
            // 
            tableLayoutPanel4.SetColumnSpan(glyphEditor1, 2);
            glyphEditor1.Dock = DockStyle.Fill;
            glyphEditor1.GlyphBackground = Color.FromArgb(122, 65, 196);
            glyphEditor1.Location = new Point(0, 0);
            glyphEditor1.Margin = new Padding(0, 0, 3, 0);
            glyphEditor1.Name = "glyphEditor1";
            glyphEditor1.Size = new Size(283, 365);
            glyphEditor1.TabIndex = 9;
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
            ClientSize = new Size(898, 461);
            Controls.Add(tableLayoutPanel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(700, 500);
            Name = "MainForm";
            Text = "Fontendo";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TextBox textFontFilePath;
        private Button btnBrowseFont;
        private ImageList imageListSheets;
        private ListView listViewSheets;
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
        private SplitContainer splitContainer1;
        private Controls.ColorPickerButton colorPickerBgColour;
        private TableLayoutPanel tableLayoutPanel3;
        private ImageList imageListCharacters;
        private SplitContainer splitContainer2;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel4;
        private PictureBox pictureBox2;
        private Controls.GlyphEditor glyphEditor1;
        private ListView listViewCharacters;
    }
}
