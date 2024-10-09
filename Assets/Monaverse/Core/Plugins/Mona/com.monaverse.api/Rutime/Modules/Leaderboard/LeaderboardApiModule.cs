using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Monaverse.Api.Modules.Common;
using Monaverse.Api.Modules.Leaderboard.Enums;
using Monaverse.Api.Modules.Leaderboard.Responses;
using Monaverse.Api.Modules.Leaderboard.Responses.Common;
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

        public async Task<ApiResult<GetTopScoresResponse>> GetTopScores(
            int limit = 20,
            int offset = 0,
            bool featured = false,
            string topic = null,
            LeaderboardSortingOrder sortingOrder = LeaderboardSortingOrder.Highest,
            LeaderboardPeriod period = LeaderboardPeriod.AllTime,
            DateTime? startTime = null,
            DateTime? endTime = null,
            bool includeAllUserScores = false
        )
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.Leaderboard.GetTopScores(_monaApiClient.ApplicationId)),
                method: RequestMethod.Get);

            monaHttpRequest
                .WithQueryParam(nameof(limit), limit)
                .WithQueryParam(nameof(offset), offset)
                .WithQueryParam(nameof(featured), featured)
                .WithQueryParam(nameof(topic), topic)
                .WithQueryParam("sorting_order", sortingOrder.ConvertToString())
                .WithQueryParam(nameof(period), period.ConvertToString())
                .WithQueryParam("start_time", startTime?.ToString("O", CultureInfo.InvariantCulture))
                .WithQueryParam("end_time", endTime?.ToString("O", CultureInfo.InvariantCulture))
                .WithQueryParam("include_all_user_scores", includeAllUserScores);

            var response = await _monaApiClient.Send(monaHttpRequest);
            return response.ConvertTo<GetTopScoresResponse>();
        }

        public async Task<ApiResult<GetUserRankResponse>> GetUserRank(
            bool featured = false,
            string topic = null,
            LeaderboardSortingOrder sortingOrder = LeaderboardSortingOrder.Highest,
            LeaderboardPeriod period = LeaderboardPeriod.AllTime,
            DateTime? startTime = null,
            DateTime? endTime = null,
            bool includeAllUserScores = false
        )
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.Leaderboard.GetUserRank),
                method: RequestMethod.Get);

            monaHttpRequest
                .WithQueryParam(nameof(featured), featured)
                .WithQueryParam(nameof(topic), topic)
                .WithQueryParam("sorting_order", sortingOrder.ConvertToString())
                .WithQueryParam(nameof(period), period.ConvertToString())
                .WithQueryParam("start_time", startTime?.ToString("O", CultureInfo.InvariantCulture))
                .WithQueryParam("end_time", endTime?.ToString("O", CultureInfo.InvariantCulture))
                .WithQueryParam("include_all_user_scores", includeAllUserScores);

            var response = await _monaApiClient.SendAuthenticated(monaHttpRequest);
            return response.ConvertTo<GetUserRankResponse>();
        }

        public async Task<ApiResult<List<TopScore>>> GetAroundMeScores(
            bool featured = false,
            string topic = null,
            LeaderboardSortingOrder sortingOrder = LeaderboardSortingOrder.Highest,
            LeaderboardPeriod period = LeaderboardPeriod.AllTime,
            DateTime? startTime = null,
            DateTime? endTime = null,
            bool includeAllUserScores = false,
            int limit = 10)
        {
            var monaHttpRequest = new MonaHttpRequest(
                url: _monaApiClient.GetUrlWithPath(Constants.Endpoints.Leaderboard.GetAroundMeScores),
                method: RequestMethod.Get);

            monaHttpRequest
                .WithQueryParam(nameof(featured), featured)
                .WithQueryParam(nameof(topic), topic)
                .WithQueryParam("sorting_order", sortingOrder.ConvertToString())
                .WithQueryParam(nameof(period), period.ConvertToString())
                .WithQueryParam("start_time", startTime?.ToString("O", CultureInfo.InvariantCulture))
                .WithQueryParam("end_time", endTime?.ToString("O", CultureInfo.InvariantCulture))
                .WithQueryParam("include_all_user_scores", includeAllUserScores)
                .WithQueryParam(nameof(limit), limit);
            
            var response = await _monaApiClient.SendAuthenticated(monaHttpRequest);
            return response.ConvertTo<List<TopScore>>();
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