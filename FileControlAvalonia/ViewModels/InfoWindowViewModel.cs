using Avalonia.Controls;
using ReactiveUI;

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
