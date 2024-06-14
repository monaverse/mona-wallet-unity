using System.Collections.Generic;
using System.Threading.Tasks;
using Monaverse.Api.Modules.Common;
using Monaverse.Api.Modules.User.Responses;

namespace Monaverse.Api.Modules.User
{
    public interface IUserApiModule
    {
        Task<ApiResult<GetUserResponse>> GetUser();

        Task<ApiResult<GetUserTokensResponse>> GetUserTokens(int chainId,
            string address,
            IEnumerable<KeyValuePair<string, object>> queryParams = null,
            string continuation = null);
    }
}