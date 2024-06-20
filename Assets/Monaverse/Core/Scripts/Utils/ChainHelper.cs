using System.Collections.Generic;

namespace Monaverse.Core.Utils
{
    public static class ChainHelper
    {
        public const string Ethereum = "Ethereum";
        public const string Polygon = "Polygon";
        public const string Arbitrum = "Arbitrum";
        public const string Optimism = "Optimism";
        public const string Base = "Base";
        
        private static readonly HashSet<int> Chains = new()
        {
            1, 137, 42161, 10, 8453
        };
        
        public static bool IsChainSupported(int chainId)
        {
            return Chains.Contains(chainId);
        }

        public static HashSet<int> SupportedChains()
            => Chains;
        
        public static string GetChainName(int chainId)
        {
            if(!Chains.TryGetValue(chainId, out var supportedChainId))
                return "Unknown";

            return supportedChainId switch
            {
                1 => Ethereum,
                137 => Polygon,
                42161 => Arbitrum,
                10 => Optimism,
                8453 => Base,
                _ => "Unknown"
            };
        }
        
        public static int GetChainId(string chainName)
        {
            return chainName switch
            {
                Ethereum => 1,
                Polygon => 137,
                Arbitrum => 42161,
                Optimism => 10,
                Base => 8453,
                _ => 0
            };
        }
    }
}