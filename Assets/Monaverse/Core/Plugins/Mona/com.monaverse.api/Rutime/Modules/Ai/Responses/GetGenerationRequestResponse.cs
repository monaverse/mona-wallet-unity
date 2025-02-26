using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Monaverse.Api.Modules.Ai.Responses.Common;

namespace Monaverse.Api.Modules.Ai.Responses
{
    public record GetGenerationRequestResponse : GenerationRequest
    {
    }

/* JSON PAYLOAD
 
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
      "createdAt": "2025-02-25T18:14:39.738Z",
      "completedAt": "2025-02-25T18:14:39.738Z"
    },
    "derivedGenerationRequests": [],
    "userCollectibles": [],
    "uuid": "string",
    "assetType": "string",
    "url": "string",
    "createdAt": "2025-02-25T18:14:39.738Z",
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
      "createdAt": "2025-02-25T18:14:39.738Z",
      "completedAt": "2025-02-25T18:14:39.738Z"
    },
    "derivedGenerationRequests": [],
    "userCollectibles": [],
    "uuid": "string",
    "assetType": "string",
    "url": "string",
    "createdAt": "2025-02-25T18:14:39.738Z",
    "size": 0
  },
  "uuid": "string",
  "desiredOutputType": "string",
  "createdAt": "2025-02-25T18:14:39.738Z",
  "stepType": "string",
  "status": "pending",
  "parameters": {},
  "inputText": "string",
  "startedAt": "2025-02-25T18:14:39.738Z",
  "completedAt": "2025-02-25T18:14:39.738Z",
  "errorMessage": "string"
}

*/
}