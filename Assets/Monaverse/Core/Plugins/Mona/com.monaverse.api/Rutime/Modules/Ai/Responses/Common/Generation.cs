using System;

namespace Monaverse.Api.Modules.Ai.Responses.Common
{
    public record Generation
    {
        public InputAssetInfo InputAsset { get; set; }
        
        public string Uuid { get; set; }
        
        public string StepType { get; set; }
        
        public string DesiredOutputType { get; set; }
        
        public string InputText { get; set; }
        
        public string Status { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime CompletedAt { get; set; }
    }
} 