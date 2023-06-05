using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.ViewModels
{
    public class InfoWindowViewModel: ReactiveObject
    {
        public void CloseWindow(Window window)
        {
            window.Close();
        }
    }
}
