using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Reflection;

namespace FileControlAvalonia.ViewModels
{
    public class InfoWindowViewModel: ReactiveObject
    {
        Assembly assembly = Assembly.GetExecutingAssembly(); // Получаем сборку
        private string _nameProgram;
        private string _nameCompany;
        private string _versionApp;
        private string _hashSumApp;

        public InfoWindowViewModel()
        {
            _nameProgram = $"Программа: Контроль целостности";
            _nameCompany = $"Компания: {((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCompanyAttribute))).Company}";
            _versionApp = $"Версия приложения: {((AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyFileVersionAttribute))).Version}";
            _hashSumApp = $"Хэш-сумма дистрибутива:------"; 
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
