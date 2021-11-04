namespace MendeleySdk.Authorisation.Options
{
    public class OAuthOptions
    {
        public string RedirectUrl { get; init; } = "http://localhost:5000/oauth/";
        public string AuthBase { get; init; } = "https://api.mendeley.com/oauth/authorize";
        public string ResponseType { get; init; } = "code";
        public string Scope { get; init; } = "all";
        public int ApplicationId { get; init; } = 10949;

        public string Secret { get; init; } = "8l6PIvjclztWUF31"; // TODO - remove this and invalidate when live
    }
}