namespace Monaverse.Api
{
    internal static class Constants
    {
        public const string MonaDomain = "monaverse.com";
        public const string BaseUrlLocal = "http://localhost:3007";
        public const string BaseUrlDevelopment = "https://api-wallet-sdk-dev.helios.monaver.se";
        public const string BaseUrlStaging = "https://api-wallet-sdk-staging.helios.monaver.se";
        public const string BaseUrlProduction = "https://api-wallet-sdk.helios.monaverse.com";
        public const string ApplicationIdHeader = "X-Mona-Application-Id";
        public const string AccessTokenStorageKey = "mona-access-token" ;
        
        public static class Endpoints
        {
            public const string PostNonce = "v1/wallet-sdk/auth/nonce";
            public const string PostAuthorize = "v1/wallet-sdk/auth/authorize";
            public const string GetWalletCollectibles = "v1/wallet-sdk/wallet/collectibles";
            public static string GetWalletCollectibleById(string id) => $"v1/wallet-sdk/wallet/collectibles/{id}";
        }
    }
}