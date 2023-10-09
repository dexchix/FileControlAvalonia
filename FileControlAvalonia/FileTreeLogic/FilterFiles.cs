using DynamicData;
using FileControlAvalonia.Core.Enums;
using FileControlAvalonia.Models;
using FileControlAvalonia.SettingsApp;
using FileControlAvalonia.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.FileTreeLogic
{
    public class FilterFiles
    {
        private static object _lock = new object();
        private static int _count = 0;

        /// <summary>
        /// Фильтрует дерево на основании статуса
        /// </summary>
        /// <param name="status"></param>
        /// <param name="mainFileTree"></param>
        /// <param name="viewFilesCollection"></param>
        async public static void Filter(StatusFile status, ObservableCollection<FileTree> mainCollection, ObservableCollection<FileTree> viewFilesCollection)
        {
            viewFilesCollection.Clear();
            List<FileTree> relevantFiles = new List<FileTree>();

            if (status != StatusFile.Checked)
                await Task.Run(async () =>
                {
                    Locator.Current.GetService<MainWindowViewModel>().ProgressBarIsVisible = true;
                    Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = "Фильтрация файлов";
                    Locator.Current.GetService<MainWindowViewModel>().ProgressBarLoopScrol = true;

                    FillList(mainCollection);


                    ObservableCollection<FileTree> filteredList = new ObservableCollection<FileTree>(relevantFiles.Where(x => x.Status == status && !x.IsDirectory));


                    await Task.Run(() =>
                    {
                        foreach (FileTree fileTree in filteredList)
                        {
                            viewFilesCollection.Add(fileTree);
                        }
                    });
                    Locator.Current.GetService<MainWindowViewModel>().ProgressBarIsVisible = false;
                    Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = string.Empty;
                    Locator.Current.GetService<MainWindowViewModel>().ProgressBarLoopScrol = false;
                });
            else
            {
                await Task.Run(async () =>
                {
                    Locator.Current.GetService<MainWindowViewModel>().ProgressBarIsVisible = true;
                    Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = "Фильтрация файлов";
                    Locator.Current.GetService<MainWindowViewModel>().ProgressBarLoopScrol = true;

                    FillCheckedElements(mainCollection);

                    foreach(var file in relevantFiles)
                    {
                        viewFilesCollection.Add(file);
                    }


                    Locator.Current.GetService<MainWindowViewModel>().ProgressBarIsVisible = false;
                    Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = string.Empty;
                    Locator.Current.GetService<MainWindowViewModel>().ProgressBarLoopScrol = false;
                });
            }


            void FillList(ObservableCollection<FileTree> folder)
            {
                foreach (FileTree fileTree in folder)
                {
                    relevantFiles.Add(fileTree);
                    if (fileTree.Children != null)
                        FillList(fileTree.Children);
                }
            }
            void FillCheckedElements(ObservableCollection<FileTree> folder)
            {
                foreach (FileTree fileTree in folder)
                {
                    if (fileTree.Status == StatusFile.Checked && fileTree.IsDirectory)
                    {
                        relevantFiles.Add(fileTree);
                    }
                    else if (fileTree.Status == StatusFile.Checked && !fileTree.IsDirectory)
                        relevantFiles.Add(fileTree);
                    else if (fileTree.Status != StatusFile.Checked && fileTree.IsDirectory)
                        FillCheckedElements(fileTree.Children!);
                }
            }



            //Dictionary<StatusFile, FileTree> structFiles = new Dictionary<StatusFile, FileTree> ();


            //if (filteredList.Count > 1000)
            //{
            //    int start = 0;
            //    int limit = 1;
            //    var section = filteredList.Count / 8;
            //    int residue = filteredList.Count - section * 8;
            //    for (int i = 0; i < 8; i++)
            //    {
            //        int localStart = start * section;
            //        int localLimit = limit * section;
            //        start++;
            //        limit++;
            //        Task.Run(() =>
            //        {
            //            for (int i = localStart; i < localLimit; i++)
            //            {

            //                lock (_lock)
            //                {
            //                    viewFilesCollection.Add(filteredList[i]);
            //                    _count++;
            //                }
            //            }
            //        });
            //    }
            //    for (int i = filteredList.Count - residue; i < filteredList.Count; i++)
            //    {

            //        lock (_lock)
            //        {
            //            viewFilesCollection.Add(filteredList[i]);
            //            _count++;
            //        }
            //    }

            //    while (true)
            //    {
            //        if (_count == filteredList.Count)
            //        {
            //            _count = 0;
            //            return;
            //        }
            //    }
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
