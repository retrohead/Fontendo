using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Fontendo.Controls
{
    public enum CirclePosition
    {
        Left,
        Right
    }

    public class ColorPickerButton : Button
    {
        private Color selectedColor = Color.Black;
        private int circleSize = 24;
        private int circleMargin = 6;
        private CirclePosition circlePosition = CirclePosition.Right;

        /// <summary>
        /// Raised whenever the SelectedColor property changes (after OK).
        /// </summary>
        public event EventHandler ColorChanged;

        /// <summary>
        /// Raised whenever the user previews a colour in the dialog (before OK).
        /// </summary>
        public event EventHandler<ColorPreviewEventArgs> PreviewColorChanged;

        public Color SelectedColor
        {
            get => selectedColor;
            set
            {
                if (selectedColor != value)
                {
                    var old = selectedColor;
                    selectedColor = value;
                    Invalidate();
                    OnColorChanged(EventArgs.Empty);
                }
            }
        }

        public int CircleSize
        {
            get => circleSize;
            set
            {
                if (value < 8) value = 8;
                circleSize = value;
                Invalidate();
            }
        }

        public int CircleMargin
        {
            get => circleMargin;
            set
            {
                if (value < 0) value = 0;
                circleMargin = value;
                Invalidate();
            }
        }

        public CirclePosition CirclePosition
        {
            get => circlePosition;
            set
            {
                circlePosition = value;
                Invalidate();
            }
        }

        public ColorPickerButton()
        {
            this.Text = "Pick Color";
            this.Click += ColorPickerButton_Click;
            Padding = new Padding(0, 2, 0, 0);
        }

        private void ColorPickerButton_Click(object sender, EventArgs e)
        {
            var picker = new ColorPickerForm(SelectedColor);
            picker.StartPosition = FormStartPosition.CenterParent;
            picker.PreviewColorChanged += (s, ev) => OnPreviewColorChanged(ev);
            if (picker.ShowDialog(MainForm.Self) == DialogResult.OK)
            {
                SelectedColor = picker.SelectedColor;
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            int diameter = Math.Min(this.Height - (CircleMargin * 2), CircleSize);
            int y = (this.Height - diameter) / 2;
            int x = circlePosition == CirclePosition.Right
                ? this.Width - diameter - CircleMargin
                : CircleMargin;

            Rectangle circleRect = new Rectangle(x, y, diameter, diameter);

            using (Brush brush = new SolidBrush(SelectedColor))
            {
                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                pevent.Graphics.FillEllipse(brush, circleRect);
            }

            using (Pen pen = new Pen(Color.Black, 1))
            {
                pevent.Graphics.DrawEllipse(pen, circleRect);
            }
        }

        protected virtual void OnColorChanged(EventArgs e)
        {
            ColorChanged?.Invoke(this, e);
        }

        protected virtual void OnPreviewColorChanged(ColorPreviewEventArgs e)
        {
            PreviewColorChanged?.Invoke(this, e);
        }
    }

    /// <summary>
    /// Event args for preview colour changes.
    /// </summary>
    public class ColorPreviewEventArgs : EventArgs
    {
        public Color PreviewColor { get; }

        public ColorPreviewEventArgs(Color previewColor)
        {
            PreviewColor = previewColor;
        }
    }
}
