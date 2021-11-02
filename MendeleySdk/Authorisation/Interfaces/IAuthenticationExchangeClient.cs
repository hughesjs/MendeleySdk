using System.Threading.Tasks;
using MendeleySdk.Authorisation.Models;

namespace MendeleySdk.Authorisation.Interfaces
{
    public interface IAuthenticationExchangeClient
    {
        public Task<OAuthToken> SwapAuthCodeForToken(string authCode);
        public Task<OAuthToken> RefreshToken(OAuthToken oAuthToken);
    }
}