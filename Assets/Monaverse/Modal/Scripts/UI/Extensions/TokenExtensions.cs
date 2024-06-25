using System.Collections.Generic;
using Monaverse.Api.Modules.User.Dtos;

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
    }
}