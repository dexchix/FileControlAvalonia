using FileControlAvalonia.Core;
using FileControlAvalonia.Models;
using FileControlAvalonia.SettingsApp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        async public static void Filter(StatusFile status, ObservableCollection<FileTree> mainCollection, ObservableCollection<FileTree> viewFilesCollection)
        {
            ObservableCollection<FileTree> filterCollection = new ObservableCollection<FileTree>();
            await Task.Run(() =>
            {
                var copy = FilesCollectionManager.GetDeepCopyFilesCollection(mainCollection);
                FillFilteredCollection(copy, filterCollection, status);
            });
            viewFilesCollection.Clear();
            foreach (var file in filterCollection!.ToList())
            {
                viewFilesCollection.Add(file);
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
                    file.Parent = null;
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
                if (filee.Status != status)
                {
                    //file.Children.Remove(filee);    
                    var delitedFile = file.Children.FirstOrDefault(filee => filee.Status != status);
                    file.Children.Remove(delitedFile);
                    
                }
                else
                {
                    if (file.IsDirectory) ClearUnEqualFiles(filee, status);
                }

            }
        }
    }
}
