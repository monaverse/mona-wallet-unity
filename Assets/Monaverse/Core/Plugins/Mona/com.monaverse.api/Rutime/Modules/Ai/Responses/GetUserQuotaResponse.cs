using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Monaverse.Api.Modules.Ai.Responses.Common;

namespace Monaverse.Api.Modules.Ai.Responses
{
    public record GetUserQuotaResponse
    {
        [JsonExtensionData]
        public Dictionary<string, object> QuotasRaw { get; set; } = new Dictionary<string, object>();

        public Dictionary<string, QuotaInfo> Quotas => QuotasRaw.ToQuotaInfoDictionary();
    }

    public static class QuotaExtensions
    {
        public static Dictionary<string, QuotaInfo> ToQuotaInfoDictionary(this Dictionary<string, object> raw)
        {
            if (raw == null) return new Dictionary<string, QuotaInfo>();

            return raw.ToDictionary(
                kvp => kvp.Key,
                kvp => JObject.FromObject(kvp.Value).ToObject<QuotaInfo>()
            );
        }
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