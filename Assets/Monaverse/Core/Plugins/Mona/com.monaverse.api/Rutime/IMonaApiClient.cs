using System.Threading.Tasks;
using Monaverse.Api.Modules.Auth;
using Monaverse.Api.Modules.Leaderboard;
using Monaverse.Api.Modules.Token;
using Monaverse.Api.Modules.User;
using Monaverse.Api.MonaHttpClient.Request;
using Monaverse.Api.MonaHttpClient.Response;
using Monaverse.Api.Session;

namespace Monaverse.Api
{
    public interface IMonaApiClient
    {
        IAuthApiModule Auth { get; }
        IUserApiModule User { get; }
        ITokenApiModule Token { get; }
        IMonaApiSession Session { get; }
        ILeaderboardApiModule Leaderboard { get; }
        string ApplicationId { get; }
        bool IsAuthorized();
        
        string GetUrlWithPath(string path);
        Task<IMonaHttpResponse> Send(IMonaHttpRequest request);
        Task<IMonaHttpResponse> SendAuthenticated(IMonaHttpRequest request);
    }
}