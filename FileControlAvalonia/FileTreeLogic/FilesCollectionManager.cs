﻿using AutoMapper;
using FileControlAvalonia.Core;
using FileControlAvalonia.Core.Enums;
using FileControlAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileControlAvalonia.FileTreeLogic
{
    public static class FilesCollectionManager
    {
        /// <summary>
        /// Добавление файлов в существующее дерево 
        /// </summary>
        private static void AddFilesToExistingFileTree(FileTree mainFileTree, FileTree addedFileTree, ObservableCollection<FileTree> addedFilesCollection)
        {
            foreach (var file in addedFileTree.Children!.ToList())
            {
                if (!mainFileTree.Children!.Any(x => x.Path == file.Path))
                {
                    var mainParent = FileTreeNavigator.SearchFileInFileTree(file.Parent!.Path, mainFileTree);
                    mainFileTree.Children!.Add(file);
                    addedFilesCollection.Add(file);
                    file.Parent = mainParent;
                }
                else if (mainFileTree.Children!.Any(x => x.Path == file.Path) && file.IsDirectory)
                {
                    AddFilesToExistingFileTree(mainFileTree.Children!.Where(x => x.Path == file.Path).FirstOrDefault()!, file, addedFilesCollection);
                }
            }
        }

        public async static Task AddFiles(ObservableCollection<FileTree> mainCollectionFiles, ObservableCollection<FileTree> addedFiles, FileStats stats)
        {
            var addedBDFilesCollection = new ObservableCollection<FileTree>();
            foreach (var addFile in addedFiles)
            {
                if (mainCollectionFiles.Any(x => x.Path == addFile.Path) && addFile.IsDirectory)
                {
                    AddFilesToExistingFileTree(mainCollectionFiles.FirstOrDefault(x => x.Path == addFile.Path)!, addFile, addedBDFilesCollection);
                }
                else if (!mainCollectionFiles.Any(x => x.Path == addFile.Path))
                {
                    mainCollectionFiles.Add(addFile);
                    addedBDFilesCollection.Add(addFile);
                }
            }
            stats.GetFilesStats(addedBDFilesCollection);

            //Locator.Current.GetService<MainWindowViewModel>().ProgressBarLoopScrol = false;

            await EtalonManager.AddFilesOrCreateEtalon(addedBDFilesCollection, false);
        }

        /// <summary>
        /// Удаление файла из главного дерева и view-коллекции
        /// </summary>
        /// <param name="delitedFile"></param>
        /// <param name="viewCollectionFiles"></param>
        /// <param name="mainFileTree"></param>
        public static void DeleteFile(FileTree delitedFile, ObservableCollection<FileTree> viewCollectionFiles,
                                      ObservableCollection<FileTree> mainFileTreeCollection, FileStats fileStats)
        {
            try
            {
                fileStats = fileStats.GetFilesStats(new List<FileTree> { delitedFile });

                var delitedFileMainCollection = FileTreeNavigator.SeachFileInFilesCollection(delitedFile.Path, mainFileTreeCollection);
                var delitedFileViewCollection = FileTreeNavigator.SeachFileInFilesCollection(delitedFile.Path, viewCollectionFiles);


                if (delitedFileMainCollection != null && delitedFileMainCollection.Parent != null)
                {
                    delitedFileMainCollection.Parent.Children!.Remove(delitedFileMainCollection);
                    delitedFileMainCollection.Parent = null;
                    FilesCollectionManager.FileTreeDeliteDestruction(delitedFileMainCollection);
                    GC.Collect();
                   
                }
                else
                {
                    var delFile = mainFileTreeCollection.Where(x => x.Path == delitedFile.Path).FirstOrDefault();
                    mainFileTreeCollection.Remove(delFile!);
                    FilesCollectionManager.FileTreeDeliteDestruction(delitedFileMainCollection);
                    GC.Collect();
                }


                if (delitedFileViewCollection != null && delitedFileViewCollection.Parent != null)
                {
                    delitedFileViewCollection.Parent.Children!.Remove(delitedFileViewCollection);
                    delitedFileViewCollection.Parent = null;
                    FilesCollectionManager.FileTreeDeliteDestruction(delitedFileMainCollection);
                    GC.Collect();
                }
                else
                {
                    var delFile = viewCollectionFiles.Where(x => x.Path == delitedFile.Path).FirstOrDefault();
                    viewCollectionFiles.Remove(delFile!);
                    FilesCollectionManager.FileTreeDeliteDestruction(delitedFileViewCollection);
                    GC.Collect();
                }

            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// Обновляет view коллекцию файлов
        /// </summary>
        /// <param name="viewCollectionFiles"></param>
        /// <param name="mainFileTree"></param>
        public static void UpdateViewFilesCollection(ObservableCollection<FileTree> viewCollectionFiles, ObservableCollection<FileTree> mainFileTreeCollectin)
        {
            viewCollectionFiles.Clear();
            foreach (var file in mainFileTreeCollectin.ToList())
            {
                viewCollectionFiles?.Add(file);
            }
        }
        /// <summary>
        /// Устанавливает эталонные значения в соответствии с фактическими (при добавлении файлов)
        /// </summary>
        /// <param name="filesCollection"></param>
        public static void SetEtalonValues(ObservableCollection<FileTree> filesCollection)
        {
            foreach (var file in filesCollection)
            {
                file.EVersion = file.FVersion;
                file.ELastUpdate = file.FLastUpdate;
                file.EHash = file.FHash;

                file.Status = StatusFile.Checked;

                if (file.IsDirectory)
                    SetEtalonValues(file.Children);
            }
        }

        public static ObservableCollection<FileTree> GetDeepCopyFilesCollection(ObservableCollection<FileTree> mainCollection)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FileTree, FileTree>();
            });

            var mapper = new Mapper(config);

            var clone = mapper.Map<ObservableCollection<FileTree>>(mainCollection);
            return clone;
        }


        public static FileTree GetDeepCopyFileTree(FileTree fileTree)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FileTree, FileTree>();
            });

            var mapper = new Mapper(config);

            var clone = mapper.Map<FileTree>(fileTree);
            return clone;
        }
        public static void FileTreeDestroction(FileTree fileTree)
        {
            var listDestroctionFiles = new List<FileTree>();
            listDestroctionFiles.Add(fileTree);




            void FillList(FileTree fileTree)
            {
                for (var i = 0; i < fileTree.Children.Count; i++)
                {
                    if (fileTree.Children[i].IsOpened == true || !fileTree.IsDirectory)
                    {
                        listDestroctionFiles.Add(fileTree.Children[i]);
                        if (fileTree.Children[i].IsDirectory) FillList(fileTree.Children[i]);
                    }
                }
            }

            FillList(fileTree);


            foreach (var file in listDestroctionFiles)
            {
                if (file.Parent != null) file.Parent = null;
                if (file.Children != null)
                {
                    file.Children.Clear();
                }
            }
        }

        public static void FileTreeDeliteDestruction(FileTree fileTree)
        {
            var delitedList = UpdateTreeToList(new ObservableCollection<FileTree>() { fileTree});

            foreach(var file in delitedList)
            {
                file.Parent = null;
                if(file.Children != null) { file.Children.Clear(); }
            }
        }

        public static int GetCountElementsByFileTree(FileTree fileTree, bool considerFolders)
        {
            static int CountElementsInFolder(IEnumerable<FileTree> children, bool considerFolders)
            {
                int count = 0;

                foreach (var child in children)
                {
                    if (child.IsDirectory && considerFolders == true)
                    {
                        count++;
                    }
                    else if (!child.IsDirectory)
                        count++;

                    if (child.IsDirectory)
                        count += CountElementsInFolder(child.Children, considerFolders);
                }
                return count;
            }
            int count = CountElementsInFolder(fileTree.Children, considerFolders);
            return count;

        }

        public static int GetCountFilesByPath(string folderPath)
        {
            static int CountElementsInFolder(string folderPath)
            {
                int count = 0;
                try
                {
                    count += Directory.GetFiles(folderPath).Length;
                    foreach (string subfolder in Directory.GetDirectories(folderPath))
                    {
                        count += CountElementsInFolder(subfolder);
                    }
                }
                catch (Exception ex)
                {

                }

                return count;
            }

            int count = CountElementsInFolder(folderPath);
            return count;
        }

        public static List<FileTree> UpdateTreeToList(ObservableCollection<FileTree> files)
        {
            var list = new List<FileTree>();
            Iterate(files);


            void Iterate(ObservableCollection<FileTree> childrens)
            {
                foreach (var file in childrens)
                {
                    list.Add(file);
                    if (file.IsDirectory)
                        Iterate(file.Children);
                }
            }

            return list;
        }
    }
}
