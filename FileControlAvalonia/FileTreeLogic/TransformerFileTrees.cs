﻿using FileControlAvalonia.FileTreeLogic;
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
        private FileTree _fileTree;
        private List<FileTree> _removedChildrens;
        private List<FileTree> _parentOfRemoveChild;
        #endregion 

        public TransformerFileTrees(FileTree rootFolder)
        {
            _fileTree = rootFolder;
            _removedChildrens = new List<FileTree>();
            _parentOfRemoveChild = new List<FileTree>();
        }

        #region METHODS
        ///// <summary>
        ///// Возвращяет дерево в котором находятся только выбранные элементы
        ///// </summary>
        ///// <returns></returns>
        public FileTree GetUpdatedFileTree()
        {
            //RemoveUnSelectedFiles(_fileTree.Children!);
            //RemoveEmptyFoldersAndUnselectedFiles();
            //return _fileTree;
            RemoveUnOpenedsElements(_fileTree); 
            RemoveUnCheckedElements(_fileTree);
            RemoveEmptyFoldersAndUnselectedFiles();
            return _fileTree;
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
                else if (file.IsDirectory && file.IsOpened)
                {
                    CheckEmptyFolders(file.Children!);
                }
            }
        }
        private void RemoveUnOpenedsElements(FileTree element)
        {
            foreach(var file in element.Children.ToList())
            {
                if(!file.IsChecked && !file.IsOpened)
                    element.Children.Remove(file);
                else if (file.IsOpened && file.IsDirectory)
                {
                    RemoveUnOpenedsElements(file);
                }
            }
        }
        private void RemoveUnCheckedElements(FileTree element)
        {
            foreach (var file in element.Children.ToList())
            {
                if (!file.IsDirectory && !file.IsChecked)
                    element.Children.Remove(file);
                else if (file.IsDirectory)
                    RemoveUnCheckedElements(file);

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
        private void RemoveEmptyFolders()
        {

        }
    }
    #endregion
}
