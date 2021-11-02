using System;
using System.Threading;
using System.Threading.Tasks;
using DemoApp.Exceptions;
using MendeleySdk.Authorisation.Interfaces;
using MendeleySdk.Authorisation.Models;

namespace DemoApp.Services
{
    public class SimpleSessionManager: ISessionManager
    {
        private readonly IAuthenticationManager _authManager;
        
        private OAuthToken? _token;
        private DateTime? _tokenCreatedOn;
        
        public SimpleSessionManager(IAuthenticationManager authManager)
        {
            _authManager = authManager;
        }

        public async Task<OAuthToken> GetToken()
        {
            if (_token is null || _tokenCreatedOn is null)
            {
                throw new NotLoggedInException();
            }
            
            DateTime expiresOn = (DateTime)_tokenCreatedOn + TimeSpan.FromSeconds(_token.ExpiresIn);

            if (expiresOn < DateTime.Now)
            {
                _token = await _authManager.RefreshToken(_token);
            }

            _tokenCreatedOn = DateTime.Now;
            return _token;
        }

        public async Task Login(CancellationToken cancellation)
        {
            _token = await _authManager.GetToken(cancellation);
        }

        public bool IsLoggedIn() => _token is not null;
    }
}