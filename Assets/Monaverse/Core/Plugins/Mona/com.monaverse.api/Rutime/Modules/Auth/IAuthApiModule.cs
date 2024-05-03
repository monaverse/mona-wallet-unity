using System.Threading.Tasks;
using Monaverse.Api.Modules.Auth.Responses;

namespace Monaverse.Api.Modules.Auth
{
    public interface IAuthApiModule
    {
        Task<PostNonceResponse> PostNonce(string walletAddress);
        Task<AuthorizeResponse> Authorize(string signature, string siweMessage);
    }
}