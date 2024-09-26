using System;
using System.Threading.Tasks;
using Monaverse.Api.Modules.Common;
using Monaverse.Api.Modules.Leaderboard.Enums;
using Monaverse.Api.Modules.Leaderboard.Responses;

namespace Monaverse.Api.Modules.Leaderboard
{
    public interface ILeaderboardApiModule
    {
        Task<ApiResult<GetTopScoresResponse>> GetTopScores(
            int limit = 20,
            int offset = 0,
            bool featured = false,
            string topic = null,
            LeaderboardSortingOrder sortingOrder = LeaderboardSortingOrder.Highest,
            LeaderboardPeriod period = LeaderboardPeriod.AllTime,
            DateTime? startTime = null,
            DateTime? endTime = null,
            bool includeAllUserScores = false
        );
        
        Task<ApiResult> PostScore(PostScoreRequest request);
    }
}