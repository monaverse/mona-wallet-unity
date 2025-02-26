using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Monaverse.Api.Modules.Ai.Responses.Common
{
    public record Asset
    {
        [JsonProperty("creator")]
        public Creator Creator { get; set; }

        [JsonProperty("sourceGeneration")]
        public Generation SourceGeneration { get; set; }

        [JsonProperty("derivedGenerationRequests")]
        public List<object> DerivedGenerationRequests { get; set; } = new();

        [JsonProperty("userCollectibles")]
        public List<object> UserCollectibles { get; set; } = new();

        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("assetType")]
        public string AssetType { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }
    }
} 