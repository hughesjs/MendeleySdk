using System.Text.Json.Serialization;

namespace MendeleySdk.Authorisation.Models
{
    public class ExchangePayload
    {
        // [JsonPropertyName("username")] //Yeah, this is dumb but that's what Mendeley called it...
        // public int ApplicationId { get; init; }
        //
        // [JsonPropertyName("password")]
        // public string Password { get; init; }
        
        [JsonPropertyName("grant_type")]
        public string GrantType { get; init; }
        
        [JsonPropertyName("code")]
        public string Code { get; init; }
        
        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; init; }
    }
}