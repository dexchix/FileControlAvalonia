using Avalonia.FreeDesktop.DBusIme;
using FileControlAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.FileTreeLogic
{
    public static class TestingFilesCollectionManager
    {
        //public static void AddFiles(ObservableCollection<FileTree> oldFileTrees, ObservableCollection<FileTree> newFileTrees)
        //{
        //    foreach (var file in newFileTrees)
        //    {
        //        //oldFileTrees.Add(new FileTree(file.Path,file.IsDirectory))
        //    }
        //    //foreach (var file in newFileTrees)
        //    //{
        //    //    if (!oldFileTrees.Any(x => x.Path == file.Path))
        //    //    {
        //    //        oldFileTrees.Add(file);
        //    //    }
        //    //    else if (oldFileTrees.Any(x => x.Path == file.Path) && file.IsDirectory)
        //    //    {
        //    //        AddFiles(oldFileTrees.Where(x => x.Path == file.Path).FirstOrDefault()!.Children!, file.Children!);
        //    //    }
        //    //}
        //}
        public static void AddFiles(this ObservableCollection<FileTree> mainCollection,
                                         ObservableCollection<FileTree> addedCollection)
        {
            foreach (var file in addedCollection)
            {
                if (!mainCollection.Any(x => x.Path == file.Path))
                {
                    mainCollection.Add(file);
                }
                else if (mainCollection.Any(x => x.Path == file.Path) && file.IsDirectory)
                {
                    AddFiles(mainCollection.Where(x => x.Path == file.Path).FirstOrDefault()!.Children!, file.Children!);
                }
            }
        }
    }
}
