using FileControlAvalonia.DataBase;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Core
{
    public static class RecorderInfoBD
    {
        /// <summary>
        /// Записывает в БД дату последней проверки
        /// </summary>
        /// <param name="dateLastCheck"></param>
        public static void RecordDateOfLastCheck(string dateLastCheck)
        {
            using (var connection = new SQLiteConnection(DataBaseOptions.Options))
            {
                var insertInfoCommand = new SQLiteCommand(connection)
                {
                    CommandText = $"UPDATE CheksTable SET DateLastCheck = '{dateLastCheck}'"
                };
                insertInfoCommand.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Записывает в БД актуальную и
        /// </summary>
        /// <param name="totalFiles"></param>
        /// <param name="checkedD"></param>
        /// <param name="partialChecked"></param>
        /// <param name="unChecked"></param>
        /// <param name="noAccess"></param>
        /// <param name="notFound"></param>
        /// <param name="notChecked"></param>
        public static void RecordInfoCountFiles(int totalFiles, int checkedD, int partialChecked, int unChecked, int noAccess, int notFound, int notChecked)
        {
            using (var connection = new SQLiteConnection(DataBaseOptions.Options))
            {
                var insertInfoCommand = new SQLiteCommand(connection)
                {
                    CommandText = $"UPDATE CheksTable SET TotalFiles = '{totalFiles}', Checked = '{checkedD}', " +
                    $"PartialChecked = '{partialChecked}',FailedChecked = '{unChecked}',NoAccess = '{noAccess}',NotFound = '{notFound}', NotChecked = '{notChecked}'"
                };
                insertInfoCommand.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Записывает в БД дату и пользователя создания эталона
        /// </summary>
        /// <param name="userLevel"></param>
        /// <param name="dateCreateEtalon"></param>
        public static void RecordInfoOfCreateEtalon(string userLevel, string dateCreateEtalon)
        {
            using (var connection = new SQLiteConnection(DataBaseOptions.Options))
            {
                var insertInfoCommand = new SQLiteCommand(connection)
                {
                    CommandText = $"UPDATE CheksTable SET Creator = '{userLevel}', Date = '{dateCreateEtalon}'"
                };
                insertInfoCommand.ExecuteNonQuery();
            }
        }
    }
}
