using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using FileControlAvalonia.Converters;
using FileControlAvalonia.Models;
using FileControlAvalonia.Views;
using HarfBuzzSharp;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace FileControlAvalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region FIELDS
        public static ObservableCollection<FileTree> fileTree;
        public HierarchicalTreeDataGridSource<FileTree> _source;
        private static IconConverter? s_iconConverter;
        private static ArrowConverter? s_arrowConverter;
        #endregion

        #region PROPERTIES
        public ObservableCollection<FileTree> Files
        {
            get => fileTree;
            set => this.RaiseAndSetIfChanged(ref fileTree, value);
        }
        public HierarchicalTreeDataGridSource<FileTree> Source
        {
            get => _source;
            set => this.RaiseAndSetIfChanged(ref _source, value);
        }
        #endregion

        public MainWindowViewModel()
        {
            Files = new ObservableCollection<FileTree>();

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
                            })
                        {
                            IsTextSearchEnabled = true,
                            TextSearchValueSelector = x => x.Name
                        },
                        x => x.Children,
                        x => x.HasChildren,
                        x => x.IsExpanded),

                    new TemplateColumn<FileTree>(
                        "Эталон",
                        "FileCell",
                        new GridLength(1,GridUnitType.Star),
                        new ColumnOptions<FileTree>(){}
                        ){ },
                    new TemplateColumn<FileTree>(
                        "Фактическое значение",
                        "FileCell",
                        new GridLength(1,GridUnitType.Star),
                        new ColumnOptions<FileTree>()
                        ){},
                    new TemplateColumn<FileTree>(
                        " ", "ButtonCell",
                        new GridLength(1,GridUnitType.Star),
                        new ColumnOptions<FileTree>{MaxWidth = new GridLength(170)})
                }
            };
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

                    using (var fileStream = assetLoader.Open(new Uri("avares://FileControlAvalonia/Assets/file.png")))
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

        public void OpenFileExplorerWindow()
        {

            var fileExplorer = new FileExplorerWindow();
            var fghfgh = fileExplorer.DataContext;
            fileExplorer.Show();
        }

        public void OpenSettingsWindow()
        {
            var settingsWindow = new SettingsWindow();
            var fghfgh = settingsWindow.DataContext;
            settingsWindow.Show();
        }

        public void WrapFileTree(TreeDataGrid fileVieawer)
        {
            //fileVieawer.Source.
        }
        
        public void UnWrapFileTree(TreeDataGrid fileVieawer)
        {

        }
        public void TEST2()
        {
            //Files.Add(new FileTree("/lib32", true));
            Files.Add(new FileTree("C:\\Users", true));
        }
        #endregion
    }
}