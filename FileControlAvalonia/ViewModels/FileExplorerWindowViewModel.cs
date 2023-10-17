using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Platform;
using Avalonia;
using ReactiveUI;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using FileControlAvalonia.Models;
using FileControlAvalonia.Converters;
using FileControlAvalonia.Helper;
using System.Collections.ObjectModel;
using FileControlAvalonia.FileTreeLogic;
using FileControlAvalonia.SettingsApp;
using FileControlAvalonia.Core;
using Splat;
using System.Threading;

namespace FileControlAvalonia.ViewModels
{
    public class FileExplorerWindowViewModel : ReactiveObject, IDisposable
    {
        #region FIELDS
        private int _itemIndex = 0;
        private static IconConverter? s_iconConverter;
        private static FileTreeNavigator _fileTreeNavigator;
        private FileTree? _fileTree;
        private int _counterSelectedFiles = 0;
        private MainWindowViewModel _mainWindowVM = Locator.Current.GetService<MainWindowViewModel>();

        private string _progressBarText;
        private bool _enabledButtons = true;
        private bool _progressBarLoopScrol;
        private bool _progressBarIsVisible = false;
        private CancellationTokenSource _ctc = new CancellationTokenSource();
        public static event Action CancellAddOperation = delegate { CurrentVM._ctc.Cancel(); };

        #endregion

        public static void CallCancelEvent()
        {
            CancellAddOperation?.Invoke();
        }
        public static async void ChangeStateProgressBarMain()
        {
            Task.Run(async () =>
            {
                ChangeState(true);
                await Task.WhenAll(FileTree._startedTask.ToList());
                ChangeState(false);
                FileTree.TaskSelectedChildrenIsStarted = false;
                FileTree._startedTask.Clear();
            });

        }

        private static void ChangeState(bool active)
        {
            if (active)
            {
                CurrentVM.ProgressBarIsVisible = true;
                CurrentVM.ProgressBarLoopScrol = true;
                CurrentVM.EnabledButtons = false;

            }
            else
            {
                CurrentVM.ProgressBarIsVisible = false;
                CurrentVM.ProgressBarLoopScrol = false;
                CurrentVM.EnabledButtons = true;
            }

        }

        #region PROPERTIES
        public static FileExplorerWindowViewModel CurrentVM { get; set; }
        public int ItemIndex
        {
            get => _itemIndex;
            set => this.RaiseAndSetIfChanged(ref _itemIndex, value);
        }
        public string Extensions
        {
            get
            {
                if (SettingsManager.SettingsString! == null ||  SettingsManager.SettingsString! == string.Empty) return "*.*";
                else return SettingsManager.SettingsString!;
            }
        }

        #region ProgressBar
        public string ProgressBarText
        {
            get => _progressBarText;
            set => this.RaiseAndSetIfChanged(ref _progressBarText, value);
        }
        public bool EnabledButtons
        {
            get => _enabledButtons;
            set => this.RaiseAndSetIfChanged(ref _enabledButtons, value);
        }
        public bool ProgressBarLoopScrol
        {
            get => _progressBarLoopScrol;
            set => this.RaiseAndSetIfChanged(ref _progressBarLoopScrol, value);
        }
        public bool ProgressBarIsVisible
        {
            get => _progressBarIsVisible;
            set => this.RaiseAndSetIfChanged(ref _progressBarIsVisible, value);
        }
        #endregion

        public static IMultiValueConverter FileIconConverter
        {
            get
            {
                if (s_iconConverter is null)
                {
                    var assetLoader = AvaloniaLocator.Current.GetRequiredService<IAssetLoader>();

                    using (var fileStream = assetLoader.Open(new Uri("avares://FileControlAvalonia/Assets/file.png")))
                    using (var folderStream = assetLoader.Open(new Uri("avares://FileControlAvalonia/Assets/folder.png")))
                    using (var folderOpenStream = assetLoader.Open(new Uri("avares://FileControlAvalonia/Assets/folder.png")))
                    {
                        s_iconConverter = new IconConverter(new Bitmap(fileStream), new Bitmap(folderStream), new Bitmap(folderOpenStream));
                    }
                }
                return s_iconConverter;
            }
        }

        public FileTree FileTree
        {
            get => _fileTreeNavigator.FileTree;
            set => this.RaiseAndSetIfChanged(ref _fileTree, value);
        }
        #endregion

