using Monaverse.Api.Modules.Collectibles.Dtos;

namespace Monaverse.Core.Utils
{
    public static class CollectibleExtensions
    {
        public static string GetImageUrl(this CollectibleDto collectibleDto, int width = 400)
            => collectibleDto.Image.ResolveTokenUrl(width);

        public static string GetMarketplaceUrl(this CollectibleDto collectibleDto)
        {
            if (!string.IsNullOrEmpty(collectibleDto.Slug)) 
                return $"{MonaConstants.MonaversePages.ArtifactDetailsBaseUrl}/{collectibleDto.Slug}/details";
            
            MonaDebug.LogError($"Collectible {collectibleDto.Id} slug is null or empty");    
            return MonaConstants.MonaversePages.Monaverse;

        }
    }
}