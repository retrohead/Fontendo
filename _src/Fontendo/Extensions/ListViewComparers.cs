using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows.Data;

public class ListViewSubItemComparer : IComparer
{
    private readonly IValueConverter converter;
    private readonly ListSortDirection direction;
    private readonly int parameter;

    public ListViewSubItemComparer(IValueConverter converter, ListSortDirection direction, int param)
    {
        this.converter = converter;
        this.direction = direction;
        this.parameter = param;
    }

    public int Compare(object? x, object? y)
    {
        if(x==null)
            return -1;
        if(y==null)
            return 1;
        var transx = converter.Convert(((dynamic)x).SubItems, typeof(string), parameter, CultureInfo.CurrentCulture) ?? "";
        var transy = converter.Convert(((dynamic)y).SubItems, typeof(string), parameter, CultureInfo.CurrentCulture) ?? "";

        // Default values to prevent null reference issues
        DateTime dateX = DateTime.MinValue, dateY = DateTime.MinValue;
        double dblX = 0, dblY = 0;
        long intX = 0, intY = 0;

        // Attempt DateTime parsing
        if (DateTime.TryParse(transx.ToString(), out DateTime tempDateX))
            dateX = tempDateX;
        if (DateTime.TryParse(transy.ToString(), out DateTime tempDateY))
            dateY = tempDateY;

        if (dateX != DateTime.MinValue && dateY != DateTime.MinValue)
        {
            return direction == ListSortDirection.Ascending
                ? Comparer.Default.Compare(dateX, dateY)
                : Comparer.Default.Compare(dateX, dateY) * -1;
        }

        // Attempt Double parsing
        if (double.TryParse(transx.ToString(), out double tempDblX))
            dblX = tempDblX;
        if (double.TryParse(transy.ToString(), out double tempDblY))
            dblY = tempDblY;

        if (dblX != 0 || dblY != 0)
        {
            return direction == ListSortDirection.Ascending
                ? Comparer.Default.Compare(dblX, dblY)
                : Comparer.Default.Compare(dblX, dblY) * -1;
        }

        // Attempt Long parsing
        if (long.TryParse(transx.ToString(), out long tempIntX))
            intX = tempIntX;
        if (long.TryParse(transy.ToString(), out long tempIntY))
            intY = tempIntY;

        if (intX != 0 || intY != 0)
        {
            return direction == ListSortDirection.Ascending
                ? Comparer.Default.Compare(intX, intY)
                : Comparer.Default.Compare(intX, intY) * -1;
        }

        return direction == ListSortDirection.Ascending
            ? Comparer.Default.Compare(transx.ToString(), transy.ToString())
            : Comparer.Default.Compare(transx.ToString(), transy.ToString()) * -1;
    }
}

public class ListViewIconComparer : IComparer
{
    private readonly ListSortDirection direction;

    public ListViewIconComparer(ListSortDirection direction)
    {
        this.direction = direction;
    }

    public int Compare(object? x, object? y)
    {
        if(x==null)
            return -1;
        if(y==null)
            return 1;
        var iconX = ((dynamic)x).icon?.ToString() ?? "";
        var iconY = ((dynamic)y).icon?.ToString() ?? "";

        return direction == ListSortDirection.Ascending
            ? Comparer.Default.Compare(Path.GetFileName(iconX), Path.GetFileName(iconY))
            : Comparer.Default.Compare(Path.GetFileName(iconX), Path.GetFileName(iconY)) * -1;
    }
}

public class ListViewIcon2Comparer : IComparer
{
    private readonly ListSortDirection direction;

    public ListViewIcon2Comparer(ListSortDirection direction)
    {
        this.direction = direction;
    }

    public int Compare(object? x, object? y)
    {
        if (x == null)
            return -1;
        if (y == null)
            return 1;
        var icon2X = ((dynamic)x).icon2?.ToString() ?? "";
        var icon2Y = ((dynamic)y).icon2?.ToString() ?? "";

        return direction == ListSortDirection.Ascending
            ? Comparer.Default.Compare(Path.GetFileName(icon2X), Path.GetFileName(icon2Y))
            : Comparer.Default.Compare(Path.GetFileName(icon2X), Path.GetFileName(icon2Y)) * -1;
    }
}

public class ListViewIconColourComparer : IComparer
{
    private readonly ListSortDirection direction;

    public ListViewIconColourComparer(ListSortDirection direction)
    {
        this.direction = direction;
    }

    public int Compare(object? x, object? y)
    {
        if (x == null)
            return -1;
        if (y == null)
            return 1;
        var iconColourX = ((dynamic)x).iconColour ?? "";
        var iconColourY = ((dynamic)y).iconColour ?? "";

        return direction == ListSortDirection.Ascending
            ? Comparer.Default.Compare(iconColourX, iconColourY)
            : Comparer.Default.Compare(iconColourX, iconColourY) * -1;
    }
}
