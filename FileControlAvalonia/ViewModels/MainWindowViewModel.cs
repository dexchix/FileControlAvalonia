using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using FileControlAvalonia.Models;
using FileControlAvalonia.Views;
using HarfBuzzSharp;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace FileControlAvalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region FIELDS
        public static ObservableCollection<FileTree> fileTree;
        public HierarchicalTreeDataGridSource<FileTree> _source;
        #endregion
        #region PROPERTIES
        public ObservableCollection<FileTree> FileTree
        {
            get => fileTree;
            set => this.RaiseAndSetIfChanged(ref fileTree, value);
        }
        public HierarchicalTreeDataGridSource<FileTree> Source 
        {
            get => _source;
            set => this.RaiseAndSetIfChanged(ref  _source, value);
        }
        #endregion

        public MainWindowViewModel()
        {
            Source = new HierarchicalTreeDataGridSource<FileTree>(FileTree)
            {
                Columns =
                {
                    new HierarchicalExpanderColumn<FileTree>(
                    new TextColumn<FileTree,string>("Имя", x=> x.Name), x=>x.Children),
                    new TextColumn<FileTree,string>("Путь", x=> x.Path),
                    new TextColumn<FileTree, string>("Родитель", x => x.Parent.Name)
                },
            };
        }

        #region COMMANDS
        public void CloseProgram()
        {
            App.CurrentApplication!.Shutdown();
        }
        public void OpenFileExplorer()
        {

            var fileExplorer = new FileExplorerWindow();
            var fghfgh = fileExplorer.DataContext;
            fileExplorer.Show();
        }
        public void TEST()
        {
            FileTree = new ObservableCollection<FileTree>()
            {
                new FileTree("C:\\1\\2",true),
            };
            Source = new HierarchicalTreeDataGridSource<FileTree>(FileTree)
            {
                Columns =
                {
                    new HierarchicalExpanderColumn<FileTree>
                        (
                            new TextColumn<FileTree,string>("Имя", x=> x.Name), x=>x.Children),
                    new TextColumn<FileTree,string>("Путь", x=> x.Path),
                    new TextColumn<FileTree, string>("Родитель", x => x.Parent.Name)
                },
            };
        }
        #endregion
    }
}