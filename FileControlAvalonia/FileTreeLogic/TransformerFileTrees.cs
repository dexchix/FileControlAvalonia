using FileControlAvalonia.FileTreeLogic;
using FileControlAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Helper
{
    public class TransformerFileTrees
    {
        #region FIELDS
        private string _pathRootFolder;
        private FileTree _fileTree;
        private List<FileTree> _removedChildrens;
        private List<FileTree> _parentOfRemoveChild;
        #endregion 

        public TransformerFileTrees(string pathRootFolder, FileTree rootFolder)
        {
            _pathRootFolder = pathRootFolder;
            _fileTree = rootFolder;
            _removedChildrens = new List<FileTree>();
            _parentOfRemoveChild = new List<FileTree>();
        }

        #region METHODS
        /// <summary>
        /// Возвращяет дерево с выбраными элементами
        /// </summary>
        /// <returns></returns>
        public FileTree RemoveUnSelectedFiles()
        {
            SortingFileTree(_fileTree.Children!);
            RemoveEmptyFoldersAndUnselectedFiles();
            return _fileTree;
        }
        /// <summary>
        /// Удаляет из копии колекции файловых деревьев все файлы с свойтсво IsChecked = false 
        /// </summary>
        /// <param name="folder"></param>
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
        /// <summary>
        /// Удаляет пустые папки и не выбранные элементы
        /// </summary>
        private void RemoveEmptyFoldersAndUnselectedFiles()
        {
            do
            {
                for (int i = 0; i < _parentOfRemoveChild.Count; i++)
                {
                    _parentOfRemoveChild[i].Children!.Remove(_removedChildrens[i]);
                }
                _parentOfRemoveChild.Clear();
                _removedChildrens.Clear();
                CheckEmptyFolders(_fileTree.Children!);
            }
            while (_parentOfRemoveChild.Count > 0);
        }
        /// <summary>
        /// Проверяет папку и добавляет её элементы и родителя в коллеции элементов которые должны быть удалены
        /// </summary>
        /// <param name="files"></param>
        private void CheckEmptyFolders(ObservableCollection<FileTree> files)
        {
            foreach (var file in files.ToList())
            {
                if (file.IsDirectory && file.Children?.Count == 0 && file.IsChecked == false)
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
    }
    #endregion
}
