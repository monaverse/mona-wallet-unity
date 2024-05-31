using System.Collections.Generic;
using Monaverse.Api.Modules.Collectibles.Dtos;
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
            MonaverseModal.ImportCollectibleClicked += OnImportCollectibleClicked;
            MonaverseModal.CollectiblesLoaded += OnCollectiblesLoaded;
        }

        /// <summary>
        /// Called when a collectibles are loaded in the Monaverse Modal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="loadedCollectibles">A list of loaded collectibles</param>
        private async void OnCollectiblesLoaded(object sender, List<CollectibleDto> loadedCollectibles)
        {
            Debug.Log("[MonaverseModalExample.OnCollectiblesLoaded] loaded " + loadedCollectibles.Count + " collectibles");
            await _compatibleItems.SetCollectibles(loadedCollectibles);
        }

        /// <summary>
        /// Called when the import button is clicked in a collectible details view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="collectible">The collectible selected for import</param>
        private void OnImportCollectibleClicked(object sender, CollectibleDto collectible)
        {
            Debug.Log("[MonaverseModalExample.OnImportCollectibleClicked] " + collectible.Title);
            _importedItem.SetCollectible(collectible);
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