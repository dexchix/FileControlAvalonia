using FileControlAvalonia.Models;
using FileControlAvalonia.ViewModels;
using Splat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileControlAvalonia.Core
{
    public class ParallelProcessing
    {
        private static int _count = 0;
        private static object _lock = new object();

        public static void ParallelCalculateFactParametrs(List<FileTree> files, int countFiles)
        {
            if (countFiles <= 1000)
            {
                foreach (FileTree file in files)
                    FactParameterizer.SetFactValues(file);
                return;
            }
            else
            {
                int start = 0;
                int limit = 1;
                var section = countFiles / 8;
                int residue = countFiles - section * 8;
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
                                //ProgressBar=====================================================================================
                                Locator.Current.GetService<MainWindowViewModel>().ProgressBarValue++;
                                Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = $"Вычисление {files[i].Path}";
                                //================================================================================================
                            }
                        }
                    });
                }
                for (int i = countFiles - residue; i < countFiles; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
                    lock (_lock)
                    {
                        _count++;
                        //ProgressBar=====================================================================================
                        Locator.Current.GetService<MainWindowViewModel>().ProgressBarValue++;
                        Locator.Current.GetService<MainWindowViewModel>().ProgressBarText = $"Вычисление {files[i].Path}";
                        //================================================================================================
                    }
                }
            }

            while (true)
            {
                if (_count == countFiles)
                {
                    //ProgressBar=====================================================================================
                    Locator.Current.GetService<MainWindowViewModel>().ProgressBarValue=0;
                    Locator.Current.GetService<MainWindowViewModel>().ProgressBarMaximum = 0;
                    //================================================================================================
                    _count = 0;
                    break;
                }
            }
        }
    }
}
