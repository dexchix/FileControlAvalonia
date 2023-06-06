using FileControlAvalonia.Models;
using FileControlAvalonia.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileControlAvalonia.Services
{
    public class SettingsManager
    {
        public static void SaveSettings(Settings settings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            using (StreamWriter streamWriter = new StreamWriter("Settings.xml"))
            {
                serializer.Serialize(streamWriter, settings);
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
                return null;
            }
        }
    }
}
