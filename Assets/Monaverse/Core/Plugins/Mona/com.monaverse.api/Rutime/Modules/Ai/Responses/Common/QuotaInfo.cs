namespace Monaverse.Api.Modules.Ai.Responses.Common
{
    public record QuotaInfo
    {
        public string GenerationType { get; set; }
        
        public string Period { get; set; }
        
        public int Limit { get; set; }
        
        public int Used { get; set; }
        
        public int Remaining { get; set; }
        
        public bool IsCustom { get; set; }
    }
} 