using FileControlAvalonia.Views;
using HarfBuzzSharp;

namespace FileControlAvalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region FIELDS
        #endregion
        #region PROPERTIES
        #endregion
        #region COMMANDS
        public void OpenFileExplorer()
        {

            var fileExplorer = new FileExplorerWindow();
            var fghfgh = fileExplorer.DataContext;
            fileExplorer.Show();
        }
        #endregion
    }
}