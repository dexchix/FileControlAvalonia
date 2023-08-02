using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FileControlAvalonia.SettingsApp;
using SQLite;
using SQLitePCL;

namespace FileControlAvalonia.DataBase
{
    public static class DataBaseOptions
    {
        public static string NameDB { get; private set; } = "FileIntegrityDB.db";
        public static string Password { get; private set; } = "Bzpa/123456789";

        public static SQLiteConnectionString Options { get; set; }

        public static void InitializeDataBaseSettings()
        {
            Password = DecryptPassword(SettingsManager.AppSettings.Password);
            Options = new SQLiteConnectionString(NameDB, true, Password);
        }
        public static void ChangeDataBaseOptions()
        {
            Password = SettingsManager.AppSettings.Password;
            Options = new SQLiteConnectionString(NameDB, false, Password);
        }


        private static byte[] key = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        private static byte[] iv = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };

        public static string CryptPassword(this string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
            byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }

        public static string DecryptPassword(this string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
            byte[] inputbuffer = Convert.FromBase64String(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.Unicode.GetString(outputBuffer);
        }
    }
}
