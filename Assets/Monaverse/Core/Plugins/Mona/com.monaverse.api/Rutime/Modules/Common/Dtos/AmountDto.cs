namespace Monaverse.Api.Modules.Common.Dtos
{
    public sealed record AmountDto
    {
        public string Raw { get; set; }
        public decimal Decimal { get; set; }
        public decimal Usd { get; set; }
        public decimal Native { get; set; }
    }
}