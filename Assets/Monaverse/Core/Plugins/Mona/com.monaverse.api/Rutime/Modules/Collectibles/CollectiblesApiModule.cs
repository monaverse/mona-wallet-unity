using System.Threading.Tasks;
using Monaverse.Api.Extensions;
using Monaverse.Api.Logging;
using Monaverse.Api.Modules.Collectibles.Responses;
using Monaverse.Api.Modules.Common;
using Monaverse.Api.MonaHttpClient;
using Monaverse.Api.MonaHttpClient.Extensions;
using Monaverse.Api.MonaHttpClient.Request;
using Monaverse.Api.Options;

namespace Monaverse.Api.Modules.Collectibles
{
    internal sealed class CollectiblesApiModule : ICollectiblesApiModule
    {
        private readonly IMonaApiClient _monaApiClient;

        public CollectiblesApiModule(IMonaApiClient monaApiClient)
        {
            _monaApiClient = monaApiClient;
        }
        
        public async Task<ApiResult<GetWalletCollectiblesResponse>> GetWalletCollectibles()
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiClient.GetUrlWithPathLegacy(Constants.Endpoints.GetWalletCollectibles),
                method: RequestMethod.Get);
            
            var response = await _monaApiClient.SendLegacy(monaHttpRequest);
            return response.ConvertTo<GetWalletCollectiblesResponse>();
        }
        
        public async Task<ApiResult<GetWalletCollectibleResponse>> GetWalletCollectibleById(string id)
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiClient.GetUrlWithPathLegacy(Constants.Endpoints.GetWalletCollectibleById(id)),
                method: RequestMethod.Get);
            
            var response = await _monaApiClient.SendLegacy(monaHttpRequest);
            return response.ConvertTo<GetWalletCollectibleResponse>();
        }
    }
}