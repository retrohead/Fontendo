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
    public partial class EndianessPicker : UserControl
    {
        public Endianness.Endian? Endian
        {
            get => (Endianness.Endian?)button1.Tag;
            set
            {
                button1.Tag = value;
                button1.Text = value == Endianness.Endian.Little ? "LE" : "BE";
            }
        }
        public EndianessPicker()
        {
            InitializeComponent();
        }
    }
}
