using FileControlAvalonia.Core;
using FileControlAvalonia.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileControlAvalonia.Services
{
    public static class SettingsManager
    {
        private static List<string> extensions = new List<string>();
        public static List<string> modifyExtensions = new List<string>();
        public static string? settingsString;

        public static void SetStartupSettings()
        {
            var settings = GetSettings();
            SaveSettings(settings);
        }

        public static void SaveSettings(Settings settings)
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
                extensions = settings.AvalibleFileExtensions!.Split('/').ToList();
                if (extensions.Count > 0 && extensions[0] != "")
                {
                    foreach (string extension in extensions)
                    {
                        modifyExtensions.Add("." + extension);
                    }
                }
            }
            catch
            {
                Program.logger.Error("Отсутствует файл Settings.xml");
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
                Program.logger.Error(ex);
                return new Settings();
            }
        }
    }
}
