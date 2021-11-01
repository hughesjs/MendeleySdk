using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using DemoApp.Tui.Views;
using DemoApp.Tui.Views.Dialogs;
using DemoApp.Tui.Windows;
using MendeleySdk.Authorisation;
using MendeleySdk.Authorisation.Services;
using MendeleySdk.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Terminal.Gui;

namespace DemoApp
{
    static class Program
    {
        static void Main(string[] args)
        {
            // Application.Init();
            
            IHostBuilder builder = CreateHostBuilder(args);
            IHost b = builder.Build();

            var manager = b.Services.GetService<IAuthenticationManager>();
            _ = manager.GetToken().Result;

            // MainWindow main = b.Services.GetService<MainWindow>() ?? throw new("AGH!");
            //
            // Application.Top.Add(main);
            // Application.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureServices(ConfigureServices);
        }

        private static void ConfigureServices(HostBuilderContext hbc, IServiceCollection services)
        {
            services.AddTransient<HttpListener>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainContent>();
            services.AddSingleton<MainMenu>();
            
            
            services.AddTransient<LegacyLoginDialog>();
            services.AddTransient<IAuthenticationListener, AuthenticationListener>();
            services.AddTransient<IAuthenticationManager, StandaloneAuthenticationManager>();

            var oauth = hbc.Configuration.Get<OAuthOptions>();
            services.AddHttpClient<IAuthenticationExchangeClient, AuthenticationExchangeClient>(c =>
                                                                                                {
                                                                                                    c.BaseAddress = new("https://api.mendeley.com/");
                                                                                                    string authString = $"{oauth.ApplicationId}:{oauth.Secret}";
                                                                                                    string encoded = Convert.ToBase64String(Encoding.ASCII.GetBytes(authString));
                                                                                                    c.DefaultRequestHeaders.Authorization = new("Basic", encoded);
                                                                                                });
        }
    }
}