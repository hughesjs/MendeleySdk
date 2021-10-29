using DemoApp.Tui.Views;
using Terminal.Gui;

namespace DemoApp.Tui.Windows
{
    public sealed class MainWindow : Window
    {
        public MainWindow() : base("Mendeley SDK Demo")
        {
            X = 0;
            Y = 1;
            Width = Dim.Fill();
            Height = Dim.Fill();
            LayoutStyle = LayoutStyle.Computed;
            Add(new MainMenu());
            Add(new MainContent());
        }
    }
}