using Monaverse.Api.Modules.Collectibles.Dtos;

namespace Monaverse.Core.Utils
{
    public static class CollectibleExtensions
    {
        public static string GetImageUrl(this CollectibleDto collectibleDto, int width = 400)
            => collectibleDto.Image.ResolveTokenUrl(width);
    }
}