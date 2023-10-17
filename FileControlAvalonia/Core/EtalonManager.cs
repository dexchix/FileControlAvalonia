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
        /// <summary>
        /// Добавляет файлы в эталон (БД) или создает новый (перезаполняет таблицу в БД).
        /// </summary>
        /// <param name="mainFileTreeCollection"></param>
        /// <param name="createEalon">Если true - создает эталон, если false - добавляет файлы</param>
        public async static Task AddFilesOrCreateEtalon(ObservableCollection<FileTree> mainFileTreeCollection, bool createEalon)
        {
            var listFiles = FilesCollectionManager.UpdateTreeToList(mainFileTreeCollection);

            var asyncConnection = new SQLiteAsyncConnection(DataBaseManager.Options);

            if (createEalon == true)
                await asyncConnection.DeleteAllAsync<FileTree>();

            await asyncConnection.InsertAllAsync(listFiles);

            asyncConnection.CloseAsync();

        }

        public static async Task<ObservableCollection<FileTree>> GetEtalon()
        {

            var asyncConnection = new SQLiteAsyncConnection(DataBaseManager.Options);
            var files = await asyncConnection.Table<FileTree>().ToListAsync();
            var outputCollection = new Converter().ConvertListToHierarchicalCollection(files);
            await asyncConnection.CloseAsync();

            return outputCollection;
        }

        public static async Task DeliteFileInDB(FileTree file)
        {
            var asyncConnection = new SQLiteAsyncConnection(DataBaseManager.Options);

            if (file.Children != null)
            {
                var listFiles = FilesCollectionManager.UpdateTreeToList(new ObservableCollection<FileTree>() { file });

                var primaryKeysToDelete = listFiles.Select(obj => obj.ID).ToArray();
                string placeholders = string.Join(",", primaryKeysToDelete);

                string deleteQuery = $"DELETE FROM FileTree WHERE ID IN ({placeholders})";
                await asyncConnection.ExecuteAsync(deleteQuery);
                await asyncConnection.CloseAsync();
            }
            else
            {
                string deleteQuery = $"DELETE FROM FileTree WHERE ID = {file.ID}";
                await asyncConnection.ExecuteAsync(deleteQuery);
                await asyncConnection.CloseAsync();
            }
        }
        public static EtalonAndChecksInfoDB GetInfo()
        {
            try
            {
                using (var connection = new SQLiteConnection(DataBaseManager.Options))
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