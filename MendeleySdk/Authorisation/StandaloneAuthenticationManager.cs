using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using MendeleySdk.Helpers.Platform;
using MendeleySdk.Options;
using Microsoft.Extensions.Options;

namespace MendeleySdk.Authorisation
{
    /// <summary>
    /// Use this to get an OAuth token for a single-user interactively.
    /// It will open up a browser window and start listening on the configured port.
    /// Don't use this server-side.
    /// </summary>
    public sealed class StandaloneAuthenticationManager: IAuthorisationManager, IDisposable
    {
        private static string? _currentToken;

        private readonly IAuthorisationListener _listener;
        private readonly IOptions<OAuthOptions> _options;
        
        public StandaloneAuthenticationManager(IAuthorisationListener listener, IOptions<OAuthOptions> options)
        {
            _listener = listener;
            _options = options;
        }

        private async Task<string> LoginInteractive(CancellationToken cToken)
        {
            OpenHelper.OpenBrowser(GetAuthUrl());
            return await _listener.ListenForOAuthToken(cToken);
        }

        public async Task<string> GetToken(CancellationToken? cToken = null)
        {
            return _currentToken ??= await LoginInteractive(cToken ?? CancellationToken.None);
        }

        private string GetAuthUrl()
        {
            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("client_id", _options.Value.ClientId.ToString());
            queryString.Add("redirect_uri", _options.Value.RedirectUrl);
            queryString.Add("response_type", _options.Value.ResponseType);
            queryString.Add("scope", _options.Value.Scope);
            return $"{_options.Value.AuthBase}?{queryString}";
        }

        public void Dispose()
        {
            _listener.Dispose();
        }
    }

    public interface IAuthorisationManager
    {
        public Task<string> GetToken(CancellationToken? cToken = null);
    }
}