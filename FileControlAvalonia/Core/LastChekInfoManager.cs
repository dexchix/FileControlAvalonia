using Avalonia.Controls;
using Avalonia.Data;
using FileControlAvalonia.Core.Enums;
using FileControlAvalonia.DataBase;
using FileControlAvalonia.Models;
using FileControlAvalonia.ViewModels;
using Splat;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus;

namespace FileControlAvalonia.Core
{
    public class LastChekInfoManager
    {
        private List<FileTree> _partialChecked = new List<FileTree>();
        private List<FileTree> _failedChecked = new List<FileTree>();
        private List<FileTree> _noAccess = new List<FileTree>();
        private List<FileTree> _notFound = new List<FileTree>();
        private List<FileTree> _notChecked = new List<FileTree>();
        private void SetInfoOfStatusFiles(ObservableCollection<FileTree> mainCollection)
        {
            foreach (var file in mainCollection)
            {
                switch (file.Status)
                {
                    case StatusFile.PartiallyChecked:
                        {
                            _partialChecked.Add(file);
                            if (file.IsDirectory)
                                SetInfoOfStatusFiles(file.Children);
                            break;
                        }
                    case StatusFile.NotFound:
                        {
                            _failedChecked.Add(file);
                            if (file.IsDirectory)
                                SetInfoOfStatusFiles(file.Children);
                            break;
                        }
                    case StatusFile.NoAccess:
                        {
                            _noAccess.Add(file);
                            if (file.IsDirectory)
                                SetInfoOfStatusFiles(file.Children);
                            break;
                        }
                    case StatusFile.FailedChecked:
                        {
                            _failedChecked.Add(file);
                            if (file.IsDirectory)
                                SetInfoOfStatusFiles(file.Children);
                            break;
                        }
                }
            }
        }
        public void UpdateFactParametresInDB(ObservableCollection<FileTree> mainCollection)
        {
            Locator.Current.GetService<MainWindowViewModel>().CancellButtonIsEnabled = false;

            SetInfoOfStatusFiles(mainCollection);
            using (var connection = new SQLiteConnection(DataBaseManager.Options))
            {
                List<FileTree> combinedList = _partialChecked
                                               .Concat(_failedChecked)
                                               .Concat(_noAccess)
                                               .Concat(_notFound)
                                               .Concat(_notChecked)
                                               .ToList();

                connection.UpdateAll(combinedList);
            }
        }
    }
}
