using Fontendo;
using Fontendo.UI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

public class listViewColumnHeaderDataType : IComparable<listViewColumnDataType>
{
    private string? _filter = "";
    public string? filter
    {
        get
        {
            return _filter;
        }
        set
        {
            _filter = value;
        }
    }

    private bool _filterInclude = false;
    public bool filterInclude
    {
        get
        {
            return _filterInclude;
        }
        set
        {
            _filterInclude = value;
        }
    }

    private bool _filterExact = false;
    public bool filterExact
    {
        get
        {
            return _filterExact;
        }
        set
        {
            _filterExact = value;
        }
    }

    private string _data = "";
    public string data
    {
        get
        {
            return _data;
        }
        set
        {
            _data = value;
        }
    }

    public listViewColumnHeaderDataType(string dataStr)
    {
        data = dataStr;
    }

    public int CompareTo(listViewColumnDataType? other)
    {
        if (this.data == null)
            return -1;
        if (other == null)
            return 1;
        // Compare string.
        return this.data.CompareTo(other?.data);
    }
}

public class listViewColumnDataType : IComparable<listViewColumnDataType>
{
    private string? _data;
    public string? data
    {
        get
        {
            return _data;
        }
        set
        {
            if (value != _data)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(data)));
                _data = value;
            }
        }
    }

    private listViewItemDataType parent;
    public listViewColumnDataType(listViewItemDataType parentData, string dataStr)
    {
        parent = parentData;
        data = dataStr;
        HasFilterResult = false;
    }

    public bool _showing = true;
    public bool HasFilterResult
    {
        get
        {
            return _showing;
        }
        set
        {
            if (value != _showing)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasFilterResult)));
                _showing = value;
            }
        }
    }


    public int CompareTo(listViewColumnDataType? other)
    {
        if (this.data == null)
            return -1;
        if (other == null)
            return -1;

        if ((this.parent.columnsWithFilterResultsCount() == this.parent.columnsWithFiltersCount()))
            // compare string
            return this.data.CompareTo(other?.data);
        else
            // item must be filtered out already
            return -1;
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        try
        {
            PropertyChanged?.Invoke(this, e);
        }
        catch
        {
        }
    }
}

public class listViewItemDataType : INotifyPropertyChanged
{
    public listViewDataType parent;

    private string _id = "";
    public string id
    {
        get
        {
            return _id;
        }
        set
        {
            if (value != _id)
            {
                _id = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(id)));
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
            if (value != _icon)
            {
                _icon = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(icon)));
            }
        }
    }

    private ImageSource? _icon2;
    public ImageSource? icon2
    {
        get
        {
            return _icon2;
        }
        set
        {
            if (value != _icon2)
            {
                _icon2 = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(icon2)));
            }
        }
    }

    private string _iconColour = "";
    public string iconColour
    {
        get
        {
            return _iconColour;
        }
        set
        {
            if (value != _iconColour)
            {
                _iconColour = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(iconColour)));
            }
        }
    }

    private bool _isChecked = false;
    public bool isChecked
    {
        get
        {
            return _isChecked;
        }
        set
        {
            if (value != _isChecked)
            {
                _isChecked = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(isChecked)));
            }
        }
    }
    public bool icon_showing = true;
    public bool icon_HasFilterResult
    {
        get
        {
            return icon_showing;
        }
        set
        {
            if (value != icon_showing)
            {
                icon_showing = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(icon_HasFilterResult)));
            }
        }
    }
    public bool icon2_showing = true;
    public bool icon2_HasFilterResult
    {
        get
        {
            return icon2_showing;
        }
        set
        {
            if (value != icon2_showing)
            {
                icon2_showing = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(icon2_HasFilterResult)));
            }
        }
    }
    public bool colour_showing = true;
    public bool colour_HasFilterResult
    {
        get
        {
            return colour_showing;
        }
        set
        {
            if (value != colour_showing)
            {
                colour_showing = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(colour_HasFilterResult)));
            }
        }
    }
    public object tag;
    public object Tag
    {
        get
        {
            return tag;
        }
        set
        {
            if (value != tag)
            {
                tag = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Tag)));
            }
        }
    }

    private List<listViewColumnDataType> _values = new List<listViewColumnDataType>();

    public List<listViewColumnDataType> SubItems
    {
        get
        {
            return _values;
        }
        set
        {
            if (value != _values)
            {
                _values = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(SubItems)));
            }
        }
    }

    public int columnsWithFilterResultsCount()
    {
        int i;
        int result = 0;
        for (i = 0; i <= SubItems.Count - 1; i++)
        {
            if ((SubItems[i].HasFilterResult == true))
                result = result + 1;
        }
        return result;
    }
    public int columnsWithFiltersCount()
    {
        int i;
        int result = 0;
        for (i = 0; i <= parent.Columns.Count - 1; i++)
        {
            if ((parent.Columns[i].filter != ""))
                result = result + 1;
        }
        return result;
    }

    public listViewItemDataType(listViewDataType parentData, string text, string value)
    {
        id = value;
        parent = parentData;
        SubItems.Add(new listViewColumnDataType(this, text));
    }

    public listViewItemDataType()
    {
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        try
        {
            PropertyChanged?.Invoke(this, e);
        }
        catch
        {
        }
    }
}

