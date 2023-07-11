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

namespace FileControlAvalonia.Core
{
    public static class SettingsManager
    {
        private static List<string> extensions = new List<string>();
        public static List<string> modifyExtensions = new List<string>();
        public static string rootPath;
        public static string? settingsString;

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
                settingsString = settings.AvalibleFileExtensions;
                extensions.Clear();
                modifyExtensions.Clear();
                if (settings.AvalibleFileExtensions != null)
                {
                    extensions = settings.AvalibleFileExtensions!.Split('/').ToList();
                    if (extensions.Count > 0 && extensions[0] != "")
                    {
                        foreach (string extension in extensions)
                        {
                            modifyExtensions.Add("." + extension);
                        }
                    }
                }
                rootPath = settings.RootPath;
                if (Directory.Exists(settings.RootPath))
                {
                    var mainVM = (MainWindowViewModel)Locator.Current.GetService(typeof(MainWindowViewModel));
                    mainVM.MainFileTree = new Models.FileTree(rootPath,true);
                    mainVM.MainFileTree.Children.Clear();
                }
             
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
                    return serializer.Deserialize(streamReader) as Settings;
                }
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex);
                var settings = new Settings();
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
