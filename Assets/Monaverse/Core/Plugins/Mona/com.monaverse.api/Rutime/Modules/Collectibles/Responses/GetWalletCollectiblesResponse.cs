using System.Collections.Generic;
using Monaverse.Api.Modules.Collectibles.Dtos;

namespace Monaverse.Api.Modules.Collectibles.Responses
{
    public sealed record GetWalletCollectiblesResponse
    {
        public List<CollectibleDto> Data { get; set; }
        public int TotalCount { get; set; }
        public string NextPageKey { get; set; }
    }
}