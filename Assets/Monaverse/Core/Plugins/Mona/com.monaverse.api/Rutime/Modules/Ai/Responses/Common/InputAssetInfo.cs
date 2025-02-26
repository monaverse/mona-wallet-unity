using Newtonsoft.Json;

namespace Monaverse.Api.Modules.Ai.Responses.Common
{
    public record InputAssetInfo
    {
        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("assetType")]
        public string AssetType { get; set; }
    }
} 