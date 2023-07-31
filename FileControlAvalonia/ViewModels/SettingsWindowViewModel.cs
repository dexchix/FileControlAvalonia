using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using FileControlAvalonia.DataBase;
using FileControlAvalonia.SettingsApp;
using FileControlAvalonia.ViewModels.Interfaces;
using Microsoft.CodeAnalysis.CSharp;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileControlAvalonia.ViewModels
{
    public class SettingsWindowViewModel : ReactiveObject, ISettingsWindowViewModel
    {
        private Settings _settings = SettingsManager.GetSettings()!;
        private string? _userVM;
        private string? _passwordVM;
        private string? _nameTableVM;
        private string? _pathOPCServerVM;
        private string? _tagTotalStatusVM;
        private string? _tagTotalNumberofFilesVM;
        private string? _tagNumberOfMatchesVM;
        private string? _tagNumberMissmatchesVM;
        private string? _tagPartiallyMatchedVM;
        private string? _tagNumberOfUnaccessedVM;
        private string? _tagNotFoundVM;
        private string? _avalibleFileExtensionsVM;
        private string? _accessParametrForCheckButtonVM;
        private string? _rootPath;

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
        public string NameTableVM
        {
            get => _nameTableVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _nameTableVM, value);
                _settings.NameTable = value;
            }
        }
        public string PathOPCServerVM
        {
            get => _pathOPCServerVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _pathOPCServerVM, value);
                _settings.PathOPCServer = value;
            }
        }
        public string TagTotalStatusVM
        {
            get => _tagTotalStatusVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _tagTotalStatusVM, value);
                _settings.TagTotalStatus = value;
            }
        }
        public string TagTotalNumberofFilesVM
        {
            get => _tagTotalNumberofFilesVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _tagTotalNumberofFilesVM, value);
                _settings.TagTotalNumberofFiles = value;
            }
        }
        public string TagNumberOfMatchesVM
        {
            get => _tagNumberOfMatchesVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _tagNumberOfMatchesVM, value);
                _settings.TagNumberOfMatches = value;
            }
        }
        public string TagNumberMissmatchesVM
        {
            get => _tagNumberMissmatchesVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _tagNumberMissmatchesVM, value);
                _settings.TagNumberMissmatches = value;
            }
        }
        public string TagPartiallyMatchedVM
        {
            get => _tagPartiallyMatchedVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _tagPartiallyMatchedVM, value);
                _settings.TagPartiallyMatched = value;
            }
        }
        public string TagNumberOfUnaccessedVM
        {
            get => _tagNumberOfUnaccessedVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _tagNumberOfUnaccessedVM, value);
                _settings.TagNumberOfUnaccessed = value;
            }
        }
        public string TagNotFoundVM
        {
            get => _tagNotFoundVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _tagNotFoundVM, value);
                _settings.TagNotFound = value;
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

        public SettingsWindowViewModel()
        {
            _settings = _settings ?? new Settings();
            _userVM = _settings.User;
            _passwordVM = _settings.Password;
            _nameTableVM = _settings.NameTable;
            _pathOPCServerVM = _settings.PathOPCServer;
            _tagTotalStatusVM = _settings.TagTotalStatus;
            _tagTotalNumberofFilesVM = _settings.TagTotalNumberofFiles;
            _tagNumberOfMatchesVM = _settings.TagNumberOfMatches;
            _tagNumberMissmatchesVM = _settings.TagNumberMissmatches;
            _tagPartiallyMatchedVM = _settings.TagPartiallyMatched;
            _tagNumberOfUnaccessedVM = _settings.TagNumberOfUnaccessed;
            _tagNotFoundVM = _settings.TagNotFound;
            _avalibleFileExtensionsVM = _settings.AvalibleFileExtensions;
            _accessParametrForCheckButtonVM = _settings.AccessParametrForCheckButton;
            _rootPath = _settings.RootPath;
        }

        #region COMMANDS
        public void CloseWindow(Window window)
        {
            window.Close();
        }
        public void Confirm(Window window)
        {
            SettingsManager.SetSettings(_settings);
            window.Close();
        }
        public void ChangePassword(TextBox awdawd)
        {
            //awdawd.PasswordChar = null;
            //awdawd.BorderBrush = Brushes.Red;
            //awdawd.BorderThickness = Thickness.Parse("3");
            awdawd.IsReadOnly = false;
            awdawd.PasswordChar = '\0';
            awdawd.Clear();
            awdawd.Watermark = "Введите новый пароль";

            //var buttonStyle = new Style()
            //{
            //    Setters =
            //    {
            //        new Setter(Button.BorderBrushProperty, new SolidColorBrush(Colors.Red)),
            //        new Setter(Button.BorderThicknessProperty, new Thickness(1)),
            //        new Setter(Button.BackgroundProperty, new SolidColorBrush(Colors.Gray))
            //    }
            //};


            //var buttonStyle1 = new Style(x => x.OfType<TextBox>().Class(":pointerover"))
            //{
            //    Setters = {
            //    new Setter(Button.BorderBrushProperty, new SolidColorBrush(Colors.Red)),
            //        new Setter(Button.BorderThicknessProperty, new Thickness(1)),
            //        new Setter(Button.BackgroundProperty, new SolidColorBrush(Colors.Gray))
            //    }
            //};

            //awdawd.Styles.Add(buttonStyle);
            //awdawd.Styles.Add(buttonStyle1);
        }
        #endregion

        //(x)=>x.Class("TextBlock:pointerover /template/ ContentPresenter")
        /* 
         * <TextBox.Styles>
							<Style Selector="TextBox">
								<Setter Property="Background" Value="Red"/>
							</Style>
							<Style Selector="TextBox:pointerover /template/ Border">
								<Setter Property="Background" Value="Red"/>
							</Style>
							<Style Selector="TextBox:focus /template/ Border">
								<Setter Property="Background" Value="Red"/>
							</Style>
						</TextBox.Styles>
         */
    }
}
