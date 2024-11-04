using System.Collections.Generic;
using System.Threading.Tasks;
using Monaverse.Api.Modules.Common;
using Monaverse.Api.Modules.Common.Dtos;
using Monaverse.Api.Modules.User.Responses;

namespace Monaverse.Api.Modules.User
{
    public interface IUserApiModule
    {
        Task<ApiResult<GetUserResponse>> GetUser();

        Task<ApiResult<GetUserTokensResponse>> GetUserTokens(int chainId,
            string address,
            TokenFiltersDto queryParams = null,
            string continuation = null);

        Task<ApiResult> DeleteAccount();
        
        
    }
}