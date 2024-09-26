using Monaverse.Api.Modules.Leaderboard.Enums;

namespace Monaverse.Api.Modules.Leaderboard
{
    public static class LeaderboardExtensions
    {
        public static string ConvertToString(this LeaderboardPeriod period)
        {
            return period switch
            {
                LeaderboardPeriod.Daily => "daily",
                LeaderboardPeriod.Weekly => "weekly",
                LeaderboardPeriod.Monthly => "monthly",
                LeaderboardPeriod.AllTime => "all_time",
                _ => "all_time"
            };
        }
        
        public static string ConvertToString(this LeaderboardSortingOrder order)
        {
            return order switch
            {
                LeaderboardSortingOrder.Highest => "highest",
                LeaderboardSortingOrder.Lowest => "lowest",
                _ => "highest"
            };
        }
    }
}