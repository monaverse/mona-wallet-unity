using System;
using Newtonsoft.Json;

namespace Monaverse.Api.Modules.Leaderboard.Responses
{
    public sealed record GetUserRankResponse
    {
        public int Rank { get; set; }
        public float Score { get; set; }
        public string Topic { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}