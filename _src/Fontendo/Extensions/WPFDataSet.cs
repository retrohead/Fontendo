using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace WPFDataSet
{
    public class WPFDataSet : INotifyPropertyChanged
    {
        public class Row : INotifyPropertyChanged
        {
            private int _id;
            public int Id
            {
                get => _id;
                set
                {
                    if (_id != value)
                    {
                        _id = value;
                        OnPropertyChanged();
                    }
                }
            }
            private ObservableCollection<object> _vals = new ObservableCollection<object>();

            public ObservableCollection<object> Vals
            {
                get => _vals;
                set
                {
                    if (_vals != value)
                    {
                        _vals = value;
                        OnPropertyChanged();
                    }
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ObservableCollection<string> _headings = new ObservableCollection<string>();
        public ObservableCollection<string> Headings
        {
            get => _headings;
            set
            {
                if (_headings != value)
                {
                    _headings = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<Row> _rows = new ObservableCollection<Row>();
        public ObservableCollection<Row> Rows
        {
            get => _rows;
            set
            {
                if (_rows != value)
                {
                    _rows = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class DataGridBindingConverter : IValueConverter
        {
            public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value is ObservableCollection<object> vals && parameter is int param)
                {
                    if (param >= vals.Count)
                        return "";
                    else
                        return vals[param]?.ToString();
                }
                return null;
            }
            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
