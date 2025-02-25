using Monaverse.Api.Modules.Ai.Enums;

namespace Monaverse.Api.Modules.Ai
{
    public static class AiExtensions
    {
        public static string ConvertToString(this AssetTypeFilter outputType)
        {
            return outputType switch
            {
                AssetTypeFilter.Image => "image",
                AssetTypeFilter.Glb => "glb",
                _ => "image"
            };
        }
        
        public static string ConvertToString(this StatusFilter status)
        {
            return status switch
            {
                StatusFilter.Pending => "pending",
                StatusFilter.InProgress => "in_progress",
                StatusFilter.Completed => "completed",
                StatusFilter.Failed => "failed",
                StatusFilter.AwaitingRetry => "awaiting_retry",
                _ => "pending"
            };
        }

        public static string ConvertToString(this StepTypeFilter stepType)
        {
            return stepType switch
            {
                StepTypeFilter.TextToImage => "text-to-image",
                StepTypeFilter.ImageTo3d => "image-to-3d",
                _ => "text-to-image"
            };
        }
    }
}