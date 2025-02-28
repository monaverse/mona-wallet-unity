using System;
using System.Collections.Generic;

namespace Monaverse.Api.Modules.Ai.Responses.Common
{
    public record Asset
    {
        public Creator Creator { get; set; }
        
        public Generation SourceGeneration { get; set; }
        
        public List<object> DerivedGenerationRequests { get; set; } = new();
        
        public List<object> UserCollectibles { get; set; } = new();
        
        public string Uuid { get; set; }
        
        public string AssetType { get; set; }
        
        public string Url { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public int Size { get; set; }
    }
} 