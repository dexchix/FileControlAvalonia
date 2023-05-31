using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Services
{
    interface IWindowService
    {
        void ShowWindow<T>() where T : Window, new()
        {

        }
    }
}
