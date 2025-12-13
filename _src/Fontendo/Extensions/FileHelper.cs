using System.Diagnostics;
using System;
using System.Windows;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using Fontendo;

public class fileHelper
{
    public static string getDrivePath(string DriveName)
    {
        dynamic objNtWork;
        dynamic objDrives;
        long lngLoop;
        string getDrivePath = "";

        objNtWork = Interaction.CreateObject("WScript.Network");
        objDrives = objNtWork.enumnetworkdrives;

        getDrivePath = DriveName;
        for (lngLoop = 0; lngLoop <= objDrives.Count - 1; lngLoop += 2)
        {
            if (Strings.UCase(objDrives.Item[lngLoop].ToString()) == Strings.UCase(DriveName))
            {
                getDrivePath = objDrives.Item[lngLoop + 1];
                break;
            }
        }
        return getDrivePath;
    }
    public static bool IsValidFileName(string filename)
    {
        if (filename == null)
            return false;
        char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
        bool hasInvalidChars = filename.Any(ch => invalidChars.Contains(ch));
        return !hasInvalidChars;
    }

    public static bool IsValidFolderPath(string path)
    {
        char[] invalidChars = System.IO.Path.GetInvalidPathChars();
        return !path.Any(ch => invalidChars.Contains(ch));
    }

    public static string getNetworkPath(string fileLoc)
    {
        string getNetworkPath = "";
        if ((fileLoc.StartsWith(@"\\")))
        {
            getNetworkPath = fileLoc;
            return getNetworkPath;
        }
        string drive;
        drive = Strings.Split(fileLoc, @"\")[0];

        getNetworkPath = getDrivePath(drive) + Strings.Right(fileLoc, Strings.Len(fileLoc) - Strings.Len(drive));
        return getNetworkPath;
    }
    public static string browseForFile(string title, string filter)
    {
        string browseForFile = "";
        OpenFileDialog dg;
        dg = new OpenFileDialog();
        dg.RestoreDirectory = true;
        dg.Title = title;
        dg.Filter = filter;
        if (dg.ShowDialog() == false)
        {
            browseForFile = "";
            return browseForFile;
        }
        browseForFile = getNetworkPath(dg.FileName);
        return browseForFile;
    }
    public static string browseForFolder(string title)
    {
        string browseForFolder = "";
        OpenFolderDialog dg;
        dg = new OpenFolderDialog();
        dg.Title = title;

        if (dg.ShowDialog() == false)
        {
            browseForFolder = "";
            return browseForFolder;
        }
        browseForFolder = getNetworkPath(dg.FolderName);
        return browseForFolder;
    }
    public static string getSaveAsName(string fileName, string title, string filter, string defExt)
    {
        string getSaveAsName = "";
        SaveFileDialog fdg = new SaveFileDialog();
        fdg.RestoreDirectory = true;
        fdg.Title = title;
        fdg.DefaultExt = defExt;
        fdg.FileName = fileName;
        fdg.Filter = filter;
        try
        {
            if (fdg.ShowDialog() == true)
            {
                if (fdg.FileName != "")
                    getSaveAsName = getNetworkPath(fdg.FileName).Replace(defExt + defExt, defExt);
                return getSaveAsName;
            }
        }catch
        {

        }
        getSaveAsName = "";
        return getSaveAsName;
    }

    private static bool clearedTemp = false;
    public static void clearTempFiles(string appDataPath, bool ignoreClearTemp)
    {
        if ((ignoreClearTemp))
            return;
        string fn;
        fn = appDataPath;

        if ((System.IO.Directory.Exists(fn) == false))
        {
            try
            {
                System.IO.Directory.CreateDirectory(fn);
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(fn, "Themes"));
            }
            catch
            {
            }
        }
        else
        {
            if ((System.IO.Directory.Exists(System.IO.Path.Combine(fn, "Themes")) == false))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.Combine(fn, "Themes"));
                }
                catch
                {
                }
            }
            bool clearTemp = true;
            if ((clearedTemp == false))
            {
                Process[] p;
                p = Process.GetProcessesByName(MainWindow.appname);
                if ((p.Count() > 1))
                    clearTemp = false;

                if ((clearTemp == true))
                {
                    string[] files = System.IO.Directory.GetFiles(fn);
                    foreach (string fn2 in files)
                    {
                        if ((fn2.EndsWith("history.txt") == false) & (fn2.EndsWith("invoices.txt") == false) & (fn2.EndsWith(".ttf") == false))
                        {
                            try
                            {
                                System.IO.File.SetAttributes(fn2, System.IO.FileAttributes.Normal);
                                System.IO.File.Delete(fn2);
                            }
                            catch (Exception ex)
                            {
                                App.CustomMessageBox.Show("Could not delete file " + Environment.NewLine + Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine + fn2, "File Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
                clearedTemp = true;
            }
        }
    }
}
