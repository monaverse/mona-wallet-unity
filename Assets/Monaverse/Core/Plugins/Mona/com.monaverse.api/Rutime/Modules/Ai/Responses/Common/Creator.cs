using Newtonsoft.Json;

namespace Monaverse.Api.Modules.Ai.Responses.Common
{
    public record Creator
    {
        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }
} 