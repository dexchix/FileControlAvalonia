using Avalonia.Controls;
using Avalonia.Data;
using FileControlAvalonia.DataBase;
using FileControlAvalonia.Models;
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
            SetInfoOfStatusFiles(mainCollection);
            using (var connection = new SQLiteConnection(DataBaseOptions.Options))
            {
                if (_partialChecked.Count != 0)
                {
                    foreach (var file in _partialChecked)
                    {
                        UpdateFactInfoInDB(file, connection);
                    }
                }
                if (_failedChecked.Count != 0)
                { 
                    foreach (var file in _failedChecked)
                    {
                        UpdateFactInfoInDB(file, connection);
                    }
                }
                if(_notFound.Count != 0)
                {
                    foreach (var file in _notFound)
                    {
                        UpdateFactInfoInDB(file, connection);
                    }
                }
                if (_notChecked.Count != 0)
                {
                    foreach (var file in _notChecked)
                    {
                        UpdateFactInfoInDB(file, connection);
                    }
                }
            }
        }

        public static void UpdateFactInfoInDB(FileTree file, SQLiteConnection connection)
        {

            var updateFactCommand = new SQLiteCommand(connection)
            {
                CommandText = $"UPDATE FilesTable SET FHashSum = '{file.FHash}', FLastUpdate = '{file.FLastUpdate}', FVersion = '{file.FVersion}'  WHERE Path = '{file.Path}';"
            };
            updateFactCommand.ExecuteNonQuery();
        }
    }
}
