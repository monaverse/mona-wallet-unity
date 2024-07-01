namespace Monaverse.Api.Modules.Common.Dtos
{
    public record OrderDto
    { 
        public string Id { get; set; }
        public PriceDto Price { get; set; }
        public string Maker { get; set; }
        public float ValidFrom { get; set; }
        public float ValidUntil { get; set; }
        public float QuantityFilled { get; set; }
        public float QuantityRemaining { get; set; }
        public SourceDto Source { get; set; }
    }
}