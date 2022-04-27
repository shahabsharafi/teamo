namespace Teamo.Identity.API.Infrastructure
{
    public class IdentitySettings
    {
        public static readonly string TOKEN_PROVIDER_NONE = "None";
        public string TokenProvider { get; set; } = TOKEN_PROVIDER_NONE;
    }
}
