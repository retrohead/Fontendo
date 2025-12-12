using Fontendo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DockManager.Controls
{
    /// <summary>
    /// Interaction logic for DockablePanelFix.xaml
    /// </summary>

    public partial class DockablePanel : UserControl
    {
        public string HeaderText { get; set; } = "Dockable Panel";

        public UserControl? AttachedControl = null;

        public DockablePanel()
        {
            InitializeComponent();
        }

        public void Reattach(UserControl control)
        {
            if (control != null)
            {
                AttachedControl = control;
                AttachedControl.VerticalAlignment = VerticalAlignment.Top;
                panelScrollablePanel.Content = null;
                panelScrollablePanel.Content = AttachedControl;
            }
        }

        public void InitializePanel(string headertext, UserControl userControl)
        {
            HeaderText = headertext;
            labelHeader.Text = HeaderText;

            AttachedControl = userControl;
            AttachedControl.VerticalAlignment = VerticalAlignment.Top;
            panelScrollablePanel.Content = AttachedControl;
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
            AttachedControl.Height = panelScrollablePanel.ActualHeight;
            AttachedControl.Width = panelScrollablePanel.ActualWidth;
        }
    }
}
