namespace Monaverse.Api.Modules.Common.Dtos
{
    public class TokenFiltersDto
    {
        public string Continuation { get; set; } = null;
        public int Limit { get; set; } = 20;
        public bool IncludeLastSale { get; set; } = true;
        public bool IncludeAttributes { get; set; } = true;
        public bool IncludeTopBid { get; set; } = true;
        public bool ExcludeBurnt { get; set; } = true;
    }
}