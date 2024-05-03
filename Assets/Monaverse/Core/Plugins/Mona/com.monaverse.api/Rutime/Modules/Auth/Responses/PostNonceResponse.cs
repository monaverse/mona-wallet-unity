namespace Monaverse.Api.Modules.Auth.Responses
{
    public sealed record PostNonceResponse
    {
        public string Nonce { get; set; }
        public bool IsExistingUser { get; set; }
    }
}