using System;
using System.Net;
using System.Threading.Tasks;
using MendeleySdk.Authorisation;
using MendeleySdk.Options;
using Microsoft.Extensions.Options;
using Terminal.Gui;

namespace DemoApp.Tui.Views
{
    public class MainContent : FrameView
    {
        public MainContent() : base("Content")
        {
            Y = 1;
            X = 25;
            Width = Dim.Fill();
            Height = Dim.Fill();

            CreateAuthWindow(); // TODO - Move this into derived class
        }

        private void CreateAuthWindow()
        {
            Button authButton = new(1, 1, "Login With OAuth2 (Opens in Browser)");
            authButton.Clicked += async () => await LogInWithOAuth();
            Add(authButton);
        }

        private async Task LogInWithOAuth()
        {
            IOptions<OAuthOptions> opts = Options.Create<OAuthOptions>(new()); // TODO - DI this crap
            StandaloneAuthenticationManager authManager = new(new AuthorisationListener(new(), opts), opts); 
            string token = await authManager.GetToken(); //TODO - Async this shit
            Add(new Label($"OAuth2 Token: {token}") { X = Pos.Center(), Y = Pos.Center() });
        }
    }
}