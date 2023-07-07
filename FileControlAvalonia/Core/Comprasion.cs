using FileControlAvalonia.FileTreeLogic;
using FileControlAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Core
{
    public class Comprasion
    {
        public static void CompareTrees(FileTree mainFileTree)
        {
            foreach (var file in mainFileTree.Children)
            {
                SetStatus(file);
                if (file.IsDirectory)
                {
                    CompareTrees(file);
                }
            }
        }

        public static void AddFiles(FileTree mainFileTree, FileTree addedFileTree)
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
        public static void SetStatus(FileTree fileTree)
        {
            if (fileTree.EHash == fileTree.FHash &&
               fileTree.ELastUpdate == fileTree.FLastUpdate &&
               fileTree.EVersion == fileTree.FVersion)
            {
                fileTree.Status = StatusFile.Checked;
                ChangeStatusParents(fileTree, fileTree.Status);
                return;
            }
            else if (fileTree.EHash == fileTree.FHash &&
               fileTree.EVersion == fileTree.FVersion &&
               fileTree.ELastUpdate != fileTree.FLastUpdate)
            {
                fileTree.Status = StatusFile.PartiallyChecked;
                ChangeStatusParents(fileTree, fileTree.Status);
                return;
            }
            else
            {
                fileTree.Status = StatusFile.UnChecked;
                ChangeStatusParents(fileTree, fileTree.Status);
                return;
            }
        }
        private static void ChangeStatusParents(FileTree fileTree, StatusFile status)
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
            //if (fileTree.Parent != null)
            //{
            //    var parent = fileTree.Parent;
            //    if (parent.Status == StatusFile.FailedChecked)
            //        return;
            //    else if (parent.Status == StatusFile.PartiallyChecked && status == StatusFile.Checked)
            //    {
            //        return;
            //    }
            //    else
            //    {
            //        parent.Status = status;
            //        ChangeStatusParents(parent, status);
            //    }
            //}
        }
    }
}
//Checked,
//PartiallyChecked,
//FailedChecked,
//UnChecked,
//NoAccess,
//Missing
