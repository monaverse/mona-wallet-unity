namespace Monaverse.Api.Modules.Auth.Responses
{
    public record RefreshTokenResponse
    {
        public string Access { get; set; }
        public string Refresh { get; set; }
    }
}