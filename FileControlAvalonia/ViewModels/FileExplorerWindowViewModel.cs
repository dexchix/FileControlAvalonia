using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Platform;
using Avalonia;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using FileControlAvalonia.Models;
using FileControlAvalonia.Converters;
using NLog.LayoutRenderers.Wrappers;

namespace FileControlAvalonia.ViewModels
{
    public class FileExplorerWindowViewModel: ReactiveObject
    {
        #region FIELDS
        private int _itemIndex = 0;
        private static IconConverter? s_iconConverter;
        private static FileTreeNavigator _fileTreeNavigator = new FileTreeNavigator();
        private static FileTree? _fileTree;
        #endregion

        #region PROPERTIES
        public int ItemIndex
        {
            get => _itemIndex;
            set => this.RaiseAndSetIfChanged(ref _itemIndex, value);
        }
        public string Extensions { get => "exe/ jpeg/ png"; }

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
            if (_fileTreeNavigator.FileTree.Parent != null)
                _fileTreeNavigator.GoBackFolder();
        }
        public void CancelCommand(Window window)
        {
            window.Close();
        }
        public void OkCommand(Window window)
        {
            MainWindowViewModel.fileTree = FileTree.Children;

            var fdgdfgghgdf = MainWindowViewModel.fileTree;

            window.Close();
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
        #endregion
    }
}
