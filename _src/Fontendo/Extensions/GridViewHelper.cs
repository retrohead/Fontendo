using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Data;
using System.Windows.Shapes;
using System.Windows.Media;
using Fontendo;
using Fontendo.UI;

public class GridViewColumnData
{
    public int origindex;
    public int index;
    public GridViewColumn? element;
}

public class GridViewColumnBehavior : Behavior<GridView>
{
    private object _syncLock = new object();
    protected override void OnAttached()
    {
        if (AssociatedObject != null)
        {
            if (AssociatedObject.Columns != null)
                InitializeColumns(AssociatedObject.Columns);
            base.OnAttached();
        }
    }

    public ObservableCollection<GridViewColumnData> ColumnsCollection
    {
        get
        {
            return (ObservableCollection<GridViewColumnData>)GetValue(ColumnsCollectionProperty);
        }
        set
        {
            SetValue(ColumnsCollectionProperty, value);
        }
    }

    public static readonly DependencyProperty ColumnsCollectionProperty = DependencyProperty.Register("ColumnsCollection", typeof(ObservableCollection<GridViewColumnData>), typeof(GridViewColumnBehavior), new PropertyMetadata(null, new PropertyChangedCallback(Columns_Changed)));

    private static void Columns_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var b = d as GridViewColumnBehavior;
        if (b == null)
            return;
        b.SetupColumns((ObservableCollection<GridViewColumnData>)e.NewValue);
    }


    public void InitializeColumns(ObservableCollection<GridViewColumn> oldColumns)
    {
        if (oldColumns.Count == 0)
            oldColumns.CollectionChanged += Columns_CollectionChanged;
    }

    public void SetupColumns(ObservableCollection<GridViewColumnData> oldColumns)
    {
        if (oldColumns != null)
            oldColumns.CollectionChanged -= Columns_CollectionChanged;

        if ((ColumnsCollection?.Count ?? 0) == 0)
            return;
        AssociatedObject.Columns.Clear();

        if (ColumnsCollection != null)
        {
            var c = ColumnsCollection?.OrderBy(c => c.index);
            if(c != null)
            foreach (var column in c)
                ColumnsCollection?.Add(column);
        }
        if(oldColumns != null)
            oldColumns.CollectionChanged += Columns_CollectionChanged;
    }

    private void Columns_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        var columnList = AssociatedObject.Columns.Select((c, i) => new
        {
            Index = i,
            Element = c.Header?.ToString()
        }).ToLookup(ci => ci.Element, ci => ci.Index);

        var vtree = AssociatedObject;
        if ((e.Action == NotifyCollectionChangedAction.Add))
        {
            if ((ColumnsCollection == null))
                ColumnsCollection = new ObservableCollection<GridViewColumnData>();
            GridViewColumnData c = new GridViewColumnData();
            c.origindex = ColumnsCollection.Count;
            c.index = ColumnsCollection.Count;
            c.element = (GridViewColumn?)e?.NewItems?[0];
            ColumnsCollection.Add(c);
        }
        foreach (GridViewColumnData c in ColumnsCollection)
            // store the users specified index in the columnList
            c.index = columnList[c?.element?.Header.ToString()].FirstOrDefault();
    }
}

public class gridViewHelper
{
    public static GridViewColumn? getColumn(string? header, listViewDataType? listViewData)
    {
        GridViewColumn? result = null;
        GridView? gridview = (GridView?)listViewData?.listview?.View;
        int i;
        for (i = 0; i <= gridview?.Columns.Count - 1; i++)
        {
            if ((gridview?.Columns[i].Header.ToString() == header))
            {
                result = gridview?.Columns[i];
                break;
            }
        }
        return result;
    }
    public static int getColumnID(string header, listViewDataType listViewData)
    {
        int i;
        for (i = 0; i <= listViewData.Columns.Count - 1; i++)
        {
            if ((listViewData.Columns[i].data == header))
                return i;
        }
        return -1;
    }

    public static int getColumnSubItemID(string header, listViewDataType listViewData)
    {
        if(listViewData == null)
            return -1;
        int adjuster = 0;
        for (int i = 0; i < listViewData.Columns.Count; i++)
        {
            if (listViewData.Columns[i].data.ToLower().StartsWith("icon") ||
                (listViewData.Columns[i].data.ToLower().Contains("check") && listViewData.Columns[i].data.ToLower().Contains("box")))
            {
                adjuster++;
            }
            if (listViewData.Columns[i].data == header)
            {
                return i - adjuster;
            }
        }
        return -1;
    }

