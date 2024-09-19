namespace Monaverse.Api.Modules.Leaderboard
{
    public record PostScoreRequest
    {
        public float Score { get; set; }
        public string Topic { get; set; }
        public long Timestamp { get; set; }
        public string Signature { get; set; }
    }
}