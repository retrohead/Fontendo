using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using Fontendo;
using Fontendo.UI;

public class comboData
{
    private string? _id;
    public string? Value
    {
        get
        {
            return _id;
        }
        set
        {
            _id = value;
        }
    }
    private string? _text;
    public string? Text
    {
        get
        {
            return _text;
        }
        set
        {
            _text = value;
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
            _icon = value;
        }
    }
    private string[]? _subvals;
    public string[]? SubValues
    {
        get
        {
            return _subvals;
        }
        set
        {
            _subvals = value;
        }
    }

    public comboData(string txt, string val)
    {
        Value = val;
        Text = txt;
    }

    public comboData(ImageSource? img, string? txt, string? val)
    {
        Value = val;
        Text = txt;
        icon = img;
    }
    public comboData(string txt, string[] values)
    {
        Value = values[0];
        SubValues = values;
        Text = txt;
    }


    public static void resetComboSelectedItem(ref ComboBox c, SelectionChangedEventArgs e)
    {
        if ((e.RemovedItems.Count > 0))
            c.SelectedItem = e.RemovedItems[0];
        else
            c.SelectedIndex = -1;
    }

    public static void populateCombo(ref ComboBox c, ref comboBoxData cd, WPFDataSet.WPFDataSet comboBoxFillData, bool allowAdditions, comboData selectAllItemsObj, WPFDataSet.WPFDataSet ? formDataToMatch = null, int dataSetColumnIDForValue = -1, comboBoxData ? populatedData = null, UI_MainWindow ? statusIconForm = null, UI_MainWindow ? statIconForm = null, UI_MainWindow ? flagIconForm = null)
    {
        int i;
        int selectedIndex = 0;
        if ((cd == null))
            cd = new comboBoxData();
        cd.Items = new ObservableCollection<comboData>();

        if ((formDataToMatch == null))
            dataSetColumnIDForValue = -1;
        else if ((formDataToMatch.Rows.Count == 0))
            dataSetColumnIDForValue = -1;
        else if ((dataSetColumnIDForValue >= formDataToMatch.Headings.Count))
        {
            App.CustomMessageBox.Show("Fatal error, trying to set a combo index by using an object outside the bounds of the columns in the data", "Fatal error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            dataSetColumnIDForValue = -1;
        }

        if (!(populatedData == null))
        {
            for (i = 0; i <= populatedData?.Items?.Count - 1; i++)
            {
                cd.Items.Add(new comboData(populatedData?.Items?[i].icon, populatedData?.Items?[i].Text, populatedData?.Items?[i].Value));
                if ((dataSetColumnIDForValue > -1))
                {
                    if ((formDataToMatch?.Rows[0].Vals[dataSetColumnIDForValue].ToString() == populatedData?.Items?[i].Value))
                        selectedIndex = i;
                }
            }
        }
        if (!(comboBoxFillData == null))
        {
            for (i = 0; i <= comboBoxFillData.Rows.Count - 1; i++)
            {
                cd.Items.Add(new comboData(comboBoxFillData.Rows[i].Vals[1].ToString() ?? "", comboBoxFillData.Rows[i].Vals[0].ToString() ?? ""));
                if ((dataSetColumnIDForValue > -1))
                {
                    if ((formDataToMatch?.Rows[0].Vals[dataSetColumnIDForValue].ToString() == comboBoxFillData.Rows[i].Vals[0].ToString()))
                        selectedIndex = i;
                }
            }
        }
        if (!(selectAllItemsObj == null))
        {
            cd.Items.Add(selectAllItemsObj);
            cd.SelectedComboItem = cd.Items[cd.Items.Count - 1];
            if ((allowAdditions))
                App.CustomMessageBox.Show("Fatal error, Allowing additions to a combo box but also allowing to select all items, this cannot be right", "Fatal error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        else if ((allowAdditions))
        {
            cd.Items.Add(new comboData("< create a new record >", "-99999"));
            cd.Items.Add(new comboData("< modify selected record >", "-99998"));
            cd.Items.Add(new comboData("< view selected record >", "-99997"));
            cd.SelectedComboItem = cd.Items[selectedIndex];
        }
        else if ((selectedIndex < cd.Items.Count))
            cd.SelectedComboItem = cd.Items[selectedIndex];
        try
        {
            c.DataContext = cd;
        }
        catch (Exception ex)
        {
            App.CustomMessageBox.Show("ComboBox Error For '" + c.Name + "': " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}

public class comboBoxData : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler ? PropertyChanged;
    private object _syncLock = new object();

    private ObservableCollection<comboData> ? _comboItems;
    public ObservableCollection<comboData>? Items
    {
        get
        {
            return _comboItems;
        }
        set
        {
            _comboItems = value;
            BindingOperations.EnableCollectionSynchronization(Items, _syncLock);
            OnPropertyChanged(new PropertyChangedEventArgs("Items"));
        }
    }

    private comboData ? _selectedItem;
    public comboData ? SelectedComboItem
    {
        get
        {
            return _selectedItem;
        }
        set
        {
            _selectedItem = value;
            OnPropertyChanged(new PropertyChangedEventArgs("SelectedComboItem"));
        }
    }

    public void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }
}


public class numberBoxData : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private string _value = "";
    public string Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            OnPropertyChanged(this, new PropertyChangedEventArgs("Value"));
        }
    }

    public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }
}

