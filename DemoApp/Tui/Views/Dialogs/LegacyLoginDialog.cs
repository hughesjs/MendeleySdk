using System;
using Terminal.Gui;

namespace DemoApp.Tui.Views.Dialogs
{
    public class LegacyLoginDialog : Dialog
    {
        public LegacyLoginDialog() : base("Legacy Authentication")
        {
            X = Pos.Center();
            Y = Pos.Center();
            Width = 40;
            Height = 9;
            Build();
        }

        public event EventHandler<(string username, string password)>? Submitted;

        private void Build()
        {
            Label usernameLabel = new("Username/Email");

            TextField usernameText = new()
                                     {
                                         Width = Dim.Fill(),
                                         Y = 1,
                                         CanFocus = true
                                     };
            Label passwordLabel = new("Password (This Will Not Be Hidden)");
            passwordLabel.Y = 3;

            TextField passwordText = new()
                                     {
                                         Width = Dim.Fill(),
                                         Y = 4,
                                         CanFocus = true
                                     };

            Button submitButton = new("Submit")
                                  {
                                      Y = 6,
                                      CanFocus = true,
                                      HotKey = Key.Enter
                                  };
            submitButton.Clicked += () => Submitted?.Invoke(this, (usernameText.Text.ToString()!, passwordText.Text.ToString()!));

            Add(usernameLabel);
            Add(usernameText);
            Add(passwordLabel);
            Add(passwordText);
            AddButton(submitButton);
        }
    }
}