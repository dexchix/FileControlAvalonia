using FileControlAvalonia.DataBase;
using FileControlAvalonia.FileTreeLogic;
using FileControlAvalonia.Models;
using FileControlAvalonia.ViewModels;
using Newtonsoft.Json;
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
    public static class EtalonManager
    {
        //public static ObservableCollection<FileTree> CurentEtalon = GetEtalon();
        //public static EtalonAndChecksInfoDB CheckInfo = GetInfo();



        /// <summary>
        /// Добавляет файлы в эталон или создает новый.
        /// </summary>
        /// <param name="mainFileTreeCollection"></param>
        /// <param name="createEalon">Если true - создает эталон, если false - добавляет файлы</param>
        public static void AddFilesOrCreateEtalon(ObservableCollection<FileTree> mainFileTreeCollection, bool createEalon)
        {
            FilesCollectionManager.SetEtalonValues(mainFileTreeCollection);
            var converter = new DataBase.DataBaseConverter();
            var etalonFilesCollection = converter.ConvertFormatFileTreeToDB(mainFileTreeCollection);

            Locator.Current.GetService<MainWindowViewModel>().ProgressBarMaximum = etalonFilesCollection.Count;

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

                foreach (var file in etalonFilesCollection)
                {
                    var insertCommandFilesTable = new SQLiteCommand(connection)
                    {
                        CommandText = "INSERT INTO FilesTable (Name, Path, ELastUpdate, EVersion, EHashSum, FLastUpdate, FVersion, FHashSum, ParentPath, Status, IsDirectory) " +
                                      $"VALUES ('{file.Name.ToString()}', '{file.Path.ToString()}', '{file.ELastUpdate}', '{file.EVersion}', '{file.EHashSum}', '{file.ELastUpdate}', '{file.EVersion}', '{file.EHashSum}', '{file.ParentPath}', '{file.Status}', {file.IsDirectory});"
                    };
                    try
                    {
                        insertCommandFilesTable.ExecuteNonQuery();
                        if (!createEalon)
                        {
                            Locator.Current.GetService<MainWindowViewModel>().ProgressBarValue++;
                            Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = $"Добавление {file.Path}";
                        }
                    }
                    catch(Exception ex)
                    {
                        Logger.logger.Error($" Ошибка записи файла {file.Path} в базу данных - {ex.Message}");
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
                Locator.Current.GetService<MainWindowViewModel>().ProgressBarMaximum = listDelitedFiles.Count;

                using (var connection = new SQLiteConnection(DataBaseOptions.Options))
                {
                    foreach (var deletedFile in listDelitedFiles)
                    {
                        var insertCommandFilesTable = new SQLiteCommand(connection)
                        {
                            CommandText = $"DELETE FROM FilesTable WHERE Path = '{deletedFile.Path}';"
                        };
                        try
                        {
                            insertCommandFilesTable.ExecuteNonQuery();
                            Locator.Current.GetService<MainWindowViewModel>().ProgressBarValue++;
                            Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = $"Удаляется {deletedFile.Path}";
                        }
                        catch(Exception ex)
                        {
                            Logger.logger.Error($"Ошибка удаления файла {deletedFile.Path} из базы данных - {ex.Message}");
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
    }
}