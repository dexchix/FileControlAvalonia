using FileControlAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Helper
{
    public class TransmitterSelectedFiles
    {
        private string _pathRootFolder;
        private FileTree _fileTree;
        private FileTree _copyFileTree;
        private List<FileTree> _removedChildrens;
        private List<FileTree> _parentOfRemoveChild;

        public TransmitterSelectedFiles(string pathRootFolder, FileTree rootFolder)
        {
            _pathRootFolder = pathRootFolder;
            _fileTree = rootFolder;
            _copyFileTree = GetCopyFileTree();
            _removedChildrens = new List<FileTree>();
            _parentOfRemoveChild = new List<FileTree>();
        }

        public ObservableCollection<FileTree> GetSelectedFiles()
        {
            SortingFileTree(_copyFileTree.Children!);
            RemoveEmptyFolders();
            return _copyFileTree.Children!;
        }
        private void SortingFileTree(ObservableCollection<FileTree> folder)
        {
            foreach (var file in folder.ToList())
            {
                if (file.IsChecked == false && !file.IsDirectory)
                {
                    folder.Remove(file);
                }
                if (file.IsDirectory)
                {
                    SortingFileTree(file.Children!);
                }
            }
        }

        private void RemoveEmptyFolders()
        {
            do
            {
                for (int i = 0; i < _parentOfRemoveChild.Count; i++)
                {
                    _parentOfRemoveChild[i].Children!.Remove(_removedChildrens[i]);
                }
                _parentOfRemoveChild.Clear();
                _removedChildrens.Clear();
                CheckEmptyFolders(_copyFileTree.Children!);
            }
            while (_parentOfRemoveChild.Count > 0);
        }

        private void CheckEmptyFolders(ObservableCollection<FileTree> files)
        {
            foreach (var file in files.ToList())
            {
                if (file.IsDirectory && file.Children?.Count == 0)
                {
                    _removedChildrens.Add(file);
                    _parentOfRemoveChild.Add(file.Parent!);
                }
                else if (file.IsDirectory)
                {
                    CheckEmptyFolders(file.Children!);
                }
            }
        }

        private void CopyIsChecked(ObservableCollection<FileTree> folder, ObservableCollection<FileTree> copy)
        {
            for (int i = 0; i < folder.Count; i++)
            {
                if (folder[i].IsChecked)
                {
                    copy[i].IsChecked = true;
                }
                if (folder[i].IsDirectory)
                {
                    CopyIsChecked(folder[i].Children, copy[i].Children);
                }
            }
        }

        private FileTree GetCopyFileTree()
        {
            var copy = new FileTree(_pathRootFolder, true);
            CopyIsChecked(_fileTree.Children!, copy.Children!);
            return copy;
        }
    }
}
