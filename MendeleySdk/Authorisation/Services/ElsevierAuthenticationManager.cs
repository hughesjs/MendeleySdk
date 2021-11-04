using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MendeleySdk.Authorisation.Interfaces;
using MendeleySdk.Authorisation.Models;
using MendeleySdk.Authorisation.Options;
using MendeleySdk.Shared.Helpers.Platform;
using Microsoft.Extensions.Options;

namespace MendeleySdk.Authorisation.Services
{
    /// <summary>
    ///     Use this to get an OAuth token for a single-user interactively.
    ///     It will open up a browser window and start listening on the configured port.
    ///     Don't use this server-side.
    /// </summary>
    public sealed class StandaloneAuthenticationManager : IAuthenticationManager, IDisposable
    {
        private readonly IAuthenticationExchangeClient _client;
        private readonly IAuthenticationListener _listener;
        private readonly IOpener _opener;
        private readonly IOptions<OAuthOptions> _options;

        public StandaloneAuthenticationManager(IAuthenticationListener listener, IAuthenticationExchangeClient client, IOpener opener, IOptions<OAuthOptions> options)
        {
            _listener = listener;
            _options = options;
            _client = client;
            _opener = opener;
        }

        public async Task<OAuthToken> GetToken(CancellationToken? cToken = null) => await LoginInteractive(cToken ?? CancellationToken.None);
        public async Task<OAuthToken> RefreshToken(OAuthToken token, CancellationToken? cancellation = null) => await _client.RefreshToken(token);

        private async Task<OAuthToken> LoginInteractive(CancellationToken cToken)
        {
            string state = Guid.NewGuid().ToString();
            _opener.OpenBrowser(GetAuthUrl(state));
            string authCode = await _listener.ListenForOAuthCode(state, cToken);
            return await _client.SwapAuthCodeForToken(authCode);
        }

        private string GetAuthUrl(string state)
        {
            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("client_id", _options.Value.ApplicationId.ToString());
            queryString.Add("redirect_uri", _options.Value.RedirectUrl);
            queryString.Add("response_type", _options.Value.ResponseType);
            queryString.Add("scope", _options.Value.Scope);
            queryString.Add("state", state);
            return $"{_options.Value.AuthBase}?{queryString}";
        }
        
        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            _listener.Dispose();
        }
    }
}