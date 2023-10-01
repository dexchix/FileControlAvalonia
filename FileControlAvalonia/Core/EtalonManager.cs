using FileControlAvalonia.DataBase;
using FileControlAvalonia.FileTreeLogic;
using FileControlAvalonia.Models;
using FileControlAvalonia.ViewModels;
using Newtonsoft.Json;
using NLog;
using Splat;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus;

namespace FileControlAvalonia.Core
{
    public static class EtalonManager
    {
        //public static ObservableCollection<FileTree> CurentEtalon = GetEtalon();
        //public static EtalonAndChecksInfoDB CheckInfo = GetInfo();



        /// <summary>
        /// Добавляет файлы в эталон (БД) или создает новый (перезаполняет таблицу в БД).
        /// </summary>
        /// <param name="mainFileTreeCollection"></param>
        /// <param name="createEalon">Если true - создает эталон, если false - добавляет файлы</param>
        public static void AddFilesOrCreateEtalon(ObservableCollection<FileTree> mainFileTreeCollection, bool createEalon)
        {
            FilesCollectionManager.SetEtalonValues(mainFileTreeCollection);
            var converter = new DataBase.DataBaseConverter();
            var etalonFilesCollection = converter.ConvertFormatFileTreeToDB(mainFileTreeCollection);

            //Locator.Current.GetService<MainWindowViewModel>().ProgressBarMaximum = etalonFilesCollection.Count;

            using (var connection = new SQLiteConnection(DataBaseOptions.Options))
            {
                if (createEalon == true)
                {
                    var commandClearTableFiles = new SQLiteCommand(connection)
                    {
                        CommandText = "DELETE FROM FilesTable"
                    };
                    commandClearTableFiles.ExecuteNonQuery();
                }

                string startQuery = "INSERT INTO FilesTable (Name, Path, ELastUpdate, EVersion, EHashSum, FLastUpdate, FVersion, FHashSum, ParentPath, Status, IsDirectory) VALUES";
                StringBuilder beginComand = new StringBuilder(startQuery);

                for (int i = 0; i < etalonFilesCollection.Count; i++)
                {
                    if (i == 0)
                    {
                        beginComand.Append($"('{etalonFilesCollection[i].Name.ToString()}', '{etalonFilesCollection[i].Path.ToString()}', '{etalonFilesCollection[i].ELastUpdate}', '{etalonFilesCollection[i].EVersion}', '{etalonFilesCollection[i].EHashSum}'," +
                            $" '{etalonFilesCollection[i].ELastUpdate}', '{etalonFilesCollection[i].EVersion}', '{etalonFilesCollection[i].EHashSum}', '{etalonFilesCollection[i].ParentPath}', '{etalonFilesCollection[i].Status}', {etalonFilesCollection[i].IsDirectory})");
                        if (etalonFilesCollection.Count == 1)
                        {
                            var insertCommandFilesTable = new SQLiteCommand(connection)
                            {
                                CommandText = beginComand.ToString()
                            };
                            insertCommandFilesTable.ExecuteNonQuery();
                            break;
                        }
                        else continue;
                    }
                    if (i == etalonFilesCollection.Count - 1)
                    {
                        beginComand.Append($", ('{etalonFilesCollection[i].Name.ToString()}', '{etalonFilesCollection[i].Path.ToString()}', '{etalonFilesCollection[i].ELastUpdate}', '{etalonFilesCollection[i].EVersion}', '{etalonFilesCollection[i].EHashSum}'," +
                            $" '{etalonFilesCollection[i].ELastUpdate}', '{etalonFilesCollection[i].EVersion}', '{etalonFilesCollection[i].EHashSum}', '{etalonFilesCollection[i].ParentPath}', '{etalonFilesCollection[i].Status}', {etalonFilesCollection[i].IsDirectory})");

                        var insertCommandFilesTable = new SQLiteCommand(connection)
                        {
                            CommandText = beginComand.ToString()
                        };
                        insertCommandFilesTable.ExecuteNonQuery();
                    }
                    else if (i % 10000 == 0)
                    {
                        beginComand.Append($", ('{etalonFilesCollection[i].Name.ToString()}', '{etalonFilesCollection[i].Path.ToString()}', '{etalonFilesCollection[i].ELastUpdate}', '{etalonFilesCollection[i].EVersion}', '{etalonFilesCollection[i].EHashSum}'," +
                            $" '{etalonFilesCollection[i].ELastUpdate}', '{etalonFilesCollection[i].EVersion}', '{etalonFilesCollection[i].EHashSum}', '{etalonFilesCollection[i].ParentPath}', '{etalonFilesCollection[i].Status}', {etalonFilesCollection[i].IsDirectory})");

                        var insertCommandFilesTable = new SQLiteCommand(connection)
                        {
                            CommandText = beginComand.ToString()
                        };
                        insertCommandFilesTable.ExecuteNonQuery();
                    }
                    else if (i > 1000 && i % 10000 == 1)
                    {
                        beginComand.Clear().Append(startQuery);
                        beginComand.Append($"('{etalonFilesCollection[i].Name.ToString()}', '{etalonFilesCollection[i].Path.ToString()}', '{etalonFilesCollection[i].ELastUpdate}', '{etalonFilesCollection[i].EVersion}', '{etalonFilesCollection[i].EHashSum}', '{etalonFilesCollection[i].ELastUpdate}', '{etalonFilesCollection[i].EVersion}', '{etalonFilesCollection[i].EHashSum}', '{etalonFilesCollection[i].ParentPath}', '{etalonFilesCollection[i].Status}', {etalonFilesCollection[i].IsDirectory})");
                    }

                    else
                    {
                        beginComand.Append($", ('{etalonFilesCollection[i].Name.ToString()}', '{etalonFilesCollection[i].Path.ToString()}', '{etalonFilesCollection[i].ELastUpdate}', '{etalonFilesCollection[i].EVersion}', '{etalonFilesCollection[i].EHashSum}'," +
                            $" '{etalonFilesCollection[i].ELastUpdate}', '{etalonFilesCollection[i].EVersion}', '{etalonFilesCollection[i].EHashSum}', '{etalonFilesCollection[i].ParentPath}', '{etalonFilesCollection[i].Status}', {etalonFilesCollection[i].IsDirectory})");
                    }
                }
            }
        }

