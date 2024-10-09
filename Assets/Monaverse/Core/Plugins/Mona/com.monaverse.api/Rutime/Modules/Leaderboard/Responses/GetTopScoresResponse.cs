using System.Collections.Generic;
using Monaverse.Api.Modules.Leaderboard.Responses.Common;

namespace Monaverse.Api.Modules.Leaderboard.Responses
{
    public sealed record GetTopScoresResponse
    {
        public List<TopScore> Items { get; set; }
        public int Count { get; set; }
    }
}