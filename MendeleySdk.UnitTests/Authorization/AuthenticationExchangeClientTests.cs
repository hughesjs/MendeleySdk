using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using MendeleySdk.Authorisation.Interfaces;
using MendeleySdk.Authorisation.Models;
using MendeleySdk.Authorisation.Services;
using MendeleySdk.Extensions;
using Shouldly;
using Xunit;

namespace MendeleySdk.UnitTests.Authorization
{
    public class AuthenticationExchangeClientTests: AuthTestBase, IDisposable
    {
        private readonly IAuthenticationExchangeClient _authClient;
        private readonly HttpListener _listener;
        private readonly HttpClient _client;

        private readonly string _authCode;
        private readonly OAuthToken _token;
        private readonly OAuthToken _refreshedToken;

        public AuthenticationExchangeClientTests()
        {
            _authCode = Fixture.Create<string>();
            _token = Fixture.Create<OAuthToken>();
            _refreshedToken = Fixture.Create<OAuthToken>();
            
            _client = new() { BaseAddress = new(OAuthOptions.Value.AuthBase) };
            _listener = new() { Prefixes = { OAuthOptions.Value.AuthBase } };
            _authClient = new AuthenticationExchangeClient(_client, OAuthOptions);
            
            _listener.Start();
        }

        [Fact]
        public async Task CanSwapAuthCodeForToken()
        {
            Task<OAuthToken> tokenTask = _authClient.SwapAuthCodeForToken(_authCode);
            HttpListenerContext ctx = await _listener.GetContextAsync();
            
            Dictionary<string, string> content = await ctx.Request.ReadUrlEncodedContentAsDictionaryAsync();
            
            content["code"].ShouldContain(_authCode);
            content["grant_type"].ShouldBe("authorization_code");

            byte[] payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(_token));
            await ctx.Response.OutputStream.WriteAsync(payload);
            ctx.Response.Close();
            
            tokenTask.Wait(TimeSpan.FromSeconds(5));

            tokenTask.Result.ShouldBe(_token);
        }

        [Fact]
        public async Task CanRefreshToken()
        {
            Task<OAuthToken> tokenTask = _authClient.RefreshToken(_token);
            HttpListenerContext ctx = await _listener.GetContextAsync();
            
            Dictionary<string, string> content = await ctx.Request.ReadUrlEncodedContentAsDictionaryAsync();

            content["refresh_token"].ShouldBe(_token.RefreshToken);
            content["grant_type"].ShouldBe("refresh_token");
            
            byte[] payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(_refreshedToken));
            await ctx.Response.OutputStream.WriteAsync(payload);
            ctx.Response.Close();

            tokenTask.Wait(TimeSpan.FromSeconds(5));
            
            tokenTask.Result.ShouldBe(_refreshedToken);
        }
        
        public void Dispose()
        {
            ((IDisposable)_listener)?.Dispose();
            _client?.Dispose();
        }
    }
}