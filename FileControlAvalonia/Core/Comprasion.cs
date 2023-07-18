using Avalonia.Threading;
using FileControlAvalonia.FileTreeLogic;
using FileControlAvalonia.Models;
using FileControlAvalonia.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
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
        public void CompareTrees(FileTree mainFileTree, int count)
        {
            foreach (var file in mainFileTree.Children)
            {
                SetStatus(file);

                Locator.Current.GetService<MainWindowViewModel>().ProgressBarValue++;
                Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = $"{Locator.Current.GetService<MainWindowViewModel>().ProgressBarValue} " +
                    $"из {Locator.Current.GetService<MainWindowViewModel>().ProgressBarMaximum}";

                if (file.IsDirectory)
                {
                    CompareTrees(file, count);
                }
            }
        }

        public void AddFiles(FileTree mainFileTree, FileTree addedFileTree)
        {
            foreach (var file in addedFileTree.Children!.ToList())
            {
                if (!mainFileTree.Children!.Any(x => x.Path == file.Path))
                {
                    var mainParent = FileTreeNavigator.SearchFile(file.Parent!.Path, mainFileTree);
                    mainFileTree.Children!.Add(file);
                    file.Parent = mainParent;
                }
                else if (mainFileTree.Children!.Any(x => x.Path == file.Path) && file.IsDirectory)
                {
                    AddFiles(mainFileTree.Children!.Where(x => x.Path == file.Path).FirstOrDefault()!, file);
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
                fileTree.Status = StatusFile.UnChecked;
                ChangeStatusParents(fileTree, fileTree.Status);
                UnChecked++;
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
                else if (status == StatusFile.UnChecked)
                {
                    var parent = fileTree.Parent;
                    parent.Status = status;
                    ChangeStatusParents(parent, status);
                }
            }
        }
    }
}
//Checked,
//PartiallyChecked,
//FailedChecked,
//UnChecked,
//NoAccess,
//Missing
