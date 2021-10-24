using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using MendeleySdk.Helpers.Platform;

namespace MendeleySdk.Clients
{
    public class AuthClient
    {
        //TODO - Put all of this in appsettings
        private const string RedirectUrl = "http://localhost:5000/oauth/";
        private const string AuthBase = "https://api.mendeley.com/oauth/authorize";
        private const string ResponseType = "code";
        private const string Scope = "all";
        private const int ClientId = 10949;
        
        private readonly ReadOnlyMemory<byte> _buf = System.Text.Encoding.UTF8.GetBytes("<html><body><h1>Successfully Authorised</h1></body></html>");
        
        private string GetAuthUrl()
        {
            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("client_id", ClientId.ToString());
            queryString.Add("redirect_uri", RedirectUrl);
            queryString.Add("response_type", ResponseType);
            queryString.Add("scope", Scope);

            return $"{AuthBase}?{queryString}";
        }

        public async Task<string> LoginInteractive()
        {
            //TODO - needs cancellation
            OpenHelper.OpenBrowser(GetAuthUrl());
            return await ListenForResponse();
        }

        private async Task<string> ListenForResponse()
        {
            HttpListener listener = new();
            listener.Prefixes.Add(RedirectUrl);
            
            listener.Start();
            HttpListenerContext ctx = await listener.GetContextAsync();
            
            NameValueCollection queryString = ctx.Request.QueryString;
            string? token = queryString["code"];

            if (string.IsNullOrEmpty(token))
            {
                throw new AuthenticationException("OAuth did not return a token");
            }
            
            //Send response
            Stream outStream = ctx.Response.OutputStream;
            await outStream.WriteAsync(_buf);
            
            outStream.Close();
            listener.Stop();
            
            return token;
        }
    }
}