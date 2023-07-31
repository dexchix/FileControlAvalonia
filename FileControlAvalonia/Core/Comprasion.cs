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
        public int Checked = 0;
        public int PartiallyChecked = 0;
        public int FailedChecked = 0;
        public int UnChecked = 0;
        public int NoAccess = 0;
        public int Missing = 0;
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
            }
        }
        public void SetStatus(FileTree fileTree)
        {
            if (fileTree.EHash == fileTree.FHash &&
               fileTree.ELastUpdate == fileTree.FLastUpdate &&
               fileTree.EVersion == fileTree.FVersion)
            {
                fileTree.Status = StatusFile.Checked;
                Checked++;
                ChangeStatusParents(fileTree, fileTree.Status);
                return;
            }
            else if (fileTree.EHash == fileTree.FHash &&
               fileTree.EVersion == fileTree.FVersion &&
               fileTree.ELastUpdate != fileTree.FLastUpdate)
            {
                fileTree.Status = StatusFile.PartiallyChecked;
                PartiallyChecked++;
                ChangeStatusParents(fileTree, fileTree.Status);
                return;
            }
            else
            {
                fileTree.Status = StatusFile.FailedChecked;
                ChangeStatusParents(fileTree, fileTree.Status);
                FailedChecked++;
                return;
            }
        }
        private void ChangeStatusParents(FileTree fileTree, StatusFile status)
        {
            if (fileTree.Parent != null)
            {
                if (status == StatusFile.Checked)
                {
                    return;
                }
                else if (status == StatusFile.NotFound)
                {
                    var parent = fileTree.Parent;
                    parent.Status = status;
                    ChangeStatusParents(parent, status);
                }
            }
        }
    }
}
