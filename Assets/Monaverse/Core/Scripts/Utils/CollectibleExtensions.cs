using Monaverse.Api.Modules.Collectibles.Dtos;

namespace Monaverse.Core.Utils
{
    public static class CollectibleExtensions
    {
        public static string GetImageUrl(this CollectibleDto collectibleDto, int width = 400, string placeholderUrl = null)
        {
            var tokenImage = collectibleDto.Image;
            
            if (string.IsNullOrEmpty(tokenImage))
            {
                MonaDebug.LogError($"{collectibleDto.Title} image is null or empty. Using placeholder");
                tokenImage = placeholderUrl ?? MonaConstants.Media.MonaPlaceholderLogoWhite;
            }

            //if token does not have a schema we assume is an Ipfs CID 
            if (!tokenImage.Contains("://"))
                tokenImage = tokenImage.ToIpfsGatewayUrl();
            
            return tokenImage.ToCloudinaryImageUrl(width);
        }

        public static string GetMarketplaceUrl(this CollectibleDto collectibleDto)
        {
            if (!string.IsNullOrEmpty(collectibleDto.Slug)) 
                return $"{MonaConstants.MonaversePages.ArtifactDetailsBaseUrl}/{collectibleDto.Slug}/details";
            
            MonaDebug.LogError($"Collectible {collectibleDto.Id} slug is null or empty");    
            return MonaConstants.MonaversePages.Monaverse;

        }
    }
}