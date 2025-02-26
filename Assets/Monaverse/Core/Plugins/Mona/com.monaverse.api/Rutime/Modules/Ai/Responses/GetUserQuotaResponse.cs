using System.Collections.Generic;
using Newtonsoft.Json;
using Monaverse.Api.Modules.Ai.Responses.Common;

namespace Monaverse.Api.Modules.Ai.Responses
{
    public record GetUserQuotaResponse
    {
        [JsonExtensionData]
        public Dictionary<string, QuotaInfo> Quotas { get; set; } = new();
    }

/* JSON PAYLOAD

{
  "additionalProp1": {
    "generationType": "string",
    "period": "string",
    "limit": 0,
    "used": 0,
    "remaining": 0,
    "isCustom": true
  },
  "additionalProp2": {
    "generationType": "string",
    "period": "string",
    "limit": 0,
    "used": 0,
    "remaining": 0,
    "isCustom": true
  },
  "additionalProp3": {
    "generationType": "string",
    "period": "string",
    "limit": 0,
    "used": 0,
    "remaining": 0,
    "isCustom": true
  }
}

*/
}