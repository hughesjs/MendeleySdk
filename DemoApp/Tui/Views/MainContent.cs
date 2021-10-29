using System;
using System.Net;
using System.Threading.Tasks;
using DemoApp.Tui.Views.Dialogs;
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
            Button oauthButton = new(1, 1, "Login With OAuth2 (Opens in Browser)");
            Button legacyButton = new(1, 2, "Login With Legacy Authentication");
            oauthButton.Clicked += async () => await LogInWithOAuth();
            legacyButton.Clicked += () =>
                                    {
                                        LegacyLoginDialog lld = new();
                                        Add(lld);
                                        lld.FocusFirst();
                                        lld.Submitted += async (_, e) =>
                                                         {
                                                             Remove(lld);
                                                             await LogInWithLegacy(e.username, e.password);
                                                         };
                                    };
            Add(oauthButton);
            Add(legacyButton);
        }

        private async Task LogInWithOAuth()
        {
            IOptions<OAuthOptions> opts = Options.Create<OAuthOptions>(new()); // TODO - DI this crap
            StandaloneAuthenticationManager authManager = new(new AuthorisationListener(new(), opts), opts); 
            string token = await authManager.GetToken(); //TODO - Async this shit
            Add(new Label($"OAuth2 Token: {token}") { X = Pos.Center(), Y = Pos.Center() });
        }

        private async Task LogInWithLegacy(string username, string password)
        {
            ;
        }
    }
}