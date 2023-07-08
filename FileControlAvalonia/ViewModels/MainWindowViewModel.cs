﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using FileControlAvalonia.Converters;
using FileControlAvalonia.Core;
using FileControlAvalonia.FileTreeLogic;
using FileControlAvalonia.Helper;
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
using System.Linq;
using System.Reactive.Linq;
using System.Security.Permissions;
using System.Windows.Input;

namespace FileControlAvalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        #region FIELDS
        public static ObservableCollection<FileTree>? fileTree;
        private static FileTree _mainFileTree;
        private ObservableCollection<FileTree>? _viewCollewtionFiles;
        private FilterFiles _filter = new FilterFiles();
        public HierarchicalTreeDataGridSource<FileTree> _source;
        private static IconConverter? s_iconConverter;
        private static ArrowConverter? s_arrowConverter;
        private int _filterIndex = 0;
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
        public FileTree MainFileTree
        {
            get => _mainFileTree;
            set => _mainFileTree = value;
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
                        FilesCollectionManager.UpdateViewFilesCollection(ViewCollectionFiles, _mainFileTree);
                        break;
                    case 1:
                        FilterFiles.Filter(StatusFile.Checked, _mainFileTree, ViewCollectionFiles);
                        break;
                    case 2:
                        FilterFiles.Filter(StatusFile.PartiallyChecked, _mainFileTree, ViewCollectionFiles);
                        break;
                    case 3:
                        FilterFiles.Filter(StatusFile.FailedChecked, _mainFileTree, ViewCollectionFiles);
                        break;
                    case 4:
                        FilterFiles.Filter(StatusFile.NoAccess, _mainFileTree, ViewCollectionFiles);
                        break;
                    case 5:
                        FilterFiles.Filter(StatusFile.Missing, _mainFileTree, ViewCollectionFiles);
                        break;
                }
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
            if (Directory.Exists(SettingsManager.rootPath))
            {
                _mainFileTree = new FileTree(SettingsManager.rootPath, true);
                _mainFileTree.Children!.Clear();
            }

            ViewCollectionFiles = new ObservableCollection<FileTree>()
            {

            };

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

            MessageBus.Current.Listen<FileTree>().Subscribe(transportFileTree =>
            {
                FilesCollectionManager.AddFiles(_mainFileTree, transportFileTree);
                Comprasion.SetStatus(_mainFileTree);
                FilesCollectionManager.UpdateViewFilesCollection(ViewCollectionFiles, _mainFileTree);
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
        public void CheckCommand()
        {
            var etalon = EtalonManager.GetEtalon();
            FilesCollectionManager.MergeFileTrees(_mainFileTree, etalon);
            Comprasion.CompareTrees(etalon);
            FilesCollectionManager.UpdateViewFilesCollection(ViewCollectionFiles, etalon);
        }
        public void CreateEtalonCommand()
        {
            var qqq = new DataBase.DataBaseConverter();
            var fgfg = qqq.ConvertFormatFileTreeToDB(_mainFileTree);
            EtalonManager.CreateEtalon(fgfg);
        }
        public void CloseProgramCommand()
        {
            App.CurrentApplication!.Shutdown();
        }

        public ICommand OpenInfoWindowCommand
        {
            get => ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await ShowDialogInfoWindow.Handle(Locator.Current.GetService<InfoWindowViewModel>()!);
            });
        }
        public ICommand OpenSettingsWindowCommand
        {
            get => ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await ShowDialogSettingsWindow.Handle(Locator.Current.GetService<SettingsWindowViewModel>()!);
            });
        }
        public ICommand OpenFileExplorerWindowCommand
        {
            get => ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await ShowDialogFileExplorerWindow.Handle(Locator.Current.GetService<FileExplorerWindowViewModel>()!);
            });
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

        public void DeliteFileCommand(FileTree delitedFile)
        {
            FilesCollectionManager.DeliteFile(delitedFile,
                ViewCollectionFiles, _mainFileTree);
        }

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