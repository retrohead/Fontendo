using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Windows.Media;
using Fontendo.Extensions;
using Fontendo.DockManager;
using Fontendo.UI;

namespace Fontendo
{
    /// <summary>
    /// Interaction logic for popUpOptions.xaml
    /// </summary>
    public partial class popUpOptions : UserControl
    {
        private MainWindow mainWindow;

        public popUps.popUpType popUpObj;
        private object listPanel;
        public int loadedId = -1;
        public bool changesMade = false;
        private int initialanimSpeed;

        private listViewDataType lstThemesData;
        private listViewItemDataType itemToSelect;
        private comboBoxData comboAnimationSpeedData;

        private BackgroundWorker bgWorkLoadThemes = new BackgroundWorker();

        public popUpOptions(MainWindow main_win, object panelWithList)
        {
            mainWindow = main_win;
            bgWorkLoadThemes = new BackgroundWorker();

            // This call is required by the designer.
            InitializeComponent();
            Name = "popUpOptions";

            // Add any initialization after the InitializeComponent() call.
            listPanel = panelWithList;

            lstThemesData = new listViewDataType(mainWindow, ref lstThemes);
            lstThemes.DataContext = lstThemesData;

            bgWorkLoadThemes.DoWork += bgWorkLoadThemes_DoWork;
            bgWorkLoadThemes.RunWorkerCompleted += bgWorkLoadThemes_RunWorkerCompleted;


            comboAnimationSpeedData = new comboBoxData();
            comboAnimationSpeedData.Items = new System.Collections.ObjectModel.ObservableCollection<comboData>();
            comboAnimationSpeedData.Items.Add(new comboData("Slower", ((int)objectAnimations.animSpeed.normal).ToString()));
            comboAnimationSpeedData.Items.Add(new comboData("Default", ((int)objectAnimations.animSpeed.medium).ToString()));
            comboAnimationSpeedData.Items.Add(new comboData("Faster", ((int)objectAnimations.animSpeed.fast).ToString()));
            comboAnimationSpeedData.Items.Add(new comboData("Instant", ((int)objectAnimations.animSpeed.instant).ToString()));
            comboAnimationSpeed.DataContext = comboAnimationSpeedData;

            int index = SettingsManager.Settings.AnimationSpeed;
            comboAnimationSpeedData.SelectedComboItem = comboAnimationSpeedData.Items[index];
            comboAnimationSpeed.DataContext = comboAnimationSpeedData;
            initialanimSpeed = index;
        }

        public class themeFileType
        {
            public string _id = "";
            public string _fn = "";
            public string _name = "";
            public string _source = "";
            public string _createdby = "";
            public string _createdon = "";
            public Theme.ThemeColorsType _themeColors;

            public themeFileType(string fn, string name, string source, string createdby, string createdon, Theme.ThemeColorsType themeColors)
            {
                _fn = fn;
                _name = name;
                _source = source;
                _createdby = createdby;
                _createdon = createdon;
                _themeColors = themeColors;
            }
        }

        private List<themeFileType> themeFiles = new List<themeFileType>();
        private int themeFileCount = 0;

        public void load()
        {
            popUpObj.cancelFunctionDelegate = cancel;

            mainWindow.panelSmallProgress.Visibility = Visibility.Visible;
            mainWindow.enablePopUp(false);
            refreshThemeList();
        }

        public void refreshThemeList()
        {
            lstThemesData.Items = new System.Collections.ObjectModel.ObservableCollection<listViewItemDataType>();
            themeFiles.Clear();
            lstThemesData.SelectedListItem = null;
            mainWindow.panelSmallProgress.Visibility = Visibility.Visible;
            mainWindow.enablePopUp(false);
            mainWindow.updateProgressLabel("Loading Themes");
            bgWorkLoadThemes.RunWorkerAsync();
        }

        private listViewItemDataType createThemeListViewItem(themeFileType themeFile)
        {
            themeFile._id = themeFiles.Count.ToString();
            themeFiles.Add(themeFile);

            listViewItemDataType item;
            item = new listViewItemDataType(lstThemesData, themeFile._name, themeFile._id);
            item.SubItems.Add(new listViewColumnDataType(item, themeFile._source));
            item.SubItems.Add(new listViewColumnDataType(item, themeFile._createdby));
            item.SubItems.Add(new listViewColumnDataType(item, themeFile._createdon));
            return item;
        }

