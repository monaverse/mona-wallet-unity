namespace Monaverse.Api.Modules.Auth.Requests
{
    public sealed record GenerateOtpRequest
    {
        public string Email { get; set; }
    }
}