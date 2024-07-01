using System;
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
        
        public static decimal GetUsdPrice(this TokenDto tokenDto)
        {
            return tokenDto?.FloorAsk?.Price?.Amount?.Usd ?? 0m;
        }
        
        public static decimal GetNativePrice(this TokenDto tokenDto, int precision = 6)
        {
            return decimal.Round(tokenDto?.FloorAsk?.Price?.Amount?.Native ?? 0m, precision, MidpointRounding.AwayFromZero);
        }

        public static string GetCurrency(this TokenDto tokenDto)
        {
            return tokenDto?.FloorAsk?.Price?.Currency?.Symbol ?? "ETH";
        }
    }
}