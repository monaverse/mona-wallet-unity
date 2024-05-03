using System.Threading.Tasks;
using Monaverse.Api.Extensions;
using Monaverse.Api.Modules.Collectibles.Responses;
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
            IMonaHttpClient monaHttpClient)
        {
            _monaApiOptions = monaApiOptions;
            _monaHttpClient = monaHttpClient;
        }
        
        public async Task<GetWalletCollectiblesResponse> GetWalletCollectibles()
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiOptions.GetUrlWithPath(Constants.Endpoints.GetWalletCollectibles),
                method: RequestMethod.Get
            ).WithBearerToken(_monaHttpClient.AccessToken);
            
            var response = await _monaHttpClient.SendAsync(monaHttpRequest);
            return response.ConvertTo<GetWalletCollectiblesResponse>();
        }
        
        public async Task<GetWalletCollectibleResponse> GetWalletCollectibleById(string id)
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiOptions.GetUrlWithPath(Constants.Endpoints.GetWalletCollectibleById(id)),
                method: RequestMethod.Get
            ).WithBearerToken(_monaHttpClient.AccessToken);
            
            var response = await _monaHttpClient.SendAsync(monaHttpRequest);
            return response.ConvertTo<GetWalletCollectibleResponse>();
        }
    }
}