        private void bgWorkLoadThemes_DoWork(object sender, DoWorkEventArgs e)
        {
            itemToSelect = null;
            // standard themes
            themeFileType themeFile;
            string selected = "";
            if (Theme.compareThemeColours(Theme.selectedThemeColours, Theme.defaultThemeColours[(int)Theme.themeType.infinity_blue]))
                selected = " (Selected Theme)";
            themeFile = new themeFileType("", "Infinity Blue", "Built-In" + selected, "Funky Skunk", "27/07/2020", Theme.defaultThemeColours[(int)Theme.themeType.infinity_blue]);
            lstThemesData.AddItem(createThemeListViewItem(themeFile));
            if (selected != "")
                itemToSelect = lstThemesData.Items[lstThemesData.Items.Count - 1];

            selected = "";
            if (Theme.compareThemeColours(Theme.selectedThemeColours, Theme.defaultThemeColours[(int)Theme.themeType.dark]))
                selected = " (Selected Theme)";
            themeFile = new themeFileType("", "Dark Theme", "Built-In" + selected, "Funky Skunk", "27/07/2020", Theme.defaultThemeColours[(int)Theme.themeType.dark]);
            lstThemesData.AddItem(createThemeListViewItem(themeFile));
            if (selected != "")
                itemToSelect = lstThemesData.Items[lstThemesData.Items.Count - 1];

            selected = "";
            if (Theme.compareThemeColours(Theme.selectedThemeColours, Theme.defaultThemeColours[(int)Theme.themeType.light]))
                selected = " (Selected Theme)";
            themeFile = new themeFileType("", "Light Theme", "Built-In" + selected, "Funky Skunk", "27/07/2020", Theme.defaultThemeColours[(int)Theme.themeType.light]);
            lstThemesData.AddItem(createThemeListViewItem(themeFile));
            if (selected != "")
                itemToSelect = lstThemesData.Items[lstThemesData.Items.Count - 1];

            selected = "";
            if (Theme.compareThemeColours(Theme.selectedThemeColours, Theme.defaultThemeColours[(int)Theme.themeType.grey_sky]))
                selected = " (Selected Theme)";
            themeFile = new themeFileType("", "Grey Sky Theme", "Built-In" + selected, "Funky Skunk", "28/10/2020", Theme.defaultThemeColours[(int)Theme.themeType.grey_sky]);
            lstThemesData.AddItem(createThemeListViewItem(themeFile));
            if (selected != "")
                itemToSelect = lstThemesData.Items[lstThemesData.Items.Count - 1];

            mainWindow.updateProgressLabel("Loading Theme File Collection");
            // load the theme files
            themeFileCount = 0;

            string[] files = System.IO.Directory.GetFiles(System.IO.Path.Combine(MainWindow.appDataPath, "Themes"), "*.Theme.vth");
            if ((files.Length > 0))
            {
                foreach (string file in files)
                {
                    string themeStr = System.IO.File.ReadAllText(file);
                    string[] themeVals = themeStr.Split('|');

                    Theme.ThemeColorsType themeColours = new Theme.ThemeColorsType();
                    try
                    {
                        themeColours.ColorDark = themeVals[0];
                        themeColours.ColorMedium = themeVals[1];
                        themeColours.ColorLight = themeVals[2];
                        themeColours.ColorPanelAlpha = themeVals[3];
                        themeColours.ColorSelected = themeVals[4];
                        themeColours.ColorHighlight = themeVals[5];
                        themeColours.ColorHeaderText = themeVals[6];
                        themeColours.ColorLabelText = themeVals[7];
                        themeColours.ColorActiveText = themeVals[8];
                        themeColours.ColorInactiveText = themeVals[9];
                        themeColours.ColorControlBorder = themeVals[10];
                        themeColours.ColorControlHighlightBorder = themeVals[11];
                        themeColours.ColorPositiveText = themeVals[12];
                        themeColours.ColorNegativeText = themeVals[13];

                        bool selectItem = false;
                        if ((Theme.compareThemeColours(Theme.selectedThemeColours, themeColours)))
                        {
                            selectItem = true;
                            themeFile = new themeFileType(file, themeVals[14], "Local File (Selected Theme)", themeVals[15], themeVals[16], themeColours);
                        }
                        else
                            themeFile = new themeFileType(file, themeVals[14], "Local File", themeVals[15], themeVals[16], themeColours);

                        lstThemesData.AddItem(createThemeListViewItem(themeFile));
                        if ((selectItem))
                            itemToSelect = lstThemesData.Items[lstThemesData.Items.Count - 1];
                        themeFileCount = themeFileCount + 1;
                    }
                    catch
                    {
                    }
                }
            }


            // finish up
            if ((itemToSelect == null))
            {
                // must be a custom theme!
                string name = Strings.LCase(Interaction.Environ("UserName"));
                themeFile = new themeFileType("", "Current Theme", "Saved To Settings", name, "N/A", Theme.selectedThemeColours);
                lstThemesData.AddItem(createThemeListViewItem(themeFile));
                itemToSelect = lstThemesData.Items[lstThemesData.Items.Count - 1];
            }
        }

