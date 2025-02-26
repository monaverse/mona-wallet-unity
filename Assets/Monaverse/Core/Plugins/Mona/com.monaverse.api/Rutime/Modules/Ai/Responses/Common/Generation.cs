using Newtonsoft.Json;
using System;

namespace Monaverse.Api.Modules.Ai.Responses.Common
{
    public record Generation
    {
        [JsonProperty("inputAsset")]
        public InputAssetInfo InputAsset { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("stepType")]
        public string StepType { get; set; }

        [JsonProperty("desiredOutputType")]
        public string DesiredOutputType { get; set; }

        [JsonProperty("inputText")]
        public string InputText { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("completedAt")]
        public DateTime CompletedAt { get; set; }
    }
} 