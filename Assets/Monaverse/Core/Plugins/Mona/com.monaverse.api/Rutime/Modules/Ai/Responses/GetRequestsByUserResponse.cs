using Newtonsoft.Json;
using System.Collections.Generic;
using Monaverse.Api.Modules.Ai.Responses.Common;

namespace Monaverse.Api.Modules.Ai.Responses
{
    public record GetRequestsByUserResponse
    {
        [JsonProperty("items")]
        public List<GenerationRequest> Items { get; set; } = new();

        [JsonProperty("count")]
        public int Count { get; set; }
    }

/* JSON PAYLOAD

{
  "items": [
    {
      "outputAsset": {
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
          "createdAt": "2025-02-25T18:14:39.716Z",
          "completedAt": "2025-02-25T18:14:39.716Z"
        },
        "derivedGenerationRequests": [],
        "userCollectibles": [],
        "uuid": "string",
        "assetType": "string",
        "url": "string",
        "createdAt": "2025-02-25T18:14:39.716Z",
        "size": 0
      },
      "inputAsset": {
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
          "createdAt": "2025-02-25T18:14:39.716Z",
          "completedAt": "2025-02-25T18:14:39.716Z"
        },
        "derivedGenerationRequests": [],
        "userCollectibles": [],
        "uuid": "string",
        "assetType": "string",
        "url": "string",
        "createdAt": "2025-02-25T18:14:39.716Z",
        "size": 0
      },
      "uuid": "string",
      "desiredOutputType": "string",
      "createdAt": "2025-02-25T18:14:39.716Z",
      "stepType": "string",
      "status": "pending",
      "parameters": {},
      "inputText": "string",
      "startedAt": "2025-02-25T18:14:39.716Z",
      "completedAt": "2025-02-25T18:14:39.716Z",
      "errorMessage": "string"
    }
  ],
  "count": 0
}

*/
}