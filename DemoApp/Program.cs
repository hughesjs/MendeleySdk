using System;
using System.Net;
using System.Threading;
using MendeleySdk.Authorisation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DemoApp
{
    static class Program
    {
        static void Main(string[] args)
        {
            IHostBuilder builder = CreateHostBuilder(args);
            IHost b = builder.Build();

            IAuthorisationManager? manager = b.Services.GetService<IAuthorisationManager>();
            
            //StandaloneAuthenticationManager manager = new(new AuthorisationListener(new(), Options.Create(new OAuthOptions())),Options.Create(new OAuthOptions()));
            CancellationTokenSource tokenSource = new();
            tokenSource.CancelAfter(TimeSpan.FromMinutes(1));
            Console.WriteLine(manager.GetToken(tokenSource.Token).Result);
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureServices(ConfigureServices);
        }

        private static void ConfigureServices(HostBuilderContext hbc, IServiceCollection services)
        {
            services.AddTransient<HttpListener>();
            services.AddTransient<IAuthorisationListener, AuthorisationListener>();
            services.AddSingleton<IAuthorisationManager, StandaloneAuthenticationManager>();

        }
    }
}