        private void bgWorkLoadThemes_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lstThemesData.SelectedListItem = itemToSelect;
            mainWindow.panelSmallProgress.Visibility = Visibility.Hidden;
            mainWindow.enablePopUp(true);
            listViewDataType.autoResizeListBoxCols(ref lstThemes, ref lstThemesData);
            // nothing to load
            popUpObj.appear(appearDelayComplete);
        }

        public void appearDelayComplete()
        {
            listViewDataType.autoResizeListBoxCols(ref lstThemes, ref lstThemesData);
        }
        public void resize(double newsize)
        {
        }

        private void fieldWasChanged(object o)
        {
            if ((popUpObj.IsOpen & popUpObj.window.panelPopupContentPanel.Opacity == 1))
                changesMade = true;
        }

        private void applyListViewTheme(object sender)
        {
            themeFileType themeFile = themeFiles[System.Convert.ToInt32(lstThemesData.SelectedListItem.id)];
            try
            {
                ColorDark.SelectedColor = (Color)ColorConverter.ConvertFromString(themeFile._themeColors.ColorDark);
                ColorMedium.SelectedColor = (Color)ColorConverter.ConvertFromString(themeFile._themeColors.ColorMedium);
                ColorLight.SelectedColor = (Color)ColorConverter.ConvertFromString(themeFile._themeColors.ColorLight);
                ColorPanelAlpha.SelectedColor = (Color)ColorConverter.ConvertFromString(themeFile._themeColors.ColorPanelAlpha);
                ColorSelected.SelectedColor = (Color)ColorConverter.ConvertFromString(themeFile._themeColors.ColorSelected);
                ColorHighlight.SelectedColor = (Color)ColorConverter.ConvertFromString(themeFile._themeColors.ColorHighlight);
                ColorHeaderText.SelectedColor = (Color)ColorConverter.ConvertFromString(themeFile._themeColors.ColorHeaderText);
                ColorLabelText.SelectedColor = (Color)ColorConverter.ConvertFromString(themeFile._themeColors.ColorLabelText);
                ColorActiveText.SelectedColor = (Color)ColorConverter.ConvertFromString(themeFile._themeColors.ColorActiveText);
                ColorInactiveText.SelectedColor = (Color)ColorConverter.ConvertFromString(themeFile._themeColors.ColorInactiveText);
                ColorControlBorder.SelectedColor = (Color)ColorConverter.ConvertFromString(themeFile._themeColors.ColorControlBorder);
                ColorControlHighlightBorder.SelectedColor = (Color)ColorConverter.ConvertFromString(themeFile._themeColors.ColorControlHighlightBorder);
                ColorPositiveText.SelectedColor = (Color)ColorConverter.ConvertFromString(themeFile._themeColors.ColorPositiveText);
                ColorNegativeText.SelectedColor = (Color)ColorConverter.ConvertFromString(themeFile._themeColors.ColorNegativeText);

                DockHandler.ApplyThemeColorsToOpenWindows(Theme.getThemeColorsFromWindowResources(mainWindow));
            }
            catch
            {
            }
            fieldWasChanged(sender);
        }

