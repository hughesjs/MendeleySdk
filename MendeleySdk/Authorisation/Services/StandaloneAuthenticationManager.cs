using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using MendeleySdk.Authorisation.Models;
using MendeleySdk.Helpers.Platform;
using MendeleySdk.Options;
using Microsoft.Extensions.Options;

namespace MendeleySdk.Authorisation.Services
{
    /// <summary>
    /// Use this to get an OAuth token for a single-user interactively.
    /// It will open up a browser window and start listening on the configured port.
    /// Don't use this server-side.
    /// </summary>
    public sealed class StandaloneAuthenticationManager: IAuthenticationManager, IDisposable
    {
        private string? _state;

        private readonly IAuthenticationListener _listener;
        private readonly IAuthenticationExchangeClient _client;
        
        private readonly IOptions<OAuthOptions> _options;
        
        
        public StandaloneAuthenticationManager(IAuthenticationListener listener, IAuthenticationExchangeClient client, IOptions<OAuthOptions> options)
        {
            _listener = listener;
            _options = options;
            _client = client;
        }

        private async Task<OAuthToken> LoginInteractive(CancellationToken cToken)
        {
            OpenHelper.OpenBrowser(GetAuthUrl());
            string authCode =  await _listener.ListenForOAuthCode(_state, cToken);
            return await _client.SwapAuthCodeForToken(authCode);
        }

        public async Task<OAuthToken> GetToken(CancellationToken? cToken = null)
        {
            // TODO - Caching??
            
            return await LoginInteractive(cToken ?? CancellationToken.None);
        }

        private string GetAuthUrl()
        {
            _state = Guid.NewGuid().ToString();
            
            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("client_id", _options.Value.ApplicationId.ToString());
            queryString.Add("redirect_uri", _options.Value.RedirectUrl);
            queryString.Add("response_type", _options.Value.ResponseType);
            queryString.Add("scope", _options.Value.Scope);
            queryString.Add("state", _state);
            return $"{_options.Value.AuthBase}?{queryString}";
        }

        public void Dispose()
        {
            _listener.Dispose();
        }
    }

    public interface IAuthenticationManager
    {
        public Task<OAuthToken> GetToken(CancellationToken? cToken = null);
    }
}