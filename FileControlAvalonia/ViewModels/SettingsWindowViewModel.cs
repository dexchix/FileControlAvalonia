using Avalonia.Controls;
using FileControlAvalonia.DataBase;
using FileControlAvalonia.SettingsApp;
using ReactiveUI;

namespace FileControlAvalonia.ViewModels
{
    public class SettingsWindowViewModel : ReactiveObject
    {
        private Settings _settings = SettingsManager.GetSettings()!;
        private string? _userVM;
        private string? _passwordVM;
        private string? _nameTableVM;
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
        private string _pastPassword;
        private int _windowHeightVM;
        private int _windowWidthVM;
        private int _xLocationVM;
        private int _yLocationVM;
        private bool _dragAndDropWindowVM;
        private bool _isEnabledPasswordTextBox = false;

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

        public int WindowHeightVM
        {
            get => _windowHeightVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _windowHeightVM, value);
                _settings.WindowHeight = value;
            }
        }
        public int WindowWidthVM
        {
            get => _windowWidthVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _windowWidthVM, value);
                _settings.WindowWidth = value;
            }
        }
        public int XLocationVM
        {
            get => _xLocationVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _xLocationVM, value);
                _settings.XLocation = value;
            }
        }

        public int YLocationVM
        {
            get => _yLocationVM!;
            set
            {
                this.RaiseAndSetIfChanged(ref _yLocationVM, value);
                _settings.YLocation = value;
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
            _pastPassword = _settings.Password;
            _nameTableVM = _settings.NameTable;
            _opcConnectionStringVM = _settings.OpcConnectionString;
            _opcCommonTagVM = _settings.OpcCommonTag;
            _OpcCountTagVM = _settings.OpcCountTag;
            _opcPassedTagVM = _settings.OpcPassedTag;
            _opcFailedTagVM = _settings.OpcFailedTag;
            _opcSemiPassedTagVM = _settings.OpcSemiPassedTag;
            _opcNoAccessTagVM = _settings.OpcNoAccessTag;
            _opcNotFoundTagVM = _settings.OpcNotFoundTag;
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
            if (IsEnabledPasswordTextBox == true && PasswordVM == null || PasswordVM == "")
            {

            }
            else
            {
                if (PasswordVM != _pastPassword)
                    DataBaseManager.ChangePasswordDataBase(PasswordVM);
                SettingsManager.SetSettings(_settings);
                IsEnabledPasswordTextBox = false;
                window.Close();
            }
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
