using System.Threading.Tasks;
using Monaverse.Api.Modules.Auth.Responses;

namespace Monaverse.Api.Modules.Auth
{
    public interface IAuthApiModule
    {
        Task<PostNonceResponse> PostNonce(string walletAddress);
        Task<ValidateWalletAddressResponse> ValidateWalletAddress(string walletAddress);
        Task<bool> Authorize(string signature, string siweMessage);
    }
}