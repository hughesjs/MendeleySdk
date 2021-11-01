using System;
using System.Threading;
using System.Threading.Tasks;

namespace MendeleySdk.Authorisation.Interfaces
{
    public interface IAuthenticationListener : IDisposable
    {
        public Task<string> ListenForOAuthCode(string? state, CancellationToken cancelled);
    }
}