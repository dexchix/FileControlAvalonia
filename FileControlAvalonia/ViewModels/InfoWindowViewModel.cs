using Avalonia.Controls;
using FileControlAvalonia.ViewModels.Interfaces;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.ViewModels
{
    public class InfoWindowViewModel: ReactiveObject, IInfoWindowViewModel
    {
        public void CloseWindow(Window window)
        {
            window.Close();
        }
    }
}
