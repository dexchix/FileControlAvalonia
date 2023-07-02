using FileControlAvalonia.Core;
using FileControlAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.FileTreeLogic
{
    public class FilterFiles
    {
        public void Filter(StatusFile status, FileTree mainFileTree, ObservableCollection<FileTree> viewFilesCollection)
        {
            var copy = GetSimpleCopy(mainFileTree);
            RemoveNotMatchStatusFiles(copy, status);
            DeliteEmptyFolders(copy);

            viewFilesCollection.Clear();
            foreach (var file in copy.Children!.ToList())
            {
                viewFilesCollection.Add(file);
            }
        }
        private void RemoveNotMatchStatusFiles(FileTree fileTree, StatusFile status)
        {
            foreach (var file in fileTree.Children!.ToList())
            {
                if(!file.IsDirectory && file.Status != status)
                {
                    fileTree.Children.Remove(file);
                }
                else if (file.IsDirectory)
                {
                    RemoveNotMatchStatusFiles(file, status);
                }
            }
        }
        public FileTree GetSimpleCopy(FileTree mainFileTree)
        {
            var copyFileTree = new FileTree(FileTreeNavigator.pathRootFolder, true);
            RemoveNotExistentElementsAndCopyState(mainFileTree.Children, copyFileTree.Children);
            return copyFileTree;
        }
        private void RemoveNotExistentElementsAndCopyState(ObservableCollection<FileTree> main, ObservableCollection<FileTree> copy)
        {
            foreach(var file in copy.ToList())
            {
                if(!main.Any(x=> x.Path == file.Path))
                {
                    copy.Remove(file);
                }
                else
                {
                    file.Status = main.Single(x => x.Path == file.Path).Status;
                }
            }
            foreach (var file in copy.ToList())
            {
                if (file.IsDirectory)
                {
                    RemoveNotExistentElementsAndCopyState(main.Single(x => x.Path == file.Path).Children!, file.Children);
                }
            }
        }
        private void DeliteEmptyFolders(FileTree fileTree)
        {
            foreach(var file in fileTree.Children!.ToList())
            {
                if (file.IsDirectory && (file.Children == null || file.Children.Count == 0))
                {
                    fileTree.Children.Remove(file); 
                }
                else if (file.IsDirectory && file.Children.Count >0)
                {
                    DeliteEmptyFolders(file);
                }
            }
        }
    }
}
