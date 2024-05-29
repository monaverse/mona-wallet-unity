using System.Collections.Generic;
using Monaverse.Api.Modules.Collectibles.Dtos;

namespace Monaverse.Modal.UI.Extensions
{
    public static class CollectibleExtensions
    {
        public static bool CanBeImported(this CollectibleDto collectible)
            =>  MonaverseModal.Instance.CollectibleFilter == null || MonaverseModal.Instance.CollectibleFilter(collectible);
        
        public static List<CollectibleDto> GetFilteredCollectibles(this List<CollectibleDto> collectibles)
            => MonaverseModal.Instance.CollectibleFilter == null
                ? collectibles
                : collectibles.FindAll(i=> MonaverseModal.Instance.CollectibleFilter(i));
    }
}