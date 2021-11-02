using System.Net;
using DemoApp.Tui.Views;
using DemoApp.Tui.Views.Dialogs;
using DemoApp.Tui.Windows;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui;

namespace DemoApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseTextUserInterface(this IServiceCollection services)
        {
            Application.Init();
            services.AddTransient<HttpListener>();
            services.AddTransient<MainWindow>();
            services.AddTransient<MainContent>();
            services.AddTransient<MainMenu>();
            services.AddTransient<LegacyLoginDialog>();
            return services;
        }
    }
}