        public FileExplorerWindowViewModel()
        {
            CurrentVM = this;
            _fileTreeNavigator = new FileTreeNavigator();
            _fileTree = _fileTreeNavigator.FileTree;
            _fileTreeNavigator.PropertyChanged += OnMyPropertyChanged!;
        }
        private void OnMyPropertyChanged(object sender, EventArgs e)
        {
            FileTree = _fileTreeNavigator.FileTree!;
        }
        #region COMMANDS
        public void GoToFolderCommand()
        {
            if (ItemIndex >= 0 && Directory.Exists(FileTree.Children?[ItemIndex].Path) &&
                FileTree.Children.Count != 0)
                _fileTreeNavigator.GoToFolder(FileTree.Children[ItemIndex]);
        }
        public void GoBackFolderCommand()
        {
            if (_fileTreeNavigator.FileTree != null && _fileTreeNavigator.FileTree.Parent != null)
                _fileTreeNavigator.GoBackFolder();
        }
        public void CancelCommand(Window window)
        {

            if (FileTree != null)
            {
                if (_fileTreeNavigator.watcher != null)
                    _fileTreeNavigator.watcher.StopWatch();
                window.Close();
                CurrentVM = null;
                Dispose();
            }
            else window.Close();
        }
        async public void OkCommand(Window window)
        {
            if (FileTree != null)
            {
                if (_fileTreeNavigator.watcher != null)
                    _fileTreeNavigator.watcher.StopWatch();
                else
                {

                }
                _mainWindowVM.ProgressBarIsVisible = true;
                _mainWindowVM.ProgressBarLoopScrol = true;
                _mainWindowVM.ProgressBarText = "Загрузка";
                _mainWindowVM.ProgressBarValue = 0;
                _mainWindowVM.EnabledButtons = false;
                _mainWindowVM.CancellButtonIsVisible = true;
                _mainWindowVM.CancellButtonIsEnabled = true;

                window.Close();


                await TransitFiles();
                CurrentVM = null;
            }
            else window.Close();
        }
        public void UpCommand()
        {
            if (ItemIndex > 0)
                ItemIndex--;
            else if (ItemIndex <= 0)
                ItemIndex = FileTree.Children!.Count - 1;
        }
        public void DownCommand()
        {
            if (ItemIndex < FileTree.Children?.Count - 1)
                ItemIndex++;
            else if (ItemIndex == FileTree.Children?.Count - 1)
                ItemIndex = 0;
        }

        async private Task TransitFiles()
        {
            var childrenTFL = new ObservableCollection<FileTree>();
            var transformFileTree = new TransformerFileTrees(FileTreeNavigator.SearchFileInFileTree(SettingsManager.RootPath, FileTree)).GetUpdatedFileTree();
            await Task.Run(async () =>
            {
                childrenTFL = transformFileTree.Children;
                foreach (var children in childrenTFL.ToList())
                    children.Parent = null;

                var count = FilesCollectionManager.GetCountElementsByFileTree(transformFileTree, false);
                var newList = FilesCollectionManager.UpdateTreeToList(childrenTFL);

                if (_ctc.IsCancellationRequested)
                {
                    return;
                }

                //ProgressBar========================
                _mainWindowVM.ProgressBarLoopScrol = false;
                _mainWindowVM.ProgressBarMaximum = count;
                //===================================

                ParallelProcessing.ParallelCalculateFactParametrs(newList, count, _ctc.Token);
            });

            if (_ctc.IsCancellationRequested)
            {
                Dispose();
                _mainWindowVM.EnabledButtons = true;
                _mainWindowVM.ProgressBarIsVisible = false;
                _mainWindowVM.ProgressBarText = string.Empty;
                _mainWindowVM.CancellButtonIsVisible = false;
                return;
            }
            _mainWindowVM.CancellButtonIsEnabled = false;
            MessageBus.Current.SendMessage<ObservableCollection<FileTree>>(childrenTFL!);

        }

        public void Dispose()
        {
            try
            {
                var rootParent = FileTreeNavigator.SearchFileInFileTree(SettingsManager.RootPath, FileTree);
                FilesCollectionManager.FileTreeDestroction(rootParent);
                rootParent = null;

                _fileTreeNavigator.PropertyChanged -= OnMyPropertyChanged;
                _fileTreeNavigator.FileTree = null;
                FileTree = null;
                _fileTreeNavigator = null;


                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch
            {

            }

        }
        #endregion

        ~FileExplorerWindowViewModel()
        {

        }
    }

}
