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
    public class FilterFiles
    {
        private FileTree _copyFileTree = new FileTree(FileTreeNavigator.pathRootFolder, true);

        public void Filter(StatusFile status, ObservableCollection<FileTree> files,
                                                     ObservableCollection<FileTree> filteredFiles)
        {
            filteredFiles.Clear();

        }
        private void RemoveNotMatchStatusFiles(FileTree fileTree)
        {
            foreach (var file in fileTree.Children)
            {
                if (!_copyFileTree.Children.Any(x => x.Path == file.Path))
                {
                    //FileTreeNavigator.SearchFile(file.Parent.Path, )
                    _copyFileTree.Children.Remove(file);
                }
                if (_copyFileTree.Children.Any(x => x.Path == file.Path) && file.IsDirectory)
                {

                }
            }
        }
        //private FileTree GetCopyFileTree(FileTree fileTree)
        //{
            
        //}
    }
}
