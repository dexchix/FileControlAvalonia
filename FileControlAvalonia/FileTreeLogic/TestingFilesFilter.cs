using FileControlAvalonia.Core;
using FileControlAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.FileTreeLogic
{
    public static class TestingFilesFilter
    {
        public static void Filter(this ObservableCollection<FileTree> files, StatusFile filterStatus, ObservableCollection<FileTree> filteredFiles)
        {
            filteredFiles.Clear();
            foreach (var file in files.ToList())
            {
                if(file.IsDirectory || file.Status == filterStatus)
                {
                    filteredFiles.Add(file);
                }
            }
            RemoveEmptyFolders(files);
        }
        private static void RemoveEmptyFolders(ObservableCollection<FileTree> files)
        {
            foreach (var file in files.ToList())
            {
                if (file.Children == null || file.Children.Count == 0 && file.Parent == null)
                {
                    files.Remove(file);
                }
                else if (file.Children == null || file.Children.Count == 0 && file.Parent != null)
                {
                    file.Parent!.Children!.Remove(file);
                }
            }
        }
    }
}
