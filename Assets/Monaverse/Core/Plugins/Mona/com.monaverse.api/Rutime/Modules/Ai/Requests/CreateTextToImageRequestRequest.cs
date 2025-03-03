using Newtonsoft.Json;
using System.Collections.Generic;

namespace Monaverse.Api.Modules.Ai.Requests
{
    public record CreateTextToImageRequestRequest
    {
        [JsonProperty("prompt")]
        public string Prompt { get; set; }

        [JsonProperty("parameters")]
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
    
/* JSON PAYLOAD
 
{
    "prompt": "string",
    "parameters": {}
}

 */
}