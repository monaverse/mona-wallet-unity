namespace Monaverse.Api.Modules.Auth.Requests
{
    public sealed record PostNonceRequest
    {
        public string WalletAddress { get; set; }
    }
}