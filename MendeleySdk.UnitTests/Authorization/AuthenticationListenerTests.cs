using System;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using MendeleySdk.Authorisation.Interfaces;
using MendeleySdk.Authorisation.Services;
using Shouldly;
using Xunit;

namespace MendeleySdk.UnitTests.Authorization
{
    public sealed class AuthenticationListenerTests: AuthTestBase, IDisposable
    {
        private readonly IAuthenticationListener _authListener;
        private readonly Task<string> _listenTask;
        private readonly Guid _state;
        private readonly HttpClient _testClient;
        private readonly string _expectedAuthCode;
        

        public AuthenticationListenerTests()
        {
            _state = Guid.NewGuid();
            _expectedAuthCode = Fixture.Create<string>();
            _testClient = new(){BaseAddress = new(OAuthOptions.Value.RedirectUrl)};
            _authListener = new AuthenticationListener(new(), OAuthOptions);
            _listenTask = _authListener.ListenForOAuthCode(_state.ToString(), CancellationToken.None);
        }

        [Fact]
        public async Task GetsAuthCodeFromResponse()
        {
            _ = await _testClient.GetAsync($"?state={_state}&code={_expectedAuthCode}");

            _listenTask.Wait(TimeSpan.FromSeconds(5));
            
            _listenTask.Result.ShouldBe(_expectedAuthCode);
        }

        [Fact]
        public async Task ThrowsExceptionIfReplyIsNullOrEmpty()
        {
            _ = await _testClient.GetAsync($"?state={_state}&code=");
            
            Should.Throw<AggregateException>(_listenTask.Wait)
                  .InnerExceptions.First().ShouldBeOfType<AuthenticationException>();
        }

        [Fact]
        public async Task ThrowsExceptionIfPossibleCsfDetected()
        {
            _ = await _testClient.GetAsync($"?state=bollocks&code={_expectedAuthCode}");

            Should.Throw<AggregateException>(_listenTask.Wait)
                  .InnerExceptions.First().ShouldBeOfType<AuthenticationException>();
        }

        public void Dispose()
        {
            _authListener.Dispose();
            _listenTask.Dispose();
            _testClient.Dispose();
        }
    }
}