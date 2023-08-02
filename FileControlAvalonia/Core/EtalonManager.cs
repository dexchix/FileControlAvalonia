using FileControlAvalonia.DataBase;
using FileControlAvalonia.FileTreeLogic;
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
    public class EtalonManager
    {
        public static ObservableCollection<FileTree> CurentEtalon = GetEtalon();
        public static int CountFilesEtalon { get; set; }
        public static void AddFilesOrCreateEtalon(ObservableCollection<FileTree> mainFileTreeCollection, bool createEalon)
        {
            FilesCollectionManager.SetEtalonValues(mainFileTreeCollection);
            var converter = new DataBase.DataBaseConverter();
            var etalonFilesCollection = converter.ConvertFormatFileTreeToDB(mainFileTreeCollection);

            using (var connection = new SQLiteConnection(DataBaseOptions.Options))
            {
                if(createEalon == true)
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
                        CommandText = "INSERT INTO FilesTable (ID, ParentID, Name, Path, ELastUpdate, EVersion, EHashSum, FLastUpdate, FVersion, FHashSum, ParentPath, Status) " +
                                   $"VALUES ({file.ID}, {file.ParentID}, '{file.Name}', '{file.Path}', '{file.ELastUpdate}', '{file.EVersion}', '{file.EHashSum}', '{file.ELastUpdate}', '{file.EVersion}', '{file.EHashSum}', '{file.ParentPath}', '{file.Status}');"
                    };

                    insertCommandFilesTable.ExecuteNonQuery();
                }

                var commandClearTableCheks = new SQLiteCommand(connection)
                {
                    CommandText = "DELETE FROM CheksTable"
                };
                commandClearTableCheks.ExecuteNonQuery();

                var insertCommandChecksTable = new SQLiteCommand(connection)
                {
                    CommandText = "INSERT INTO CheksTable (ID, Creator, Date) " +
                                   $"VALUES ({1}, 'Admin' , '{DateTime.Now.ToString()}');"
                };
                insertCommandChecksTable.ExecuteNonQuery();
            }
        }

        public static ObservableCollection<FileTree> GetEtalon()
        {
            List<FileDB> etalon;

            using (var connection = new SQLiteConnection(DataBaseOptions.Options))
            {
                var command = new SQLiteCommand(connection)
                {
                    CommandText = "SELECT ID, ParentID, Name, Path, ELastUpdate, EVersion, EHashSum, FLastUpdate, FVersion, FHashSum, ParentPath, Status FROM FilesTable"
                };
                etalon = command.ExecuteQuery<FileDB>();
            }
            var converter = new DataBaseConverter();
            var etalonInDBContext = converter.ConvertFormatDBToFileTreeCollection(etalon);
            CountFilesEtalon = etalon.Count;
            FileTree._counter = -1;
            return etalonInDBContext;
        }

        public static void DeliteFileInDB(FileTree file)
        {
            if (file.Children != null)
            {
                var listDelitedFiles = new DataBaseConverter().ConvertFormatFileTreeToDB(new ObservableCollection<FileTree>() {file});
                using (var connection = new SQLiteConnection(DataBaseOptions.Options))
                {
                    foreach (var diletedFile in listDelitedFiles)
                    {
                        var insertCommandFilesTable = new SQLiteCommand(connection)
                        {
                            CommandText = $"DELETE FROM FilesTable WHERE Path = '{diletedFile.Path}';"
                        };
                        insertCommandFilesTable.ExecuteNonQuery();
                    }
                }
            }
            else
            {
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
    }
}
