using System.Threading.Tasks;
using Monaverse.Api.Modules.Collectibles.Responses;

namespace Monaverse.Api.Modules.Collectibles
{
    public interface ICollectiblesApiModule
    {
        Task<GetWalletCollectiblesResponse> GetWalletCollectibles();
        Task<GetWalletCollectibleResponse> GetWalletCollectibleById(string id);
    }
}