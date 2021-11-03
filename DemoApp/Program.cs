using DemoApp.Extensions;
using DemoApp.Services;
using DemoApp.Tui.Windows;
using MendeleySdk.Extensions;
using MendeleySdk.Helpers.Platform;
using MendeleySdk.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Terminal.Gui;

namespace DemoApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            IHostBuilder builder = CreateHostBuilder(args);
            IHost b = builder.Build();

            MainWindow main = b.Services.GetService<MainWindow>() ?? throw new("AGH!");
            Application.Run(main);
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices);

        private static void ConfigureServices(HostBuilderContext hbc, IServiceCollection services)
        {
            services.UseTextUserInterface();
            services.UseMendeleySdk(hbc.Configuration.Get<OAuthOptions>());

            services.AddTransient<IOpener, OpenHelper>();
            services.AddSingleton<ISessionManager, SimpleSessionManager>();
        }
    }
}