using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using FileControlAvalonia.Converters;
using FileControlAvalonia.Models;
using FileControlAvalonia.Services;
using FileControlAvalonia.Views;
using HarfBuzzSharp;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FileControlAvalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region FIELDS
        public static ObservableCollection<FileTree> fileTree;
        public HierarchicalTreeDataGridSource<FileTree> _source;
        private static IconConverter? s_iconConverter;
        private static ArrowConverter? s_arrowConverter;
        private WindowServise _windowServise = new WindowServise();
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
            ReactiveUI.MessageBus.Current.Listen<ObservableCollection<FileTree>>().Subscribe(x =>
            {
                foreach (var item in x)
                {
                    Files.Add(item);
                }
            });

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

        public void OpenFileExplorerWindow()
        {
            _windowServise.ShowWindow<FileExplorerWindow>();
        }

        public void OpenSettingsWindow()
        {
            _windowServise.ShowWindow<SettingsWindow>();
        }

        public void ExpandAllNodes(TreeDataGrid fileVieawer)
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

        public void CollapseAllNodes(TreeDataGrid fileVieawer)
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
        public void TEST2()
        {
            //Files.Add(new FileTree("/lib32", true));
            //Files.Add(new FileTree("C:\\Users", true));
            //Helper.MessageBus.Bus += AddFilesInTreeDataGrid;

            byte[] fsdfgg = { 3, 4, 5, 6, 4 };
            var sdfsdf = LinuxAPI.read(34, fsdfgg, 44);
         

        }

        private void ChangeIsExpandedProp(FileTree folder, bool flag)
        {
            folder.IsExpanded = flag;
            foreach (var file in folder.Children)
            {
                if (file.IsDirectory)
                {
                    file.IsExpanded = flag;
                    ChangeIsExpandedProp(file, flag);
                }
            }
        }

        //===================
        private void AddFilesInTreeDataGrid(object selectedFiles)
        {
            var files = selectedFiles as ObservableCollection<FileTree>;
            foreach (var item in files)
            {
                Files.Add(item);
            }

        }
        #endregion
    }
}