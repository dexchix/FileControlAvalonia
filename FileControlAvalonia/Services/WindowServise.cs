using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Services
{
    public class WindowServise : IWindowService
    {
        public void ShowWindow<T>() where T : Window, new()
        {
            var childWindow = new T();
            childWindow.Show();
        }
    }
}
