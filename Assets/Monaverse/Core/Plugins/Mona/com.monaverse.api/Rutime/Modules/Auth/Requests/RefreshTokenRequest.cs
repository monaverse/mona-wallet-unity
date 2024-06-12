namespace Monaverse.Api.Modules.Auth.Requests
{
    public sealed record RefreshTokenRequest
    {
        public string Refresh { get; set; }
    }
}