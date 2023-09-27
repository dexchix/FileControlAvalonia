﻿using Avalonia.Controls;
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
using FileControlAvalonia.DataBase;
using System.Collections.Generic;
using System.Data;

namespace FileControlAvalonia.ViewModels
{
    public class FileExplorerWindowViewModel : ReactiveObject, IDisposable
    {
        #region FIELDS
        private int _itemIndex = 0;
        private static IconConverter? s_iconConverter;
        private static FileTreeNavigator _fileTreeNavigator;
        private FileTree? _fileTree;
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
            if (FileTree != null)
            {
                if (_fileTreeNavigator.watcher != null)
                    _fileTreeNavigator.watcher.StopWatch();
                window.Close();
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
                Locator.Current.GetService<MainWindowViewModel>().ProgressBarIsVisible = true;
                //Locator.Current.GetService<MainWindowViewModel>().ProgressBarLoopScrol = true;
                Locator.Current.GetService<MainWindowViewModel>().ProgressBarValue = 0;
                Locator.Current.GetService<MainWindowViewModel>().EnabledButtons = false;
                //Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = "Вычисление параметров";

                //Locator.Current.GetService(typeof(MainWindowViewModel)).
                window.Close();

                await TransitFiles();

                Dispose();
            }
            else window.Close();

            #region FileTransferBrokerRealization
            //if(FileTransferBroker.AddedFiles.Count > 0)
            //{
            //    window.Close();
            //    Locator.Current.GetService<MainWindowViewModel>().ProgressBarIsVisible = true;
            //    Locator.Current.GetService<MainWindowViewModel>().ProgressBarLoopScrol = true;
            //    Locator.Current.GetService<MainWindowViewModel>().EnabledButtons = false;
            //    Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = "Добавление файлов";

            //    ObservableCollection<FileTree> test = null;
            //    await Task.Run(async () =>
            //    {
            //        var awdwa = new DataBaseConverter().ConvertFormatDBToFileTreeCollection(FileTransferBroker.AddedFiles);
            //        FactParameterizer.SetFactValuesInFilesCollection(awdwa);
            //        FilesCollectionManager.SetEtalonValues(awdwa);
            //        test = awdwa;
            //    });
            //    MessageBus.Current.SendMessage<ObservableCollection<FileTree>>(test!);

            //    FileTransferBroker.AddedFiles.Clear();
            //    Dispose();
            //}
            //else
            //{
            //    FileTransferBroker.AddedFiles.Clear();
            //    Dispose();
            //    window.Close();
            //}
            #endregion
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

                var newFileTree = FilesCollectionManager.GetDeepCopyFileTree(transformFileTree);

                childrenTFL = newFileTree.Children;
                foreach (var children in childrenTFL.ToList())
                    children.Parent = null;

                var count = FilesCollectionManager.GetCountElementsByFileTree(newFileTree, false);
                var newList = FilesCollectionManager.UpdateTreeToList(childrenTFL);
                
                //ProgressBar======================
                Locator.Current.GetService<MainWindowViewModel>().ProgressBarMaximum = count;

                //===================================

                ParallelProcessing.ParallelCalculateFactParametrs(newList, count);
            });
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
    }
    public class AAAA
    {
        public virtual void SSSS()
        {

        }
    }
}
