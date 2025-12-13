using Fontendo.UI;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Fontendo
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class popUpChangeEntry : UserControl
    {
        public popUps.popUpType? popUpObj;
        private MainWindow? mainWindow;
        private dynamic? listPanel;
        public bool changesMade = false;
        public dynamic? completeFunctionObj;
        public completedFunctionDelegate? completeFunctionDelegate;
        private comboBoxData comboEntryNewData = new comboBoxData();
        private string? type = "";
        private numberBoxData txtEntryNewNumData = new numberBoxData();
        private numberBoxData txtEntryNewPercentData = new numberBoxData();
        private bool useNumber = false;
        private bool usePercent = false;
        public string? oldVal
        {
            get => txtEntryCurrent.Content.ToString() ?? "";
            set => txtEntryCurrent.Content = value;
        }

        public string? newVal
        {
            get => txtEntryNew.Text;
            set => txtEntryNew.Text = value;
        }

        public int maxLen
        {
            get => txtEntryNew.MaxLength;
            set => txtEntryNew.MaxLength = value;
        }


        public enum refresh_objects
        {
            none
        }

        public popUpChangeEntry(MainWindow? mainWin, string? value_name, string? orig_val, int maxLen, object? panelWithList, object? OnCompleteFunctionObj, completedFunctionDelegate? OnCompleteFunctionDelegate)
        {

            // This call is required by the designer.
            InitializeComponent();
            Name = "popUpChangeEntry";
            mainWindow = mainWin;

            // Add any initialization after the InitializeComponent() call.
            mainWindow = mainWin;
            listPanel = panelWithList;

            txtEntryNew.MaxLength = maxLen;
            newVal = orig_val;
            oldVal = orig_val;
            txtCurText.Content = "Current " + value_name;
            txtNewText.Content = "New " + value_name;
            type = value_name;


            long longVal;
            double doubleVal;
            txtEntryNewNum.DataContext = txtEntryNewNumData;
            txtEntryNewPercent.DataContext = txtEntryNewPercentData;
            if (orig_val?.Replace("%", "") != orig_val)
            {
                doubleVal = double.Parse(orig_val?.Replace("%", "") ?? "0");
                oldVal = String.Format("{0:n2}", doubleVal) + "%";
                newVal = String.Format("{0:n2}", doubleVal);
                txtEntryNewPercentData.Value = newVal;
                txtEntryNewPercent.Visibility = Visibility.Visible;
                txtEntryNew.Visibility = Visibility.Hidden;
                usePercent = true;
            }
            else
            {
                txtEntryNewPercent.Visibility = Visibility.Hidden;
            }
            if (long.TryParse(orig_val, out longVal) && !usePercent)
            {
                oldVal = String.Format("{0:n0}", longVal);
                txtEntryNewNumData.Value = orig_val;
                txtEntryNewNum.Visibility = Visibility.Visible;
                txtEntryNew.Visibility = Visibility.Hidden;
                useNumber = true;
            }
            else
            {
                txtEntryNewNum.Visibility = Visibility.Hidden;
            }

            comboEntryNewData.Items = new System.Collections.ObjectModel.ObservableCollection<comboData>();

            if ((!useNumber) && (!usePercent))
                txtEntryNew.Visibility = Visibility.Visible;
            if (!useNumber)
                txtEntryNewNum.Visibility = Visibility.Hidden;
            if (!usePercent)
                txtEntryNewPercent.Visibility = Visibility.Hidden;
            if ((!useNumber) || (!usePercent))
                comboEntryNew.Visibility = Visibility.Hidden;

            comboEntryNew.DataContext = comboEntryNewData;
            completeFunctionObj = OnCompleteFunctionObj;
            completeFunctionDelegate = OnCompleteFunctionDelegate;
        }
        public void resize(double newsize)
        {
        }
        public void refresh(refresh_objects obj)
        {
           App.CustomMessageBox.Show("Refresh not implemented", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        public void load()
        {
            if (popUpObj == null)
                return;
            popUpObj.cancelFunctionDelegate = cancel;
            // nothing to load

            popUpObj.appear();
        }
        private void fieldWasChanged(object o)
        {
            if ((popUpObj == null))
                return;
            if ((popUpObj.IsOpen & popUpObj.window.panelPopupContentPanel?.Opacity == 1))
                changesMade = true;
        }
        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (popUpObj == null)
                return;
            // close the popup
            popUpObj.closePopUp(fadeCompleted, null);
        }

        private void comboEntryNew_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboEntryNewData.SelectedComboItem == null)
                return;
            if (type == "special effect hex mod")
            {
                switch (comboEntryNew.SelectedIndex)
                {
                    case 0:
                        txtEntryNew.Text = txtEntryCurrent.Content.ToString();
                        break;
                    case 1:
                        txtEntryNew.Text = "60C4850F030805000000";
                        break;
                }
            }
            else
                txtEntryNew.Text = comboEntryNewData.SelectedComboItem.Value;
        }

        private void txtEntryNew_TextChanged(object sender, TextChangedEventArgs e)
        {
            fieldWasChanged(sender);
        }

        private void cancel()
        {
            if (mainWindow?.canLoseChanges(this) == false)
                return;
            changesMade = false;
            popUpObj?.closePopUp(fadeCompletedCancel, null);
        }


        public void fadeCompletedCancel()
        {
            if(mainWindow == null)
                return;
            changesMade = false;
            mainWindow.panelSmallProgress.Visibility = Visibility.Hidden;
            if ((listPanel?.Name.StartsWith("popUp")))
                mainWindow.enablePopUp(true);
            else
                mainWindow.enableForm(true);
            completeFunctionObj?.Dispatcher.BeginInvoke(completeFunctionDelegate, "");
        }

        public void fadeCompleted()
        {
            changesMade = false;
            if (useNumber)
                newVal = txtEntryNewNumData.Value.ToString();
            if (usePercent)
                newVal = txtEntryNewPercentData.Value.ToString();
            completeFunctionObj?.Dispatcher.BeginInvoke(completeFunctionDelegate, newVal);
        }

        public delegate void completedFunctionDelegate(string result);
    }
}
