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
    public partial class DockablePanel : UserControl
    {
        public string HeaderText { get; set; } = "Dockable Panel";

        public Control? AttachedControl = null;

        public DockablePanel()
        {
            InitializeComponent();
        }

        public void Reattach(Control control)
        {
            if (control != null)
            {
                AttachedControl = control;
                panelScrollablePanel.Controls.Add(AttachedControl);
                AttachedControl.Dock = DockStyle.Top;
            }
        }

        public void InitializePanel(string headertext, UserControl userControl)
        {
            HeaderText = headertext;
            labelHeader.Text = HeaderText;

            AttachedControl = userControl;
            AttachedControl.Dock = DockStyle.Fill;
            panelScrollablePanel.Controls.Add(AttachedControl);
        }

        private void btnPopOut_Click(object sender, EventArgs e)
        {
            if (AttachedControl != null)
                DockManager.PopOut(AttachedControl);
        }

        private void DockablePanel_Resize(object sender, EventArgs e)
        {
            if (AttachedControl == null)
                return;
            AttachedControl.Height = panelScrollablePanel.ClientSize.Height;
            AttachedControl.Width = panelScrollablePanel.ClientSize.Width;
        }
    }
}
