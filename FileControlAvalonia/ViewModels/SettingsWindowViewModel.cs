using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileControlAvalonia.ViewModels
{
    [XmlRoot("SettingsWindowViewModel")]
    public class SettingsWindowViewModel : ReactiveObject
    {
        private string _serverVM;
        private string _dataBaseVM;
        private string _userVM;
        private string _passwordVM;
        private string _nameTableVM;
        private string _pathOPCServerVM;
        private string _tagTotalStatusVM;
        private string _tagTotalNumberofFilesVM;
        private string _tagNumberOfMatchesVM;
        private string _tagNumberMissmatchesVM;
        private string _tagPartiallyMatchedVM;
        private string _tagNumberOfUnaccessedVM;
        private string _tagNotFoundVM;
        private string _avalibleFileExtensionsVM;
        private string _accessParametrForCheckButtonVM;

        public string ServerVM { get => _serverVM; set => this.RaiseAndSetIfChanged(ref _serverVM, value); }
        public string DataBaseVM { get => _dataBaseVM; set => this.RaiseAndSetIfChanged(ref _dataBaseVM, value); }
        public string UserVM { get => _userVM; set => this.RaiseAndSetIfChanged(ref _userVM, value); }
        public string PasswordVM { get => _passwordVM; set => this.RaiseAndSetIfChanged(ref _passwordVM, value); }
        public string NameTableVM { get => _nameTableVM; set => this.RaiseAndSetIfChanged(ref _nameTableVM, value); }
        public string PathOPCServerVM { get => _pathOPCServerVM; set => this.RaiseAndSetIfChanged(ref _pathOPCServerVM, value); }
        public string TagTotalStatusVM { get => _tagTotalStatusVM; set => this.RaiseAndSetIfChanged(ref _tagTotalStatusVM, value); }
        public string TagTotalNumberofFilesVM { get => _tagTotalNumberofFilesVM; set => this.RaiseAndSetIfChanged(ref _tagTotalNumberofFilesVM, value); }
        public string TagNumberOfMatchesVM { get => _tagNumberOfMatchesVM; set => this.RaiseAndSetIfChanged(ref _tagNumberOfMatchesVM, value); }
        public string TagNumberMissmatchesVM { get => _tagNumberMissmatchesVM; set => this.RaiseAndSetIfChanged(ref _tagNumberMissmatchesVM, value); }
        public string TagPartiallyMatchedVM { get => _tagPartiallyMatchedVM; set => this.RaiseAndSetIfChanged(ref _tagPartiallyMatchedVM, value); }
        public string TagNumberOfUnaccessedVM { get => _tagNumberOfUnaccessedVM; set => this.RaiseAndSetIfChanged(ref _tagNumberOfUnaccessedVM, value); }
        public string TagNotFoundVM { get => _tagNotFoundVM; set => this.RaiseAndSetIfChanged(ref _tagNotFoundVM, value); }
        public string AvalibleFileExtensionsVM { get => _avalibleFileExtensionsVM; set => this.RaiseAndSetIfChanged(ref _avalibleFileExtensionsVM, value); }
        public string AccessParametrForCheckButtonVM { get => _accessParametrForCheckButtonVM; set => this.RaiseAndSetIfChanged(ref _accessParametrForCheckButtonVM, value); }
     
        
        #region COMMANDS
        public void CloseWindow(Window window)
        {
            window.Close();
        }
        public void Confirm()
        {

        }
        #endregion
    }
}
