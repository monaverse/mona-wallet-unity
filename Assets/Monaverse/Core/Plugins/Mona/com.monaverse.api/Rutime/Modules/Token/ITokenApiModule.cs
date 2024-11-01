using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Monaverse.Api.Modules.Common;
using Monaverse.Api.Modules.Token.Responses;

namespace Monaverse.Api.Modules.Token
{
    public interface ITokenApiModule
    {
        Task<ApiResult<GetTokenAnimationResponse>> GetTokenAnimation(BigInteger chainId, string contract, string tokenId);

        Task<ApiResult<GetCommunityTokensResponse>> GetCommunityTokens(int chainId,
            IEnumerable<KeyValuePair<string, object>> queryParams = null,
            string continuation = null);
    }
}