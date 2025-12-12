using Fontendo;
using Fontendo.UI;
using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography;

public class FileSystemHelper
{
    public enum FileType
    {
        All,
        BinaryCrustFont,
        NDS,
        NDS3D,
        GBA,
        Text,
        Image,
        Json,
        Xml,
        Binary,
        Archive,
        /* specific images */
        Png,
        Jpg,
        Bmp
    }

    public class FileTypeInfo
    {
        /// <summary>
        /// Human‑readable name of the file type (e.g. "Nintendo DS ROM").
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// List of associated file extensions (e.g. ".nds", ".srl").
        /// </summary>
        public IReadOnlyList<string> Extensions { get; }

        public FileTypeInfo(string name, params string[] extensions)
        {
            Name = name;
            Extensions = extensions;
        }

        /// <summary>
        /// Builds a filter string suitable for OpenFileDialog/SaveFileDialog.
        /// </summary>
        public string ToFilter()
        {
            string extList = string.Join(";", Extensions);
            return $"{Name} files ({extList})|{extList}";
        }
    }

    public class FileTypes
    {
        /// <summary>
        /// Dictionary mapping FileType enum values to their metadata.
        /// </summary>
        public static readonly Dictionary<FileType, FileTypeInfo> Types =
            new Dictionary<FileType, FileTypeInfo>
            {
                    // font files
                    { FileType.BinaryCrustFont,    new FileTypeInfo("Binary Crust Font", "*.bcfnt") },
                    // roms
                    { FileType.NDS,    new FileTypeInfo("Nintendo DS ROM", "*.nds", "*.srl") },
                    { FileType.NDS3D,    new FileTypeInfo("Nintendo 3DS ROM", "*.3ds", "*.cia") },
                    { FileType.GBA,    new FileTypeInfo("Game Boy Advance ROM", "*.gba") },
                    // standard files
                    { FileType.Text,   new FileTypeInfo("Text", "*.txt") },
                    { FileType.Image,  new FileTypeInfo("Image", "*.png", "*.jpg", "*.jpeg", "*.bmp") },
                    { FileType.Png,    new FileTypeInfo("PNG Image", "*.png") },
                    { FileType.Jpg,    new FileTypeInfo("JPEG Image", "*.jpg", "*.jpeg") },
                    { FileType.Bmp,    new FileTypeInfo("Bitmap Image", "*.bmp") },
                    { FileType.Json,   new FileTypeInfo("JSON", "*.json") },
                    { FileType.Xml,    new FileTypeInfo("XML", "*.xml") },
                    { FileType.Binary, new FileTypeInfo("Binary", "*.bin") },
                    { FileType.Archive,new FileTypeInfo("Archive", "*.7z", "*.zip", "*.rar") }
            };

        /// <summary>
        /// Gets the filter string for a given FileType enum.
        /// </summary>
        public static string GetFilter(FileType type)
        {
            return Types[type].ToFilter();
        }
    }

    /// <summary>
    /// Initializes the supported file types dictionary with a selected list of types.
    /// Builds a new dictionary from the canonical <see cref="Types"/> collection.
    /// Ensures that the "All files" entry is always present.
    /// </summary>
    /// <param name="fileTypes">A list of <see cref="FileType"/> values to include.</param>
    public static void Initialize(List<FileType> fileTypes)
    {
        SupportedFileTypes = new Dictionary<FileType, FileTypeInfo>();

        // Add selected types from the canonical Types dictionary
        foreach (var type in fileTypes)
        {
            if (FileTypes.Types.TryGetValue(type, out var info))
            {
                SupportedFileTypes[type] = info;
            }
        }

        // Ensure "All files" entry exists
        if (!SupportedFileTypes.ContainsKey(FileType.All))
        {
            SupportedFileTypes.Add(FileType.All, new FileTypeInfo("All", "*.*"));
        }
    }

    /// <summary>
    /// Dictionary mapping FileType enum values to their metadata.
    /// </summary>
    public static Dictionary<FileType, FileTypeInfo> SupportedFileTypes { get; private set; }
        = new Dictionary<FileType, FileTypeInfo>();

    /// <summary>
    /// Checks whether a given <see cref="FileType"/> is currently supported
    /// in the <see cref="SupportedFileTypes"/> dictionary.
    /// </summary>
    /// <param name="type">The <see cref="FileType"/> enum value to check.</param>
    /// <returns>
    /// True if the file type exists in <see cref="SupportedFileTypes"/>, 
    /// otherwise false.
    /// </returns>
    public static bool IsFileTypeSupported(FileType type)
    {
        return SupportedFileTypes.ContainsKey(type);
    }

