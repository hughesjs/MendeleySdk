using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using MendeleySdk.Authorisation;
using MendeleySdk.Options;
using Microsoft.Extensions.Options;

namespace DemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AuthorisationManager manager = new(new(new(), Options.Create(new OAuthOptions())),Options.Create(new OAuthOptions()));
            CancellationTokenSource tokenSource = new();
            tokenSource.CancelAfter(TimeSpan.FromMinutes(1));
            Console.WriteLine(manager.GetToken(tokenSource.Token).Result);
        }
    }
}