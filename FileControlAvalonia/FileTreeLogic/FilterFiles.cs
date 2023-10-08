using FileControlAvalonia.Core.Enums;
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

            //Dictionary<StatusFile, FileTree> structFiles = new Dictionary<StatusFile, FileTree> ();
            List<FileTree> files = new List<FileTree>();
            FillList(mainCollection);

            void FillList(ObservableCollection<FileTree> folder)
            {
                foreach (FileTree fileTree in folder)
                {
                    files.Add(fileTree);
                    if (fileTree.Children != null)
                        FillList(fileTree.Children);
                }
            }
            var filteredList = files.Where(x => x.Status == status && !x.IsDirectory).ToList();
            ObservableCollection<FileTree> filteredCollection = new ObservableCollection<FileTree>();
            foreach (FileTree fileTree in filteredList)
            {
                filteredCollection.Add(fileTree);
            }

            viewFilesCollection.Clear();
            foreach (var file in filteredList!.ToList())
            {
                viewFilesCollection.Add(file);
            }
            //ObservableCollection<FileTree> filterCollection = new ObservableCollection<FileTree>();
            //await Task.Run(() =>
            //{
            //    var copy = FilesCollectionManager.GetDeepCopyFilesCollection(mainCollection);
            //    FillFilteredCollection(copy, filterCollection, status);
            //});
            //viewfilescollection.clear();
            //foreach (var file in filtercollection!.tolist())
            //{
            //    viewfilescollection.add(file);
            //}


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
                    if (file.IsDirectory) { ClearUnEqualFiles(file, status); }
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
