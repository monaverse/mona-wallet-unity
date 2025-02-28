using System;
using System.Collections.Generic;

namespace Monaverse.Api.Modules.Ai.Responses.Common
{
    public record GenerationRequest
    {
        public Asset OutputAsset { get; set; }
        
        public Asset InputAsset { get; set; }
        
        public string Uuid { get; set; }
        
        public string DesiredOutputType { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public string StepType { get; set; }
        
        public string Status { get; set; }
        
        public Dictionary<string, object> Parameters { get; set; } = new();
        
        public string InputText { get; set; }
        
        public DateTime? StartedAt { get; set; }
        
        public DateTime? CompletedAt { get; set; }
        
        public string ErrorMessage { get; set; }
    }
} 