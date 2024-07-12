namespace Monaverse.Api.Modules.Token.Responses
{
    public record GetTokenAnimationResponse
    {
        public string AnimationUrl { get; set; }
        public string AnimationType { get; set; }
        public string AnimationFiletype { get; set; }
    }
}