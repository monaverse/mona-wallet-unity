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

        public static (decimal Price, string Currency) GetNativePrice(this TokenDto tokenDto, int precision = 6)
        {
            if (tokenDto == null)
                return (0m, "ETH");

            if (tokenDto.TopBid == null && tokenDto.FloorAsk == null)
                return (0m, "ETH");

            var floorAskPrice = decimal.Round(tokenDto.FloorAsk?.Price?.Amount?.Native ?? 0m, precision);
            var floorCurrency = tokenDto.FloorAsk?.Price?.Currency?.Symbol ?? "ETH";
            var topBidPrice = decimal.Round(tokenDto.TopBid?.Price?.Amount?.Native ?? 0m, precision);
            var topBidCurrency = tokenDto.TopBid?.Price?.Currency?.Symbol ?? "ETH";

            return topBidPrice > floorAskPrice ? (topBidPrice, topBidCurrency) : (floorAskPrice, floorCurrency);
        }

        public static string GetCurrency(this TokenDto tokenDto)
        {
            return tokenDto?.FloorAsk?.Price?.Currency?.Symbol ?? "ETH";
        }
    }
}