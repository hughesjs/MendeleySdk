using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MendeleySdk.Authorisation.Interfaces;
using MendeleySdk.Authorisation.Options;
using Microsoft.Extensions.Options;

namespace MendeleySdk.Authorisation.Services
{
    public sealed class AuthenticationListener : IAuthenticationListener // TODO - Make this internal
    {
        private readonly ReadOnlyMemory<byte> _buf = Encoding.UTF8.GetBytes("<html><body><h1>Successfully Authorised</h1></body></html>");

        private readonly HttpListener _listener;

        public AuthenticationListener(HttpListener listener, IOptions<OAuthOptions> options)
        {
            _listener = listener;
            listener.Prefixes.Add(options.Value.RedirectUrl);
        }

        public async Task<string> ListenForOAuthCode(string? state, CancellationToken cancelled)
        {
            _listener.Start();
            HttpListenerContext ctx = await _listener.GetContextAsync(); //TODO - Make this cancellable

            NameValueCollection queryString = ctx.Request.QueryString;
            string? token = queryString["code"];
            string? stateRec = queryString["state"];
            if (state != stateRec)
            {
                ctx.Response.OutputStream.Close();
                throw new AuthenticationException("Possible CSF attack detected");
            }

            if (string.IsNullOrEmpty(token))
            {
                ctx.Response.OutputStream.Close();
                throw new AuthenticationException("OAuth did not return a token");
            }

            //Send response
            Stream outStream = ctx.Response.OutputStream;
            await outStream.WriteAsync(_buf, cancelled);

            outStream.Close();
            _listener.Stop();

            return token;
        }

        public void Dispose()
        {
            ((IDisposable)_listener).Dispose();
        }
    }
}