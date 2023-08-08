using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.Utilities;
using FileControlAvalonia.Converters;
using FileControlAvalonia.Core;
using FileControlAvalonia.FileTreeLogic;
using FileControlAvalonia.Helper;
using FileControlAvalonia.Models;
using FileControlAvalonia.Services;
using FileControlAvalonia.SettingsApp;
using FileControlAvalonia.ViewModels.Interfaces;
using FileControlAvalonia.Views;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileControlAvalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        #region FIELDS
        public static ObservableCollection<FileTree>? fileTree;
        private static ObservableCollection<FileTree> _mainFileTreeCollection;
        private ObservableCollection<FileTree>? _viewCollewtionFiles;
        private FilterFiles _filter = new FilterFiles();
        public HierarchicalTreeDataGridSource<FileTree> _source;
        private static IconConverter? s_iconConverter;
        private static ArrowConverter? s_arrowConverter;
        private int _filterIndex = 0;
        private string _userLevel;

        #region Info
        private int _totalFiles;
        private int _checked;
        private int _partialChecked;
        private int _failedChecked;
        private int _noAccess;
        private int _notFound;
        private int _notChecked;
        private string _dateLastCheck;
        private string _dateCreateEtalon;
        private string _userLevelCreateEtalon;
        #endregion

        private bool _enabledButtons = true;

        #region ProgressBar
        private bool _progressBarIsVisible = false;
        private int _progressBarValue = 0;
        private string _progressBarText;
        private bool _progressBarLoopScrol = false;
        private int _progressBarMaximum;
        #endregion

        new public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        #region PROPERTIES
        public ObservableCollection<FileTree> MainFileTreeCollection
        {
            get => _mainFileTreeCollection;
            set => _mainFileTreeCollection = value;
        }
        public ObservableCollection<FileTree> ViewCollectionFiles
        {
            get => _viewCollewtionFiles!;
            set => this.RaiseAndSetIfChanged(ref _viewCollewtionFiles, value);
        }
        public int FilterIndex
        {
            get => _filterIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref _filterIndex, value);
                switch (_filterIndex)
                {
                    case 0:
                        FilesCollectionManager.UpdateViewFilesCollection(ViewCollectionFiles, MainFileTreeCollection);
                        break;
                    case 1:
                        FilterFiles.Filter(StatusFile.Checked, MainFileTreeCollection, ViewCollectionFiles);
                        break;
                    case 2:
                        FilterFiles.Filter(StatusFile.PartiallyChecked, MainFileTreeCollection, ViewCollectionFiles);
                        break;
                    case 3:
                        FilterFiles.Filter(StatusFile.FailedChecked, MainFileTreeCollection, ViewCollectionFiles);
                        break;
                    case 4:
                        FilterFiles.Filter(StatusFile.NoAccess, MainFileTreeCollection, ViewCollectionFiles);
                        break;
                    case 5:
                        FilterFiles.Filter(StatusFile.NotFound, MainFileTreeCollection, ViewCollectionFiles);
                        break;
                }
            }
        }

        public bool EnabledButtons
        {
            get => _enabledButtons;
            set => this.RaiseAndSetIfChanged(ref _enabledButtons, value);
        }

        public HierarchicalTreeDataGridSource<FileTree> Source
        {
            get => _source;
            set => this.RaiseAndSetIfChanged(ref _source, value);
        }
        public string UserLevel
        {
            get => _userLevel;
            set => this.RaiseAndSetIfChanged(ref _userLevel, value);
        }
        public int TotalFiles
        {
            get => _totalFiles;
            set => this.RaiseAndSetIfChanged(ref _totalFiles, value);
        }
        public int Checked
        {
            get => _checked;
            set => this.RaiseAndSetIfChanged(ref _checked, value);
        }
        public int PartialChecked
        {
            get => _partialChecked;
            set => this.RaiseAndSetIfChanged(ref _partialChecked, value);
        }
        public int FailedChecked
        {
            get => _failedChecked;
            set => this.RaiseAndSetIfChanged(ref _failedChecked, value);
        }
        public int NoAccess
        {
            get => _noAccess;
            set => this.RaiseAndSetIfChanged(ref _noAccess, value);
        }
        public int NotFound
        {
            get => _notFound;
            set => this.RaiseAndSetIfChanged(ref _notFound, value);
        }
        public int NotChecked
        {
            get => _notChecked;
            set => this.RaiseAndSetIfChanged(ref _notChecked, value);
        }
        public string DateLastCheck
        {
            get => _dateLastCheck;
            set => this.RaiseAndSetIfChanged(ref _dateLastCheck, value);
        }
        public string DateCreateEtalon
        {
            get => _dateCreateEtalon;
            set => this.RaiseAndSetIfChanged(ref _dateCreateEtalon, value);
        }
        public string UserLevelCreateEtalon
        {
            get => _userLevelCreateEtalon;
            set => this.RaiseAndSetIfChanged(ref _userLevelCreateEtalon, value);
        }

        #region ProgressBar
        public bool ProgressBarIsVisible
        {
            get => _progressBarIsVisible;
            set => this.RaiseAndSetIfChanged(ref _progressBarIsVisible, value);
        }
        public int ProgressBarValue
        {
            get => _progressBarValue;
            set => this.RaiseAndSetIfChanged(ref _progressBarValue, value);
        }
        public string ProgressBarText
        {
            get => _progressBarText;
            set => this.RaiseAndSetIfChanged(ref _progressBarText, value);
        }
        public bool ProgressBarLoopScrol
        {
            get => _progressBarLoopScrol;
            set => this.RaiseAndSetIfChanged(ref _progressBarLoopScrol, value);
        }
        public int ProgressBarMaximum
        {
            get => _progressBarMaximum;
            set => this.RaiseAndSetIfChanged(ref _progressBarMaximum, value);
        }
        #endregion

        public List<string> Filters => new List<string>() { "ВСЕ ФАЙЛЫ", "ПРОШЕДШИЕ ПРОВЕРКУ", "ЧАСТИЧНО ПРОШЕДШИЕ ПРОВЕРКУ", "НЕ ПРОШЕДШИЕ ПРОВЕРКУ", "БЕЗ ДОСТУПА", "ОТСУТСТВУЮЩИЕ" };
        public Interaction<InfoWindowViewModel, InfoWindowViewModel?> ShowDialogInfoWindow { get; }
        public Interaction<SettingsWindowViewModel, SettingsWindowViewModel?> ShowDialogSettingsWindow { get; }
        public Interaction<FileExplorerWindowViewModel, FileExplorerWindowViewModel?> ShowDialogFileExplorerWindow { get; }
        #endregion

        public MainWindowViewModel()
        {
            ViewCollectionFiles = new ObservableCollection<FileTree>();
            MainFileTreeCollection = new ObservableCollection<FileTree>();

            Source = new HierarchicalTreeDataGridSource<FileTree>(ViewCollectionFiles)
            {
                Columns =
                {
                    new HierarchicalExpanderColumn<FileTree>(
                        new TemplateColumn<FileTree>(
                            "Имя файла",
                            "FileNameCell",
                            new GridLength(1, GridUnitType.Star),
                            new ColumnOptions<FileTree>
                            {
                                MaxWidth = GridLength.Parse("350")
                            }),
                        x => x.Children,
                        x => x.HasChildren,
                        x => x.IsExpanded
                        ),

                    new TemplateColumn<FileTree>(
                        $"{new string(' ',38)}Эталон{new string(' ',33)}|{new string(' ',25)}Фактическое значение{new string(' ',19)}|",
                        "FileCell",
                        new GridLength(1,GridUnitType.Star),
                        new ColumnOptions<FileTree>(){}
                        ){},
                }
            };

            MessageBus.Current.Listen<ObservableCollection<FileTree>>().Subscribe(async transportFileTree =>
            {
                //ProgressBarIsVisible = true;
                //ProgressBarLoopScrol = true;
                //EnabledButtons = false;
                //ProgressBarText = "Добавление файлов";

                FileStats fileStats = new FileStats();

                await Task.Run(() =>
                {
                    FilesCollectionManager.AddFiles(MainFileTreeCollection, transportFileTree, fileStats);
                });
                FilesCollectionManager.UpdateViewFilesCollection(ViewCollectionFiles, MainFileTreeCollection);

                ProgressBarLoopScrol = false;
                ProgressBarText = "Добавление файлов завершно";

                TotalFiles += fileStats.TotalFiles;
                Checked += fileStats.Checked;

                RecorderInfoBD.RecordInfoCountFiles(TotalFiles, Checked, PartialChecked, FailedChecked, NoAccess, NotFound, NotChecked);
                await Task.Delay(1000);
                ProgressBarIsVisible = false;
                EnabledButtons = true;
            });

            ShowDialogInfoWindow = new Interaction<InfoWindowViewModel, InfoWindowViewModel?>();
            ShowDialogSettingsWindow = new Interaction<SettingsWindowViewModel, SettingsWindowViewModel?>();
            ShowDialogFileExplorerWindow = new Interaction<FileExplorerWindowViewModel, FileExplorerWindowViewModel?>();

        }

        #region CONVERTERS
        public static IMultiValueConverter ArrowIconConverter
        {
            get
            {
                if (s_arrowConverter is null)
                {
                    var assetLoader = AvaloniaLocator.Current.GetRequiredService<IAssetLoader>();

                    using (var arrowRightStream = assetLoader.Open(new Uri("avares://FileControlAvalonia/Assets/ArrowRight1.png")))
                    using (var arrowDownStream = assetLoader.Open(new Uri("avares://FileControlAvalonia/Assets/ArrowDown1.png")))
                    using (var emptyImageStream = assetLoader.Open(new Uri("avares://FileControlAvalonia/Assets/EmptyImage.png")))
                    {
                        var arowRightIcon = new Bitmap(arrowRightStream);
                        var arrowDownIcon = new Bitmap(arrowDownStream);
                        var emptyImageIcon = new Bitmap(emptyImageStream);

                        s_arrowConverter = new ArrowConverter(arowRightIcon, arrowDownIcon, emptyImageIcon);
                    }
                }

                return s_arrowConverter;
            }
        }

        public static IMultiValueConverter FileIconConverter
        {
            get
            {
                if (s_iconConverter is null)
                {
                    var assetLoader = AvaloniaLocator.Current.GetRequiredService<IAssetLoader>();

                    using (var fileStream = assetLoader.Open(new Uri("avares://FileControlAvalonia/Assets/file2.png")))
                    using (var folderStream = assetLoader.Open(new Uri("avares://FileControlAvalonia/Assets/folder-open.png")))
                    using (var folderOpenStream = assetLoader.Open(new Uri("avares://FileControlAvalonia/Assets/folder.png")))
                    {
                        s_iconConverter = new IconConverter(new Bitmap(fileStream), new Bitmap(folderStream), new Bitmap(folderOpenStream));
                    }
                }

                return s_iconConverter;
            }
        }
        #endregion

        #region COMMANDS
        async public void CheckCommand()
        {
            var comparator = new Comprasion();
            ProgressBarIsVisible = true;
            EnabledButtons = false;
            ProgressBarLoopScrol = true;
            ProgressBarText = "Осуществляется проверка";
            //ProgressBarMaximum = EtalonManager.CountFilesEtalon;
            //ProgressBarValue = 0;
            await Task.Run(() =>
            {
                FactParameterizer.SetFactValuesInFilesCollection(MainFileTreeCollection);
                comparator.CompareFiles(MainFileTreeCollection, ProgressBarValue);

                TotalFiles = comparator.TotalFiles;
                Checked = comparator.Checked;
                PartialChecked = comparator.PartialChecked;
                FailedChecked = comparator.FailedChecked;
                NoAccess = comparator.NoAccess;
                NotFound = comparator.NotFound;
                NotChecked = comparator.NotChecked;

                new LastChekInfoManager().UpdateFactParametresInDB(MainFileTreeCollection);
            });
            FilesCollectionManager.UpdateViewFilesCollection(ViewCollectionFiles, MainFileTreeCollection);
            DateLastCheck = DateTime.Now.ToString();


            //============================================================================================
            //Запись в БД

            RecorderInfoBD.RecordDateOfLastCheck(DateLastCheck);
            RecorderInfoBD.RecordInfoCountFiles(TotalFiles, Checked, PartialChecked, FailedChecked, NoAccess, NotFound, NotChecked);
            //============================================================================================

            ProgressBarText = "Проверка прошла успешно";
            ProgressBarLoopScrol = false;
            await Task.Delay(1000);
            ProgressBarIsVisible = false;
            EnabledButtons = true;

        }

        async public void CreateEtalonCommand()
        {
            ProgressBarIsVisible = true;
            ProgressBarLoopScrol = true;
            EnabledButtons = false;
            ProgressBarText = "Создание эталона";

            int countFiles = 0;

            await Task.Run(() =>
            {
                EtalonManager.AddFilesOrCreateEtalon(MainFileTreeCollection, true);
            });
            FilesCollectionManager.UpdateViewFilesCollection(ViewCollectionFiles, MainFileTreeCollection);

            //============================================================================================
            //Запись в БД
            TotalFiles = countFiles;
            Checked = countFiles;
            PartialChecked = 0;
            FailedChecked = 0;
            NoAccess = 0;
            NotFound = 0;
            NotChecked = 0;
            DateCreateEtalon = DateTime.Now.ToString();

            RecorderInfoBD.RecordInfoOfCreateEtalon("Admin", DateCreateEtalon);
            //RecorderInfoBD.RecordInfoCountFiles(TotalFiles, Checked, PartialChecked, FailedChecked, NoAccess, NotFound, NotChecked);
            //============================================================================================

            ProgressBarMaximum = 0;
            ProgressBarValue = 0;
            ProgressBarText = "Создание эталона завершено";

            await Task.Delay(1000);

            ProgressBarIsVisible = false;


            UserLevelCreateEtalon = "Admin";
            DateCreateEtalon = DateTime.Now.ToString();
            EnabledButtons = true;
        }
        public void CloseProgramCommand()
        {
            App.CurrentApplication!.Shutdown();
        }

        public ICommand OpenInfoWindowCommand
        {
            get => ReactiveCommand.CreateFromTask(async () =>
            {
                MainWindow.IsChildWindowOpen = true;
                var result = await ShowDialogInfoWindow.Handle(Locator.Current.GetService<InfoWindowViewModel>()!);
            });
        }
        public ICommand OpenSettingsWindowCommand
        {
            get => ReactiveCommand.CreateFromTask(async () =>
            {
                MainWindow.IsChildWindowOpen = true;
                var result = await ShowDialogSettingsWindow.Handle(Locator.Current.GetService<SettingsWindowViewModel>()!);
            });
        }
        public ICommand OpenFileExplorerWindowCommand
        {
            get => ReactiveCommand.CreateFromTask(async () =>
            {
                MainWindow.IsChildWindowOpen = true;
                var result = await ShowDialogFileExplorerWindow.Handle(Locator.Current.GetService<FileExplorerWindowViewModel>()!);
            });
        }
        async public void ShowEtalon()
        {
            var comparator = new Comprasion();
            ProgressBarIsVisible = true;
            ProgressBarLoopScrol = true;
            await Task.Delay(2000);
            await Task.Run(() =>
            {
                try
                {
                    MainFileTreeCollection = EtalonManager.GetEtalon();
                    comparator.CompareFiles(MainFileTreeCollection, ProgressBarValue);
                }
                catch
                {

                }
            });

            FilesCollectionManager.UpdateViewFilesCollection(ViewCollectionFiles, MainFileTreeCollection);

            ProgressBarLoopScrol = false;
            ProgressBarIsVisible = false;

            var info = EtalonManager.GetInfo();


            DateCreateEtalon = info.Date;
            DateLastCheck = info.DateLastCheck;
            UserLevel = info.Creator;

            TotalFiles = comparator.TotalFiles;
            Checked = comparator.Checked;
            PartialChecked = comparator.PartialChecked;
            FailedChecked = comparator.FailedChecked;
            NoAccess = comparator.NoAccess;
            NotFound = comparator.NotFound;
            NotChecked = comparator.NotChecked;
        }

        public void ExpandAllNodesCommand()
        {
            try
            {
                for (int i = 0; i < ViewCollectionFiles.Count; i++)
                {
                    Source.Expand(i);
                }
            }
            catch
            {

            }
            try
            {
                foreach (var file in ViewCollectionFiles)
                {
                    if (file.IsDirectory)
                    {
                        file.IsExpanded = true;
                        ChangeIsExpandedProp(file, true);
                    }
                }
            }
            catch
            {

            }
        }

        public void CollapseAllNodesCommand()
        {
            try
            {
                for (int i = 0; i < ViewCollectionFiles.Count; i++)
                {
                    Source.Collapse(i);
                }
            }
            catch
            {

            }
            try
            {
                foreach (var file in ViewCollectionFiles)
                {
                    if (file.IsDirectory)
                    {
                        file.IsExpanded = false;
                        ChangeIsExpandedProp(file, false);
                    }
                }
            }
            catch
            {

            }
        }

        async public void DeliteFileCommand(FileTree delitedFile)
        {
            EnabledButtons = false;
            ProgressBarIsVisible = true;
            ProgressBarLoopScrol = true;
            ProgressBarText = "Удаление файлов";

            //int deliteTotalFilesCount = 0;
            //int deliteCheckedCount = 0;
            //int delitePartialCheckedCount = 0;
            //int deliteFailedCheckedCount = 0;
            //int deliteNoAccessCount = 0;
            //int deliteNotFoundCount = 0;
            //int deliteNotCheckedCount = 0;

            FileStats stats = new FileStats();



            await Task.Run(() =>
            {
                FilesCollectionManager.DeliteFile(delitedFile, ViewCollectionFiles, MainFileTreeCollection, stats);
                EtalonManager.DeliteFileInDB(delitedFile);
            });

            if (TotalFiles > 0) TotalFiles -= stats.TotalFiles;
            if (Checked > 0) Checked -= stats.Checked;
            if (PartialChecked > 0) PartialChecked -= stats.PartialChecked;
            if (FailedChecked > 0) FailedChecked -= stats.FailedChecked;
            if (NoAccess > 0) NoAccess -= stats.NoAccess;
            if (NotFound > 0) NotFound -= stats.NotFound;
            if (NotChecked > 0) NotChecked -= stats.NotChecked;

            RecorderInfoBD.RecordInfoCountFiles(TotalFiles, Checked, PartialChecked, FailedChecked, NoAccess, NotFound, NotChecked);

            ProgressBarIsVisible = false;
            ProgressBarLoopScrol = false;
            EnabledButtons = true;
        }

        //TotalFiles INTEGER,
        //Checked INTEGER,
        //PartialChecked INTEGER
        //FailedChecked INTEGER,
        //NoAccess INTEGER,
        //NotFound INTEGER,
        //NotChecked INTEGER

        #endregion

        #region METHODS
        private void ChangeIsExpandedProp(FileTree folder, bool flag)
        {
            folder.IsExpanded = flag;
            foreach (var file in folder.Children!)
            {
                if (file.IsDirectory)
                {
                    file.IsExpanded = flag;
                    ChangeIsExpandedProp(file, flag);
                }
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}