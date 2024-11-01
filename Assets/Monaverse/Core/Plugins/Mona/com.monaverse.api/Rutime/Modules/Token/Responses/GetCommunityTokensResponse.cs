using System.Collections.Generic;
using Monaverse.Api.Modules.User.Dtos;

namespace Monaverse.Api.Modules.Token.Responses
{
    public record GetCommunityTokensResponse
    {
        public List<TokenDto> Tokens { get; set; }
        public string Continuation { get; set; }
    }
}