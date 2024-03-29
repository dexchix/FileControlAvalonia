﻿using System.Xml.Serialization;

namespace FileControlAvalonia.SettingsApp
{
    [XmlRoot("Settings")]
    public class Settings
    {
        public string? User { get; set; }
        public string? Password { get; set; }
        public string? NameDataBase { get; set; }
        public string? OpcConnectionString { get; set; }
        public string? OpcCommonTag { get; set; }
        public string? OpcCountTag { get; set; }
        public string? OpcPassedTag { get; set; }
        public string? OpcFailedTag { get; set; }
        public string? OpcSemiPassedTag { get; set; }
        public string? OpcNoAccessTag { get; set; }
        public string? OpcNotFoundTag { get; set; }
        public string? AvalibleFileExtensions { get; set; }
        public string? AccessParametrForCheckButton { get; set; }
        public string? RootPath { get; set; }
        public double WindowHeight { get; set; }
        public double WindowWidth { get; set; }
        public double XLocation { get; set; }
        public double YLocation { get; set; }
        public bool DragAndDropWindow { get; set; }
    }
}
