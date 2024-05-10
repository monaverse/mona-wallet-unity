namespace Monaverse.Api.Modules.Auth.Requests
{
    public sealed record AuthorizeRequest
    {
        public string Signature { get; set; }
        public string Message { get; set; }
    }
}