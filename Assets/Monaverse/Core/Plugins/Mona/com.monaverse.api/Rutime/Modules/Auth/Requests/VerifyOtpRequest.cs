namespace Monaverse.Api.Modules.Auth.Requests
{
    public sealed record VerifyOtpRequest
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}