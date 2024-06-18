using System.Collections.Generic;

namespace Monaverse.Api.Modules.User.Responses
{
    public record GetUserResponse
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public List<string> Wallets { get; set; }
    }
}