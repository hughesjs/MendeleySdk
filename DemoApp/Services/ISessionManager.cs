using System.Threading;
using System.Threading.Tasks;
using MendeleySdk.Authorisation.Models;

namespace DemoApp.Services
{
    public interface ISessionManager
    {
        public Task Login(CancellationToken cancellation);
        public bool IsLoggedIn();
        public Task<OAuthToken> GetToken();
    }
}