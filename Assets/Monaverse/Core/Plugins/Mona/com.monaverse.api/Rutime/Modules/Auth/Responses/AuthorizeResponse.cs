namespace Monaverse.Api.Modules.Auth.Responses
{
    public sealed record AuthorizeResponse
    {
        public string AccessToken { get; set; }
    }
}