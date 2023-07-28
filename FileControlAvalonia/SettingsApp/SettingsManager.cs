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
            var settings = GetSettings();
            SetSettings(settings);
        }

        public static void SetSettings(Settings settings)
        {
            try
            {
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
                //if (Directory.Exists(settings.RootPath))
                //{
                //    var mainVM = (MainWindowViewModel)Locator.Current.GetService(typeof(MainWindowViewModel));
                //    mainVM.MainFileTreeCollection = new Models.FileTree(RootPath, true);
                //    mainVM.MainFileTreeCollection.Children.Clear();
                //}

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
                    AppSettings = settings;
                    return settings;
                }
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex);
                var settings = new Settings();
                settings.Password = DataBaseOptions.Password;
                AppSettings = settings;
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                using (StreamWriter streamWriter = new StreamWriter("Settings.xml"))
                {
                    serializer.Serialize(streamWriter, settings);
                }
                return settings;
            }
        }
    }
}
