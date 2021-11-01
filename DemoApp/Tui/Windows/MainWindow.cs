using DemoApp.Tui.Views;
using Terminal.Gui;

namespace DemoApp.Tui.Windows
{
    public sealed class MainWindow : Window
    {
        private readonly MainContent _mainContent;
        private readonly MainMenu _mainMenu;

        public MainWindow(MainContent mainContent, MainMenu mainMenu) : base("Mendeley SDK Demo")
        {
            _mainContent = mainContent;
            _mainMenu = mainMenu;

            X = 0;
            Y = 1;
            Width = Dim.Fill();
            Height = Dim.Fill();
            LayoutStyle = LayoutStyle.Computed;

            Init();
        }

        private void Init()
        {
            Add(_mainMenu);
            Add(_mainContent);
        }
    }
}