using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Monaverse.Api.Modules.Ai.Responses.Common
{
    public record GenerationRequest
    {
        [JsonProperty("outputAsset")]
        public Asset OutputAsset { get; set; }

        [JsonProperty("inputAsset")]
        public Asset InputAsset { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("desiredOutputType")]
        public string DesiredOutputType { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("stepType")]
        public string StepType { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("parameters")]
        public Dictionary<string, object> Parameters { get; set; } = new();

        [JsonProperty("inputText")]
        public string InputText { get; set; }

        [JsonProperty("startedAt")]
        public DateTime StartedAt { get; set; }

        [JsonProperty("completedAt")]
        public DateTime CompletedAt { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }
} 