
using static Fontendo.Controls.CustomWindow;

namespace Fontendo.DockManager
{
    public class CustomWindowOptions
    {
        public WindowTypes WindowType { get; set; } = WindowTypes.Resizable;
        public bool ShowGripperWhenResizable { get; set; } = true;
    }
}
