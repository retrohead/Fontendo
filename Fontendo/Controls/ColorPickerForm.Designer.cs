
namespace Fontendo.Controls
{
    partial class ColorPickerForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorPickerForm));
            tableLayoutPanel2 = new TableLayoutPanel();
            label7 = new Label();
            textBrightnessVal = new TextBox();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            textHueVal = new TextBox();
            redBar = new TrackBar();
            greenBar = new TrackBar();
            blueBar = new TrackBar();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            textRedVal = new TextBox();
            textGreenVal = new TextBox();
            textBlueVal = new TextBox();
            saturationTrackBar1 = new SaturationTrackBar();
            textSaturationVal = new TextBox();
            hueTrackBar1 = new HueTrackBar();
            brightnessTrackBar1 = new BrightnessTrackBar();
            textHex = new TextBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            btnConfirm = new Button();
            btnCancel = new Button();
            tableLayoutPanel3 = new TableLayoutPanel();
            tableLayoutPanel4 = new TableLayoutPanel();
            previewPanel = new Panel();
            pictureBox1 = new PictureBox();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)redBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)greenBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)blueBar).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(label7, 0, 6);
            tableLayoutPanel2.Controls.Add(textBrightnessVal, 1, 2);
            tableLayoutPanel2.Controls.Add(label6, 0, 2);
            tableLayoutPanel2.Controls.Add(label5, 0, 1);
            tableLayoutPanel2.Controls.Add(label4, 0, 0);
            tableLayoutPanel2.Controls.Add(textHueVal, 1, 0);
            tableLayoutPanel2.Controls.Add(redBar, 2, 3);
            tableLayoutPanel2.Controls.Add(greenBar, 2, 4);
            tableLayoutPanel2.Controls.Add(blueBar, 2, 5);
            tableLayoutPanel2.Controls.Add(label1, 0, 3);
            tableLayoutPanel2.Controls.Add(label2, 0, 4);
            tableLayoutPanel2.Controls.Add(label3, 0, 5);
            tableLayoutPanel2.Controls.Add(textRedVal, 1, 3);
            tableLayoutPanel2.Controls.Add(textGreenVal, 1, 4);
            tableLayoutPanel2.Controls.Add(textBlueVal, 1, 5);
            tableLayoutPanel2.Controls.Add(saturationTrackBar1, 2, 1);
            tableLayoutPanel2.Controls.Add(textSaturationVal, 1, 1);
            tableLayoutPanel2.Controls.Add(hueTrackBar1, 2, 0);
            tableLayoutPanel2.Controls.Add(brightnessTrackBar1, 2, 2);
            tableLayoutPanel2.Controls.Add(textHex, 2, 6);
            tableLayoutPanel2.Location = new Point(3, 103);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 8;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(302, 229);
            tableLayoutPanel2.TabIndex = 1;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(3, 199);
            label7.Margin = new Padding(3, 7, 3, 0);
            label7.Name = "label7";
            label7.Size = new Size(28, 15);
            label7.TabIndex = 19;
            label7.Text = "Hex";
            // 
            // textBrightnessVal
            // 
            textBrightnessVal.BackColor = SystemColors.ControlLight;
            textBrightnessVal.Dock = DockStyle.Fill;
            textBrightnessVal.Enabled = false;
            textBrightnessVal.Location = new Point(83, 67);
            textBrightnessVal.Name = "textBrightnessVal";
            textBrightnessVal.Size = new Size(44, 23);
            textBrightnessVal.TabIndex = 17;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(3, 71);
            label6.Margin = new Padding(3, 7, 3, 0);
            label6.Name = "label6";
            label6.Size = new Size(62, 15);
            label6.TabIndex = 16;
            label6.Text = "Brightness";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(3, 39);
            label5.Margin = new Padding(3, 7, 3, 0);
            label5.Name = "label5";
            label5.Size = new Size(61, 15);
            label5.TabIndex = 12;
            label5.Text = "Saturation";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(3, 7);
            label4.Margin = new Padding(3, 7, 3, 0);
            label4.Name = "label4";
            label4.Size = new Size(29, 15);
            label4.TabIndex = 10;
            label4.Text = "Hue";
            // 
            // textHueVal
            // 
            textHueVal.BackColor = SystemColors.ControlLight;
            textHueVal.Enabled = false;
            textHueVal.Location = new Point(83, 3);
            textHueVal.Name = "textHueVal";
            textHueVal.Size = new Size(44, 23);
            textHueVal.TabIndex = 3;
            // 
            // redBar
            // 
            redBar.Dock = DockStyle.Fill;
            redBar.Location = new Point(133, 99);
            redBar.Maximum = 255;
            redBar.Name = "redBar";
            redBar.Size = new Size(166, 26);
            redBar.TabIndex = 0;
            redBar.ValueChanged += OnColourValueChanged;
            // 
            // greenBar
            // 
            greenBar.Dock = DockStyle.Fill;
            greenBar.Location = new Point(133, 131);
            greenBar.Maximum = 255;
            greenBar.Name = "greenBar";
            greenBar.Size = new Size(166, 26);
            greenBar.TabIndex = 1;
            greenBar.ValueChanged += OnColourValueChanged;
            // 
            // blueBar
            // 
            blueBar.Dock = DockStyle.Fill;
            blueBar.Location = new Point(133, 163);
            blueBar.Maximum = 255;
            blueBar.Name = "blueBar";
            blueBar.Size = new Size(166, 26);
            blueBar.TabIndex = 2;
            blueBar.ValueChanged += OnColourValueChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 103);
            label1.Margin = new Padding(3, 7, 3, 0);
            label1.Name = "label1";
            label1.Size = new Size(27, 15);
            label1.TabIndex = 4;
            label1.Text = "Red";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 135);
            label2.Margin = new Padding(3, 7, 3, 0);
            label2.Name = "label2";
            label2.Size = new Size(38, 15);
            label2.TabIndex = 5;
            label2.Text = "Green";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 167);
            label3.Margin = new Padding(3, 7, 3, 0);
            label3.Name = "label3";
            label3.Size = new Size(30, 15);
            label3.TabIndex = 6;
            label3.Text = "Blue";
            // 
            // textRedVal
            // 
            textRedVal.BackColor = SystemColors.ControlLight;
            textRedVal.Dock = DockStyle.Fill;
            textRedVal.Enabled = false;
            textRedVal.Location = new Point(83, 99);
            textRedVal.Name = "textRedVal";
            textRedVal.Size = new Size(44, 23);
            textRedVal.TabIndex = 7;
            // 
            // textGreenVal
            // 
            textGreenVal.BackColor = SystemColors.ControlLight;
            textGreenVal.Dock = DockStyle.Fill;
            textGreenVal.Enabled = false;
            textGreenVal.Location = new Point(83, 131);
            textGreenVal.Name = "textGreenVal";
            textGreenVal.Size = new Size(44, 23);
            textGreenVal.TabIndex = 8;
            // 
            // textBlueVal
            // 
            textBlueVal.BackColor = SystemColors.ControlLight;
            textBlueVal.Dock = DockStyle.Fill;
            textBlueVal.Enabled = false;
            textBlueVal.Location = new Point(83, 163);
            textBlueVal.Name = "textBlueVal";
            textBlueVal.Size = new Size(44, 23);
            textBlueVal.TabIndex = 9;
            // 
            // saturationTrackBar1
            // 
            saturationTrackBar1.Hue = 0;
            saturationTrackBar1.Location = new Point(133, 35);
            saturationTrackBar1.Name = "saturationTrackBar1";
            saturationTrackBar1.Saturation = 0;
            saturationTrackBar1.Size = new Size(166, 26);
            saturationTrackBar1.TabIndex = 11;
            saturationTrackBar1.Text = "saturationTrackBar1";
            saturationTrackBar1.SaturationChanged += OnColourHSBValueChanged;
            // 
            // textSaturationVal
            // 
            textSaturationVal.BackColor = SystemColors.ControlLight;
            textSaturationVal.Dock = DockStyle.Fill;
            textSaturationVal.Enabled = false;
            textSaturationVal.Location = new Point(83, 35);
            textSaturationVal.Name = "textSaturationVal";
            textSaturationVal.Size = new Size(44, 23);
            textSaturationVal.TabIndex = 13;
            // 
            // hueTrackBar1
            // 
            hueTrackBar1.Hue = 0;
            hueTrackBar1.Location = new Point(133, 3);
            hueTrackBar1.Name = "hueTrackBar1";
            hueTrackBar1.Size = new Size(166, 26);
            hueTrackBar1.TabIndex = 14;
            hueTrackBar1.Text = "hueTrackBar1";
            hueTrackBar1.HueChanged += OnColourHSBValueChanged;
            // 
            // brightnessTrackBar1
            // 
            brightnessTrackBar1.Brightness = 0;
            brightnessTrackBar1.Dock = DockStyle.Fill;
            brightnessTrackBar1.Hue = 0;
            brightnessTrackBar1.Location = new Point(133, 67);
            brightnessTrackBar1.Name = "brightnessTrackBar1";
            brightnessTrackBar1.Size = new Size(166, 26);
            brightnessTrackBar1.TabIndex = 15;
            brightnessTrackBar1.Text = "brightnessTrackBar1";
            brightnessTrackBar1.BrightnessChanged += OnColourHSBValueChanged;
            // 
            // textHex
            // 
            textHex.Dock = DockStyle.Fill;
            textHex.Location = new Point(145, 195);
            textHex.Margin = new Padding(15, 3, 15, 3);
            textHex.Name = "textHex";
            textHex.Size = new Size(142, 23);
            textHex.TabIndex = 18;
            textHex.TextChanged += textHex_TextChanged;
            textHex.KeyDown += textHex_KeyDown;
            textHex.Leave += textHex_Leave;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(btnConfirm, 0, 0);
            tableLayoutPanel1.Controls.Add(btnCancel, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 335);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(309, 32);
            tableLayoutPanel1.TabIndex = 10;
            // 
            // btnConfirm
            // 
            btnConfirm.Dock = DockStyle.Fill;
            btnConfirm.Location = new Point(3, 3);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new Size(144, 26);
            btnConfirm.TabIndex = 0;
            btnConfirm.Text = "Confirm";
            btnConfirm.UseVisualStyleBackColor = true;
            btnConfirm.Click += btnConfirm_Click;
            // 
            // btnCancel
            // 
            btnCancel.Dock = DockStyle.Fill;
            btnCancel.Location = new Point(153, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(144, 26);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel3.Controls.Add(tableLayoutPanel1, 0, 2);
            tableLayoutPanel3.Controls.Add(tableLayoutPanel2, 0, 1);
            tableLayoutPanel3.Controls.Add(tableLayoutPanel4, 0, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(0, 0);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 3;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel3.Size = new Size(309, 367);
            tableLayoutPanel3.TabIndex = 4;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 3;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
            tableLayoutPanel4.Controls.Add(previewPanel, 1, 0);
            tableLayoutPanel4.Controls.Add(pictureBox1, 0, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(3, 3);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.Size = new Size(303, 94);
            tableLayoutPanel4.TabIndex = 11;
            // 
            // previewPanel
            // 
            previewPanel.BackColor = Color.Black;
            previewPanel.BorderStyle = BorderStyle.Fixed3D;
            previewPanel.Dock = DockStyle.Fill;
            previewPanel.Location = new Point(140, 0);
            previewPanel.Margin = new Padding(0);
            previewPanel.Name = "previewPanel";
            previewPanel.Size = new Size(153, 94);
            previewPanel.TabIndex = 6;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Image = Properties.Resources.Fontendo;
            pictureBox1.Location = new Point(3, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(134, 88);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 7;
            pictureBox1.TabStop = false;
            // 
            // ColorPickerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(309, 367);
            Controls.Add(tableLayoutPanel3);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MaximumSize = new Size(329, 410);
            MinimizeBox = false;
            MinimumSize = new Size(329, 410);
            Name = "ColorPickerForm";
            Text = "Fontendo Colour Picker";
            FormClosed += ColorPickerForm_FormClosed;
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)redBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)greenBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)blueBar).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }


        #endregion

        private TableLayoutPanel tableLayoutPanel2;
        private TrackBar redBar;
        private TrackBar greenBar;
        private TrackBar blueBar;
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox textRedVal;
        private TextBox textGreenVal;
        private TextBox textBlueVal;
        private TableLayoutPanel tableLayoutPanel1;
        private Button btnConfirm;
        private Button btnCancel;
        private TextBox textHueVal;
        private TableLayoutPanel tableLayoutPanel3;
        private Label label5;
        private Label label4;
        private SaturationTrackBar saturationTrackBar1;
        private TextBox textSaturationVal;
        private HueTrackBar hueTrackBar1;
        private TextBox textBrightnessVal;
        private Label label6;
        private BrightnessTrackBar brightnessTrackBar1;
        private Label label7;
        private TextBox textHex;
        private TableLayoutPanel tableLayoutPanel4;
        private Panel previewPanel;
        private PictureBox pictureBox1;
    }
}