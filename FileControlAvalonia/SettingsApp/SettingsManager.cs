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

        public static void SetStartupSettings()
        {

            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Settings.xml")))
            {
                GetSettings();
            }
            else
            {
                var settings = new Settings();
                settings.Password = DataBaseOptions.CryptPassword(DataBaseOptions.Password);
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                using (StreamWriter streamWriter = new StreamWriter("Settings.xml"))
                {
                    serializer.Serialize(streamWriter, settings);
                }
                settings.Password = DataBaseOptions.Password;
                AppSettings = settings;
                RootPath = settings.RootPath;
            }
        }

        public static void SetSettings(Settings settings)
        {
            try
            {
                settings.Password = DataBaseOptions.CryptPassword(settings.Password);

                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                using (StreamWriter streamWriter = new StreamWriter("Settings.xml"))
                {
                    serializer.Serialize(streamWriter, settings);
                }
                SettingsString = settings.AvalibleFileExtensions;
                extensions.Clear();
                ModifyExtensions.Clear();
                if (settings.AvalibleFileExtensions != null)
                {
                    extensions = settings.AvalibleFileExtensions!.Split('/').ToList();
                    if (extensions.Count > 0 && extensions[0] != "")
                    {
                        foreach (string extension in extensions)
                        {
                            ModifyExtensions.Add("." + extension);
                        }
                    }
                }
                RootPath = settings.RootPath;
                AppSettings = settings;
            }
            catch
            {
                LogManager.GetCurrentClassLogger().Error("Отсутствует файл Settings.xml");
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
                    settings.Password = DataBaseOptions.DecryptPassword(settings.Password);
                    AppSettings = settings;
                    RootPath = settings.RootPath;
                    return AppSettings;
                }
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex);
                var settings = new Settings();

                var password = DataBaseOptions.Password;
                var cryptPassword = DataBaseOptions.CryptPassword(DataBaseOptions.Password);

                settings.Password = cryptPassword;

                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                using (StreamWriter streamWriter = new StreamWriter("Settings.xml"))
                {
                    serializer.Serialize(streamWriter, settings);
                }
                settings.Password = password;
                AppSettings = settings;
                return AppSettings;
            }
        }
    }
}
