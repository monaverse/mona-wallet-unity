using System.Threading.Tasks;
using Monaverse.Api.Modules.Common;
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
    }
}