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
        public const string ApiDomainStaging = "https://api-staging.monaverse.com";
        public const string ApiDomainProduction = "https://api.monaverse.com";
        
        public const string ApplicationIdHeader = "X-Mona-Application-Id";
        public const string AccessTokenStorageKey = "mona-access-token" ;
        
        public static class Endpoints
        {
            public const string PostNonce = "v1/wallet-sdk/auth/nonce";
            public const string PostAuthorize = "v1/wallet-sdk/auth/authorize";
            public const string GetWalletCollectibles = "v1/wallet-sdk/wallet/collectibles";
            public static string GetWalletCollectibleById(string id) => $"v1/wallet-sdk/wallet/collectibles/{id}";
            
            public const string GenerateOtp = "public/auth/otp/generate";
            public const string VerifyOtp = "public/auth/otp/verify";
            public const string RefreshToken = "public/auth/token/refresh";
        }
    }
}