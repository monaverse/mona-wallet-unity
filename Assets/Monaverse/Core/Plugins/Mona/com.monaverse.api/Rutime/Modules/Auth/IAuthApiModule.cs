using System.Threading.Tasks;
using Monaverse.Api.Modules.Auth.Requests;
using Monaverse.Api.Modules.Auth.Responses;

namespace Monaverse.Api.Modules.Auth
{
    public interface IAuthApiModule
    {
        Task<PostNonceResponse> PostNonce(PostNonceRequest request);
        Task<AuthorizeResponse> Authorize(AuthorizeRequest request);
    }
}