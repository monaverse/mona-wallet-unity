using System;
using System.Collections.Generic;
using Monaverse.Api.Modules.User.Dtos;
using Monaverse.Core;
using Monaverse.Modal.UI.Components;

namespace Monaverse.Modal.UI.Extensions
{
    public static class TokenExtensions
    {
        public static bool CanBeImported(this TokenDto token)
            => MonaverseModal.Instance.Options.TokenFilter == null
               || MonaverseModal.Instance.Options.TokenFilter(token);

        public static List<TokenDto> GetFilteredTokens(this List<TokenDto> tokens)
            => MonaverseModal.Instance.Options.TokenFilter == null
                ? tokens
                : tokens.FindAll(i =>
                {
                    try
                    {
                        return MonaverseModal.Instance.Options.TokenFilter(i);
                    }
                    catch (Exception e)
                    {
                        MonaDebug.LogError($"failed to filter {i?.Name} token: {e.Message}");
                        return false;
                    }
                });

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