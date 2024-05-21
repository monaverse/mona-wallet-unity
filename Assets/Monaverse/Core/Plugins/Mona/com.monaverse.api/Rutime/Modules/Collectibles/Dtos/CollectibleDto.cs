using System.Collections.Generic;

namespace Monaverse.Api.Modules.Collectibles.Dtos
{
    public record CollectibleDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public bool Minted { get; set; }
        public bool Nsfw { get; set; }
        public bool Promoted { get; set; }
        public int ActiveVersion { get; set; }
        public string Artist { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public string Slug { get; set; }
        public bool Checked { get; set; }
        public string CollectionId { get; set; }
        public bool Hidden { get; set; }
        public string Image { get; set; }
        public string LastSaleEventId { get; set; }
        public float LastSalePrice { get; set; }
        public string Owner { get; set; }
        public string ParentId { get; set; }
        public float Price { get; set; }
        public List<string> Properties { get; set; }
        public string Title { get; set; }
        public CollectibleNft Nft { get; set; }
        public List<CollectibleVersion> Versions { get; set; }
        public Dictionary<string,string> Traits { get; set; }
        
        public sealed record CollectibleNft
        {
            public string Contract { get; set; }
            public string IpfsUrl { get; set; }
            public string Network { get; set; }
            public string TokenHash { get; set; }
            public int TokenId { get; set; }
            public string TokenUri { get; set; }
            public string TransactionId { get; set; }
        }

        public sealed record CollectibleVersion
        {
            public CollectibleVersionAssets Assets { get; set; }
            public string Asset { get; set; }
        }
        
        public sealed record CollectibleVersionAssets
        {
            public string Artifacts { get; set; }
            public string Portals { get; set; }
            public string Space { get; set; }
        }
    }
}