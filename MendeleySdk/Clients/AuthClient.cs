using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MendeleySdk.Helpers.Platform;
using WebWindowNetCore;

namespace MendeleySdk.Clients
{
    public class AuthClient
    {
        private const string Prefix = "http://localhost:5000/oauth/";
        private const string Response = "<html><body><h1>Successfully Authorised</h1></body></html>";
        private readonly byte[] _buf = System.Text.Encoding.UTF8.GetBytes(Response);
        //TODO - This should not be here
        public string GetAuthUrl()
        {
            const string authBase = "https://api.mendeley.com/oauth/authorize";
            var clientId = 10949;
            var redirect_uri = "http://localhost:5000/oauth";
            var response_type = "code";
            var scope = "all";

            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("client_id", clientId.ToString());
            queryString.Add("redirect_uri", redirect_uri);
            queryString.Add("response_type", response_type);
            queryString.Add("scope", scope);

            string authUrl = $"{authBase}?{queryString}";
            return authUrl;
        }

        public async Task<string> LoginInteractive()
        {
            string url = GetAuthUrl();
            OpenHelper.OpenBrowser(url);
            var token = await ListenForResponse();
            return token;
        }

        public async Task<string> ListenForResponse()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(Prefix);
            listener.Start();
            var ctx = await listener.GetContextAsync();
            var queryString = ctx.Request.QueryString;
            var token = queryString["code"];

            //Send response
            var outStream = ctx.Response.OutputStream;
            await outStream.WriteAsync(_buf, 0, _buf.Length);
            outStream.Close();
            listener.Stop();
            return token;
        }
    }
}