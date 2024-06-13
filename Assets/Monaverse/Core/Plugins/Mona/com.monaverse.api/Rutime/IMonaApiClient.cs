using System.Threading.Tasks;
using Monaverse.Api.Modules.Auth;
using Monaverse.Api.Modules.Collectibles;
using Monaverse.Api.Modules.User;
using Monaverse.Api.MonaHttpClient.Request;
using Monaverse.Api.MonaHttpClient.Response;

namespace Monaverse.Api
{
    public interface IMonaApiClient
    {
        IAuthApiModule Auth { get; }
        ICollectiblesApiModule Collectibles { get; }
        IUserApiModule User { get; }
        bool IsAuthorized();
        void ClearSession();
        string GetLegacyAccessToken();
        
        void SaveLegacySession(string accessToken);
        void SaveSession(string accessToken, string refreshToken);
        string GetUrlWithPath(string path);
        string GetUrlWithPathLegacy(string path);

        Task<IMonaHttpResponse> Send(IMonaHttpRequest request);
        Task<IMonaHttpResponse> SendLegacy(IMonaHttpRequest request);
        Task<IMonaHttpResponse> SendAuthenticated(IMonaHttpRequest request);
    }
}