using System.Collections.Generic;
using Monaverse.Api.Modules.User.Dtos;
using Monaverse.Modal;
using UnityEngine;

namespace Monaverse.Examples
{
    public class MonaverseModalExample : MonoBehaviour
    {
        [SerializeField] private MonaCollectibleListExample _compatibleItems;
        [SerializeField] private MonaCollectibleItemExample _importedItem;
        private void Start()
        {
            MonaverseModal.ImportTokenClicked += OnImportTokenClicked;
            MonaverseModal.TokensLoaded += OnTokensLoaded;
            MonaverseModal.TokensViewOpened += OnTokensViewOpened;
        }

        private void OnTokensViewOpened(object sender, List<TokenDto> tokens)
        {
            Debug.Log("[MonaverseModalExample.OnTokensViewOpened] loaded " + tokens.Count + " tokens");
        }

        /// <summary>
        /// Called when tokens are loaded from the Monaverse API
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="loadedTokens">A list of loaded tokens</param>
        private async void OnTokensLoaded(object sender, List<TokenDto> loadedTokens)
        {
            Debug.Log("[MonaverseModalExample.OnTokensLoaded] loaded " + loadedTokens.Count + " tokens");
            await _compatibleItems.SetCollectibles(loadedTokens);
        }

        /// <summary>
        /// Called when the import button is clicked in a token details view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="token">The token selected for import</param>
        private void OnImportTokenClicked(object sender, TokenDto token)
        {
            Debug.Log("[MonaverseModalExample.OnImportTokenClicked] " + token.Name);
            _importedItem.SetCollectible(token);
        }
        
        /// <summary>
        /// This is the entry point for the Monaverse Modal
        /// Called on button click
        /// </summary>
        public void OpenModal()
        {
            MonaverseModal.Open();
        }
    }
}