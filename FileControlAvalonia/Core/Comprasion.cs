using Avalonia.Threading;
using FileControlAvalonia.Core.Enums;
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
        public Dictionary<string, (FileTree, StatusFile)> OldStatuses = new Dictionary<string, (FileTree, StatusFile)>();
        public void CompareFiles(ObservableCollection<FileTree> mainFileTreeCollection)
        {
            var filesList = FilesCollectionManager.UpdateTreeToList(mainFileTreeCollection);
            var start = DateTime.Now;
            if (filesList.Count <= 1000)
            {
                foreach (var file in filesList)
                {
                    SetStatus(file);
                    if (!file.IsDirectory)
                        TotalFiles++;

                }
                ChangeStatusParents();


                //foreach (var file in mainFileTreeCollection.ToList())
                //{
                //    SetStatus(file);

                //    if (file.IsDirectory)
                //    {
                //        CompareFiles(file.Children);
                //    }
                //    else
                //    {

                //        //await Task.Delay(500);
                //        TotalFiles++;
                //        //Locator.Current.GetService<MainWindowViewModel>().ProgressBarValue = TotalFiles;
                //        //Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = $"Проверяется {file.Path}"
                //    };

                //}
            }
            else
            {
                ParallelCompareFiles(filesList, filesList.Count);
            }
            var end = DateTime.Now;

        }
        public void SetStatus(FileTree fileTree)
        {
            if (fileTree.Status == StatusFile.FailedChecked)
                return;
            //NoAccess
            if (fileTree.FHash == "Отказано в доступе" || fileTree.FLastUpdate == "Отказано в доступе" || fileTree.FVersion == "Отказано в доступе")
            {
                if (!OldStatuses.ContainsKey(fileTree.Path)) OldStatuses.Add(fileTree.Path, (fileTree, fileTree.Status));
                Dispatcher.UIThread.Post(() => fileTree.Status = StatusFile.NoAccess);
                if (!fileTree.IsDirectory)
                    lock (_lock)
                        NoAccess++;
                changedStatusFiles.Add(fileTree);
                //ChangeStatusParents(fileTree, StatusFile.FailedChecked);
                return;

            }
            //NotFound
            else if (fileTree.FHash == "Файл отсутствует" || fileTree.FLastUpdate == "Файл отсутствует" || fileTree.FVersion == "Файл отсутствует")
            {
                if (!OldStatuses.ContainsKey(fileTree.Path)) OldStatuses.Add(fileTree.Path, (fileTree, fileTree.Status));
                Dispatcher.UIThread.Post(() => fileTree.Status = StatusFile.NotFound);
                if (!fileTree.IsDirectory)
                    lock (_lock)
                        NotFound++;
                changedStatusFiles.Add(fileTree);
                //ChangeStatusParents(fileTree, StatusFile.FailedChecked);
                return;
            }
            //Checked
            else if (fileTree.EHash == fileTree.FHash &&
               fileTree.ELastUpdate == fileTree.FLastUpdate &&
               fileTree.EVersion == fileTree.FVersion)
            {
                if (!OldStatuses.ContainsKey(fileTree.Path)) OldStatuses.Add(fileTree.Path, (fileTree, fileTree.Status));
                Dispatcher.UIThread.Post(() => fileTree.Status = StatusFile.Checked);
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
                    if (!OldStatuses.ContainsKey(fileTree.Path)) OldStatuses.Add(fileTree.Path, (fileTree, fileTree.Status));
                    Dispatcher.UIThread.Post(() => fileTree.Status = StatusFile.PartiallyChecked);
                    lock (_lock)
                        PartialChecked++;
                    changedStatusFiles.Add(fileTree);
                    //ChangeStatusParents(fileTree, fileTree.Status);
                }
                else
                    fileTree.Status = StatusFile.Checked;


                return;
            }
            //FailedChecked
            else
            {
                if (!OldStatuses.ContainsKey(fileTree.Path)) OldStatuses.Add(fileTree.Path, (fileTree, fileTree.Status));
                Dispatcher.UIThread.Post(() => fileTree.Status = StatusFile.FailedChecked);
                if (!fileTree.IsDirectory)
                    FailedChecked++;
                changedStatusFiles.Add(fileTree);
                //ChangeStatusParents(fileTree, fileTree.Status);
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

                        lock (_lock)
                        {
                            SetStatus(files[i]);
                            if (!files[i].IsDirectory) TotalFiles++;
                            _count++;
                        }
                    }
                });
            }
            for (int i = countFiles - residue; i < countFiles; i++)
            {

                lock (_lock)
                {
                    SetStatus(files[i]);
                    if (!files[i].IsDirectory) TotalFiles++;
                    _count++;
                }
            }

            while (true)
            {
                if (_count == countFiles)
                {
                    _count = 0;
                    ChangeStatusParents();
                    break;
                }
            }
        }
        public List<FileTree> changedStatusFiles = new List<FileTree>();

        public void ChangeStatusParents()
        {
            foreach (var file in changedStatusFiles)
            {
                if (file.Parent != null && file.Parent.Status == StatusFile.FailedChecked)
                    continue;
                else if (file.Parent != null && file.Parent.Status == StatusFile.FailedChecked && file.Status == StatusFile.PartiallyChecked)
                    continue;
                else if (file.Parent != null && file.Parent.Status == StatusFile.Checked && file.Status == StatusFile.PartiallyChecked)
                    ChangeParentRecursive(file, file.Status);
                else
                    ChangeParentRecursive(file, StatusFile.FailedChecked);
            }

            void ChangeParentRecursive(FileTree file, StatusFile status)
            {
                if (file.Parent != null)
                {
                    file.Parent.Status = status;
                    ChangeParentRecursive(file.Parent, status);
                }
            }
        }
    }
}
