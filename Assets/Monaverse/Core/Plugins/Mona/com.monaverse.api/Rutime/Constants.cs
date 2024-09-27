using System.Numerics;

namespace Monaverse.Api
{
    internal static class Constants
    {
        public const string MonaDomain = "monaverse.com";
        
        
        public const string BaseUrlLocalLegacy = "http://localhost:3007";
        public const string BaseUrlDevelopmentLegacy = "https://api-wallet-sdk-dev.helios.monaver.se";
        public const string BaseUrlStagingLegacy = "https://api-wallet-sdk-staging.helios.monaver.se";
        public const string BaseUrlProductionLegacy = "https://api-wallet-sdk.helios.monaverse.com";
        
        public const string ApiDomainLocal = "http://localhost:8000";
        public const string ApiDomainStaging = " https://api-mp-staging.monaverse.com";
        public const string ApiDomainProduction = "https://api.monaverse.com";
        
        public const string ApplicationIdHeader = "X-Mona-Application-Id";
        public const string LegacyAccessTokenStorageKey = "mona-access-token" ;
        public const string AccessKey = "mona-access" ;
        public const string RefreshKey = "mona-refresh" ;
        
        public static class Endpoints
        {
            public const string PostNonce = "v1/wallet-sdk/auth/nonce";
            public const string PostAuthorize = "v1/wallet-sdk/auth/authorize";
            public const string GetWalletCollectibles = "v1/wallet-sdk/wallet/collectibles";
            public static string GetWalletCollectibleById(string id) => $"v1/wallet-sdk/wallet/collectibles/{id}";
            
            public static class Auth
            {
                public const string GenerateOtp = "public/auth/otp/generate";
                public const string VerifyOtp = "public/auth/otp/verify";
                public const string RefreshToken = "public/auth/token/refresh";
            }
            
            public static class User
            {
                public const string GetUser = "public/user";
                public static string GetUserTokens(int chainId, string address) => $"public/user/{chainId}/{address}/tokens";
            }
            
            public static class Token
            {
                public static string GetTokenAnimation(BigInteger chainId, string contract, string tokenId) 
                    => $"public/tokens/{chainId}/{contract}/{tokenId}/animation";
            }
            
            public static class Leaderboard
            { 
                public const string PostScore = "public/leaderboards/sdk/score";
                public static string GetTopScores(string applicationId) 
                    => $"public/leaderboards/{applicationId}/top-scores";
                
                public const string GetUserRank = "public/leaderboards/rank";
            }
        }
    }
}