using FileControlAvalonia.FileTreeLogic;
using FileControlAvalonia.Models;
using FileControlAvalonia.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FileControlAvalonia.Core
{
    public class Comprasion
    {
        public int TotalFiles = 0;
        public int Checked = 0;
        public int PartialChecked = 0;
        public int FailedChecked = 0;
        public int NoAccess = 0;
        public int NotChecked = 0;
        public int NotFound = 0;

        public void CompareFiles(ObservableCollection<FileTree> mainFileTreeCollection)
        {
            var filesList = FilesCollectionManager.UpdateTreeToList(mainFileTreeCollection);
            var start = DateTime.Now;
            if (filesList.Count <= 1000)
            {
                foreach (var file in mainFileTreeCollection.ToList())
                {
                    SetStatus(file);

                    if (file.IsDirectory)
                    {
                        CompareFiles(file.Children);
                    }
                    else
                    {

                        //await Task.Delay(500);
                        TotalFiles++;
                        //Locator.Current.GetService<MainWindowViewModel>().ProgressBarValue = TotalFiles;
                        //Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = $"Проверяется {file.Path}"
                    };

                }
            }
            else
            {
                ParallelCompareFiles(filesList, filesList.Count);
            }
            var end = DateTime.Now;

        }
        public void SetStatus(FileTree fileTree)
        {
            //NoAccess
            if (fileTree.FHash == "Отказано в доступе" || fileTree.FLastUpdate == "Отказано в доступе" || fileTree.FVersion == "Отказано в доступе")
            {
                fileTree.Status = StatusFile.NoAccess;
                if (!fileTree.IsDirectory)
                    lock (_lock)
                        NoAccess++;
                ChangeStatusParents(fileTree, StatusFile.FailedChecked);
                return;

            }
            //NotFound
            else if (fileTree.FHash == "Файл отсутствует" || fileTree.FLastUpdate == "Файл отсутствует" || fileTree.FVersion == "Файл отсутствует")
            {
                fileTree.Status = StatusFile.NotFound;
                if (!fileTree.IsDirectory)
                    lock (_lock)
                        NotFound++;
                ChangeStatusParents(fileTree, StatusFile.FailedChecked);
                return;
            }
            //Checked
            else if (fileTree.EHash == fileTree.FHash &&
               fileTree.ELastUpdate == fileTree.FLastUpdate &&
               fileTree.EVersion == fileTree.FVersion)
            {
                fileTree.Status = StatusFile.Checked;
                if (!fileTree.IsDirectory)
                    lock (_lock)
                        Checked++;
                return;
            }
            //PartialChecked
            else if (fileTree.EHash == fileTree.FHash &&
               fileTree.EVersion == fileTree.FVersion &&
               fileTree.ELastUpdate != fileTree.FLastUpdate)
            {
                if (!fileTree.IsDirectory)
                {
                    fileTree.Status = StatusFile.PartiallyChecked;
                    lock (_lock)
                        PartialChecked++;
                    ChangeStatusParents(fileTree, fileTree.Status);
                }
                else
                    fileTree.Status = StatusFile.Checked;


                return;
            }
            //FailedChecked
            else
            {
                fileTree.Status = StatusFile.FailedChecked;
                if (!fileTree.IsDirectory)
                    FailedChecked++;
                ChangeStatusParents(fileTree, fileTree.Status);
                return;
            }
        }

        private static int _count = 0;
        private static object _lock = new object();

        public void ParallelCompareFiles(List<FileTree> files, int countFiles)
        {
            int start = 0;
            int limit = 1;
            var section = countFiles / 8;
            int residue = countFiles - section * 8;
            for (int i = 0; i < 8; i++)
            {
                int localStart = start * section;
                int localLimit = limit * section;
                start++;
                limit++;
                Task.Run(() =>
                {
                    for (int i = localStart; i < localLimit; i++)
                    {
                        SetStatus(files[i]);
                        lock (_lock)
                        {
                            Locator.Current.GetService<MainWindowViewModel>().ProgressBarValue += 50;
                            Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = $"Проверка {files[i].Name}";
                            _count++;
                        }
                    }
                });
            }
            for (int i = countFiles - residue; i < countFiles; i++)
            {
                SetStatus(files[i]);
                lock (_lock)
                {
                    Locator.Current.GetService<MainWindowViewModel>().ProgressBarValue+=50;
                    Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = $"Проверка {files[i].Name}";
                    _count++;
                }
            }

            while (true)
            {
                if (_count == countFiles)
                {
                    _count = 0;
                    break;
                }
            }
        }

        private void ChangeStatusParents(FileTree fileTree, StatusFile status)
        {
            if (fileTree != null && fileTree.Parent != null)
            {
                if (status == StatusFile.PartiallyChecked && fileTree.Parent.Status != StatusFile.FailedChecked)
                {
                    fileTree.Parent.Status = status;
                    ChangeStatusParents(fileTree.Parent, status);
                }
                else if (fileTree.Parent.Status == StatusFile.NotFound)
                {
                    ChangeStatusParents(fileTree.Parent, StatusFile.FailedChecked);
                }
                else
                {
                    fileTree.Parent.Status = StatusFile.FailedChecked;
                    ChangeStatusParents(fileTree.Parent, StatusFile.FailedChecked);
                }
            }
        }
    }
}
