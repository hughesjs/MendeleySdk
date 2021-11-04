using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MendeleySdk.Extensions
{
    public static class HttpListenerRequestExtensions
    {
        private static readonly Regex QueryBreakupRegex = new(@"(?<key>.+?)=(?<value>.+?)(&|$)");

        public static async Task<Dictionary<string, string>> ReadUrlEncodedContentAsDictionaryAsync(this HttpListenerRequest req)
        {
            string content = await new StreamReader(req.InputStream, req.ContentEncoding).ReadToEndAsync();
            Dictionary<string, string> queryParams = QueryBreakupRegex.Matches(content).ToDictionary(m => m.Groups["key"].Value, m => m.Groups["value"].Value);
            return queryParams;
        }
    }
}