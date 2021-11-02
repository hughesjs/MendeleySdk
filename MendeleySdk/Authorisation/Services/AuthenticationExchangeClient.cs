using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Threading.Tasks;
using MendeleySdk.Authorisation.Interfaces;
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

        private IEnumerable<KeyValuePair<string?, string?>> ConstructPayload(string authCode)
            => new List<KeyValuePair<string?, string?>>
               {
                   new("grant_type", "authorization_code"),
                   new("code", authCode),
                   new("redirect_uri", _options.RedirectUrl)
               };
    }
}