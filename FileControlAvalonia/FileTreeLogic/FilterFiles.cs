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
        /// <summary>
        /// Фильтрует дерево на основании статуса
        /// </summary>
        /// <param name="status"></param>
        /// <param name="mainFileTree"></param>
        /// <param name="viewFilesCollection"></param>
        public static void Filter(StatusFile status, FileTree mainFileTree, ObservableCollection<FileTree> viewFilesCollection)
        {
            var copy = GetSimpleCopy(mainFileTree);
            RemoveNotMatchStatusFiles(copy, status);
            DeliteEmptyFolders(copy);
            DeliteEmptyFolders(copy);

            viewFilesCollection.Clear();
            foreach (var file in copy.Children!.ToList())
            {
                viewFilesCollection.Add(file);
            }
        }
        /// <summary>
        /// Удаляет файлы статусы которых не соответствуют требуемым 
        /// </summary>
        /// <param name="fileTree"></param>
        /// <param name="status"></param>
        private static void RemoveNotMatchStatusFiles(FileTree fileTree, StatusFile status)
        {
            foreach (var file in fileTree.Children!.ToList())
            {
                if(!file.IsDirectory && file.Status != status)
                {
                    fileTree.Children!.Remove(file);
                }
                else if (file.IsDirectory)
                {
                    RemoveNotMatchStatusFiles(file, status);
                }
            }
        }
        /// <summary>
        /// Создает простую копию файлового дерева
        /// </summary>
        /// <param name="mainFileTree"></param>
        /// <returns></returns>
        public static FileTree GetSimpleCopy(FileTree mainFileTree)
        {
            var copyFileTree = new FileTree(SettingsManager.rootPath, true);
            RemoveNotExistentElementsAndCopyState(mainFileTree.Children!, copyFileTree.Children!);
            return copyFileTree;
        }
        /// <summary>
        /// Удаляет файлы которые отсутствуют в главном дереве и переприсваевает статусы файлов в новом дереве
        /// </summary>
        /// <param name="main"></param>
        /// <param name="copy"></param>
        private static void RemoveNotExistentElementsAndCopyState(ObservableCollection<FileTree> main, ObservableCollection<FileTree> copy)
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
        /// <summary>
        /// Удаляет пустые папки
        /// </summary>
        /// <param name="fileTree"></param>
        private static void DeliteEmptyFolders(FileTree fileTree)
        {
            foreach(var file in fileTree.Children!.ToList())
            {
                if(file.Path == "C:\\1\\2\\Новая папка")
                {
                    object a = null;
                }

                if (file.IsDirectory && (file.Children == null || file.Children.Count == 0))
                {
                    file.Parent.Children.Remove(file); 
                }
                else if (file.IsDirectory && file.Children.Count >0)
                {
                    DeliteEmptyFolders(file);
                }
            }
        }
    }
}
