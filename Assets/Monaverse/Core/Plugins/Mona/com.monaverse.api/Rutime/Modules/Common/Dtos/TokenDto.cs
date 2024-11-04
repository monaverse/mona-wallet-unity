using System.Collections.Generic;
using System.Numerics;
using Monaverse.Api.Modules.Common.Dtos;

namespace Monaverse.Api.Modules.User.Dtos
{
    public class TokenDto
    {
        public BigInteger ChainId { get; set; }
        public string Contract { get; set; }
        public string TokenId { get; set; }
        public string Kind { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string ImageSmall { get; set; }
        public string ImageLarge { get; set; }
        public string Description { get; set; }
        public string Media { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
        public float RarityScore { get; set; }
        public int RarityRank { get; set; }
        public int Supply { get; set; }
        public CollectionDto Collection { get; set; } = new();
        public List<TokenAttributeDto> Attributes { get; set; } = new();
        public List<TokenFileDto> Files { get; set; } = new();
        public OrderDto FloorAsk { get; set; }
        public OrderDto TopBid { get; set; }
        public bool IsCommunityToken { get; set; }
    }
}