    /// <summary>
    /// Opens a file browser dialog with filters for all supported file types.
    /// "All files" is always included at the top.
    /// </summary>
    /// <param name="title">Dialog title (e.g. "Select a file").</param>
    /// <returns>Full path of the selected file, or empty string if cancelled.</returns>
    public static string BrowseForSupportedFile(string title = "Select a file")
    {
        if (SupportedFileTypes.Count == 0) throw new NotImplementedException("FileSystem has not been initialized with any supported file types, make sure to run FileSystem.Initialize()");
        string filePath = string.Empty;

        // Build filter string: All files first, then each supported type
        var filters = new List<string>
        {
            SupportedFileTypes[FileType.All].ToFilter()
        };

        foreach (var kvp in SupportedFileTypes)
        {
            if (kvp.Key == FileType.All) continue; // skip duplicate
            filters.Add(kvp.Value.ToFilter());
        }
        return OpenFileDialog(filters, title);
    }

    private static string OpenFileDialog(List<string> filters, string title)
    {
        string filePath = "";
        string filter = string.Join("|", filters);
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = filter;
        openFileDialog.Title = title;
        openFileDialog.RestoreDirectory = true;

        if (openFileDialog.ShowDialog(UI_MainWindow.Self.Window) == true)
        {
            filePath = openFileDialog.FileName;
        }

        return filePath;
    }

    /// <summary>
    /// Opens a file browser dialog with a filter for a particular file type.
    /// </summary>
    /// <param name="title">Dialog title (e.g. "Select a file").</param>
    /// <returns>Full path of the selected file, or empty string if cancelled.</returns>
    public static string BrowseForFile(FileType fileType = FileType.All, string title = "Select a file")
    {
        string filePath = string.Empty;

        // Build filter string: All files first, then each supported type
        var filters = new List<string>
        {
            FileTypes.GetFilter(fileType)
        };
        return OpenFileDialog(filters, title);
    }

    /// <summary>
    /// Opens a save file dialog with a custom filter and title.
    /// </summary>
    /// <param name="filter">File filter string (e.g. "Text files (*.txt)|*.txt").</param>
    /// <param name="title">Dialog title (e.g. "Save your file").</param>
    /// <returns>Full path of the file to save, or empty string if cancelled.</returns>
    public static string BrowseForSaveFile(FileType? fileType = null, string title = "Save file as", string defaultFilename = "")
    {
        string filePath = string.Empty;
        string filter = fileType == null ? "All files (*.*)|*.*" : FileTypes.GetFilter((FileType)fileType);
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        {
            saveFileDialog.Filter = filter;
            saveFileDialog.Title = title;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = defaultFilename;

            if (saveFileDialog.ShowDialog(UI_MainWindow.Self.Window) == true)
            {
                filePath = saveFileDialog.FileName;
            }
        }

        return filePath;
    }

    /// <summary>
    /// Opens a folder browser dialog with a custom description.
    /// </summary>
    /// <param name="description">Dialog description (e.g. "Select a folder").</param>
    /// <returns>Full path of the selected directory, or empty string if cancelled.</returns>
    public static string BrowseForDirectory(string description = "Select a folder")
    {
        string folderPath = string.Empty;

        OpenFolderDialog folderDialog = new OpenFolderDialog();
        {
            folderDialog.Title = description;

            if (folderDialog.ShowDialog(UI_MainWindow.Self.Window) == true)
            {
                folderPath = folderDialog.FolderName;
            }
        }

        return folderPath;
    }

    /// <summary>
    /// Shortens long file system paths into a more readable form while keeping the start and end intact.
    /// </summary>
    /// <param name="path">The full file system path to shorten.</param>
    /// <param name="maxLength">Maximum allowed length of the returned path (default = 50).</param>
    /// <returns>
    /// A shortened version of the path. If the path length is less than or equal to maxLength,
    /// the original path is returned. Otherwise, the middle segments are collapsed with "..." 
    /// while preserving the first and last segments.
    /// </returns>
    public static string ShortenPath(string path, int maxLength = 50)
    {
        if (string.IsNullOrEmpty(path) || path.Length <= maxLength)
            return path;

        // Split into directory segments
        string[] parts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        // Always keep first and last segment
        string first = parts[0];
        string last = parts[^1];

        // Build middle segments until we exceed maxLength
        var middle = new List<string>();
        int totalLength = first.Length + last.Length + 5; // 5 for "...\"
        for (int i = 1; i < parts.Length - 1; i++)
        {
            int nextLen = parts[i].Length + 1; // +1 for separator
            if (totalLength + nextLen > maxLength)
            {
                middle.Add("...");
                break;
            }
            middle.Add(parts[i]);
            totalLength += nextLen;
        }

        return string.Join(Path.DirectorySeparatorChar.ToString(),
            new[] { first }.Concat(middle).Concat(new[] { last }));
    }


    /// <summary>
    /// Returns a relative path from one directory to another.
    /// </summary>
    public static string GetRelativePath(string basePath, string targetPath)
    {
        return Path.GetRelativePath(basePath, targetPath);
    }

    /// <summary>
    /// Ensures a directory exists, creating it if necessary.
    /// </summary>
    public static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    /// <summary>
    /// Copies a file safely, overwriting if requested.
    /// </summary>
    public static void CopyFileSafe(string source, string destination, bool overwrite = false)
    {
        File.Copy(source, destination, overwrite);
    }

