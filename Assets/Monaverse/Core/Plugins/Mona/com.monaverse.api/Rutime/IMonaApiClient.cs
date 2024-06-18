using System.Threading.Tasks;
using Monaverse.Api.Modules.Auth;
using Monaverse.Api.Modules.Collectibles;
using Monaverse.Api.Modules.User;
using Monaverse.Api.MonaHttpClient.Request;
using Monaverse.Api.MonaHttpClient.Response;
using Monaverse.Api.Session;

namespace Monaverse.Api
{
    public interface IMonaApiClient
    {
        IAuthApiModule Auth { get; }
        ICollectiblesApiModule Collectibles { get; }
        IUserApiModule User { get; }
        IMonaApiSession Session { get; }
        bool IsAuthorized();
        
        string GetUrlWithPath(string path);
        string GetUrlWithPathLegacy(string path);

        Task<IMonaHttpResponse> Send(IMonaHttpRequest request);
        Task<IMonaHttpResponse> SendLegacy(IMonaHttpRequest request);
        Task<IMonaHttpResponse> SendAuthenticated(IMonaHttpRequest request);
    }
}