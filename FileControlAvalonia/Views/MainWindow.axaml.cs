using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Avalonia.Platform;
using System;

namespace FileControlAvalonia.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Deactivated += TEST; 
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            Position = new PixelPoint(300, 300);
            CanResize = false;
        }
        private void TEST (object sender, EventArgs e)
        {
            
        }
    }
}