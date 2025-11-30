using Fontendo.Extensions;

namespace Fontendo.Controls
{
    public partial class GlyphEditor : UserControl
    {
        private float zoomFactor = 1.0f;
        private float maxZoomFactor = 1.0f;
        private Bitmap? currentGlyph;
        private bool userHasSetZoom = false;
        private bool suppressZoomEvent = false;

        private Color glyphBackground = Color.White;
        public Color GlyphBackground
        {
            get => glyphBackground;
            set
            {
                glyphBackground = value;
                if (currentGlyph == null)
                    pictureBox1.BackColor = GlyphBackground;
                else
                    pictureBox1.Invalidate(); // force redraw with new background
            }
        }
        public GlyphEditor()
        {
            InitializeComponent();
        }

        public void ShowGlyphDetails(Glyph? glyph)
        {
            if (MainForm.Self == null) return;
            ShowGlyphImage(glyph);

            //    var encodingProp = (FontPropertyList.Property<int>)PropertyValues[FontProperty.CharEncoding];
            //    int encodingValue = encodingProp.Value; // 932

            //    switch (MainForm.Self.FontendoFont.Font.FontPropertyDescriptors[(int)FontProperty.CharEncoding])
            //    {
            //        case CharEncodings.UTF8:
            //        case CharEncodings.UTF16:
            //        case CharEncodings.CP1252:
            //            break;
            //        case CharEncodings.ShiftJIS:
            //            try
            //            {
            //                code = MainForm.Self.SJIS.CodeToUTF16(code);
            //            }
            //            catch (Exception e)
            //            {
            //                //glyphLabel->setText("");
            //                //glyphNameLabel->setText("");
            //                return;
            //            }
            //            break;
            //        case CharEncodings.Num:
            //            return;
            //    }
            //    UInt16[] chr = { code, 0x0000 };
            //    glyphLabel->setText(QString::fromUtf16(chr));
            //    auto test = globals->unicode->getCharNameFromUnicodeCodepoint(code);
            //    glyphNameLabel->setText(QString::fromStdString(globals->unicode->getCharNameFromUnicodeCodepoint(code)));
            //}
        }

        private void ShowGlyphImage(Glyph? glyph) 
        {
            pictureBox1.Image = null; // prevent auto-draw

            if (glyph == null)
            {
                ClearGlyphDetails();
                return;
            }

            currentGlyph = glyph.Pixmap;
            if (currentGlyph == null)
            {
                ClearGlyphDetails();
                return;
            }

            // Compute max zoom so image fits inside PictureBox
            float scaleX = (float)pictureBox1.Width / currentGlyph.Width;
            float scaleY = (float)pictureBox1.Height / currentGlyph.Height;
            maxZoomFactor = Math.Min(scaleX, scaleY);

            // Configure TrackBar range
            trackBarZoom.Minimum = 100;
            trackBarZoom.Maximum = (int)(maxZoomFactor * 100);

            // Only set slider to halfway if user has not changed it
            if (!userHasSetZoom)
            {
                int midValue = (trackBarZoom.Minimum + trackBarZoom.Maximum) / 2;
                suppressZoomEvent = true;
                trackBarZoom.Value = midValue;
                suppressZoomEvent = false;
                zoomFactor = midValue / 100f;
            }
            else
            {
                // Respect user’s chosen zoom
                zoomFactor = trackBarZoom.Value / 100f;
            }

            pictureBox1.Invalidate();
        }

        public void ClearGlyphDetails()
        {

        }

        private void trackBarZoom_ValueChanged(object? sender, EventArgs? e)
        {
            if (suppressZoomEvent) return; // ignore programmatic changes
            userHasSetZoom = true; // only set when user actually moves the slider
            zoomFactor = trackBarZoom.Value / 100f;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object? sender, PaintEventArgs? e)
        {
            if (currentGlyph == null)
            {
                using var brush = new SolidBrush(SystemColors.ControlLight);
                e.Graphics.FillRectangle(brush, pictureBox1.ClientRectangle);
                return;
            }

            float scaledWidth = currentGlyph.Width * zoomFactor;
            float scaledHeight = currentGlyph.Height * zoomFactor;

            float centerX = pictureBox1.ClientSize.Width / 2f;
            float centerY = pictureBox1.ClientSize.Height / 2f;

            float drawX = centerX - scaledWidth / 2f;
            float drawY = centerY - scaledHeight / 2f;

            // Fill full background
            using var bgBrush = new SolidBrush(SystemColors.ControlLight);
            e.Graphics.FillRectangle(bgBrush, pictureBox1.ClientRectangle);

            // Fill glyph background
            using var glyphBrush = new SolidBrush(glyphBackground);
            e.Graphics.FillRectangle(glyphBrush, drawX, drawY, scaledWidth, scaledHeight);

            // Draw glyph
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(currentGlyph, drawX, drawY, scaledWidth, scaledHeight);
        }
    }
}
