using Avalonia.X11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileControlAvalonia.Models
{
    [XmlRoot("Settings")]
    public class Settings
    {
        public string Server { get; set; }
        public string DataBase { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string NameTable { get; set; }
        public string PathOPCServer { get; set; }
        public string TagTotalStatus { get; set; }
        public string TagTotalNumberofFiles { get; set; }
        public string TagNumberOfMatches { get; set; }
        public string TagNumberMissmatches { get; set; }
        public string TagPartiallyMatched { get; set; }
        public string TagNumberOfUnaccessed { get; set; }
        public string TagNotFound { get; set; }
        public string AvalibleFileExtensions { get; set; }
        public string AccessParametrForCheckButton { get; set; }
    }
}
