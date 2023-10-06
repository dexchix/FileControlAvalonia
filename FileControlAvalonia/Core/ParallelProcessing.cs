using Avalonia.Threading;
using FileControlAvalonia.Core.Enums;
using FileControlAvalonia.Models;
using FileControlAvalonia.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileControlAvalonia.Core
{
    public class ParallelProcessing
    {
        private static int _count = 0;
        private static object _lock = new object();
        private static MainWindowViewModel _mainWindowVM = Locator.Current.GetService<MainWindowViewModel>();
        private static Dictionary<FileTree, (string, string, string, StatusFile)> _oldStats = new Dictionary<FileTree, (string, string, string, StatusFile)>();

        public async static Task ParallelCalculateFactParametrs(List<FileTree> files, int countFiles, CancellationToken token)
        {
            if (files.Count <= 1000)
            {
                foreach (FileTree file in files)
                {
                    if (!file.IsDirectory) _mainWindowVM.ProgressBarValue++;
                    _mainWindowVM.ProgressBarText = $"Добавлено {_mainWindowVM.ProgressBarValue} из {countFiles}";
                    FactParameterizer.SetFactValues(file);
                    
                }
                _mainWindowVM.ProgressBarValue = 0;
                _mainWindowVM.ProgressBarMaximum = 0;
                _mainWindowVM.ProgressBarText = string.Empty;
                return;
            }
            else
            {
                int start = 0;
                int limit = 1;
                var section = files.Count / 8;
                int residue = files.Count - section * 8;
                for (int i = 0; i < 8; i++)
                {
                    int localStart = start * section;
                    int localLimit = limit * section;
                    start++;
                    limit++;
                    Task.Run(() =>
                    {
                        for (int i = localStart; i < localLimit; i++)
                        {
                            FactParameterizer.SetFactValues(files[i]);
                            lock (_lock)
                            {
                                _count++;

                                if (!files[i].IsDirectory)
                                {
                                    //ProgressBar=====================================================================================
                                    _mainWindowVM.ProgressBarValue++;
                                    _mainWindowVM.ProgressBarText = $"Добавлено {_mainWindowVM.ProgressBarValue} из {countFiles}";
                                    //Locator.Current.GetService<MainWindowViewModel>().ProgressBarValue++;
                                    //Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = $"Добавлено {_count} из {countFiles}";
                                    //================================================================================================
                                }

                                if(token.IsCancellationRequested)
                                {
                                    return;
                                }

                            }
                        }
                    });
                }
                for (int i = files.Count - residue; i < files.Count; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
                    lock (_lock)
                    {
                        _count++;
                        if (!files[i].IsDirectory)
                        {
                            //ProgressBar=====================================================================================
                            _mainWindowVM.ProgressBarValue++;
                            _mainWindowVM.ProgressBarText = $"Добавлено {_mainWindowVM.ProgressBarValue} из {countFiles}";
                            //Locator.Current.GetService<MainWindowViewModel>().ProgressBarValue++;
                            //Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = $"Добавлено {_count} из {countFiles}";
                            //================================================================================================
                        }
                    }
                }
            }

            while (true)
            {
                if (_count == files.Count || token.IsCancellationRequested)
                {
                    //ProgressBar=====================================================================================
                    //Locator.Current.GetService<MainWindowViewModel>().ProgressBarValue = 0;
                    //Locator.Current.GetService<MainWindowViewModel>().ProgressBarMaximum = 0;
                    _mainWindowVM.ProgressBarValue=0;
                    _mainWindowVM.ProgressBarMaximum = 0;
                    _mainWindowVM.ProgressBarText = string.Empty;
                    //================================================================================================
                    _count = 0;
                    break;
                }
            }
        }

        public static Comprasion ParallelComprasion(List<FileTree> files, int countFiles, CancellationToken token)
        {
            var comparer = new Comprasion();
            if (files.Count <= 1000)
            {
                foreach (FileTree file in files)
                {
                    FactParameterizer.SetFactValues(file);
                    comparer.SetStatus(file);
                }
                return comparer;
            }
            else
            {
                int start = 0;
                int limit = 1;
                var section = files.Count / 8;
                int residue = files.Count - section * 8;
                for (int i = 0; i < 8; i++)
                {
                    int localStart = start * section;
                    int localLimit = limit * section;
                    start++;
                    limit++;
                    Task.Run(() =>
                    {
                        for (int i = localStart; i < localLimit; i++)
                        {
                            string oldHash = files[i].FHash;
                            string oldVersion = files[i].FVersion;
                            string oldLastUpdate = files[i].FLastUpdate;
                            StatusFile oldStatus = files[i].Status;

                            FactParameterizer.SetFactValues(files[i]);

                            lock (_lock)
                            {
                                if (oldHash != files[i].FHash || oldVersion != files[i].FVersion || oldLastUpdate != files[i].FLastUpdate)
                                {
                                    _oldStats.Add(files[i], (oldHash, oldVersion, oldLastUpdate, oldStatus));
                                }

                            }
                            lock (_lock)
                            {
                                comparer.SetStatus(files[i]);
                                _count++;

                                if (!files[i].IsDirectory)
                                {
                                    //ProgressBar=====================================================================================
                                    _mainWindowVM.ProgressBarValue++;
                                    comparer.TotalFiles++;
                                    _mainWindowVM.ProgressBarText = $"Проверено {_mainWindowVM.ProgressBarValue} из {countFiles}";
                                    //================================================================================================
                                }

                                if (token.IsCancellationRequested)
                                    return;

                            }
                        }
                    });
                }
                for (int i = files.Count - residue; i < files.Count; i++)
                {
                    string oldHash = files[i].FHash;
                    string oldVersion = files[i].FVersion;
                    string oldLastUpdate = files[i].FLastUpdate;
                    StatusFile oldStatus = files[i].Status;

                    FactParameterizer.SetFactValues(files[i]);

                    lock (_lock)
                    {
                        if (oldHash != files[i].FHash || oldVersion != files[i].FVersion || oldLastUpdate != files[i].FLastUpdate)
                        {
                            _oldStats.Add(files[i], (oldHash, oldVersion, oldLastUpdate, oldStatus));
                        }

                    }
                    lock (_lock)
                    {
                        comparer.SetStatus(files[i]);

                        _count++;
                        if (!files[i].IsDirectory)
                        {
                            //ProgressBar=====================================================================================
                            _mainWindowVM.ProgressBarValue++;
                            comparer.TotalFiles++;
                            _mainWindowVM.ProgressBarText = $"Проверено {_mainWindowVM.ProgressBarValue} из {countFiles}";
                            //================================================================================================
                        }
                    }
                }
            }

            while (true)
            {
                if (_count == files.Count || token.IsCancellationRequested)
                {

                    if (token.IsCancellationRequested)
                    {
                        lock(_lock)
                        {
                            SetOldFactParametres(files);
                            foreach (var item in comparer.OldStatuses.ToList())
                            {
                                item.Value.Item1.Status = item.Value.Item2;
                            }
                        }
                    }

                    //ProgressBar=====================================================================================
                    _mainWindowVM.ProgressBarValue = 0;
                    _mainWindowVM.ProgressBarMaximum = 0;
                    _mainWindowVM.ProgressBarText = string.Empty;
                    //================================================================================================
                    _count = 0;
                    _oldStats.Clear();
                    break;
                }
            }
            return comparer;
        }

        private static void SetOldFactParametres(List<FileTree> files)
        {
            foreach(var oldFile in _oldStats)
            {
                var file = files.Where(path=> path.Path == oldFile.Key.Path).FirstOrDefault();

                file.FHash = oldFile.Value.Item1;
                file.FVersion = oldFile.Value.Item2;
                file.FLastUpdate = oldFile.Value.Item3;
                Dispatcher.UIThread.Post(()=> file.Status = oldFile.Value.Item4);
            }
        }
    }
}
