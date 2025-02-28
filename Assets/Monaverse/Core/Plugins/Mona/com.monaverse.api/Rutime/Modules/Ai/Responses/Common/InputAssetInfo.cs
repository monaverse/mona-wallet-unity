namespace Monaverse.Api.Modules.Ai.Responses.Common
{
    public record InputAssetInfo
    {
        public string Uuid { get; set; }
        
        public string Url { get; set; }
        
        public string AssetType { get; set; }
    }
} 