namespace Monaverse.Api.Modules.Common.Dtos
{
    public record SourceDto
    {
        public string Id { get; set; }
        public string Domain { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
    }
}