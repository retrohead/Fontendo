using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fontendo.Extensions
{
    internal class RecentFiles
    {

        private const int MAX_RECENT_ITEMS = 20;
        public class RecentFile
        {
            public string Name { get; set; } = "";
            public string FilePath { get; set; } = "";

            public RecentFile(string filePath)
            {
                Name = Path.GetFileName(filePath);
                FilePath = filePath;
            }
        }

        public static List<RecentFile> Items = new List<RecentFile>();


        public static void Initialize()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.RecentFiles))
            {
                var loadedItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RecentFile>>(Properties.Settings.Default.RecentFiles);
                if (loadedItems != null)
                    Items = loadedItems;
                else
                    Items = new List<RecentFile>();
            }
        }

        public static void Add(string FilePath)
        {
            if (FilePath == "")
                return;
            var existingFile = Items.Find(r => r.FilePath.Equals(FilePath));
            Remove(FilePath);
            Items.Insert(0, new RecentFile(FilePath));
            while (Items.Count > MAX_RECENT_ITEMS)
                Items.RemoveRange(MAX_RECENT_ITEMS, Items.Count - MAX_RECENT_ITEMS);
            
            Properties.Settings.Default.RecentFiles = Newtonsoft.Json.JsonConvert.SerializeObject(Items);
            Properties.Settings.Default.Save();
        }

        public static void Remove(string FilePath)
        {
            var existingFile = Items.Find(r => r.FilePath.Equals(FilePath));
            if(existingFile != null)
                Items.Remove(existingFile);
        }
    }
}
