namespace Monaverse.Api.Modules.User.Dtos
{
    public record TokenAttributeDto
    {
        public string Key { get; set; }
        public string Kind { get; set; }
        public string Value { get; set; }
        public int TokenCount { get; set; }
    }
}