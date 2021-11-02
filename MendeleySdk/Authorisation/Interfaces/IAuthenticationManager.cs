using System.Threading;
using System.Threading.Tasks;
using MendeleySdk.Authorisation.Models;

namespace MendeleySdk.Authorisation.Interfaces
{
    public interface IAuthenticationManager
    {
        public Task<OAuthToken> GetToken(CancellationToken? cancellation = null);

        public Task<OAuthToken> RefreshToken(OAuthToken token, CancellationToken? cancellation = null);
    }
}