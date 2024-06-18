namespace Monaverse.Api.Modules.User.Dtos
{
    public sealed record CollectionDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Symbol { get; set; }
        public string ImageUrl { get; set; }
        public int TokenCount { get; set; }
        public string ContractDeployedAt { get; set; }
    }
}