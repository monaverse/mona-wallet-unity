using Monaverse.Api.Modules.Ai.Responses.Common;

namespace Monaverse.Api.Modules.Ai.Responses
{
    public record GetAssetResponse : Asset
    {
    }
    
/* JSON PAYLOAD
 
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
   "createdAt": "2025-02-25T18:14:39.710Z",
   "completedAt": "2025-02-25T18:14:39.710Z"
   },
   "derivedGenerationRequests": [],
   "userCollectibles": [],
   "uuid": "string",
   "assetType": "string",
   "url": "string",
   "createdAt": "2025-02-25T18:14:39.710Z",
   "size": 0
}
 
 */
}