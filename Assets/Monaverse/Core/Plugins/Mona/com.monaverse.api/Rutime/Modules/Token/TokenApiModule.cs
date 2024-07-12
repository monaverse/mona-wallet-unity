using System.Numerics;
using System.Threading.Tasks;
using Monaverse.Api.Modules.Common;
using Monaverse.Api.Modules.Token.Responses;
using Monaverse.Api.MonaHttpClient.Extensions;
using Monaverse.Api.MonaHttpClient.Request;

namespace Monaverse.Api.Modules.Token
{
    public sealed class TokenApiModule : ITokenApiModule
    {
        private readonly IMonaApiClient _monaApiClient;

        public TokenApiModule(IMonaApiClient monaApiClient)
        {
            _monaApiClient = monaApiClient;
        }

        public async Task<ApiResult<GetTokenAnimationResponse>> GetTokenAnimation(BigInteger chainId, string contract, string tokenId)
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiClient.GetUrlWithPath(
                    Constants.Endpoints.Token.GetTokenAnimation(chainId, contract, tokenId)),
                method: RequestMethod.Get);
            
            var response = await _monaApiClient.SendAuthenticated(monaHttpRequest);
            return response.ConvertTo<GetTokenAnimationResponse>();
        }
    }
}