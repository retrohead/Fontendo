using Fontendo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fontendo.Controls
{
    public partial class GlyphEditor : UserControl
    {
        public GlyphEditor()
        {
            InitializeComponent();
        }

        public void ShowGlyphDetails(Glyph glyph)
        {
            pictureBox1.Image = glyph.Pixmap;
        }

        public void ClearGlyphDetails()
        {

        }
    }
}
