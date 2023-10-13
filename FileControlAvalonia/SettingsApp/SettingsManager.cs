using FileControlAvalonia.DataBase;
using FileControlAvalonia.ViewModels;
using NLog;
using Splat;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileControlAvalonia.SettingsApp
{
    public static class SettingsManager
    {
        private static List<string> extensions = new List<string>();

        public static List<string> ModifyExtensions { get; set; } = new List<string>();
        public static string RootPath { get; set; }
        public static string? SettingsString { get; set; }
        public static Settings AppSettings { get; set; }

        public static void SetStartupSettings() => GetSettings();


        public static void SetSettings(Settings settings)
        {
            try
            {
                var decryptPassword = settings.Password;
                settings.Password = DataBaseManager.CryptPassword(settings.Password);

                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                using (StreamWriter streamWriter = new StreamWriter("Settings.xml"))
                {
                    serializer.Serialize(streamWriter, settings);
                }

                settings.Password = decryptPassword;
                RootPath = settings.RootPath;
                AppSettings = settings;
                SetExtensions(AppSettings);
            }
            catch
            {
                LogManager.GetCurrentClassLogger().Error("Отсутствует файл Settings.xml");
            }
        }

        private static void SetExtensions(Settings settings)
        {
            SettingsString = settings.AvalibleFileExtensions;
            extensions.Clear();
            ModifyExtensions.Clear();
            if (settings.AvalibleFileExtensions != null)
            {
                extensions = settings.AvalibleFileExtensions!.Split('/').ToList();
                if (extensions.Count > 0 && extensions[0] != string.Empty)
                {
                    foreach (string extension in extensions)
                    {
                        ModifyExtensions.Add("." + extension);
                    }
                }
            }
        }

        public static Settings? GetSettings()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                using (StreamReader streamReader = new StreamReader("Settings.xml"))
                {
                    var settings = serializer.Deserialize(streamReader) as Settings;
                    settings.Password = DataBaseManager.DecryptPassword(settings.Password);
                    AppSettings = settings;
                    SetExtensions(AppSettings);
                    RootPath = settings.RootPath;
                    return settings;
                }
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex);
                var settings = new Settings();

                var password = DataBaseManager.Password;
                var cryptPassword = DataBaseManager.CryptPassword(DataBaseManager.Password);

                settings.Password = cryptPassword;
                settings.WindowHeight = 600;
                settings.WindowWidth = 1200;
                settings.XLocation = 0;
                settings.YLocation = 0;
                settings.NameDataBase = "FileIntegrityDB";
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                using (StreamWriter streamWriter = new StreamWriter("Settings.xml"))
                {
                    serializer.Serialize(streamWriter, settings);
                }
                settings.Password = password;
                AppSettings = settings;
                SetExtensions(AppSettings);
                return AppSettings;
            }
        }
    }
}
