using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml.Templates;
using FileControlAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Core
{
    public static class FactParameterizer
    {
        private static string GetMD5Hash(string filePath)
        {
            bool folderExist = Directory.Exists(filePath);
            bool fileExist = File.Exists(filePath);
            if (folderExist || fileExist)
            {
                if (folderExist)
                    return "-";
                else
                    try
                    {
                        using (var md5 = MD5.Create())
                        {

                            using (var stream = File.OpenRead(filePath))
                            {
                                byte[] hashBytes = md5.ComputeHash(stream);
                                string hashString = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                                return hashString;
                            }
                        }
                    }
                    catch
                    {
                        return "Отказано в доступе";
                    }
            }
            return "Файл отсутствует";

        }
        private static string GetLastUpdate(string filePath)
        {
            bool folderExist = Directory.Exists(filePath);
            bool fileExist = File.Exists(filePath);
            if (folderExist || fileExist)
            {  
                if (folderExist)
                    return new DirectoryInfo(filePath).LastWriteTime.ToString();
                else
                {
                    try
                    {
                        return new FileInfo(filePath).LastWriteTime.ToString();

                    }
                    catch
                    {
                        return "Отказано в доступе";
                    }
                }
            }
            return "Файл отсутствует";
        }
        private static string GetVersion(string filePath)
        {
            bool folderExist = Directory.Exists(filePath);
            bool fileExist = File.Exists(filePath);
            if (folderExist || fileExist)
            {
                if (fileExist)
                {
                    try
                    {
                        var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(filePath);
                        return versionInfo.FileVersion == null || versionInfo.FileVersion == "" ? "-" : versionInfo.FileVersion!;
                    }
                    catch
                    {
                        return "Отказано в доступе";
                    }

                }
                else return "-";
            }
            return "Файл отсутствует";
        }
        public static void SetFactValues(this FileTree file)
        {
            file.FVersion = GetVersion(file.Path);
            file.FHash = GetMD5Hash(file.Path);
            file.FLastUpdate = GetLastUpdate(file.Path);
        }
        public static void SetFactValuesInFilesCollection(ObservableCollection<FileTree> files)
        {
            foreach (var file in files)
            {
                file.SetFactValues();

                if (file.IsDirectory)
                {
                    SetFactValuesInFilesCollection(file.Children);
                }
            }
        }
    }

    //public class FactParameterizer
    //{

    //}
}
