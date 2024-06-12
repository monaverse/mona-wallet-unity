namespace Monaverse.Api.Modules.Auth.Responses
{
    public record VerifyOtpResponse
    {
        public string Access { get; set; }
        public string Refresh { get; set; }
    }
}