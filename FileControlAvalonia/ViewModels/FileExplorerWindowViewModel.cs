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
using FileControlAvalonia.Helper;
using System.Collections.ObjectModel;
using FileControlAvalonia.ViewModels.Interfaces;
using FileControlAvalonia.FileTreeLogic;
using FileControlAvalonia.SettingsApp;
using FileControlAvalonia.Core;

namespace FileControlAvalonia.ViewModels
{
    public class FileExplorerWindowViewModel : ReactiveObject, IFileExplorerWindowViewModel, IDisposable
    {
        #region FIELDS
        private int _itemIndex = 0;
        private static IconConverter? s_iconConverter;
        private static FileTreeNavigator _fileTreeNavigator; 
        private static FileTree? _fileTree;
        #endregion

        #region PROPERTIES
        public int ItemIndex
        {
            get => _itemIndex;
            set => this.RaiseAndSetIfChanged(ref _itemIndex, value);
        }
        public string Extensions { get => SettingsManager.SettingsString!; }

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
            Dispose();
            window.Close();
        }
        async public void OkCommand(Window window)
        {
            //await Task.Run(() =>
            //{
                var transformFileTree = new TransformerFileTrees(FileTreeNavigator.SearchFileInFileTree(SettingsManager.RootPath, FileTree)).GetUpdatedFileTree();
                var childrenTFL = transformFileTree.Children;
                foreach (var children in childrenTFL)
                    children.Parent = null;
                FactParameterizer.SetFactValuesInFilesCollection(childrenTFL);
                FilesCollectionManager.SetEtalonValues(childrenTFL);

            //});
            MessageBus.Current.SendMessage<ObservableCollection<FileTree>>(childrenTFL!);


            Dispose();
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

        public void Dispose()
        {
            try
            {
                _fileTreeNavigator.PropertyChanged -= OnMyPropertyChanged;
                _fileTreeNavigator.watcher.StopWatch();
                _fileTreeNavigator = null;
            }
            catch
            {

            }

        }
        #endregion
    }
}
