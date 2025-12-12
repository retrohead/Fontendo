using Fontendo.UI;
using Fontendo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace Fontendo
{
    public class RecentFilesManager
    {

        private static int MaxRecentFiles = 15;

        public class RecentFile : INotifyPropertyChanged
        {
            private string _name = "";
            public string Name
            {
                get { return _name; }
                set
                {
                    if (_name != value)
                    {
                        _name = value;
                        OnPropertyChanged(nameof(Name));
                    }
                }
            }

            private string _filePath = "";
            public string FilePath
            {
                get { return _filePath; }
                set
                {
                    if (_filePath != value)
                    {
                        _filePath = value;
                        OnPropertyChanged(nameof(FilePath));
                    }
                }
            }

            private DateTime? _lastAccessed;
            public DateTime? LastAccessed
            {
                get { return _lastAccessed; }
                set
                {
                    if (_lastAccessed != value)
                    {
                        _lastAccessed = value;
                        OnPropertyChanged(nameof(LastAccessed));
                    }
                }
            }

            public RecentFile(string filePath, string name)
            {
                Name = name;
                FilePath = filePath;
                LastAccessed = DateTime.Now;
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static ObservableCollection<RecentFile>? RecentFiles = Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<RecentFile>>(Properties.Settings.Default.RecentFiles);

        public static void AddRecentFile(string filePath, string? name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            // Create list if does not exist already
            if (RecentFiles == null)
                RecentFiles = new ObservableCollection<RecentFile>();
            // Remove if already exists (to reinsert at the top)
            RemoveRecentFile(filePath);

            // Add new file at the beginning
            RecentFiles.Insert(0, new RecentFile(filePath, name));

            // Trim the list if it exceeds the max amount
            while (RecentFiles.Count > MaxRecentFiles)
            {
                RecentFiles.RemoveAt(RecentFiles.Count - 1);
            }

            // Save back to settings
            Properties.Settings.Default.RecentFiles = Newtonsoft.Json.JsonConvert.SerializeObject(RecentFiles, Newtonsoft.Json.Formatting.None);
            Properties.Settings.Default.Save();
        }
        public static void RemoveRecentFile(string filePath)
        {
            // Remove if already exists
            var file = RecentFiles?.FirstOrDefault(f => f.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase));
            if (file != null)
                RecentFiles?.Remove(file);
        }

        public static async void LoadRecentFile(string filePath)
        {
            var file = RecentFiles?.FirstOrDefault(f => f.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase));
            if (file == null)
            {
                App.CustomMessageBox.Show("There was an error finding the file in your recent files list.\n\n" + file?.FilePath, "Recent File Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!File.Exists(file?.FilePath))
            {
                if (App.CustomMessageBox.Show("The selected file no longer exists.\n\n" +
                    file?.FilePath + "\n\n" +
                    "Would you like to remove it from your recent files list?", "File no longer exists", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    if (file != null)
                    {
                        RemoveRecentFile(file.FilePath);
                    }
                }
                return;
            }
            UI_MainWindowContent.Self?.OpenRecentFile(file.FilePath);
        }

    }
}
