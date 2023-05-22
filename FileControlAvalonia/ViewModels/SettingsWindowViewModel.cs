using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.ViewModels
{
    public class SettingsWindowViewModel
    {
        public void CloseWindow(Window window)
        {
            window.Close();
        }
    }
}
