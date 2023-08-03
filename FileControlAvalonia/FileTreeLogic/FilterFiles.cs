using FileControlAvalonia.Core;
using FileControlAvalonia.Models;
using FileControlAvalonia.SettingsApp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public static void Filter(StatusFile status, ObservableCollection<FileTree> mainCollection, ObservableCollection<FileTree> viewFilesCollection)
        {
            var copy = FilesCollectionManager.GetDeepCopy(mainCollection);
            //RemoveNotMatchStatusFiles(copy, status);
            //DeliteEmptyFolders(copy);
            //DeliteEmptyFolders(copy);

            TEST(copy,status);

            viewFilesCollection.Clear();
            foreach (var file in copy!.ToList())
            {
                viewFilesCollection.Add(file);
            }
        }
        /// <summary>
        /// Удаляет файлы статусы которых не соответствуют требуемым 
        /// </summary>
        /// <param name="fileTree"></param>
        /// <param name="status"></param>
        private static void RemoveNotMatchStatusFiles(ObservableCollection<FileTree> copy, StatusFile status)
        {
            foreach (var file in copy!.ToList())
            {
                if(!file.IsDirectory && file.Status != status)
                {
                    copy!.Remove(file);
                }
                else if (file.IsDirectory)
                {
                    RemoveNotMatchStatusFiles(file.Children, status);
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
            var copyFileTree = new FileTree(SettingsManager.RootPath, true);
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
        private static void DeliteEmptyFolders(ObservableCollection<FileTree> mainCollection)
        {
            foreach(var file in mainCollection!.ToList())
            {
                if (file.IsDirectory && (file.Children == null || file.Children.Count == 0))
                {
                    file.Parent.Children.Remove(file); 
                }
                else if (file.IsDirectory && file.Children.Count >0)
                {
                    DeliteEmptyFolders(file.Children);
                }
            }
        }



        private static void TEST(ObservableCollection<FileTree> copy, StatusFile status)
        {
            foreach(var file in copy.ToList())
            {
                if (file.Status != status)
                    copy.Remove(file);
                else if (file.Status == status && file.IsDirectory)
                {
                    TEST(file.Children, status);
                }   
            }
        }
    }
}
