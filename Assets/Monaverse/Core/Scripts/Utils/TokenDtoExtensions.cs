using Monaverse.Api.Modules.User.Dtos;

namespace Monaverse.Core.Scripts.Utils
{
    public static class TokenDtoExtensions
    {
        public static string GetMarketplaceUrl(this TokenDto tokenDto)
        {
            return tokenDto == null
                ? MonaConstants.MonaversePages.Marketplace
                : $"{MonaConstants.MonaversePages.Marketplace}/collections/{tokenDto.ChainId}/{tokenDto.Contract}/{tokenDto.TokenId}";
        }
    }
}