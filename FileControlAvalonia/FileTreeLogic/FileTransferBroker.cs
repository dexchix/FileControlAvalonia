using FileControlAvalonia.DataBase;
using FileControlAvalonia.Models;
using FileControlAvalonia.SettingsApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.FileTreeLogic
{
    //public static class FileTransferBroker
    //{
    //    public static List<FileDB> AddedFiles = new List<FileDB>();

    //    public static void FillListAddedFiles(FileTree file, bool IsChecked)
    //    {
    //        if (IsChecked)
    //        {
    //            var addFile = new FileDB()
    //            {
    //                Path = file.Path,
    //                ParentPath = file.Parent.Path == SettingsManager.RootPath ? "" : file.Parent.Path,
    //                IsDirectory = file.IsDirectory
    //            };
    //            AddedFiles.Add(addFile);
    //        }
    //        else DeliteFileInAddedFilesCollection(file.Path);
    //    }
    //    private static void DeliteFileInAddedFilesCollection(string filePath)
    //    {
    //        var delFile = AddedFiles.ToList().FirstOrDefault(x => x.Path == filePath);
    //        AddedFiles.Remove(delFile);

    //    }
    //}
}
