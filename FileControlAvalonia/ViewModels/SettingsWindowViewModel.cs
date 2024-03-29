﻿using Avalonia.Controls;
using FileControlAvalonia.DataBase;
using FileControlAvalonia.SettingsApp;
using ReactiveUI;
using MessageBox;


using System;
using MessageBox.Avalonia.Enums;
using MessageBox.Avalonia;
using Splat;
using FileControlAvalonia.Views;
using Newtonsoft.Json.Linq;

namespace FileControlAvalonia.ViewModels
{
    public class SettingsWindowViewModel : ReactiveObject
    {
        private Settings _settings = SettingsManager.GetSettings()!;
        private string? _userVM;
        private string? _passwordVM;
        private string? _nameDataBaseVM;
        private string? _opcConnectionStringVM;
        private string? _opcCommonTagVM;
        private string? _OpcCountTagVM;
        private string? _opcPassedTagVM;
        private string? _opcFailedTagVM;
        private string? _opcSemiPassedTagVM;
        private string? _opcNoAccessTagVM;
        private string? _opcNotFoundTagVM;
        private string? _avalibleFileExtensionsVM;
        private string? _accessParametrForCheckButtonVM;
        private string? _rootPath;
        private string _oldPassword;
        private string? _oldNameDB;
        private string _windowHeightVM;
        private string _windowWidthVM;
        private string _xLocationVM;
        private string _yLocationVM;

        private double? _windowHeight;
        private double? _windowWidth;
        private double? _xLocation;
        private double? _yLocation;

        private bool _dragAndDropWindowVM;
        //private 

        private bool _isEnabledPasswordTextBox = false;
        private event Action<double, double> ResizeWindow;
        public event Action<double, double> ChangeLocationWindow;

