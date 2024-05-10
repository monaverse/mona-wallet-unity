using System.Threading.Tasks;
using Monaverse.Api.Modules.Auth.Responses;
using Monaverse.Api.Modules.Common;

namespace Monaverse.Api.Modules.Auth
{
    public interface IAuthApiModule
    {
        Task<ApiResult<PostNonceResponse>> PostNonce(string walletAddress);
        Task<ApiResult<ValidateWalletResponse>> ValidateWallet(string walletAddress);
        Task<ApiResult> Authorize(string signature, string siweMessage);
    }
}