    public static void grid_Sorting(ListView? sender, listViewDataType? listViewData, ListSortDirection direction, GridViewColumnHeader? headerClicked = null, GridViewColumn? clm = null)
    {
        if (listViewData == null)
        {
            App.CustomMessageBox.Show("Fatal error, you must provide either listViewData for gridview sorting", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (clm == null)
        {
            if (headerClicked == null)
            {
                App.CustomMessageBox.Show("Fatal error, you must provide either a column header or a column for gridview sorting", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            clm = headerClicked.Column;
        }

        if (clm != null)
        {
            IValueConverter ? converter = null;

            if (clm.DisplayMemberBinding != null)
            {
                Binding? binding = clm.DisplayMemberBinding as Binding;
                if (binding?.Converter != null)
                {
                    converter = binding.Converter;
                }
            }
            else if (clm.CellTemplate != null)
            {
                DataTemplate template = clm.CellTemplate;
                object obj = clm.CellTemplate.LoadContent();

                if (obj is Image)
                {
                    ListCollectionView? lcv = CollectionViewSource.GetDefaultView(sender?.ItemsSource) as ListCollectionView;
                    if (lcv != null)
                    {
                        Binding binding = BindingOperations.GetBinding(obj as DependencyObject, Image.SourceProperty);
                        if (binding?.Path.Path == "icon")
                        {
                            lcv.CustomSort = new ListViewIconComparer(direction);

                            if (sender?.SelectionMode == SelectionMode.Single && listViewData.SelectedListItem != null)
                            {
                                sender.ScrollIntoView(listViewData.SelectedListItem);
                            }
                        }
                        else if (binding?.Path.Path == "icon2")
                        {
                            lcv.CustomSort = new ListViewIcon2Comparer(direction);

                            if (sender?.SelectionMode == SelectionMode.Single && listViewData.SelectedListItem != null)
                            {
                                sender.ScrollIntoView(listViewData.SelectedListItem);
                            }
                        }
                    }
                    return;
                }
                else if (obj is Ellipse)
                {
                    ListCollectionView? lcv = CollectionViewSource.GetDefaultView(sender?.ItemsSource) as ListCollectionView;
                    if (lcv != null)
                    {
                        ImageBrush? imageBrush = (obj as Shape)?.Fill as ImageBrush;
                        if (imageBrush != null)
                        {
                            Binding binding = BindingOperations.GetBinding(imageBrush, ImageBrush.ImageSourceProperty);
                            if (binding?.Path.Path == "icon")
                            {
                                lcv.CustomSort = new ListViewIconComparer(direction);

                                if (sender?.SelectionMode == SelectionMode.Single && listViewData.SelectedListItem != null)
                                {
                                    sender.ScrollIntoView(listViewData.SelectedListItem);
                                }
                            }
                            else if (binding?.Path.Path == "icon2")
                            {
                                lcv.CustomSort = new ListViewIcon2Comparer(direction);

                                if (sender?.SelectionMode == SelectionMode.Single && listViewData.SelectedListItem != null)
                                {
                                    sender.ScrollIntoView(listViewData.SelectedListItem);
                                }
                            }
                        }
                        else
                        {
                            Binding? binding = null;
                            try
                            {
                                binding = BindingOperations.GetBinding(obj as DependencyObject, Shape.FillProperty);
                            }
                            catch (Exception)
                            {
                                // Handle exception if necessary
                            }

                            if (binding?.Path.Path == "iconColour")
                            {
                                lcv.CustomSort = new ListViewIconColourComparer(direction);

                                if (sender?.SelectionMode == SelectionMode.Single && listViewData.SelectedListItem != null)
                                {
                                    sender.ScrollIntoView(listViewData.SelectedListItem);
                                }
                            }
                        }
                    }
                    return;
                }
                else
                {
                    // Standard grid
                    Grid? grid = obj as Grid;
                    TextBlock? textBlock = grid?.Children.OfType<TextBlock>().FirstOrDefault();
                    Binding? binding = BindingOperations.GetBinding(textBlock, TextBlock.TextProperty);

                    if (binding == null)
                    {
                        // Could be a deeper text box
                        var textBlocks = grid?.Children.OfType<TextBlock>().Where(e => BindingOperations.GetBinding(e, TextBlock.TextProperty) != null);
                        if (textBlocks != null && textBlocks.Any())
                        {
                            binding = BindingOperations.GetBinding(textBlocks.First(), TextBlock.TextProperty);
                        }
                    }

                    if (binding?.Converter != null)
                    {
                        converter = binding.Converter;
                    }
                }
            }

            if (clm.Header == null)
            {
                return;
            }

            int dataCol = getColumnSubItemID(clm.Header.ToString() ?? "", listViewData);

            if (converter != null)
            {
                ListCollectionView? lcv = CollectionViewSource.GetDefaultView(sender?.ItemsSource) as ListCollectionView;
                if (lcv != null)
                {
                    lcv.CustomSort = new ListViewSubItemComparer(converter, direction, dataCol);

                    if (sender?.SelectionMode == SelectionMode.Single && listViewData.SelectedListItem != null)
                    {
                        sender.ScrollIntoView(listViewData.SelectedListItem);
                    }
                }
            }
        }
    }

    public static void GridViewApplyLastSortOrder(ListView listView, listViewDataType listViewData)
    {
        if (!(listViewData._lastColumnHeaderClicked == null))
            gridViewHelper.grid_Sorting(listView, listViewData, listViewData._lastSortDirection, null, listViewData._lastColumnHeaderClicked);
    }

    public static void updateGridViewColumnHeaderStyle(UI_MainWindow? MainWindow, listViewDataType? listViewData, ListSortDirection direction, GridViewColumn? column)
    {
        if (MainWindow == null || listViewData == null)
            throw new Exception("Failed to find main form");
        int colId = gridViewHelper.getColumnID(column?.Header.ToString() ?? "", listViewData);
        if (colId == -1)
        {
            return;
        }
        string filter = listViewData.Columns[colId].filter ?? "";
        if (!string.IsNullOrEmpty(filter))
        {
            filter = "Filter";
        }
        string hdr = column?.Header.ToString() ?? "".ToLower();
        if (column != null)
        {
            if (hdr.Replace("icon", "") != hdr)
            {
                // Set the new theme
                if (direction == ListSortDirection.Ascending)
                {
                    column.HeaderContainerStyle = MainWindow.FindResource($"GridViewColumnHeaderStyleHiddenSortASC{filter}") as Style;
                }
                else
                {
                    column.HeaderContainerStyle = MainWindow.FindResource($"GridViewColumnHeaderStyleHiddenSortDESC{filter}") as Style;
                }
            }
            else
            {
                // Set the new theme
                if (direction == ListSortDirection.Ascending)
                {
                    column.HeaderContainerStyle = MainWindow.FindResource($"GridViewColumnHeaderStyleSortASC{filter}") as Style;
                }
                else
                {
                    column.HeaderContainerStyle = MainWindow.FindResource($"GridViewColumnHeaderStyleSortDESC{filter}") as Style;
                }
            }
        }

        // Reset the last column
        if (listViewData._lastColumnHeaderClicked != null && listViewData._lastColumnHeaderClicked.Header.ToString() != column?.Header.ToString())
        {
            colId = gridViewHelper.getColumnID(listViewData._lastColumnHeaderClicked.Header.ToString() ?? "", listViewData);
            filter = listViewData.Columns[colId].filter ?? "";
            if (!string.IsNullOrEmpty(filter))
            {
                filter = "Filter";
            }
            string lastHdr = listViewData._lastColumnHeaderClicked.Header.ToString() ?? "".ToLower();
            if (lastHdr.Replace("icon", "") != lastHdr)
            {
                listViewData._lastColumnHeaderClicked.HeaderContainerStyle = MainWindow.FindResource($"GridViewColumnHeaderStyleHidden{filter}") as Style;
            }
            else
            {
                listViewData._lastColumnHeaderClicked.HeaderContainerStyle = MainWindow.FindResource($"GridViewColumnHeaderStyle{filter}") as Style;
            }
        }

        listViewData._lastColumnHeaderClicked = column;
        listViewData._lastSortDirection = direction;
    }


    public static void GridViewColumnResized(object sender, DragDeltaEventArgs e)
    {
        if ((e.OriginalSource as Thumb == null))
            return;
        if ((((Thumb)e.OriginalSource).TemplatedParent as GridViewColumnHeader == null))
            return;
        GridViewColumnHeader header = (GridViewColumnHeader)((Thumb)e.OriginalSource).TemplatedParent;
        if ((header == null))
            return;
        if ((header.Content == null))
            header.Column.Width = 30;
        else if ((header.Column.ActualWidth < 70))
            header.Column.Width = 70;
    }

    public static void GridViewColumnHeaderClicked(UI_MainWindow? MainWindow, ListView listView, listViewDataType listViewData, object sender, RoutedEventArgs e)
    {
        if ((e.OriginalSource as GridViewColumnHeader == null))
            return;
        GridViewColumnHeader headerClicked = (GridViewColumnHeader)e.OriginalSource;

        if ((headerClicked.Content == null))
            return;
        ListSortDirection direction;

        if (headerClicked != null)
        {
            if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
            {
                if ((listViewData._lastColumnHeaderClicked == null))
                    direction = ListSortDirection.Ascending;
                else if ((headerClicked.Content != listViewData._lastColumnHeaderClicked.Header))
                    direction = ListSortDirection.Ascending;
                else if (listViewData._lastSortDirection == ListSortDirection.Ascending)
                    direction = ListSortDirection.Descending;
                else
                    direction = ListSortDirection.Ascending;

                var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;
                grid_Sorting(listView, listViewData, direction, (GridViewColumnHeader)e.OriginalSource);
                updateGridViewColumnHeaderStyle(MainWindow, listViewData, direction, headerClicked.Column);
            }
        }
    }
}
