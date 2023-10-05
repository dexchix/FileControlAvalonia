using FileControlAvalonia.Models;
using FileControlAvalonia.ViewModels;
using Splat;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FileControlAvalonia.Core
{
    public class ParallelProcessing
    {
        private static int _count = 0;
        private static object _lock = new object();
        private static MainWindowViewModel _mainWindowVM = Locator.Current.GetService<MainWindowViewModel>();

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

        public static Comprasion ParallelComprasion(List<FileTree> files, int countFiles)
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
                            FactParameterizer.SetFactValues(files[i]);
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
                    });
                }
                for (int i = files.Count - residue; i < files.Count; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
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
                if (_count == files.Count)
                {
                    //ProgressBar=====================================================================================
                    _mainWindowVM.ProgressBarValue = 0;
                    _mainWindowVM.ProgressBarMaximum = 0;
                    _mainWindowVM.ProgressBarText = string.Empty;
                    //================================================================================================
                    _count = 0;
                    break;
                }
            }
            return comparer;
        }
    }
}