        public string UserVM
        {
            get => _userVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _userVM, value);
                _settings.User = value;
            }
        }
        public string PasswordVM
        {
            get => _passwordVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _passwordVM, value);
                _settings.Password = value;
            }
        }
        public string NameDataBaseVM
        {
            get => _nameDataBaseVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _nameDataBaseVM, value);
                _settings.NameDataBase = value;
            }
        }
        public string OpcConnectionStringVM
        {
            get => _opcConnectionStringVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _opcConnectionStringVM, value);
                _settings.OpcConnectionString = value;
            }
        }
        public string OpcCommonTagVM
        {
            get => _opcCommonTagVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _opcCommonTagVM, value);
                _settings.OpcCommonTag = value;
            }
        }
        public string OpcCountTagVM
        {
            get => _OpcCountTagVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _OpcCountTagVM, value);
                _settings.OpcCountTag = value;
            }
        }
        public string OpcPassedTagVM
        {
            get => _opcPassedTagVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _opcPassedTagVM, value);
                _settings.OpcPassedTag = value;
            }
        }
        public string OpcFailedTagVM
        {
            get => _opcFailedTagVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _opcFailedTagVM, value);
                _settings.OpcFailedTag = value;
            }
        }
        public string OpcSemiPassedTagVM
        {
            get => _opcSemiPassedTagVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _opcSemiPassedTagVM, value);
                _settings.OpcSemiPassedTag = value;
            }
        }
        public string OpcNoAccessTagVM
        {
            get => _opcNoAccessTagVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _opcNoAccessTagVM, value);
                _settings.OpcNoAccessTag = value;
            }
        }
        public string OpcNotFoundTagVM
        {
            get => _opcNotFoundTagVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _opcNotFoundTagVM, value);
                _settings.OpcNotFoundTag = value;
            }
        }
        public string AvalibleFileExtensionsVM
        {
            get => _avalibleFileExtensionsVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _avalibleFileExtensionsVM, value);
                _settings.AvalibleFileExtensions = value;
            }
        }
        public string AccessParametrForCheckButtonVM
        {
            get => _accessParametrForCheckButtonVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _accessParametrForCheckButtonVM, value);
                _settings.AccessParametrForCheckButton = value;
            }
        }
        public string RootPath
        {
            get => _rootPath!;
            set
            {
                this.RaiseAndSetIfChanged(ref _rootPath, value);
                _settings.RootPath = value;
            }
        }

        public string WindowHeightVM
        {
            get => _windowHeightVM!;
            set
            {

                this.RaiseAndSetIfChanged(ref _windowHeightVM, value);
                //_settings.WindowHeight = Convert.ToInt32(value);
            }
        }
        public string WindowWidthVM
        {
            get => _windowWidthVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _windowWidthVM, value);
                //_settings.WindowWidth = Convert.ToInt32(value);
            }
        }
        public string XLocationVM
        {
            get => _xLocationVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _xLocationVM, value);
                //_settings.XLocation = Convert.ToInt32(value);
            }
        }

        public string YLocationVM
        {
            get => _yLocationVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _yLocationVM, value);
                //_settings.YLocation = Convert.ToInt32(value);
            }
        }
        public bool DragAndDropWindowVM
        {
            get => _dragAndDropWindowVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _dragAndDropWindowVM, value);
                _settings.DragAndDropWindow = value;
            }
        }

        public bool IsEnabledPasswordTextBox
        {
            get => _isEnabledPasswordTextBox;
            set => this.RaiseAndSetIfChanged(ref _isEnabledPasswordTextBox, value);
        }

        public SettingsWindowViewModel()
        {
            _settings = _settings ?? new Settings();
            _userVM = _settings.User;
            _passwordVM = _settings.Password;

            _oldPassword = _settings.Password;
            _oldNameDB = _settings.NameDataBase;

            _nameDataBaseVM = _settings.NameDataBase == null ? DataBaseManager.NameDB : _settings.NameDataBase;
            _opcConnectionStringVM = _settings.OpcConnectionString;
            _opcCommonTagVM = _settings.OpcCommonTag;
            _OpcCountTagVM = _settings.OpcCountTag;
            _opcPassedTagVM = _settings.OpcPassedTag;
            _opcFailedTagVM = _settings.OpcFailedTag;
            _opcSemiPassedTagVM = _settings.OpcSemiPassedTag;
            _opcNoAccessTagVM = _settings.OpcNoAccessTag;
            _opcNotFoundTagVM = _settings.OpcNotFoundTag;
            _avalibleFileExtensionsVM = _settings.AvalibleFileExtensions == null || _settings.AvalibleFileExtensions == string.Empty ? "*,*" : _settings.AvalibleFileExtensions;
            _accessParametrForCheckButtonVM = _settings.AccessParametrForCheckButton;
            _rootPath = _settings.RootPath;

            _windowHeightVM = _settings.WindowHeight == null ? "600" : _settings.WindowHeight.ToString();
            _windowWidthVM = _settings.WindowWidth == null ? "1200" : _settings.WindowWidth.ToString();
            //_xLocationVM = _settings.XLocation == null ? "0" : _settings.XLocation.ToString();
            //_yLocationVM = _settings.YLocation == null ? "0" : _settings.YLocation.ToString();

            _xLocationVM = Locator.Current.GetService<MainWindow>().Position.X.ToString();
            _yLocationVM = Locator.Current.GetService<MainWindow>().Position.Y.ToString();

            _dragAndDropWindowVM = _settings.DragAndDropWindow == null ? false : _settings.DragAndDropWindow;


            _windowHeight = _settings.WindowHeight;
            _windowWidth = _settings.WindowWidth;
            _xLocation = _settings.XLocation;
            _yLocation = _settings.YLocation;

            ResizeWindow += Locator.Current.GetService<MainWindow>().ResizeWindow;
            ResizeWindow += Locator.Current.GetService<MainWindowViewModel>().ResizeWindow;
            ChangeLocationWindow += Locator.Current.GetService<MainWindow>().ChangeLocation;


        }

        #region COMMANDS
        public void CloseWindow(Window window)
        {
            window.Close();
        }
        public void Confirm(Window window)
        {
            if (PasswordVM != _oldPassword && !string.IsNullOrWhiteSpace(PasswordVM))
                DataBaseManager.ChangePasswordDataBase(PasswordVM);
            else if (PasswordVM != _oldPassword && string.IsNullOrWhiteSpace(PasswordVM))
                _settings.Password = _oldPassword;

            if (NameDataBaseVM != _oldNameDB && !string.IsNullOrWhiteSpace(NameDataBaseVM))
                DataBaseManager.ChangeDataBaseName(NameDataBaseVM);
            else if (NameDataBaseVM != _oldNameDB && string.IsNullOrWhiteSpace(NameDataBaseVM))
                _settings.NameDataBase = _oldNameDB;

            _settings.WindowHeight = Convert.ToInt32(WindowHeightVM);
            _settings.WindowWidth = Convert.ToInt32(WindowWidthVM);


            var mainWindow = Locator.Current.GetService<MainWindow>();
            if (DragAndDropWindowVM)
                mainWindow.title.PointerPressed += mainWindow.DragMoveWindow;
            else
                mainWindow.title.PointerPressed -= mainWindow.DragMoveWindow;


            IsEnabledPasswordTextBox = false;


            if (SettingsManager.AppSettings.WindowHeight != _windowHeight || SettingsManager.AppSettings.WindowWidth != _windowWidth)
            {
                ResizeWindow.Invoke((double)_settings.WindowWidth, (double)_settings.WindowHeight);
            }
            var XLocation = Convert.ToInt32(XLocationVM);
            var YLocation = Convert.ToInt32(YLocationVM);
            if (SettingsManager.AppSettings.XLocation != XLocation || SettingsManager.AppSettings.YLocation != YLocation)
            {
                ChangeLocationWindow.Invoke((double)(XLocation), (double)(YLocation));
                if (SettingsManager.AppSettings.DragAndDropWindow)
                {
                    mainWindow.title.PointerPressed -= mainWindow.DragMoveWindow;
                    mainWindow.title.PointerPressed += mainWindow.DragMoveWindow;
                }
            }
            _settings.XLocation = Convert.ToDouble(XLocationVM);
            _settings.YLocation = Convert.ToDouble(YLocationVM);

            SettingsManager.SetSettings(_settings);
            window.Close();
        }
        public void ChangePassword(TextBox textBox)
        {
            textBox.IsReadOnly = false;
            IsEnabledPasswordTextBox = true;
            textBox.PasswordChar = '\0';
            textBox.Clear();
            textBox.Watermark = "Введите новый пароль";
        }
        #endregion

    }
}
