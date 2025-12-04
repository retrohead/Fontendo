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
        public bool LitteEndian
        {
            get => (bool)(button1.Tag ?? false);
            set
            {
                button1.Tag = value;
                button1.Text = value ? "LE" : "BE";
            }
        }
        public EndianessPicker()
        {
            InitializeComponent();
            button1.Click += EndianessPicker_Click;
        }

        private void EndianessPicker_Click(object? sender, EventArgs e)
        {
            LitteEndian = !LitteEndian;
        }
    }
}
