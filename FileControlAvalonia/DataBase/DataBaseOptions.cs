using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FileControlAvalonia.SettingsApp;
using SQLite;
using SQLitePCL;
using ServiceLib;

namespace FileControlAvalonia.DataBase
{
    public static class DataBaseOptions
    {
        public static string NameDB { get; private set; } = "FileIntegrityDB.db";
        public static string Password { get; private set; } = "Bzpa/123456789";

        public static SQLiteConnectionString Options { get; set; }

        public static void SetOptions()
        {
            Password = SettingsManager.AppSettings.Password;
            Options = new SQLiteConnectionString(NameDB, true, Password);
        }

        #region CRYPT_Password

        public static string CryptPassword(this string text) => EncryptingFunctions.EncryptPasswordReg(text);


        public static string DecryptPassword(this string text) => EncryptingFunctions.DecryptPasswordReg(text);
        #endregion

        //result.Add("Database password", ServiceLib.EncryptingFunctions.Encrypt(Password));
    }
}
