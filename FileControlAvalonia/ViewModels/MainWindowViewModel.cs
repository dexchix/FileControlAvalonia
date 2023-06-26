using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using FileControlAvalonia.Converters;
using FileControlAvalonia.Core;
using FileControlAvalonia.FileTreeLogic;
using FileControlAvalonia.Models;
using FileControlAvalonia.Services;
using FileControlAvalonia.ViewModels.Interfaces;
using FileControlAvalonia.Views;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reactive.Linq;
using System.Windows.Input;

namespace FileControlAvalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        #region FIELDS
        public static ObservableCollection<FileTree>? fileTree;
        private ObservableCollection<FileTree>? _filteredFiles;
        public HierarchicalTreeDataGridSource<FileTree> _source;
        private static IconConverter? s_iconConverter;
        private static ArrowConverter? s_arrowConverter;
        private WindowServise _windowServise = new WindowServise();
        private int _filterIndex = 0;
        private bool _mainWindowState;
        private string _userLevel;
        private int _totalFiles;
        private int _corresponds;
        private int _partialCorresponds;
        private int _dontCoresponds;
        private int _noAccess;
        private int _notFound;
        private int _notChecked;
        new public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        #region PROPERTIES
        public bool MainWindowState
        {
            get => _mainWindowState;
            set => this.RaiseAndSetIfChanged(ref _mainWindowState, value);
        }
        public ObservableCollection<FileTree> Files
        {
            get => fileTree!;
            set => this.RaiseAndSetIfChanged(ref fileTree, value);
        }
        public ObservableCollection<FileTree> FilteredFiles
        {
            get => _filteredFiles!;
            set => this.RaiseAndSetIfChanged(ref _filteredFiles, value); 
        }
        public int FilterIndex
        {
            get=> _filterIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref _filterIndex, value);
                //FileTreeFilter.Filter(StatusFile.NoAccess, FilteredFiles);
            }
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
        public int Corresponds
        {
            get => _corresponds;
            set => this.RaiseAndSetIfChanged(ref _corresponds, value);
        }
        public int PartialCorresponds
        {
            get => _partialCorresponds;
            set => this.RaiseAndSetIfChanged(ref _partialCorresponds, value);
        }
        public int DontCoresponds
        {
            get => _dontCoresponds;
            set => this.RaiseAndSetIfChanged(ref _dontCoresponds, value);
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
        public List<string> Filters => new List<string>() { "ВСЕ ФАЙЛЫ", "ПРОШЕДШИЕ ПРОВЕРКУ", "ЧАСТИЧНО ПРОШЕДШИЕ ПРОВЕРКУ", "НЕ ПРОШЕДШИЕ ПРОВЕРКУ", "БЕЗ ДОСТУПА", "ОТСУТСТВУЮЩИЕ" };
        public Interaction<InfoWindowViewModel, InfoWindowViewModel?> ShowDialogInfoWindow { get; }
        public Interaction<SettingsWindowViewModel, SettingsWindowViewModel?> ShowDialogSettingsWindow { get; }
        public Interaction<FileExplorerWindowViewModel, FileExplorerWindowViewModel?> ShowDialogFileExplorerWindow { get; }

        #endregion

        public MainWindowViewModel()
        {

            MainWindowState = true;
            Files = new ObservableCollection<FileTree>()
            {
                new FileTree("C:\\1\\2", true),
            };
            FilteredFiles = new ObservableCollection<FileTree>()
            {
                new FileTree("C:\\1\\2", true),
            };

            Source = new HierarchicalTreeDataGridSource<FileTree>(Files)
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

            MessageBus.Current.Listen<ObservableCollection<FileTree>>().Subscribe(transportFiles =>
            {
                TestingFilesCollectionManager.AddFiles(Files, transportFiles);
                //foreach (var file in files)
                //{
                //    filteredfiles.add(file);
                //}
                //FilteredFiles = transportFiles;
            });

            ShowDialogInfoWindow = new Interaction<InfoWindowViewModel, InfoWindowViewModel?>();
            ShowDialogSettingsWindow = new Interaction<SettingsWindowViewModel, SettingsWindowViewModel?>();
            ShowDialogFileExplorerWindow = new Interaction<FileExplorerWindowViewModel, FileExplorerWindowViewModel?>();

            _userLevel = "admin";
            _totalFiles = 0;
            _corresponds = 0;
            _partialCorresponds = 0;
            _dontCoresponds = 0;
            _noAccess = 0;
            _notFound = 0;
            _notChecked = 0;
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
        public void CloseProgram()
        {
            App.CurrentApplication!.Shutdown();
        }

        public ICommand OpenInfoWindow
        {
            get => ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await ShowDialogInfoWindow.Handle(Locator.Current.GetService<InfoWindowViewModel>()!);
            });
        }
        public ICommand OpenSettingsWindow
        {
            get => ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await ShowDialogSettingsWindow.Handle(Locator.Current.GetService<SettingsWindowViewModel>()!);
            });
        }
        public ICommand OpenFileExplorerWindow
        {
            get => ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await ShowDialogFileExplorerWindow.Handle(Locator.Current.GetService<FileExplorerWindowViewModel>()!);
            });
        }

        public void ExpandAllNodes()
        {
            try
            {
                for (int i = 0; i < Files.Count; i++)
                {
                    Source.Expand(i);
                }
            }
            catch
            {

            }
            try
            {
                foreach (var file in Files)
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

        public void CollapseAllNodes()
        {
            try
            {
                for (int i = 0; i < Files.Count; i++)
                {
                    Source.Collapse(i);
                }
            }
            catch
            {

            }
            try
            {
                foreach (var file in Files)
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

        public void DeliteFile(FileTree element)
        {
            try
            {
                foreach (var file in Files)
                {
                    if (file.Path == element.Path)
                    {
                        Files.Remove(file);
                        return;
                    }
                }
                for (int i = 0; i < Files.Count; i++)
                {
                    var delitedFile = FileTreeNavigator.SearchFile(element.Path, Files[i]);
                    if (delitedFile != null)
                    {
                        FileTreeNavigator.SearchFile(delitedFile.Parent!.Path, Files[i]).Children!.Remove(FileTreeNavigator.SearchFile(delitedFile.Path, Files[i]));
                    }

                }
            }
            catch (Exception ex)
            {
                Program.logger.Error($"{ex}, Не удалось удалить файл");
            }
        }

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
        #endregion
        #region METHODS
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void FilterElements()
        {
            //FilteredItems = FileTree.File.Children;
            //var extensions = Extensions.Split('/');
            //FilteredItems = FileTree.File.Children!.Where(file =>
            //{
            //    if (Directory.Exists(file.Path))
            //        return true;
            //    string fileExtensions = Path.GetExtension(file.Name).TrimStart('.');
            //    return extensions.Contains(fileExtensions);
            //});
        }
        #endregion
    }
}