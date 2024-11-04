using System.Collections.Generic;
using System.Threading.Tasks;
using Monaverse.Api.Extensions;
using Monaverse.Api.Modules.Common;
using Monaverse.Api.Modules.Common.Dtos;
using Monaverse.Api.Modules.User.Responses;
using Monaverse.Api.MonaHttpClient.Extensions;
using Monaverse.Api.MonaHttpClient.Request;

namespace Monaverse.Api.Modules.User
{
    internal class UserApiModule : IUserApiModule
    {
        private readonly IMonaApiClient _monaApiClient;

        public UserApiModule(IMonaApiClient monaApiClient)
        {
            _monaApiClient = monaApiClient;
        }

        public async Task<ApiResult<GetUserResponse>> GetUser()
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.User.GetUser),
                method: RequestMethod.Get);

            var response = await _monaApiClient.SendAuthenticated(monaHttpRequest);
            return response.ConvertTo<GetUserResponse>();
        }

        public async Task<ApiResult<GetUserTokensResponse>> GetUserTokens(int chainId,
            string address,
            TokenFiltersDto queryParams = null,
            string continuation = null)
        {
            queryParams ??= new TokenFiltersDto();

            var monaHttpRequest = new MonaHttpRequest(
                    url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.User.GetUserTokens(chainId, address)),
                    method: RequestMethod.Get)
                .WithQueryParams(queryParams.ToPropertyDictionary())
                .WithQueryParam("continuation", continuation);

            var response = await _monaApiClient.SendAuthenticated(monaHttpRequest);
            return response.ConvertTo<GetUserTokensResponse>();
        }

       
        
        public async Task<ApiResult> DeleteAccount()
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.User.DeleteAccount),
                method: RequestMethod.Post);

            var response = await _monaApiClient.SendAuthenticated(monaHttpRequest);
            return response.ToApiResult();
        }
    }
}