using System.Numerics;

namespace Monaverse.Core
{
    public sealed record MonaWalletConnection
    {
        public MonaWalletProvider MonaWalletProvider { get; set; }
        public BigInteger ChainId { get; set; }
    }
}