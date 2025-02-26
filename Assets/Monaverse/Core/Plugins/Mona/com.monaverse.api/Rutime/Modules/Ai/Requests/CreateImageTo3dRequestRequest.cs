using Newtonsoft.Json;
using System.Collections.Generic;

namespace Monaverse.Api.Modules.Ai.Requests
{
    public record CreateImageTo3dRequestRequest
    {
        [JsonProperty("image_id")]
        public string ImageId { get; set; }

        [JsonProperty("parameters")]
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
  
/* JSON PAYLOAD
 
{
    "image_id": "string",
    "parameters": {}
}

*/
}