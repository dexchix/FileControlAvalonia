using Avalonia.Controls;
using FileControlAvalonia.Core;
using ReactiveUI;
using System;
using System.IO;
using System.Reflection;

namespace FileControlAvalonia.ViewModels
{
    public class InfoWindowViewModel : ReactiveObject
    {
        Assembly assembly = Assembly.GetExecutingAssembly(); // Получаем сборку
        private string _nameProgram;
        private string _nameCompany;
        private string _versionApp;
        private string _hashSumApp;
        private string _launchFileName;

        public InfoWindowViewModel()
        {
            _nameProgram = $"Программа: Контроль целостности";
            _nameCompany = $"Компания: {((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCompanyAttribute))).Company}";
            _versionApp = $"Версия приложения: {((AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyFileVersionAttribute))).Version}";


            if (Environment.OSVersion.Platform == PlatformID.Win32NT) _launchFileName = "FileControlAvalonia.exe";
            else _launchFileName = "FileControlAvalonia";

            string databasePath = Path.Combine(Directory.GetCurrentDirectory(), _launchFileName);
            string hash = FactParameterizer.GetMD5Hash(databasePath);

            _hashSumApp = $"Хэш-сумма дистрибутива: {hash}";
        }

        public string NameProgram
        {
            get => _nameProgram;
            set => this.RaiseAndSetIfChanged(ref _nameProgram, value);
        }
        public string NameCompany
        {
            get => _nameCompany;
            set => this.RaiseAndSetIfChanged(ref _nameCompany, value);
        }
        public string VersionApp
        {
            get => _versionApp;
            set => this.RaiseAndSetIfChanged(ref _versionApp, value);
        }
        public string HashSumApp
        {
            get => _hashSumApp;
            set => this.RaiseAndSetIfChanged(ref _hashSumApp, value);
        }

        public void CloseWindow(Window window)
        {
            window.Close();
        }
    }
}
