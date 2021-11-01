using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MendeleySdk.Authorisation.Models;
using MendeleySdk.Options;
using Microsoft.Extensions.Options;

namespace MendeleySdk.Authorisation.Services
{
    public class AuthenticationExchangeClient : IAuthenticationExchangeClient
    {
        private readonly HttpClient _client;
        private readonly OAuthOptions _options;


        public AuthenticationExchangeClient(HttpClient client, IOptions<OAuthOptions> options)
        {
            _client = client;
            //_client = new(new LoggingHandler(new HttpClientHandler()));
            // _client.BaseAddress = new("https://api.mendeley.com/");
            // string authString = $"{options.Value.ApplicationId}:{options.Value.Secret}";
            // string encoded = Convert.ToBase64String(Encoding.ASCII.GetBytes(authString));
            // _client.DefaultRequestHeaders.Authorization = new("Basic", encoded);
            _options = options.Value;
        }

        public async Task<OAuthToken> SwapAuthCodeForToken(string authCode)
        {
            HttpRequestMessage req = new();
            req.Content = new FormUrlEncodedContent(ConstructPayload(authCode));
            req.RequestUri = new($"{_client.BaseAddress}oauth/token");
            req.Method = HttpMethod.Post;
            
            HttpResponseMessage res = await _client.SendAsync(req);
            OAuthToken? token = await res.Content.ReadFromJsonAsync<OAuthToken>();
            if (token is null)
            {
                throw new AuthenticationException("OAuth Exchange Failed");
            }

            return token;
        }

        private IEnumerable<KeyValuePair<string?,string?>> ConstructPayload(string authCode)
            => new List<KeyValuePair<string?,string?>>
               {
                   new("grant_type", "authorization_code"),
                   new("code", authCode),
                   new("redirect_uri", _options.RedirectUrl)
               };

}

    public interface IAuthenticationExchangeClient
    {
        public Task<OAuthToken> SwapAuthCodeForToken(string authCode);
    }
    
    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine("Request:");
            Console.WriteLine(request.ToString());
            if (request.Content != null)
            {
                Console.WriteLine(await request.Content.ReadAsStringAsync());
            }
            Console.WriteLine();

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            Console.WriteLine("Response:");
            Console.WriteLine(response.ToString());
            if (response.Content != null)
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            Console.WriteLine();

            return response;
        }
    }
}