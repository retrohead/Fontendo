using Fontendo.DockManager;
using System.Windows;
using System.Windows.Controls;

namespace Fontendo.Controls
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
                DockHandler.PopOut(AttachedControl);
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
