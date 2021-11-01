using System;
using System.Net;
using System.Text;
using DemoApp.Tui.Views;
using DemoApp.Tui.Views.Dialogs;
using DemoApp.Tui.Windows;
using MendeleySdk.Authorisation.Interfaces;
using MendeleySdk.Authorisation.Services;
using MendeleySdk.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DemoApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // Application.Init();

            IHostBuilder builder = CreateHostBuilder(args);
            IHost b = builder.Build();

            IAuthenticationManager? manager = b.Services.GetService<IAuthenticationManager>();
            Console.WriteLine(manager.GetToken().Result.AccessToken);
                                                                    
            // MainWindow main = b.Services.GetService<MainWindow>() ?? throw new("AGH!");
            //
            // Application.Top.Add(main);
            // Application.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices);

        private static void ConfigureServices(HostBuilderContext hbc, IServiceCollection services)
        {
            services.AddTransient<HttpListener>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainContent>();
            services.AddSingleton<MainMenu>();


            services.AddTransient<LegacyLoginDialog>();
            services.AddTransient<IAuthenticationListener, AuthenticationListener>();
            services.AddTransient<IAuthenticationManager, StandaloneAuthenticationManager>();

            OAuthOptions? oauth = hbc.Configuration.Get<OAuthOptions>();
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