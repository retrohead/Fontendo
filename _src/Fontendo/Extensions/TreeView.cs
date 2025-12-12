using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;

namespace Fontendo
{

    public class treeViewItemDataType : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private object _syncLock = new object();

        private bool _top_level = false;
        public bool IsTopLevel
        {
            get
            {
                return _top_level;
            }
            set
            {
                _top_level = value;
            }
        }

        private dynamic? _parent;
        public dynamic? parentItem
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        private string? _id;
        public string? id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(id)));
                }
            }
        }

        private string? _tooltip;
        public string? tooltip
        {
            get
            {
                return _tooltip;
            }
            set
            {
                if (_tooltip != value)
                {
                    _tooltip = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(tooltip)));
                }
            }
        }

        private bool _selected = false;
        public bool IsSelected
        {
            get
            {
                return _selected;
            }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        private bool _checked = false;
        public bool ItemIsChecked
        {
            get
            {
                return _checked;
            }
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(ItemIsChecked)));
                }
            }
        }

        private bool _expanded = false;
        public bool IsExpanded
        {
            get
            {
                return _expanded;
            }
            set
            {
                if (_expanded != value)
                {
                    _expanded = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsExpanded)));
                }
            }
        }

        private ImageSource? _icon;
        public ImageSource? icon
        {
            get
            {
                return _icon;
            }
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(icon)));
                }
            }
        }

        private ImageSource? _icon_expanded;
        public ImageSource? icon_expanded
        {
            get
            {
                return _icon_expanded;
            }
            set
            {
                if (_icon_expanded != value)
                {
                    _icon_expanded = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(icon_expanded)));
                }
            }
        }

        private string? _txt;
        public string? text
        {
            get
            {
                return _txt;
            }
            set
            {
                if (_txt != value)
                {
                    _txt = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(text)));
                }
            }
        }

        private dynamic? _tagObj;
        public dynamic? TagObj
        {
            get
            {
                return _tagObj;
            }
            set
            {
                _tagObj = value;
            }
        }

        private int _itemCount;
        public int ItemCount
        {
            get
            {
                return _itemCount;
            }
            set
            {
                if (_itemCount != value)
                {
                    _itemCount = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(ItemCount)));
                }
            }
        }

        private ObservableCollection<treeViewItemDataType>? _listItems;
        public ObservableCollection<treeViewItemDataType>? Items
        {
            get
            {
                return _listItems;
            }
            set
            {
                if(_listItems != value)
                {
                    if(value?.Count == 0 && _listItems != null)
                    {
                        // items have been reset, need to clear the ID's from the tree collection
                        _parent?.ClearIdsFromList(_listItems);
                    }
                    _listItems = value;
                    ItemCount = _listItems?.Count ?? 0;
                    BindingOperations.EnableCollectionSynchronization(Items, _syncLock);
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Items)));
                }
            }
        }

        public void ClearIdsFromList(ObservableCollection<treeViewItemDataType>? items)
        {
            // ids are not held here, try parent
            _parent?.ClearIdsFromList(items);
        }

        public int data_set_row;
        public treeViewItemDataType(int data_set_row_id)
        {
            data_set_row = data_set_row_id;
            Items = new ObservableCollection<treeViewItemDataType>();
        }

        public void OnPropertyChanged(PropertyChangedEventArgs property)
        {
            PropertyChanged?.Invoke(this, property);
        }

        public static dynamic? getTreeViewItemFromObject(dynamic? obj)
        {
            if ((obj as TextBlock == null) & (obj as Border == null))
            {
                return null;
            }
            TreeViewItem? returnObj = null;
            bool ok = false;
            while ((ok == false))
            {
                obj = VisualTreeHelper.GetParent(obj);
                try
                {
                    returnObj = obj;
                    ok = true;
                    return returnObj;
                }
                catch
                {
                }
            }
            return returnObj;
        }
    }

    public class treeViewDataType : INotifyPropertyChanged
    {
        private TreeView treeView;
        private List<string> idList = new List<string>();
        public void ClearIdsFromList(ObservableCollection<treeViewItemDataType>? items)
        {
            if (items == null)
                return;
            foreach(treeViewItemDataType item in items)
            {
                if(item.id != null && idList.Contains(item.id))
                    idList.Remove(item.id);
                ClearIdsFromList(item.Items);
            }
        }
        public treeViewDataType(TreeView treeViewObj)
        {
            treeView = treeViewObj;
            Items = new ObservableCollection<treeViewItemDataType>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private object _syncLock = new object();

        private int _itemCount;
        public int ItemCount
        {
            get
            {
                return _itemCount;
            }
            set
            {
                if (_itemCount != value)
                {
                    _itemCount = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(ItemCount)));
                }
            }
        }

        private ObservableCollection<treeViewItemDataType>? _listItems;
        public ObservableCollection<treeViewItemDataType>? Items
        {
            get
            {
                return _listItems;
            }
            set
            {
                if (_listItems != value)
                {
                    if (value?.Count == 0 && _listItems != null)
                    {
                        // items have been reset, need to clear the ID's from the tree collection
                        ClearIdsFromList(_listItems);
                    }
                    _listItems = value;
                    BindingOperations.EnableCollectionSynchronization(Items, _syncLock);
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Items)));
                }
            }
        }

        public static void AddItem(TreeView treeView,ref dynamic? parentItem,ref dynamic? item, bool skipIdCheck = false, treeViewDataType? treeData = null, bool overwriteId = false)
        {
            if (parentItem == null)
                return;
            if (item == null)
                return;
            treeViewDataType? treeViewData;
            if (treeData == null)
                treeViewData = (treeViewDataType)treeView.DataContext;
            else
                treeViewData = treeData;
            item.parentItem = parentItem;

            if ((treeViewData?.Items?.Count == 0))
                treeViewData.idList = new List<string>();

            if (skipIdCheck == false && item.id != null)
            {
                if (overwriteId)
                    treeViewData?.idList.Remove(item.id);
                if (treeViewData?.idList.Contains(item.id) ?? false)
                {
                    App.CustomMessageBox.Show("Fatal error: ID '" + item.id + "' already exists in tree view", "Tree View Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                treeViewData?.idList.Add(item.id);
            }
            if (parentItem.GetType() == typeof(treeViewDataType))
                item.IsTopLevel = true;
            else
                item.IsTopLevel = false;

            parentItem.ItemCount = parentItem.ItemCount + 1;
            parentItem.Items.Add(item);

            if (!(treeViewData?.SelectedTreeViewItemHistory == null))
            {
                if (item.id == treeViewData.SelectedTreeViewItemHistory.id)
                {
                    treeViewData.SelectedTreeViewItem = item;
                    item.IsSelected = true;

                    // expand all the parent objects
                    dynamic parent = item;
                    while (!(parent.parentItem == null) && (parentItem.GetType() == typeof(treeViewItemDataType)))
                    {
                        if ((parent.parentItem == null) & (parent.IsTopLevel == false))
                            App.CustomMessageBox.Show("Fatal error: must set the parent item for node with id " + parent.id, "Tree View Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        else
                        {
                            parent.parentItem.IsExpanded = true;
                            parent = parent.parentItem;
                        }
                    }
                }
            }
        }

        private bool findNodeText(ObservableCollection<treeViewItemDataType>? nodes, string search)
        {
            bool result = false;
            int i;
            for (i = 0; i < nodes?.Count; i++)
            {
                treeViewItemDataType? n = nodes[i];
                if(n == null)
                    continue;
                int index = n.text?.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) ?? -1;
                if (index != -1)
                {
                    if (searchedIds.Contains(n.id ?? "") == false)
                    {
                        searchedIds.Add(n.id ?? "");
                        selectNode(ref n);
                        result = true;
                    }
                }

                if ((result == false))
                    result = findNodeText(n.Items, search);
                else
                    break;
            }
            return result;
        }

        private string lastSearchString = "";
        private List<string> searchedIds = new List<string>();

        public bool search(string txt)
        {
            bool result = false;
            if ((txt != lastSearchString))
                searchedIds = new List<string>();
            collapseAll();
            if ((findNodeText(Items, txt) == false))
            {
                if ((searchedIds.Count == 0))
                {
                    App.CustomMessageBox.Show("The search criteria was not found in any nodes", "Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
                    result = false;
                }
                else if ((App.CustomMessageBox.Show("The search criteria reached the end of the available items." + Constants.vbCrLf + Constants.vbCrLf + "Would you like to start at the beginning again?", "Search Ended", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes))
                {
                    treeView.Focus();
                    lastSearchString = "";
                    result = search(txt);
                    return result;
                }
                else
                    result = false;
            }
            else
            {
                result = true;
                treeView.DataContext = null;
                treeView.DataContext = this;
            }
            lastSearchString = txt;
            return result;
        }

        public void selectNode(ref treeViewItemDataType n)
        {
            n.IsSelected = true;
            SelectedTreeViewItem = n;
            dynamic? parent = n.parentItem;
            if(parent != null)
                expandParentNode(ref parent);
            n.parentItem = parent;
        }

        public void expandParentNode(ref dynamic n, bool refresh_items = true)
        {
            if (n.GetType() != typeof(treeViewDataType))
            {
                n.IsExpanded = true;
                if (n.parentItem.GetType() != typeof(treeViewDataType))
                    expandParentNode(ref n.parentItem, false);
            }
            if ((refresh_items))
                refreshItems();
        }

        public void collapseAll()
        {
            collapseSubItems(this, true);
        }

        public void collapseSubItems(dynamic obj, bool refresh_items = true)
        {
            foreach (treeViewItemDataType item in obj.Items)
            {
                item.IsExpanded = false;
                item.IsSelected = false;
                collapseSubItems(item, false);
            }
            if ((refresh_items & obj.Items.Count > 0))
                refreshItems();
        }

        public void refreshItems()
        {
            treeView.DataContext = null;
            treeView.DataContext = this;
        }
        public static CheckBox getCheckboxFromTreeViewItem(TreeViewItem tvi)
        {
            CheckBox chkBox;
            dynamic o = VisualTreeHelper.GetChild(tvi, 0); // grid
            o = VisualTreeHelper.GetChild(o, 1); // border
            o = VisualTreeHelper.GetChild(o, 0); // content presenter
            o = VisualTreeHelper.GetChild(o, 0); // grid

            chkBox = VisualTreeHelper.GetChild(o, 0);
            return chkBox;
        }

        public void checkSubItemsData(ref treeViewItemDataType obj, bool @checked)
        {
            if ((obj == null))
                return;
            // just check the data instead, node has never been expanded
            for (var i = 0; i <= obj.Items?.Count - 1; i++)
            {
                treeViewItemDataType? data = obj.Items?[i];
                if (data != null)
                {
                    data.ItemIsChecked = @checked;
                    checkSubItemsData(ref data, @checked);
                }
            }
        }
        public void checkSubItems(TreeViewItem obj, bool @checked)
        {
            if ((obj == null))
                return;
            var contentPanel = getContainerPanelFromTreeViewItem(obj);

            int i;
            if ((contentPanel == null))
            {
                // just check the data instead, node has never been expanded
                for (i = 0; i <= obj.Items.Count - 1; i++)
                {
                    treeViewItemDataType data = (treeViewItemDataType)obj.Items[i];
                    data.ItemIsChecked = @checked;
                    checkSubItemsData(ref data, @checked);
                }
                return;
            }

            // scan the nodes and change them physically
            for (i = 0; i <= contentPanel.Children.Count - 1; i++)
            {
                CheckBox chkBox = getCheckboxFromTreeViewItem(contentPanel.Children(i));
                chkBox.IsChecked = @checked;
                checkSubItems(contentPanel.Children(i), @checked);
            }
        }
        public static void synchNodeChildCheckStatesToData(TreeViewItem item)
        {
            StackPanel? stackPanel = getContainerPanelFromTreeViewItem(item);
            if ((stackPanel == null))
                return;
            foreach (TreeViewItem tvi in stackPanel.Children)
            {
                CheckBox chkBox = getCheckboxFromTreeViewItem(tvi);
                chkBox.IsChecked = ((treeViewItemDataType)chkBox.DataContext).ItemIsChecked;
            }
        }
        public static TreeViewItem? getNodeFromTreeViewItem(TreeViewItem item, string id)
        {
            StackPanel? stackPanel = getContainerPanelFromTreeViewItem(item);
            if (!(stackPanel == null))
            {
                dynamic? tvi = null;
                // loop through all the sub items
                int childrenC = VisualTreeHelper.GetChildrenCount(stackPanel);

                int i;
                for (i = 0; i <= childrenC - 1; i++)
                {
                    tvi = VisualTreeHelper.GetChild(stackPanel, i);
                    if ((tvi.Header.id == id))
                    {
                        return tvi;
                    }
                    tvi = getNodeFromTreeViewItem(tvi, id);
                    if (!(tvi == null))
                        return tvi;
                }
            }
            return null;
        }

        public static dynamic? getContainerPanelFromTreeViewItem(TreeViewItem tvi)
        {
            dynamic? o = null;
            if ((VisualTreeHelper.GetChildrenCount(tvi) == 0))
            {
                return null;
            }
            o = VisualTreeHelper.GetChild(tvi, 0); // grid
            o = VisualTreeHelper.GetChild(o, 2); // items presenter
            if ((VisualTreeHelper.GetChildrenCount(o) > 0))
                o = VisualTreeHelper.GetChild(o, 0); // stack panel
            else
                o = null;
            return o;
        }

        public static TreeViewItem? getNodeFromCheckBox(TreeView treeView, CheckBox chkBox, string id)
        {
            // get the containing treeviewitem (always top level?)
            TreeViewItem? tvi = (TreeViewItem)treeView.ContainerFromElement(chkBox);
            if ((tvi == null))
                return tvi;

            // if the top level item was clicked then just return it
            if (((treeViewItemDataType)tvi.Header).id == id)
            {
                return tvi;
            }

            tvi = getNodeFromTreeViewItem(tvi, id);
            return tvi;
        }


        private treeViewItemDataType? _selectedItem;
        public treeViewItemDataType? SelectedTreeViewItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (!(_selectedItem == null))
                    _selectedItem.IsSelected = false;
                _selectedItem = value;
                if (!(value == null))
                    SelectedTreeViewItemHistory = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SelectedTreeViewItem"));
            }
        }

        private treeViewItemDataType? _historicalSelectedItem;
        public treeViewItemDataType? SelectedTreeViewItemHistory
        {
            get
            {
                return _historicalSelectedItem;
            }
            set
            {
                _historicalSelectedItem = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SelectedTreeViewItemHistory"));
            }
        }
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }


        /// <summary>
        /// This method is used to prevent the horizontal scroll bar from appearing when a TreeViewItem is selected. Ensure it is called after the TreeView has been populated with items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PreventHorizontalScrollOnSelect()
        {
            // Attach events here once items exist
            foreach (TreeViewItem item in FindVisualChildren<TreeViewItem>(treeView))
            {
                // detach first
                item.RequestBringIntoView -= TreeViewItem_RequestBringIntoView;
                item.Selected -= OnSelected;

                // re-attach event
                item.RequestBringIntoView += TreeViewItem_RequestBringIntoView;
                item.Selected += OnSelected;
            }
        }

        private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {    
            // Ignore re-entrant calls
            if (mSuppressRequestBringIntoView)
                return;

            // Cancel the current scroll attempt
            e.Handled = true;

            // Call BringIntoView using a rectangle that extends into "negative space" to the left of our
            // actual control. This allows the vertical scrolling behaviour to operate without adversely
            // affecting the current horizontal scroll position.
            mSuppressRequestBringIntoView = true;

            TreeViewItem? tvi = sender as TreeViewItem;
            if (tvi != null)
            {
                Rect newTargetRect = new Rect(-1000, 0, tvi.ActualWidth + 1000, tvi.ActualHeight);
                tvi.BringIntoView(newTargetRect);
            }

            mSuppressRequestBringIntoView = false;
        }

        private bool mSuppressRequestBringIntoView;
        private void OnSelected(object sender, RoutedEventArgs e)
        {
            ((TreeViewItem)sender).BringIntoView();

            //e.Handled = true;
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    if (child is T typedChild)
                    {
                        yield return typedChild;
                    }

                    foreach (var descendant in FindVisualChildren<T>(child))
                    {
                        yield return descendant;
                    }
                }
            }
        }

        private ScrollViewer? FindScrollViewer(DependencyObject obj)
        {
            if (obj is null)
                return null;
            if (obj is ScrollViewer viewer)
                return viewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                var result = FindScrollViewer(child);
                if (result != null)
                    return result;
            }

            return null;
        }

    }
}
