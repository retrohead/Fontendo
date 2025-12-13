using System.IO;
using System.Text.Json;

namespace Fontendo.Extensions
{
    public class AppSettings
    {
        public int SelectedTheme { get; set; } = -1;
        public string ThemeColours { get; set; } = "";
        public bool FullSize { get; set; } = false;
        public string RecentFiles { get; set; } = "";
        public int AnimationSpeed { get; set; } = 1;
        public string FontBackgroundColor { get; set; } = "#452175";
    }
    public static class SettingsManager
    {
        private static readonly string SettingsPath;
        private static readonly JsonSerializerOptions JsonOptions;
        private static AppSettings _settings;

        static SettingsManager()
        {
            // Where the settings file lives
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, MainWindow.appname);
            Directory.CreateDirectory(folder);

            SettingsPath = Path.Combine(folder, "app_settings.json");

            JsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                AllowTrailingCommas = true
            };

            _settings = LoadInternal();
        }

        public static AppSettings Settings => _settings;

        public static void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(_settings, JsonOptions);
                File.WriteAllText(SettingsPath, json);
            }
            catch
            {
                // swallow — settings should never crash the app
            }
        }

        private static AppSettings LoadInternal()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    var loaded = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions);
                    if (loaded != null)
                        return loaded;
                }
            }
            catch
            {
                // corrupted JSON → fall back to defaults
            }

            // Return defaults if missing or corrupt
            return new AppSettings();
        }
    }
}
