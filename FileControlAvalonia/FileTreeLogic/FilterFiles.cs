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
            ObservableCollection<FileTree> filterCollection = new ObservableCollection<FileTree>();

            FillFilteredCollection(copy, filterCollection, status);

            viewFilesCollection.Clear();
            foreach (var file in filterCollection!.ToList())
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
                if (!file.IsDirectory && file.Status != status)
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
        /// Удаляет файлы которые отсутствуют в главном дереве и переприсваевает статусы файлов в новом дереве
        /// </summary>
        /// <param name="main"></param>
        /// <param name="copy"></param>
        private static void RemoveNotExistentElementsAndCopyState(ObservableCollection<FileTree> main, ObservableCollection<FileTree> copy)
        {
            foreach (var file in copy.ToList())
            {
                if (!main.Any(x => x.Path == file.Path))
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
            foreach (var file in mainCollection!.ToList())
            {
                if (file.IsDirectory && (file.Children == null || file.Children.Count == 0))
                {
                    file.Parent.Children.Remove(file);
                }
                else if (file.IsDirectory && file.Children.Count > 0)
                {
                    DeliteEmptyFolders(file.Children);
                }
            }
        }


        /// <summary>
        /// Заполняет коллекцию для фильтрации
        /// </summary>
        /// <param name="copy"></param>
        /// <param name="filteredCollection"></param>
        /// <param name="status"></param>
        private static void FillFilteredCollection(ObservableCollection<FileTree> copy, ObservableCollection<FileTree> filteredCollection, StatusFile status)
        {
            foreach (var file in copy.ToList())
            {
                if (file.Status == status)
                {
                    filteredCollection.Add(file);
                    if(file.IsDirectory) { ClearUnEqualFiles(file, status); }
                }
       
                else if (file.Status != status && file.IsDirectory)
                {
                    FillFilteredCollection(file.Children, filteredCollection, status);
                }
            }
        }
        /// <summary>
        /// Удаляет файлы в дереве, статус которых не соответствует требуемому
        /// </summary>
        /// <param name="file"></param>
        /// <param name="status"></param>
        private static void ClearUnEqualFiles(FileTree file, StatusFile status)
        {
            foreach (var filee in file.Children.ToList())
            {
                if(filee.Status != status)
                {
                    file.Children.Remove(filee);    
                }
            }
        }
    }
}
