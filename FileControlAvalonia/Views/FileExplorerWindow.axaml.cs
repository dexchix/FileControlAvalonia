using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FileControlAvalonia.ViewModels;
using FileControlAvalonia.Helper;
using System;
using System.Timers;

namespace FileControlAvalonia.Views
{
    public partial class FileExplorerWindow : Window
    {
        private Timer timer;
        public FileExplorerWindow()
        {
            InitializeComponent();
            Deactivated += TEST;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        private void TEST(object sender, EventArgs args)
        {
            //Close();
            //new FileExplorerWindow().Show();
            ////Activate();
            ////Focus();
        }
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            Position = new PixelPoint(WindowAssistant.X_Coordinate + 400, WindowAssistant.Y_Coordinate + 70);
            CanResize = false;
        }
    }
}
