using System;
using System.Net.Http;
using MendeleySdk.Clients;

namespace DemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AuthClient client = new AuthClient();
            string token = client.LoginInteractive().Result;
            Console.WriteLine(token);
        }
    }
}