        private void lstThemes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems.Count > 0))
                ((ListView)sender).ScrollIntoView(e.AddedItems[0]);

            if (!(lstThemesData.SelectedListItem == null))
            {
                applyListViewTheme(sender);
                fieldWasChanged(sender);
            }
        }

        private void lstThemesColumn_Click(object sender, RoutedEventArgs e)
        {
            gridViewHelper.GridViewColumnHeaderClicked(mainWindow, lstThemes, lstThemesData, sender, e);
        }

        private void lst_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
        }

        private void btnSaveTheme_Click(object sender, RoutedEventArgs e)
        {
            if ((changesMade == true))
            {
                if ((MessageBox.Show("You must apply your changes before you can save the Theme." + Constants.vbNewLine + Constants.vbNewLine + "Do you want to apply your changes now?", "Confirm Apply", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel))
                    return;
            }
            setThemeEditorAsSelectedTheme();
            // check if the theme exists first
            int i;
            for (i = 0; i <= themeFiles.Count - 1; i++)
            {
                if ((themeFiles[i]._source != "Saved To Settings"))
                {
                    if ((Theme.compareThemeColours(Theme.selectedThemeColours, themeFiles[i]._themeColors)))
                    {
                        MessageBox.Show("There is already a theme with the same colours in your collection so you cannot save it", "Cannot Save", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }
            }
            string name = Strings.LCase(Interaction.Environ("UserName"));
            dynamic pop = new popUpChangeEntry(mainWindow, "Theme Name", "Theme By " + name, 50, this, this, new popUpChangeEntry.completedFunctionDelegate(btnSaveThemeChooseName_Complete));
            popUps.loadPopUp(mainWindow, "Select a name for your theme", "Theme.png", ref pop);
        }

        public void btnSaveThemeChooseName_Complete(object result)
        {
            if ((result == null))
                return;
            string fn = System.IO.Path.Combine(MainWindow.appDataPath, "Themes", themeFileCount + ".Theme.vth");
            if (!System.IO.Directory.Exists(System.IO.Path.Combine(MainWindow.appDataPath, "Themes")))
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(MainWindow.appDataPath, "Themes"));
            if ((System.IO.File.Exists(fn)))
            {
                MessageBox.Show("Fatal Error: A theme file already exists in the location but this should have been recognised already!" + Constants.vbNewLine + Constants.vbNewLine + fn, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string themeString = selectedThemeColoursString();
            string name = Strings.LCase(Interaction.Environ("UserName"));
            themeString = themeString + "|" + result + "|" + name + "|" + DateTime.Now.ToString(App.dateFormat);
            System.IO.File.WriteAllText(fn, themeString);
            tabsMain.SelectedItem = tabOptions;
            refreshThemeList();
        }

        private void lstThemes_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            listViewItemDataType itemClicked = null;
            if ((lstThemesData.getItemClicked(mainWindow, ref itemClicked, sender, e) == false))
            {
                e.Handled = true;
                return;
            }
            FrameworkElement fe = (FrameworkElement)e.Source;
            if ((itemClicked == null))
            {
                fe.ContextMenu = new ContextMenu();
                fe.ContextMenu.Visibility = Visibility.Hidden;
                e.Handled = true;
                return;
            }
            lstThemesData.SelectedListItem = itemClicked;

            themeFileType themeFile = themeFiles[System.Convert.ToInt32(lstThemesData.SelectedListItem.id)];

            List<contextMenuHelper.contextMenuData> items = new List<contextMenuHelper.contextMenuData>();
            items.Add(new contextMenuHelper.contextMenuData("import.png", "Import Theme File", new RoutedEventHandler(btnImportThemeFile_Click), contextMenuHelper.contextMenuItemType.item_coloured_icon));
            if ((themeFile._source.StartsWith("Local")) | (themeFile._source.Contains("Database")))
                items.Add(new contextMenuHelper.contextMenuData("export.png", "Export Theme File", new RoutedEventHandler(btnExportThemeFile_Click), contextMenuHelper.contextMenuItemType.item_coloured_icon));
            if ((themeFile._source.StartsWith("Local")))
                items.Add(new contextMenuHelper.contextMenuData("recycle.png", "Delete Theme File", new RoutedEventHandler(btnDeleteThemeFile_Click), contextMenuHelper.contextMenuItemType.item_coloured_icon));

            fe.ContextMenu = contextMenuHelper.createContextMenu(mainWindow, fe, items);
            fe.ContextMenu = new ContextMenu();
            fe.ContextMenu.Visibility = Visibility.Visible;
            e.Handled = true;
        }

        private void btnExportThemeFile_Click(object sender, RoutedEventArgs e)
        {
            themeFileType themeFile = themeFiles[System.Convert.ToInt32(lstThemesData.SelectedListItem.id)];
            string fn = fileHelper.getSaveAsName(themeFile._name, "Choose a location to save the theme file.", "Vestas SAP and Database Portal Theme File (*.Theme.vth)|*.Theme.vth", ".Theme.vth");
            if ((fn == ""))
                return;
            if ((System.IO.File.Exists(fn)))
                System.IO.File.Delete(fn);
            if ((themeFile._fn == ""))
            {
                string themeString = selectedThemeColoursString();
                themeString = themeString + "|" + System.IO.Path.GetFileNameWithoutExtension(fn).Replace(".theme", "") + "|" + Strings.LCase(Interaction.Environ("UserName")) + "|" + DateTime.Now.ToString(App.dateFormat);
                System.IO.File.WriteAllText(fn, themeString);
            }
            else
                System.IO.File.Copy(themeFile._fn, fn);
            MessageBox.Show("The theme file was exported successfully", "Save Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnImportThemeFile_Click(object sender, RoutedEventArgs e)
        {
            string fn = fileHelper.browseForFile("Select a theme file that you would like to import.", "Vestas SAP and Database Portal Theme File (*.Theme.vth)|*.Theme.vth");
            if ((fn == ""))
                return;
            try
            {
                string themeStr = System.IO.File.ReadAllText(fn);
                string[] themeVals = themeStr.Split('|');

                Theme.ThemeColorsType themeColours = new Theme.ThemeColorsType();
                themeColours.ColorDark = themeVals[0];
                themeColours.ColorMedium = themeVals[1];
                themeColours.ColorLight = themeVals[2];
                themeColours.ColorPanelAlpha = themeVals[3];
                themeColours.ColorSelected = themeVals[4];
                themeColours.ColorHighlight = themeVals[5];
                themeColours.ColorHeaderText = themeVals[6];
                themeColours.ColorLabelText = themeVals[7];
                themeColours.ColorActiveText = themeVals[8];
                themeColours.ColorInactiveText = themeVals[9];
                themeColours.ColorControlBorder = themeVals[10];
                themeColours.ColorControlHighlightBorder = themeVals[11];
                themeColours.ColorPositiveText = themeVals[12];
                themeColours.ColorNegativeText = themeVals[13];

                // check the other themes, we do not want a duplication
                int i;
                for (i = 0; i <= themeFiles.Count - 1; i++)
                {
                    if ((themeFiles[i]._source != "Saved To Settings"))
                    {
                        if ((Theme.compareThemeColours(themeFiles[i]._themeColors, themeColours)))
                        {
                            MessageBox.Show("You appear to already have a theme with the same colours as the selected file called '" + themeFiles[i]._name + "'." + Constants.vbNewLine + Constants.vbNewLine + "The import process was cancelled", "Import Cancelled", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                    }
                }
                string destfn = System.IO.Path.Combine(MainWindow.appDataPath, "Themes", themeFileCount + ".Theme.vth");

                if ((System.IO.File.Exists(destfn)))
                {
                    MessageBox.Show("Fatal Error: A theme file already exists in the location but this should have been recognised already!" + Constants.vbNewLine + Constants.vbNewLine + fn, "Import Cancelled", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                Theme.selectedThemeColours = themeColours;
                System.IO.File.Copy(fn, destfn);
            }
            catch
            {
                MessageBox.Show("Something went wrong importing the theme file", "Import Cancelled", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            refreshThemeList();
        }

        private void btnDeleteThemeFile_Click(object sender, RoutedEventArgs e)
        {
            if ((MessageBox.Show("Are you sure you want to delete the selected theme file?", "Confirm Delete", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel))
                return;
            themeFileType themeFile = themeFiles[System.Convert.ToInt32(lstThemesData.SelectedListItem.id)];
            System.IO.File.Delete(themeFile._fn);
            refreshThemeList();
        }

        private void cancel()
        {
            if ((mainWindow.canLoseChanges(this) == false))
                return;
            SettingsManager.Settings.AnimationSpeed = initialanimSpeed;
            changesMade = false;
            Theme.applyTheme(mainWindow);
            popUpObj.closePopUp(null, null);
        }
        private void setThemeEditorAsSelectedTheme()
        {
            changesMade = false;
            Theme.selectedThemeColours.ColorDark = Theme.getResourceVal(mainWindow, "ColorDark");
            Theme.selectedThemeColours.ColorMedium = Theme.getResourceVal(mainWindow, "ColorMedium");
            Theme.selectedThemeColours.ColorLight = Theme.getResourceVal(mainWindow, "ColorLight");
            Theme.selectedThemeColours.ColorPanelAlpha = Theme.getResourceVal(mainWindow, "ColorPanelAlpha");
            Theme.selectedThemeColours.ColorSelected = Theme.getResourceVal(mainWindow, "ColorSelected");
            Theme.selectedThemeColours.ColorHighlight = Theme.getResourceVal(mainWindow, "ColorHighlight");
            Theme.selectedThemeColours.ColorHeaderText = Theme.getResourceVal(mainWindow, "ColorHeaderText");
            Theme.selectedThemeColours.ColorLabelText = Theme.getResourceVal(mainWindow, "ColorLabelText");
            Theme.selectedThemeColours.ColorActiveText = Theme.getResourceVal(mainWindow, "ColorActiveText");
            Theme.selectedThemeColours.ColorInactiveText = Theme.getResourceVal(mainWindow, "ColorInactiveText");
            Theme.selectedThemeColours.ColorControlBorder = Theme.getResourceVal(mainWindow, "ColorControlBorder");
            Theme.selectedThemeColours.ColorControlHighlightBorder = Theme.getResourceVal(mainWindow, "ColorControlHighlightBorder");
            Theme.selectedThemeColours.ColorPositiveText = Theme.getResourceVal(mainWindow, "ColorPositiveText");
            Theme.selectedThemeColours.ColorNegativeText = Theme.getResourceVal(mainWindow, "ColorNegativeText");
        }
        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            setThemeEditorAsSelectedTheme();

            SettingsManager.Settings.AnimationSpeed = int.Parse(comboAnimationSpeedData.SelectedComboItem.Value);
            popUpObj.closePopUp(fadeCompleted, null);
        }

        private string selectedThemeColoursString()
        {
            string colorStr = "";
            colorStr = colorStr + Theme.selectedThemeColours.ColorDark + "|";
            colorStr = colorStr + Theme.selectedThemeColours.ColorMedium + "|";
            colorStr = colorStr + Theme.selectedThemeColours.ColorLight + "|";
            colorStr = colorStr + Theme.selectedThemeColours.ColorPanelAlpha + "|";
            colorStr = colorStr + Theme.selectedThemeColours.ColorSelected + "|";
            colorStr = colorStr + Theme.selectedThemeColours.ColorHighlight + "|";
            colorStr = colorStr + Theme.selectedThemeColours.ColorHeaderText + "|";
            colorStr = colorStr + Theme.selectedThemeColours.ColorLabelText + "|";
            colorStr = colorStr + Theme.selectedThemeColours.ColorActiveText + "|";
            colorStr = colorStr + Theme.selectedThemeColours.ColorInactiveText + "|";
            colorStr = colorStr + Theme.selectedThemeColours.ColorControlBorder + "|";
            colorStr = colorStr + Theme.selectedThemeColours.ColorControlHighlightBorder + "|";
            colorStr = colorStr + Theme.selectedThemeColours.ColorPositiveText + "|";
            colorStr = colorStr + Theme.selectedThemeColours.ColorNegativeText;
            return colorStr;
        }
        public void fadeCompleted()
        {
            // save options to database
            Theme.selectedTheme = Theme.loadedTheme;

            Theme.selectedTheme = -1; // this is no longer needed once the user has saved their own custom colours, reset to dark theme
            SettingsManager.Settings.SelectedTheme = Theme.selectedTheme;
            SettingsManager.Settings.ThemeColours = selectedThemeColoursString();
            SettingsManager.Save();
        }

        private void btnTestAnimations_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.Settings.AnimationSpeed = int.Parse(comboAnimationSpeedData.SelectedComboItem.Value);
            mainWindow.enablePopUp(false);
            objectAnimations.makeDisappear(mainWindow, popUpObj.window, false, this, testAnim_FadeOut_Completed);
        }

        private void testAnim_FadeOut_Completed()
        {
            objectAnimations.makeAppear(ref mainWindow, popUpObj.window, false, this, testAnim_Completed);
        }

        private void testAnim_Completed()
        {
            mainWindow.enablePopUp(true);
            SettingsManager.Settings.AnimationSpeed = initialanimSpeed;
        }

        private void Color_SelectedColorChanged(object sender, Controls.ColorPicker.ColorChangedEventArgs e)
        {
            Theme.overwriteResource(mainWindow, ((dynamic)sender).Name, e.SelectedColor.ToString());
            fieldWasChanged(sender);
        }

        private void Color_PreviewSelectedColorChanged(object sender, Controls.ColorPicker.ColorChangedEventArgs e)
        {
            Theme.overwriteResource(mainWindow, ((dynamic)sender).Name, e.SelectedColor.ToString());
            DockHandler.ApplyThemeColorsToOpenWindows(Theme.getThemeColorsFromWindowResources(mainWindow));
        }
    }

}
