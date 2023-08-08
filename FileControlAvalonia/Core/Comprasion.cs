using Avalonia.Threading;
using FileControlAvalonia.FileTreeLogic;
using FileControlAvalonia.Models;
using FileControlAvalonia.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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

        public void CompareFiles(ObservableCollection<FileTree> mainFileTreeCollection, int count)
        {
            foreach (var file in mainFileTreeCollection.ToList())
            {
                SetStatus(file);

                Locator.Current.GetService<MainWindowViewModel>().ProgressBarValue++;
                Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = $"Проверяется {file.Path}";

                if (file.IsDirectory)
                {
                    CompareFiles(file.Children, count);
                }
                else
                    TotalFiles++;
            }
        }
        public void SetStatus(FileTree fileTree)
        {
            //NoAccess
            if (fileTree.FHash == "Отказано в доступе" || fileTree.FLastUpdate == "Отказано в доступе" || fileTree.FVersion == "Отказано в доступе")
            {
                fileTree.Status = StatusFile.NoAccess;
                if (!fileTree.IsDirectory)
                    NoAccess++;
                ChangeStatusParents(fileTree, StatusFile.FailedChecked);
                return;

            }
            //NotFound
            else if (fileTree.FHash == "Файл отсутствует" || fileTree.FLastUpdate == "Файл отсутствует" || fileTree.FVersion == "Файл отсутствует")
            {
                fileTree.Status = StatusFile.NotFound;
                if (!fileTree.IsDirectory)
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
                    Checked++;
                return;
            }
            //PartialChecked
            else if (fileTree.EHash == fileTree.FHash &&
               fileTree.EVersion == fileTree.FVersion &&
               fileTree.ELastUpdate != fileTree.FLastUpdate)
            {
                fileTree.Status = StatusFile.PartiallyChecked;
                if (!fileTree.IsDirectory)
                    PartialChecked++;
                ChangeStatusParents(fileTree, fileTree.Status);
                return;
            }

            else
            {
                fileTree.Status = StatusFile.FailedChecked;
                if (!fileTree.IsDirectory)
                    FailedChecked++;
                ChangeStatusParents(fileTree, fileTree.Status);
                return;
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
                else if(fileTree.Parent.Status == StatusFile.NotFound)
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
