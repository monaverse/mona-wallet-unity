using System.Threading.Tasks;
using Monaverse.Api.Modules.Common;
using Monaverse.Api.MonaHttpClient.Extensions;
using Monaverse.Api.MonaHttpClient.Request;

namespace Monaverse.Api.Modules.Leaderboard
{
    public sealed class LeaderboardApiModule : ILeaderboardApiModule
    {
        private readonly IMonaApiClient _monaApiClient;

        public LeaderboardApiModule(IMonaApiClient monaApiClient)
        {
            _monaApiClient = monaApiClient;
        }

        public async Task<ApiResult> PostScore(PostScoreRequest request)
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.Leaderboard.PostScore),
                method: RequestMethod.Post)
                .WithBody(request);
            
            var response = await _monaApiClient.SendAuthenticated(monaHttpRequest);
            return response.ToApiResult();
        }
    }
}