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
                        return versionInfo.FileVersion == null || versionInfo.FileVersion == string.Empty ? "-" : versionInfo.FileVersion!;
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
        //public static void SetFactValues(this FileTree file)
        //{
        //    var version = GetVersion(file.Path);
        //    var fhash = GetMD5Hash(file.Path);
        //    var lastUpdate = GetLastUpdate(file.Path);

        //    file.FVersion = version;
        //    file.FHash = fhash;
        //    file.FLastUpdate = lastUpdate;

        //    if (version == "Отказано в доступе")
        //        file.FVersion = "-";

        //    if (lastUpdate == "Отказано в доступе")
        //        file.FLastUpdate = "-";

        //    if (fhash == "Отказано в доступе")
        //    {
        //        file.FVersion = "Отказано в доступе";
        //        file.FHash = "Отказано в доступе";
        //        file.FLastUpdate = "Отказано в доступе";
        //    }
        //}

        public static void SetFactValues(this FileTree file)
        {
            var version = GetVersion(file.Path);
            var lastUpdate = GetLastUpdate(file.Path);
            string fhash = "-";

            if (version == "Отказано в доступе" || lastUpdate == "Отказано в доступе")
            {
                var md5Task = Task.Run(() => GetMD5HashWithTimeout(file.Path, TimeSpan.FromSeconds(5)));
                if (md5Task.Wait(TimeSpan.FromSeconds(2)))
                {
                    fhash = md5Task.Result;
                }
            }
            else
            {
                fhash = GetMD5Hash(file.Path);
            }
            file.FVersion = version;
            file.FHash = fhash;
            file.FLastUpdate = lastUpdate;

            if (version == "Отказано в доступе")
                file.FVersion = "-";

            if (lastUpdate == "Отказано в доступе")
                file.FLastUpdate = "-";

            if (fhash == "Отказано в доступе")
            {
                file.FVersion = "Отказано в доступе";
                file.FHash = "Отказано в доступе";
                file.FLastUpdate = "Отказано в доступе";
            }
        }

        private static string GetMD5HashWithTimeout(string filePath, TimeSpan timeout)
        {
            try
            {
                string result = null;
                var task = Task.Run(() =>
                {
                    result = GetMD5Hash(filePath);
                });

                if (task.Wait(timeout))
                {
                    return result;
                }
                else
                {
                    return "Отказано в доступе"; // Если метод не завершен в течение таймаута.
                }
            }
            catch
            {
                return "Отказано в доступе"; // Обработка других исключений, если необходимо.
            }
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
