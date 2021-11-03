using System;
using System.Net;
using System.Text;
using MendeleySdk.Authorisation.Interfaces;
using MendeleySdk.Authorisation.Services;
using MendeleySdk.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MendeleySdk.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseMendeleySdk(this IServiceCollection services, OAuthOptions options) // Replace options with a parent wrapper if needs be
        {
            services.AddTransient<HttpListener>();
            services.AddTransient<IAuthenticationListener, AuthenticationListener>();
            services.AddTransient<IAuthenticationManager, StandaloneAuthenticationManager>();
            
            services.AddHttpClient<IAuthenticationExchangeClient, AuthenticationExchangeClient>(c =>
                                                                                                {
                                                                                                    c.BaseAddress = new("https://api.mendeley.com/");
                                                                                                    string authString = $"{options.ApplicationId}:{options.Secret}";
                                                                                                    string encoded = Convert.ToBase64String(Encoding.ASCII.GetBytes(authString));
                                                                                                    c.DefaultRequestHeaders.Authorization = new("Basic", encoded);
                                                                                                });
            return services;
        }
    }
}