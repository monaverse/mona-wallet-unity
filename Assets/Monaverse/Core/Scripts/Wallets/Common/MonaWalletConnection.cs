using System.Numerics;

namespace Monaverse.Core.Scripts.Wallets.Common
{
    public sealed record MonaWalletConnection
    {
        public MonaWalletProvider MonaWalletProvider { get; set; }
        public BigInteger ChainId { get; set; }
    }
}