using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fontendo.Controls
{
    public class HexNumericUpDown : NumericUpDown
    {
        private TextBox? innerBox;
        public HexNumericUpDown()
        {
            Minimum = 0;
            Maximum = 0xFFFF; // or long.MaxValue if you want
            TextAlign = HorizontalAlignment.Right;

            innerBox = Controls.OfType<TextBox>().FirstOrDefault();
            if (innerBox != null)
            {
                innerBox.KeyDown += InnerBox_KeyDown;
            }
            MouseDown += NumberBox_MouseDown;
            MouseMove += NumberBox_MouseDown;
        }

        protected override void UpdateEditText()
        {
            int digits = (int)Math.Ceiling(Math.Log((double)Maximum + 1, 16));
            Text = $"0x{((long)Value).ToString($"X{digits}")}";
        }

        protected override void ValidateEditText()
        {
            string raw = Text.Trim().Replace("0x", "");
            if (long.TryParse(raw, System.Globalization.NumberStyles.HexNumber, null, out var parsed))
            {
                if (parsed >= (long)Minimum && parsed <= (long)Maximum)
                    Value = parsed;
            }
            else
            {
                base.ValidateEditText();
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // Prevent typing before index 2
            if (Controls.OfType<TextBox>().FirstOrDefault() is TextBox tb)
            {
                if (tb.SelectionStart < 2)
                {
                    e.Handled = true;
                    tb.SelectionStart = 2;
                }
            }
            base.OnKeyPress(e);
        }

        private void InnerBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (innerBox == null) return;

            // Clamp caret so it never goes before index 2
            if (innerBox.SelectionStart <= 2)
            {
                innerBox.SelectionStart = 2;

                if(innerBox.SelectionLength > 2)
                    innerBox.SelectionLength -= 2;
                else
                    innerBox.SelectionLength = 0;
            }
            if (innerBox.SelectionStart + innerBox.SelectionLength <= 2)
            {
                innerBox.SelectionStart = 2;
                innerBox.SelectionLength = 0;
            }

            // Block Backspace/Delete if caret is at or before prefix
            if (innerBox.SelectionStart < 2 && e.KeyCode != Keys.Enter)
            {
                e.Handled = true;
            }
        }

        private void NumberBox_MouseDown(object? sender, EventArgs e)
        {
            if (innerBox != null && innerBox.SelectionStart < 2)
            {
                innerBox.SelectionStart = 2; // force caret after "0x"
            }
        }
    }



}
