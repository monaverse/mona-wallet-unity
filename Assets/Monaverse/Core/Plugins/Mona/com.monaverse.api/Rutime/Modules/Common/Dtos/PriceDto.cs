namespace Monaverse.Api.Modules.Common.Dtos
{
    public record PriceDto
    {
        public AmountDto Amount { get; set; }
        public CurrencyDto Currency { get; set; }
    }
}