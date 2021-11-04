using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using MendeleySdk.Authorisation.Interfaces;
using MendeleySdk.Authorisation.Models;
using MendeleySdk.Authorisation.Services;
using MendeleySdk.Shared.Helpers.Platform;
using NSubstitute;
using Shouldly;
using Xunit;

namespace MendeleySdk.UnitTests.Authorization
{
    public class StandaloneAuthenticationManagerTests: AuthTestBase
    {
        private readonly StandaloneAuthenticationManager _authManager;
        
        private readonly string _expectedAuthCode;
        private readonly OAuthToken _expectedToken;
        private readonly OAuthToken _expectedRefreshToken;

        private readonly IAuthenticationListener _mockListener;
        private readonly IAuthenticationExchangeClient _mockClient;
        private readonly IOpener _mockOpener;

        public StandaloneAuthenticationManagerTests()
        {
            _expectedAuthCode = Fixture.Create<string>();
            _expectedToken = Fixture.Create<OAuthToken>();
            _expectedRefreshToken = Fixture.Create<OAuthToken>();

            _mockListener = Substitute.For<IAuthenticationListener>();
            _mockClient = Substitute.For<IAuthenticationExchangeClient>();
            _mockOpener = Substitute.For<IOpener>();

            _mockListener.ListenForOAuthCode(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(_expectedAuthCode);
            _mockClient.SwapAuthCodeForToken(Arg.Any<string>()).Returns(_expectedToken);
            _mockClient.RefreshToken(Arg.Any<OAuthToken>()).Returns(_expectedRefreshToken);
            
            _authManager = new(_mockListener, _mockClient, _mockOpener, OAuthOptions);
        }

        [Fact]
        public async Task CanGetToken()
        {
            OAuthToken res = await _authManager.GetToken();

            await _mockListener.Received(1).ListenForOAuthCode(Arg.Any<string>(), Arg.Any<CancellationToken>());
            _mockOpener.Received(1).OpenBrowser(Arg.Any<string>());
            await _mockClient.Received(1).SwapAuthCodeForToken(_expectedAuthCode);
            
            res.ShouldBe(_expectedToken);
        }

        [Fact]
        public async Task CanRefreshToken()
        {
            OAuthToken res = await _authManager.RefreshToken(_expectedToken);

            await _mockClient.Received(1).RefreshToken(_expectedToken);
            
            res.ShouldBe(_expectedRefreshToken);
        }
    }
}