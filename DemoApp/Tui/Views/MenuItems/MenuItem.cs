using Terminal.Gui;

namespace DemoApp.Tui.Views.MenuItems
{
    abstract class MenuItem
    {
        private Toplevel Top;
        private Window Window;

        protected virtual void Init(Toplevel? top = null)
        {
            Top = top ?? Application.Top;

            Window = new Window("M");
            Top.Add(Window);
        }
    }
}