using Newtonsoft.Json;
using System.Collections.Generic;
using Monaverse.Api.Modules.Ai.Responses.Common;

namespace Monaverse.Api.Modules.Ai.Responses
{
    public record GetAssetsByUserResponse
    {
        [JsonProperty("items")]
        public List<Asset> Items { get; set; } = new();

        [JsonProperty("count")]
        public int Count { get; set; }
    }
    
/* JSON PAYLOAD

{
   "items": [
   {
   "creator": {
   "uuid": "string",
   "username": "string"
   },
   "sourceGeneration": {
   "inputAsset": {
   "uuid": "string",
   "url": "string",
   "assetType": "string"
   },
   "uuid": "string",
   "stepType": "string",
   "desiredOutputType": "string",
   "inputText": "string",
   "status": "pending",
   "createdAt": "2025-02-25T18:14:39.723Z",
   "completedAt": "2025-02-25T18:14:39.723Z"
   },
   "derivedGenerationRequests": [],
   "userCollectibles": [],
   "uuid": "string",
   "assetType": "string",
   "url": "string",
   "createdAt": "2025-02-25T18:14:39.723Z",
   "size": 0
   }
   ],
   "count": 0
} 

 */
}