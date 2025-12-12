using Fontendo.Controls;
using System.Windows;
using System.Windows.Controls;

namespace Fontendo.DockManager
{
    /// <summary>
    /// Base class for content hosted within a CustomWindow. helps with interacting with the parent window.
    /// </summary>
    public abstract class CustomWindowContentBase : UserControl
    {

        /// <summary>
        /// True once the control has been initialized by the dock manager.
        /// </summary>
        public readonly bool IsCustomWindowContentInitialized = true;

        /// <summary>
        /// Reference to the parent CustomWindow hosting this control.
        /// </summary>
        public CustomWindow? Window { get; set; }

        public bool? DialogResult
        {
            get
            {
                return Window?.DialogResult;
            }
            set
            {
                if (Window == null)
                {
                    throw new System.InvalidOperationException("Cannot set DialogResult because ParentCustomWindow is null.");
                }
                Window.DialogResult = value;
            }
        }

        public string Title
        {
            get => Window?.Title ?? "";
            set { VerifyWindow(); Window!.Title = value; }
        }
        public WindowState WindowState
        {
            get => Window!.WindowState;
            set { VerifyWindow(); Window!.WindowState = value; }
        }

        private void VerifyWindow()
        {
            if(Window == null)
            {
                throw new System.InvalidOperationException("Cannot access ParentCustomWindow because it is null, please try setting your values later.");
            }
        }

        public void Close()
        {
            if(Window == null)
            {
                throw new System.InvalidOperationException("Cannot close CustomWindowContentBase because ParentCustomWindow is null.");
            }
            Window.Close();
        }

    }
}
