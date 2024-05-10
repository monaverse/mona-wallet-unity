using System.Threading.Tasks;
using Monaverse.Api.Modules.Collectibles.Responses;
using Monaverse.Api.Modules.Common;

namespace Monaverse.Api.Modules.Collectibles
{
    public interface ICollectiblesApiModule
    {
        Task<ApiResult<GetWalletCollectiblesResponse>> GetWalletCollectibles();
        Task<ApiResult<GetWalletCollectibleResponse>> GetWalletCollectibleById(string id);
    }
}