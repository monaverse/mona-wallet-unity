using System.Threading.Tasks;
using Monaverse.Api.Modules.Common;

namespace Monaverse.Api.Modules.Leaderboard
{
    public interface ILeaderboardApiModule
    {
        Task<ApiResult> PostScore(PostScoreRequest request);
    }
}