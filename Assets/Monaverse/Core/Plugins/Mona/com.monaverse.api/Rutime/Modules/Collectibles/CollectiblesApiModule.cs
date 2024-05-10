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
        private readonly IMonaApiOptions _monaApiOptions;
        private readonly IMonaHttpClient _monaHttpClient;

        public CollectiblesApiModule(IMonaApiOptions monaApiOptions,
            IMonaApiLogger monaApiLogger,
            IMonaHttpClient monaHttpClient)
        {
            _monaApiOptions = monaApiOptions;
            _monaHttpClient = monaHttpClient;
        }
        
        public async Task<ApiResult<GetWalletCollectiblesResponse>> GetWalletCollectibles()
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiOptions.GetUrlWithPath(Constants.Endpoints.GetWalletCollectibles),
                method: RequestMethod.Get);
            
            var response = await _monaHttpClient.SendAsync(monaHttpRequest);
            return response.ConvertTo<GetWalletCollectiblesResponse>();
        }
        
        public async Task<ApiResult<GetWalletCollectibleResponse>> GetWalletCollectibleById(string id)
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiOptions.GetUrlWithPath(Constants.Endpoints.GetWalletCollectibleById(id)),
                method: RequestMethod.Get);
            
            var response = await _monaHttpClient.SendAsync(monaHttpRequest);
            return response.ConvertTo<GetWalletCollectibleResponse>();
        }
    }
}