public interface IColumnInterface
{
    event EventHandler CollectionChanged;
}

public class listViewDataType : INotifyPropertyChanged
{
    public UI_MainWindow? mainWindow;
    public GridViewColumn? _lastColumnHeaderClicked = null;
    public ListSortDirection _lastSortDirection = ListSortDirection.Ascending;

    public event PropertyChangedEventHandler? PropertyChanged;
    private object _syncLock = new object();
    private string outFn = "";

    private List<listViewColumnHeaderDataType> _columns = new List<listViewColumnHeaderDataType>();
    public List<listViewColumnHeaderDataType> Columns
    {
        get
        {
            return _columns;
        }
        set
        {
            _columns = value;
        }
    }
    public ListView? listview;
    private SelectionMode? selectMode;
    public listViewDataType(UI_MainWindow? mainWin, ref ListView list)
    {
        Items = new ObservableCollection<listViewItemDataType>();
        mainWindow = mainWin;
        loadListView(ref list);
    }

    public void loadListView(ref ListView lst)
    {
        if ((lst == null))
            return;
        listview = lst;
        selectMode = listview.SelectionMode;
        GridView grid = (GridView)lst.View;
        int i;
        Columns = new List<listViewColumnHeaderDataType>();
        for (i = 0; i <= grid.Columns.Count - 1; i++)
        {
            if (grid.Columns[i].Header == null)
                continue;
            if (grid.Columns[i].Header.ToString() != "")
                Columns.Add(new listViewColumnHeaderDataType(grid.Columns[i].Header.ToString() ?? ""));
        }
        lst.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(gridViewHelper.GridViewColumnResized), true);
        lst.AddHandler(ListView.ContextMenuOpeningEvent, new ContextMenuEventHandler(lst_ContextMenuOpening), true);
    }

    public void AddItem(listViewItemDataType item)
    {
        Items?.Add(item);
        if (!(SelectedListItemHistory == null))
        {
            if ((SelectedListItemHistory.id == item.id))
                SelectedListItem = Items?[Items.Count - 1];
        }
    }
    public void ApplySelectedItem()
    {
        if (SelectedListItemHistory != null)
        {
            if (Items != null)
            {
                foreach (listViewItemDataType item in Items)
                {
                    if ((SelectedListItemHistory.id == item.id))
                    {
                        SelectedListItem = item;
                        listview?.ScrollIntoView(item);
                        return;
                    }
                }
            }
        }
    }

    private ObservableCollection<listViewItemDataType>? _listItems;
    public ObservableCollection<listViewItemDataType>? Items
    {
        get
        {
            return _listItems;
        }
        set
        {
            _listItems = value;
            BindingOperations.EnableCollectionSynchronization(Items, _syncLock);
            OnPropertyChanged(new PropertyChangedEventArgs("Items"));
        }
    }

    private listViewItemDataType? _selectedItem;
    public listViewItemDataType? SelectedListItem
    {
        get
        {
            return _selectedItem;
        }
        set
        {
            _selectedItem = value;
            if (!(value == null) & (selectMode == SelectionMode.Single))
                SelectedListItemHistory = value;
            OnPropertyChanged(new PropertyChangedEventArgs("SelectedListItem"));
        }
    }

    private listViewItemDataType? _historicalSelectedItem;
    public listViewItemDataType? SelectedListItemHistory
    {
        get
        {
            return _historicalSelectedItem;
        }
        set
        {
            _historicalSelectedItem = value;

            OnPropertyChanged(new PropertyChangedEventArgs("SelectedListItemHistory"));
        }
    }

    public void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        try
        {
            PropertyChanged?.Invoke(this, e);
        }
        catch
        {
        }
    }

    public enum exportTypes
    {
        excel,
        raw_csv,
        encrypted_csv
    }

    private exportTypes exportType;
    private string reportName = "";
    private List<int> columnsToExportId = new List<int>();
    private GridViewColumn? columnClicked;
    private void lst_ContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        listViewItemDataType? itemClicked = null;
        if ((getItemClicked(mainWindow, ref itemClicked, sender, e) == false))
        {
            e.Handled = true;
            return;
        }
    }
    private void btnSortColumnASC_Click(object sender, RoutedEventArgs e)
    {
        if (listview == null || columnClicked == null)
            return;
        gridViewHelper.grid_Sorting(listview, (listViewDataType)listview.DataContext, ListSortDirection.Ascending, null, columnClicked);
        gridViewHelper.updateGridViewColumnHeaderStyle(((listViewDataType)listview.DataContext).mainWindow, (listViewDataType)listview.DataContext, ListSortDirection.Ascending, columnClicked);
    }
    private void btnSortColumnDESC_Click(object sender, RoutedEventArgs e)
    {
        gridViewHelper.grid_Sorting(listview, (listViewDataType?)listview?.DataContext, ListSortDirection.Descending, null, columnClicked);
        gridViewHelper.updateGridViewColumnHeaderStyle(((listViewDataType?)listview?.DataContext)?.mainWindow, (listViewDataType?)listview?.DataContext, ListSortDirection.Descending, columnClicked);
    }
    private void btnFilter_Click(object sender, RoutedEventArgs e)
    {
        if (columnClicked == null || listview == null)
            throw new Exception("columnClicked or listview are null");
        listViewDataType data = (listViewDataType)listview.DataContext;
        string? currentFilter = data.Columns[gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data)].filter;
        if ((!data.Columns[gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data)].filterInclude | !data.Columns[gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data)].filterExact))
            currentFilter = "";
        dynamic pop = new popUpChangeEntry(data.mainWindow, columnClicked.Header.ToString() ?? "", currentFilter, 20, data.mainWindow, data.mainWindow, new popUpChangeEntry.completedFunctionDelegate(btnFilter_Complete));
        popUps.loadPopUp(data.mainWindow, "Filter - Include Value", "filter.png", ref pop);
    }

    public void btnFilter_Complete(object? result)
    {
        if ((result == null))
            return;
        if (columnClicked == null || listview == null)
            throw new Exception("columnClicked or listview are null");
        listViewDataType data = (listViewDataType)listview.DataContext;
        int colId = gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data);
        data.Columns[colId].filter = result.ToString() ?? "";
        data.Columns[colId].filterInclude = true;
        data.Columns[colId].filterExact = false;
        ICollectionView view = CollectionViewSource.GetDefaultView(listview.ItemsSource);

        view.Filter = new Predicate<object>(ColumnFilter);
        if ((result.ToString() == ""))
            applyFilterColumnHeaderStyle((listViewDataType)listview.DataContext, false);
        else
            applyFilterColumnHeaderStyle((listViewDataType)listview.DataContext, true);
        ApplySelectedItem();
    }
    private void btnFilterExclude_Click(object sender, RoutedEventArgs e)
    {
        if (columnClicked == null || listview == null)
            throw new Exception("columnClicked or listview are null");
        listViewDataType data = (listViewDataType)listview.DataContext;
        string currentFilter = data.Columns[gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data)].filter ?? "";
        if ((data.Columns[gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data)].filterInclude | data.Columns[gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data)].filterExact))
            currentFilter = "";
        dynamic pop = new popUpChangeEntry(data.mainWindow, columnClicked.Header.ToString() ?? "", currentFilter, 20, data.mainWindow, data.mainWindow, new popUpChangeEntry.completedFunctionDelegate(btnFilterExclude_Complete));
        popUps.loadPopUp(data.mainWindow, "Filter - Exclude Value", "filter.png", ref pop);
    }

    public void btnFilterExclude_Complete(object? result)
    {
        if (result == null)
            return;
        if (columnClicked == null || listview == null)
            throw new Exception("columnClicked or listview are null");
        listViewDataType data = (listViewDataType)listview.DataContext;
        int colId = gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data);
        data.Columns[colId].filter = result.ToString() ?? "";
        data.Columns[colId].filterInclude = false;
        data.Columns[colId].filterExact = false;
        ICollectionView view = CollectionViewSource.GetDefaultView(listview.ItemsSource);

        view.Filter = new Predicate<object>(ColumnFilter);
        if ((result.ToString() == ""))
            applyFilterColumnHeaderStyle((listViewDataType)listview.DataContext, false);
        else
            applyFilterColumnHeaderStyle((listViewDataType)listview.DataContext, true);
        ApplySelectedItem();
    }

    private void btnFilterExact_Click(object? sender, RoutedEventArgs e)
    {
        if (columnClicked == null || listview == null)
            throw new Exception("columnClicked or listview are null");
        listViewDataType data = (listViewDataType)listview.DataContext;
        string currentFilter = data.Columns[gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data)].filter ?? "";
        if ((!data.Columns[gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data)].filterInclude | !data.Columns[gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data)].filterExact))
            currentFilter = "";
        dynamic pop = new popUpChangeEntry(data.mainWindow, columnClicked.Header.ToString() ?? "", currentFilter, 20, data.mainWindow, data.mainWindow, new popUpChangeEntry.completedFunctionDelegate(btnFilterExact_Complete));
        popUps.loadPopUp(data.mainWindow, "Filter - Include Exact Value", "filter.png", ref pop);
    }

    public void btnFilterExact_Complete(object? result)
    {
        if ((result == null))
            return;
        if (columnClicked == null || listview == null)
            throw new Exception("columnClicked or listview are null");
        listViewDataType data = (listViewDataType)listview.DataContext;
        int colId = gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data);
        data.Columns[colId].filter = result.ToString();
        data.Columns[colId].filterInclude = true;
        data.Columns[colId].filterExact = true;
        ICollectionView view = CollectionViewSource.GetDefaultView(listview.ItemsSource);

        view.Filter = new Predicate<object>(ColumnFilter);
        if ((result.ToString() == ""))
            applyFilterColumnHeaderStyle((listViewDataType)listview.DataContext, false);
        else
            applyFilterColumnHeaderStyle((listViewDataType)listview.DataContext, true);
        ApplySelectedItem();
    }
    private void btnFilterExcludeExact_Click(object sender, RoutedEventArgs e)
    {
        if (columnClicked == null || listview == null)
            throw new Exception("columnClicked or listview are null");
        listViewDataType data = (listViewDataType)listview.DataContext;
        string currentFilter = data.Columns[gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data)].filter ?? "";
        if ((data.Columns[gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data)].filterInclude | !data.Columns[gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data)].filterExact))
            currentFilter = "";
        dynamic pop = new popUpChangeEntry(data.mainWindow, columnClicked.Header.ToString() ?? "", currentFilter, 20, data.mainWindow, data.mainWindow, new popUpChangeEntry.completedFunctionDelegate(btnFilterExcludeExact_Complete));
        popUps.loadPopUp(data.mainWindow, "Filter - Exclude Exact Value", "filter.png", ref pop);
    }

    public void btnFilterExcludeExact_Complete(object? result)
    {
        if ((result == null))
            return;
        if (columnClicked == null || listview == null)
            throw new Exception("columnClicked or listview are null");
        listViewDataType data = (listViewDataType)listview.DataContext;
        int colId = gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data);
        data.Columns[colId].filter = result.ToString() ?? "";
        data.Columns[colId].filterInclude = false;
        data.Columns[colId].filterExact = true;
        ICollectionView view = CollectionViewSource.GetDefaultView(listview.ItemsSource);

        view.Filter = new Predicate<object>(ColumnFilter);
        if ((result.ToString() == ""))
            applyFilterColumnHeaderStyle((listViewDataType)listview.DataContext, false);
        else
            applyFilterColumnHeaderStyle((listViewDataType)listview.DataContext, true);
        ApplySelectedItem();
    }

    public void resetItemShowingState(int column)
    {
        int i;
        if (Items == null)
            return;
        for (i = 0; i <= Items.Count - 1; i++)
            Items[i].SubItems[column].HasFilterResult = false;
    }

    public void removeColumnFilter(UI_MainWindow mainWin, int colId)
    {
        if ((mainWin == null))
            return;
        if (columnClicked == null || listview == null)
            throw new Exception("columnClicked or listview are null");
        listViewDataType? data = (listViewDataType)listview.DataContext;
        columnClicked = gridViewHelper.getColumn(data.Columns[colId].data, data);

        ICollectionView view = CollectionViewSource.GetDefaultView(listview.ItemsSource);
        if ((view == null))
            return;
        view.Filter = null;
        data.Columns[colId].filter = "";
        if (!(data._lastColumnHeaderClicked == null))
        {
            if ((data._lastColumnHeaderClicked.Header == columnClicked?.Header))
            {
                // we are already sorting by this column, apply the sort style
                if ((data._lastSortDirection == ListSortDirection.Ascending))
                {
                    columnClicked.HeaderContainerStyle = (System.Windows.Style)mainWin.FindResource("GridViewColumnHeaderStyleSortASC");
                    return;
                }
                else
                {
                    columnClicked.HeaderContainerStyle = (System.Windows.Style)mainWin.FindResource("GridViewColumnHeaderStyleSortDESC");
                    return;
                }
            }
        }
        if (!(columnClicked?.HeaderContainerStyle == null))
            columnClicked.HeaderContainerStyle = (System.Windows.Style)mainWin.FindResource("GridViewColumnHeaderStyle");
    }


    public void removeColumnFilters(UI_MainWindow mainWin, ListView list)
    {
        listViewDataType data = (listViewDataType)list.DataContext;
        int i;
        for (i = 0; i <= data.Columns.Count - 1; i++)
            removeColumnFilter(mainWin, i);
    }

    private bool SetHasFilterResult(ref bool hasFilterResult, listViewItemDataType data, string filter, string haystackStr, int colId)
    {
        int stringCount = 0;
        if (string.IsNullOrEmpty(filter))
        {
            hasFilterResult = false;
            return data.columnsWithFilterResultsCount() == data.columnsWithFiltersCount();
        }
        else
        {
            stringCount = 1;
        }

        if (data.parent.Columns[colId].filterExact)
        {
            if (string.Equals(filter, haystackStr, StringComparison.OrdinalIgnoreCase))
            {
                hasFilterResult = data.parent.Columns[colId].filterInclude;
                return data.columnsWithFilterResultsCount() == data.columnsWithFiltersCount();
            }
            else
            {
                if (double.TryParse(filter, out double doubleVal))
                {
                    if (double.TryParse(haystackStr, out double doubleColVal))
                    {
                        if (doubleColVal == doubleVal)
                        {
                            hasFilterResult = data.parent.Columns[colId].filterInclude;
                            return data.columnsWithFilterResultsCount() == data.columnsWithFiltersCount();
                        }
                        else
                        {
                            hasFilterResult = !data.parent.Columns[colId].filterInclude;
                            return data.columnsWithFilterResultsCount() == data.columnsWithFiltersCount();
                        }
                    }
                }

                hasFilterResult = !data.parent.Columns[colId].filterInclude;
                return data.columnsWithFilterResultsCount() == data.columnsWithFiltersCount();
            }
        }

        if (haystackStr.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            hasFilterResult = data.parent.Columns[colId].filterInclude;
            return data.columnsWithFilterResultsCount() == data.columnsWithFiltersCount();
        }
        else if (!data.parent.Columns[colId].filterInclude)
        {
            hasFilterResult = true;
            return data.columnsWithFilterResultsCount() == data.columnsWithFiltersCount();
        }

        int foundCount = 0;
        if (filter.Contains("*"))
        {
            stringCount = 0;
            string[] strings = filter.Split('*');
            foreach (string filterVal in strings)
            {
                if (!string.IsNullOrEmpty(filterVal))
                {
                    stringCount++;
                    if (haystackStr.IndexOf(filterVal, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        foundCount++;
                        if (foundCount == strings.Length)
                        {
                            hasFilterResult = data.parent.Columns[colId].filterInclude;
                            return data.columnsWithFilterResultsCount() == data.columnsWithFiltersCount();
                        }
                    }
                }
            }
        }

        if (foundCount == stringCount)
        {
            hasFilterResult = data.parent.Columns[colId].filterInclude;
            return data.columnsWithFilterResultsCount() == data.columnsWithFiltersCount();
        }

        hasFilterResult = false;
        return data.columnsWithFilterResultsCount() == data.columnsWithFiltersCount();
    }

    private bool ColumnFilter(object item)
    {
        if (columnClicked == null)
            return false;
        listViewItemDataType data = (listViewItemDataType)item;
        int colId = gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data.parent);
        if (colId == -1)
        {
            return true;
        }
        int subItemId = gridViewHelper.getColumnSubItemID(columnClicked.Header.ToString() ?? "", data.parent);
        string filter = data.parent.Columns[colId].filter ?? "";
        if (string.IsNullOrEmpty(filter))
        {
            var subItem = data.SubItems[subItemId];
            subItem.HasFilterResult = false;

            return data.columnsWithFilterResultsCount() == data.columnsWithFiltersCount();
        }

        bool isIcon = false;
        string? str = "";
        object? objectToCheck = null;

        if (columnClicked.Header.ToString() == "Icon")
        {
            str = data.icon?.ToString();
            isIcon = true;
            objectToCheck = data.icon;
        }
        else if (columnClicked.Header.ToString() == "Icon2")
        {
            str = data.icon2?.ToString();
            isIcon = true;
            objectToCheck = data.icon2;
        }
        else if (columnClicked.Header.ToString() == "IconColour")
        {
            str = data.iconColour;
            isIcon = true;
            objectToCheck = data.iconColour;
        }
        else
        {
            str = data.SubItems[subItemId].data ?? "";
            objectToCheck = data.SubItems[subItemId];
        }
        if (str == null)
        {
            str = "";
        }
        if (!isIcon)
        {
            var subItem = data.SubItems[subItemId];
            bool result = SetHasFilterResult(ref subItem._showing, data, filter, str, colId);
            return data.columnsWithFilterResultsCount() == data.columnsWithFiltersCount();
        }
        else
        {
            if (columnClicked.Header.ToString() == "Icon")
            {
                bool result = SetHasFilterResult(ref data.icon_showing, data, filter, str, colId);
                return data.columnsWithFilterResultsCount() == data.columnsWithFiltersCount();
            }
            else if (columnClicked.Header.ToString() == "Icon2")
            {
                bool result = SetHasFilterResult(ref data.icon2_showing, data, filter, str, colId);
                return data.columnsWithFilterResultsCount() == data.columnsWithFiltersCount();
            }
            else if (columnClicked.Header.ToString() == "IconColour")
            {
                bool result = SetHasFilterResult(ref data.colour_showing, data, filter, str, colId);
                return data.columnsWithFilterResultsCount() == data.columnsWithFiltersCount();
            }
        }

        return false;
    }

    public void applyFilterColumnHeaderStyle(listViewDataType data, bool filtered)
    {
        if (columnClicked == null || mainWindow == null)
            return;
        string filter = "";
        if ((filtered))
            filter = "Filter";
        // apply the style
        bool needDoubleStyle = false;
        if (!(data._lastColumnHeaderClicked == null))
        {
            if ((data._lastColumnHeaderClicked.Header == columnClicked.Header))
                // we are already sorting by this column, apply the sort + filter style
                needDoubleStyle = true;
        }
        if ((needDoubleStyle))
        {
            // we are not sorting by this column, just apply the filter style
            if ((data._lastSortDirection == ListSortDirection.Ascending))
                columnClicked.HeaderContainerStyle = (System.Windows.Style)mainWindow.FindResource("GridViewColumnHeaderStyleSortASC" + filter);
            else
                columnClicked.HeaderContainerStyle = (System.Windows.Style)mainWindow.FindResource("GridViewColumnHeaderStyleSortDESC" + filter);
        }
        else
            // we are not sorting by this column, just apply the filter style
            columnClicked.HeaderContainerStyle = (System.Windows.Style)mainWindow.FindResource("GridViewColumnHeaderStyle" + filter);
    }

    private void btnClearFilters_Click(object sender, RoutedEventArgs e)
    {
        if (columnClicked == null || listview == null)
            return;
        listViewDataType data = (listViewDataType)listview.DataContext;
        int colId = gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data);
        Columns[colId].filter = "";
        resetItemShowingState(colId);
        ICollectionView view = CollectionViewSource.GetDefaultView(listview.ItemsSource);
        view.Filter = new Predicate<object>(ColumnFilter);
        applyFilterColumnHeaderStyle(data, false);
    }


    private void buildHeaderContextMenu(UI_MainWindow mainWin, object sender, ContextMenuEventArgs e)
    {
        mainWindow = mainWin;
        FrameworkElement fe = (FrameworkElement)e.Source;
        fe.ContextMenu = new ContextMenu();

        columnClicked = gridViewHelper.getColumn(((TextBlock)e.OriginalSource).Text, (listViewDataType)((TextBlock)e.OriginalSource).DataContext);
        if ((columnClicked == null))
            App.CustomMessageBox.Show("Could not get the column header object", "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error);

        if ((columnClicked?.Header == null))
            return;

        List<contextMenuHelper.contextMenuData> items = new List<contextMenuHelper.contextMenuData>();
        items.Add(new contextMenuHelper.contextMenuData("", ((TextBlock)e.OriginalSource).Text, null, contextMenuHelper.contextMenuItemType.header));
        items.Add(new contextMenuHelper.contextMenuData("", "", null, contextMenuHelper.contextMenuItemType.splitter));
        items.Add(new contextMenuHelper.contextMenuData("filter.png", "Filter", null, contextMenuHelper.contextMenuItemType.item_coloured_icon));


        items[items.Count - 1].SubItems = new List<contextMenuHelper.contextMenuData>();
        items[items.Count - 1].SubItems?.Add(new contextMenuHelper.contextMenuData("completed.png", "Include Exact Value", new RoutedEventHandler(btnFilterExact_Click), contextMenuHelper.contextMenuItemType.item_coloured_icon));
        items[items.Count - 1].SubItems?.Add(new contextMenuHelper.contextMenuData("recycle.png", "Exclude Exact Value", new RoutedEventHandler(btnFilterExcludeExact_Click), contextMenuHelper.contextMenuItemType.item_coloured_icon));
        items[items.Count - 1].SubItems?.Add(new contextMenuHelper.contextMenuData("completed.png", "Include Matched Value", new RoutedEventHandler(btnFilter_Click), contextMenuHelper.contextMenuItemType.item_coloured_icon));
        items[items.Count - 1].SubItems?.Add(new contextMenuHelper.contextMenuData("recycle.png", "Exclude Matched Value", new RoutedEventHandler(btnFilterExclude_Click), contextMenuHelper.contextMenuItemType.item_coloured_icon));

        listViewDataType data = this;
        if ((data.Columns[gridViewHelper.getColumnID(columnClicked.Header.ToString() ?? "", data)].filter != ""))
            items.Add(new contextMenuHelper.contextMenuData("clear_filter.png", "Clear Filter", new RoutedEventHandler(btnClearFilters_Click), contextMenuHelper.contextMenuItemType.item_coloured_icon));
        items.Add(new contextMenuHelper.contextMenuData("sort_asc.png", "Sort Ascending", new RoutedEventHandler(btnSortColumnASC_Click), contextMenuHelper.contextMenuItemType.item_coloured_icon));
        items.Add(new contextMenuHelper.contextMenuData("sort_desc.png", "Sort Descending", new RoutedEventHandler(btnSortColumnDESC_Click), contextMenuHelper.contextMenuItemType.item_coloured_icon));

        fe.ContextMenu = contextMenuHelper.createContextMenu(mainWin, fe, items);
        fe.ContextMenu = new ContextMenu();
        fe.ContextMenu.Visibility = Visibility.Hidden;
    }


    public bool getItemClicked(UI_MainWindow? mainWin, ref listViewItemDataType? itemClicked, object sender, ContextMenuEventArgs e)
    {
        if (mainWin == null)
            return false;
        if (e.OriginalSource.GetType() != typeof(TextBlock))
            return false;
        if ((((TextBlock)e.OriginalSource).DataContext as listViewItemDataType == null))
        {
            if ((((TextBlock)e.OriginalSource).DataContext as listViewDataType == null))
            {
                e.Handled = true;
                return false;
            }
            else if (!(e.OriginalSource as TextBlock == null))
            {
                buildHeaderContextMenu(mainWin, sender, e);
                e.Handled = true;
                return false;
            }
            else
            {
                // scroll bar or something else clicked
                e.Handled = true;
                return false;
            }
        }
        itemClicked = (listViewItemDataType)((TextBlock)e.OriginalSource).DataContext;
        return true;
    }
    public static void autoResizeListBoxCols(ref ListView lst, ref listViewDataType data)
    {
        int i;
        GridView grid = (GridView)lst.View;
        double height = lst.Height;
        double maxheight = lst.MaxHeight;
        if ((double.IsNaN(lst.Height) == true))
        {
            lst.Height = 9999999999;
            lst.MaxHeight = 9999999999;
            maxheight = double.PositiveInfinity;
        }
        int colId = 0;


        try
        {
            for (i = grid.Columns.Count - data.Columns.Count; i <= grid.Columns.Count - 1; i++)
            {
                if (grid.Columns[i].Header.ToString() != "")
                {
                    if ((double.IsNaN(grid.Columns[i].Width)))
                    {
                        grid.Columns[i].Width = grid.Columns[i].ActualWidth;
                        grid.Columns[i].Width = double.NaN;
                    }
                }
                else
                    grid.Columns[i].Width = 32;
                if ((data.Columns[colId].filter != ""))
                {
                    data.columnClicked = grid.Columns[i];
                    if ((data.Columns[colId].filterExact))
                    {
                        if ((data.Columns[colId].filterInclude))
                            data.btnFilterExact_Complete(data.Columns[colId].filter);
                        else
                            data.btnFilterExcludeExact_Complete(data.Columns[colId].filter);
                    }
                    else if ((data.Columns[colId].filterInclude))
                        data.btnFilter_Complete(data.Columns[colId].filter);
                    else
                        data.btnFilterExclude_Complete(data.Columns[colId].filter);
                }
                colId = colId + 1;
            }
        }
        catch (Exception ex)
        {
            App.CustomMessageBox.Show("Fatal Error resizing columns: " + ex.Message);
        }

        if ((grid.Columns.Count - data.Columns.Count == 2))
        {
            grid.Columns[0].Width = 32;
            grid.Columns[1].Width = 28;
        }
        if ((grid.Columns.Count - data.Columns.Count == 1))
            grid.Columns[0].Width = 32;
        lst.Height = height;
        lst.MaxHeight = maxheight;

        gridViewHelper.GridViewApplyLastSortOrder(lst, data);
    }
}

