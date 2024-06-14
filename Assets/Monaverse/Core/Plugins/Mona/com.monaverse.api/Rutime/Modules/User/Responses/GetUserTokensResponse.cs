using System.Collections.Generic;
using Monaverse.Api.Modules.User.Dtos;

namespace Monaverse.Api.Modules.User.Responses
{
    public record GetUserTokensResponse
    {
        public List<TokenDto> Tokens { get; set; }
        public string Continuation { get; set; }
    }
}