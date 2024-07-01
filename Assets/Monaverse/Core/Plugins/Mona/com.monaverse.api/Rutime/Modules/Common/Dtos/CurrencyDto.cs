using System.Numerics;

namespace Monaverse.Api.Modules.Common.Dtos
{
    public sealed record CurrencyDto
    {
        public string Contract { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public int Decimals { get; set; }
        public BigInteger ChainId { get; set; }
    }
}