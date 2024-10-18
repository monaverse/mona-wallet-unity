namespace Monaverse.Api.Modules.Auth.Requests
{
    public sealed record SignUpRequest
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
    }
}