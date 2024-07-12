using System.Numerics;
using System.Threading.Tasks;
using Monaverse.Api.Modules.Common;
using Monaverse.Api.Modules.Token.Responses;

namespace Monaverse.Api.Modules.Token
{
    public interface ITokenApiModule
    {
        Task<ApiResult<GetTokenAnimationResponse>> GetTokenAnimation(BigInteger chainId, string contract, string tokenId);
    }
}