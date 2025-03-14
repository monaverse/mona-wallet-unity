using System.Numerics;

namespace Monaverse.Api
{
    internal static class Constants
    {
        public const string ApiDomainLocal = "http://localhost:8000";
        public const string ApiDomainStaging = " https://api-staging.monaverse.com";
        public const string ApiDomainProduction = "https://api.monaverse.com";
        
        public const string ApplicationIdHeader = "X-Mona-Application-Id";
        public const string LegacyAccessTokenStorageKey = "mona-access-token" ;
        public const string AccessKey = "mona-access" ;
        public const string RefreshKey = "mona-refresh" ;
        
        public static class Endpoints
        {
            public static class Auth
            {
                public const string SignUp = "public/auth/signup";
                public const string GenerateOtp = "public/auth/otp/generate";
                public const string VerifyOtp = "public/auth/otp/verify";
                public const string RefreshToken = "public/auth/token/refresh";
            }
            
            public static class User
            {
                public const string GetUser = "public/user";
                public static string GetUserTokens(int chainId, string address) => $"public/user/{chainId}/{address}/tokens";
                public const string DeleteAccount = "public/user/account/delete";
            }
            
            public static class Token
            {
                public static string GetCommunityTokens(int chainId) => $"public/tokens/{chainId}/community";
                public static string GetTokenAnimation(BigInteger chainId, string contract, string tokenId) 
                    => $"public/tokens/{chainId}/{contract}/{tokenId}/animation";
            }
            
            public static class Leaderboard
            { 
                public const string PostScore = "public/leaderboards/sdk/score";
                public static string GetTopScores(string applicationId) 
                    => $"public/leaderboards/{applicationId}/top-scores";
                
                public const string GetUserRank = "public/leaderboards/rank";
                public const string GetAroundMeScores = "public/leaderboards/around-me";
            }

            public static class Ai
            {
                public static string GetGenerationRequest(string requestId)
                    => $"public/ai/requests/{requestId}";
                public static string GetAsset(string assetId)
                    => $"public/ai/assets/{assetId}";
                public const string GetGenerationRequests = "public/ai/requests";
                public const string GetAssets = "public/ai/assets";
                public const string CreateTextToImageRequest = "public/ai/text-to-image";
                public const string CreateImageTo3dRequest = "public/ai/image-to-3d";
                public const string GetQuota = "public/ai/quota";
            }
        }
    }
}