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
        private FileTree _copyFileTree;
        private List<FileTree> _removedChildrens;
        private List<FileTree> _parentOfRemoveChild;
        #endregion 

        public TransformerFileTrees(string pathRootFolder, FileTree rootFolder)
        {
            _pathRootFolder = pathRootFolder;
            _fileTree = rootFolder;
            _copyFileTree = GetCopyFileTree();
            _removedChildrens = new List<FileTree>();
            _parentOfRemoveChild = new List<FileTree>();
        }

        #region METHODS
        /// <summary>
        /// Возвращяет колекцию (деревья) выбранных элементов
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<FileTree> GetSelectedFiles()
        {
            SortingFileTree(_copyFileTree.Children!);
            RemoveEmptyFoldersAndUnselectedFiles();
            return _copyFileTree.Children!;
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
                CheckEmptyFolders(_copyFileTree.Children!);
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
        /// <summary>
        /// Выставляет в копии колекции деревьев свойство IsChecked в соответствии с оригинальной колекцией
        /// </summary>
        /// <param name="original"></param>
        /// <param name="copy"></param>
        private void CopyIsChecked(ObservableCollection<FileTree> original, ObservableCollection<FileTree> copy)
        {
            for (int i = 0; i < original.Count; i++)
            {
                if (original[i].IsChecked)
                {
                    copy[i].IsChecked = true;
                }
                if (original[i].IsDirectory)
                {
                    CopyIsChecked(original[i].Children, copy[i].Children);
                }
            }
        }
        /// <summary>
        /// Создает и возвращяет копию файлового дерева
        /// </summary>
        /// <returns></returns>
        private FileTree GetCopyFileTree()
        {
            var copy = new FileTree(_pathRootFolder, true);
            CopyIsChecked(_fileTree.Children!, copy.Children!);
            return copy;
        }
    }
    #endregion
}
