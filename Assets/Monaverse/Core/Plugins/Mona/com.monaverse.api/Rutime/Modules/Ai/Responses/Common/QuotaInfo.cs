using Newtonsoft.Json;

namespace Monaverse.Api.Modules.Ai.Responses.Common
{
    public record QuotaInfo
    {
        [JsonProperty("generationType")]
        public string GenerationType { get; set; }

        [JsonProperty("period")]
        public string Period { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("used")]
        public int Used { get; set; }

        [JsonProperty("remaining")]
        public int Remaining { get; set; }

        [JsonProperty("isCustom")]
        public bool IsCustom { get; set; }
    }
} 