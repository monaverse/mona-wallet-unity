using System.Collections.Generic;
using Monaverse.Api.Modules.User.Dtos;
using Monaverse.Core;
using Monaverse.Modal.UI.Components;

namespace Monaverse.Modal.UI.Extensions
{
    public static class TokenExtensions
    {
        public static bool CanBeImported(this TokenDto token)
            =>  MonaverseModal.Instance.TokenFilter == null || MonaverseModal.Instance.TokenFilter(token);
        
        public static List<TokenDto> GetFilteredTokens(this List<TokenDto> tokens)
            => MonaverseModal.Instance.TokenFilter == null
                ? tokens
                : tokens.FindAll(i=> MonaverseModal.Instance.TokenFilter(i));
        
        public static MonaRemoteSprite GetImageRemoteSprite(this TokenDto token)
        {
            var tokenImageUrl = token?.Image;

            if (string.IsNullOrEmpty(tokenImageUrl))
            {
                MonaDebug.LogWarning($"Token {token?.Name} image url is null or empty. Using default image.");
                tokenImageUrl = MonaConstants.Media.MonaRemoteLogo;
            }
            
            return MonaRemoteSpriteFactory.GetRemoteSprite(tokenImageUrl);
        }
    }
}