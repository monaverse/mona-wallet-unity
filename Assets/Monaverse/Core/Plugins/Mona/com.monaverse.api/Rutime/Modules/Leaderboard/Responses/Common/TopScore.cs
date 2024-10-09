using System;
using Newtonsoft.Json;

namespace Monaverse.Api.Modules.Leaderboard.Responses.Common
{
    public record TopScore
    {
        public int Id { get; set; }
        public TopScoreUser User { get; set; }
        public float Score { get; set; }
        public string Topic { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        public int Rank { get; set; }
        
        public sealed record TopScoreUser
        {
            public string Username { get; set; }
            public string Name { get; set; }
        }
    }
}