    /// <summary>
    /// Moves a file safely, overwriting if requested.
    /// </summary>
    public static void MoveFileSafe(string source, string destination, bool overwrite = false)
    {
        if (overwrite && File.Exists(destination))
            File.Delete(destination);
        File.Move(source, destination);
    }

    /// <summary>
    /// Deletes a file safely (no exception if missing).
    /// </summary>
    public static void DeleteFileSafe(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }

    /// <summary>
    /// Generates a unique file name in a directory.
    /// </summary>
    public static string GetUniqueFileName(string directory, string baseName, string extension)
    {
        string filePath;
        int counter = 1;
        do
        {
            string fileName = counter == 1 ? $"{baseName}{extension}" : $"{baseName}_{counter}{extension}";
            filePath = Path.Combine(directory, fileName);
            counter++;
        } while (File.Exists(filePath));
        return filePath;
    }

    /// <summary>
    /// Returns a human-readable file size string.
    /// </summary>
    public static string GetFileSizeReadable(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

    /// <summary>
    /// Calculates the total size of a directory (recursive).
    /// </summary>
    public static long GetDirectorySize(string path)
    {
        long size = 0;
        if (Directory.Exists(path))
        {
            foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                size += new FileInfo(file).Length;
        }
        return size;
    }

    /// <summary>
    /// Checks if a path is writable.
    /// </summary>
    public static bool IsPathWritable(string path)
    {
        try
        {
            string testFile = Path.Combine(path, Path.GetRandomFileName());
            using (FileStream fs = File.Create(testFile, 1, FileOptions.DeleteOnClose)) { }
            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// Opens a directory in Windows Explorer.
    /// </summary>
    public static void OpenInExplorer(string path)
    {
        if (Directory.Exists(path) || File.Exists(path))
            System.Diagnostics.Process.Start("explorer.exe", path);
    }

    /// <summary>
    /// Returns a temporary file path.
    /// </summary>
    public static string GetTempFilePath()
    {
        return Path.GetTempFileName();
    }

    /// <summary>
    /// Computes a checksum (MD5/SHA1/SHA256) for a file.
    /// </summary>
    public static string GetChecksum(string path, string algorithm = "SHA256")
    {
        using var stream = File.OpenRead(path);
        using var hash = HashAlgorithm.Create(algorithm);
        if (hash == null) throw new ArgumentException("Invalid algorithm");
        byte[] checksum = hash.ComputeHash(stream);
        return BitConverter.ToString(checksum).Replace("-", "").ToLowerInvariant();
    }

    /// <summary>
    /// Validates if a path string is valid.
    /// </summary>
    public static bool IsValidPath(string path)
    {
        try
        {
            string full = Path.GetFullPath(path);
            return !string.IsNullOrWhiteSpace(full);
        }
        catch { return false; }
    }

    /// <summary>
    /// Normalizes a path (resolves relative segments).
    /// </summary>
    public static string NormalizePath(string path)
    {
        return Path.GetFullPath(new Uri(path).LocalPath)
                   .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    /// <summary>
    /// Gets the file extension.
    /// </summary>
    public static string GetFileExtension(string path)
    {
        return Path.GetExtension(path);
    }

    /// <summary>
    /// Changes the file extension.
    /// </summary>
    public static string ChangeFileExtension(string path, string newExtension)
    {
        return Path.ChangeExtension(path, newExtension);
    }

    /// <summary>
    /// Gets the file creation date.
    /// </summary>
    public static DateTime GetFileCreationDate(string path)
    {
        return File.GetCreationTime(path);
    }

    /// <summary>
    /// Gets the file modified date.
    /// </summary>
    public static DateTime GetFileModifiedDate(string path)
    {
        return File.GetLastWriteTime(path);
    }

    /// <summary>
    /// Lists all files recursively.
    /// </summary>
    public static IEnumerable<string> ListFilesRecursive(string path)
    {
        return Directory.Exists(path) ? Directory.GetFiles(path, "*", SearchOption.AllDirectories) : Array.Empty<string>();
    }

    /// <summary>
    /// Lists all directories recursively.
    /// </summary>
    public static IEnumerable<string> ListDirectoriesRecursive(string path)
    {
        return Directory.Exists(path) ? Directory.GetDirectories(path, "*", SearchOption.AllDirectories) : Array.Empty<string>();
    }

    /// <summary>
    /// Determines the expected <see cref="FileType"/> for a given file path
    /// based on its extension.
    /// </summary>
    /// <param name="path">Full file path to evaluate.</param>
    /// <returns>
    /// The matching <see cref="FileType"/> enum value if recognized,
    /// or <see cref="FileType.All"/> if no specific match is found.
    /// </returns>
    public static FileType GetFileTypeFromPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return FileType.All;

        string ext = Path.GetExtension(path).ToLowerInvariant();

        foreach (var kvp in FileTypes.Types)
        {
            foreach (var candidateExt in kvp.Value.Extensions)
            {
                // Extensions in SupportedFileTypes are stored like "*.cia", "*.nds"
                string cleanExt = candidateExt.TrimStart('*').ToLowerInvariant();
                if (ext == cleanExt)
                    return kvp.Key;
            }
        }

        return FileType.All;
    }

}