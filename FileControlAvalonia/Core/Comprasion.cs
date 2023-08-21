using FileControlAvalonia.Models;
using System.Collections.ObjectModel;
using System.Linq;

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

        async public void CompareFiles(ObservableCollection<FileTree> mainFileTreeCollection)
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
                if (!fileTree.IsDirectory)
                {
                    fileTree.Status = StatusFile.PartiallyChecked;
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