        public static ObservableCollection<FileTree> GetEtalon()
        {
            List<FileDB> etalon;

            using (var connection = new SQLiteConnection(DataBaseOptions.Options))
            {
                var command = new SQLiteCommand(connection)
                {
                    CommandText = "SELECT Name, Path, ELastUpdate, EVersion, EHashSum, FLastUpdate, FVersion, FHashSum, ParentPath, Status, IsDirectory FROM FilesTable"
                };
                etalon = command.ExecuteQuery<FileDB>();
            }
            var converter = new DataBaseConverter();
            var etalonInDBContext = converter.ConvertFormatDBToFileTreeCollection(etalon);
            FileTree._counter = -1;
            return etalonInDBContext;
        }

        public static void DeliteFileInDB(FileTree file)
        {
            if (file.Children != null)
            {
                var listDelitedFiles = new DataBaseConverter().ConvertFormatFileTreeToDB(new ObservableCollection<FileTree>() { file });
                listDelitedFiles.Reverse();

                Locator.Current.GetService<MainWindowViewModel>().ProgressBarMaximum = listDelitedFiles.Count;

                using (var connection = new SQLiteConnection(DataBaseOptions.Options))
                {

                    string startQuery = "DELETE FROM FilesTable WHERE Path IN (";
                    StringBuilder beginComand = new StringBuilder(startQuery);

                    for (int i = 0; i < listDelitedFiles.Count; i++)
                    {
                        if (i == 0)
                        {
                            beginComand.Append($"'{listDelitedFiles[i].Path}'");
                            if(listDelitedFiles.Count == 1)
                            {
                                beginComand.Append($")");
                                var insertCommandFilesTable = new SQLiteCommand(connection)
                                {
                                    CommandText = beginComand.ToString()
                                };
                                insertCommandFilesTable.ExecuteNonQuery();
                                break;
                            }
                            continue;
                        }
                        if (i == listDelitedFiles.Count - 1)
                        {
                            beginComand.Append($", '{listDelitedFiles[i].Path}')");

                            var insertCommandFilesTable = new SQLiteCommand(connection)
                            {
                                CommandText = beginComand.ToString()
                            };
                            insertCommandFilesTable.ExecuteNonQuery();
                        }
                        else if (i % 10000 == 0)
                        {
                            beginComand.Append($", '{listDelitedFiles[i].Path}')");

                            var insertCommandFilesTable = new SQLiteCommand(connection)
                            {
                                CommandText = beginComand.ToString()
                            };
                            insertCommandFilesTable.ExecuteNonQuery();
                        }
                        else if (i > 1000 && i % 10000 == 1)
                        {
                            beginComand.Clear().Append(startQuery);
                            beginComand.Append($"'{listDelitedFiles[i].Path}'");
                        }

                        else
                        {
                            beginComand.Append($", '{listDelitedFiles[i].Path}'");
                        }
                    }
                }
            }
            else
            {
                var listDelitedFiles = new DataBaseConverter().ConvertFormatFileTreeToDB(new ObservableCollection<FileTree>() { file });
                using (var connection = new SQLiteConnection(DataBaseOptions.Options))
                {
                    var insertCommandFilesTable = new SQLiteCommand(connection)
                    {
                        CommandText = $"DELETE FROM FilesTable WHERE Path = '{file.Path}';"
                    };
                    insertCommandFilesTable.ExecuteNonQuery();
                }
            }
        }
        public static EtalonAndChecksInfoDB GetInfo()
        {
            try
            {
                using (var connection = new SQLiteConnection(DataBaseOptions.Options))
                {
                    var getInfoCommand = new SQLiteCommand(connection)
                    {
                        CommandText = $"SELECT Creator, Date, DateLastCheck, TotalFiles, Checked, PartialChecked, FailedChecked, NoAccess, NotFound, NotChecked FROM CheksTable"
                    };
                    var info = getInfoCommand.ExecuteQuery<EtalonAndChecksInfoDB>()[0];
                    return info;
                }
            }
            catch (Exception ex)
            {
                Logger.logger.Error($"Не удалось выгрузить информацию об эталоне. Отсутствует таблица. {ex.Message}");
                return new EtalonAndChecksInfoDB();
            }

        }
    }
}