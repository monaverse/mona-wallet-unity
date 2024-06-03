using System;
using System.Collections.Generic;
using Monaverse.Api.Modules.Collectibles.Dtos;
using Monaverse.Core.Utils;
using Monaverse.Modal.UI.Components;
using UnityEngine;

namespace Monaverse.Modal
{
    public sealed class MonaverseModal : MonoBehaviour
    {
        [field: SerializeField] private bool InitializeOnAwake { get; set; } = true;

        [field: SerializeField] private bool ResumeSessionOnInit { get; set; } = true;

        [field: SerializeField, Space] private MonaModal Modal { get; set; }

        [field: SerializeField] private List<ViewConfiguration> _views;
        
        internal static MonaverseModal Instance { get; private set; }
        
        /// <summary>
        /// Returns true if the MonaverseModal is ready
        /// </summary>
        public static bool IsReady { get; private set; }

        /// <summary>
        /// Called when the MonaverseModal is ready.
        /// This is called after Awake
        /// An instance of the MonaverseModal prefab must exist in the scene
        /// </summary>
        public static event EventHandler Ready;
        
        /// <summary>
        /// Called when the MonaverseModal is opened
        /// </summary>
        public static event EventHandler ModalOpened;
        
        /// <summary>
        /// Called when the MonaverseModal is closed
        /// </summary>
        public static event EventHandler ModalClosed;
        
        /// <summary>
        /// Called when the import button is clicked in a collectible details view
        /// Only collectibles compatible with your application can be imported
        /// By default, all collectibles are importable
        /// For custom compatibility, pass an optional filter function in the Open method
        /// </summary>
        public static event EventHandler<CollectibleDto> ImportCollectibleClicked;
        
        /// This will override the default behavior of the preview button
        /// By default, it will open the respective URL in the browser
        /// </summary>
        public static event EventHandler<CollectibleDto> PreviewCollectibleClicked;
        
        /// <summary>
        /// Called when a set of collectibles are loaded in the MonaverseModal Collectibles view
        /// Only collectibles compatible with your application will be passed in this event
        /// By default, all collectibles are loaded
        /// For custom compatibility, pass an optional filter function in the Open method 
        /// </summary>
        public static event EventHandler<List<CollectibleDto>> CollectiblesLoaded;
        
        /// <summary>
        /// Called when a collectible is selected in the Collectibles view
        /// This is a UI only event.
        /// Essentially, this is called when a collectible item is clicked on the Collectible view
        /// and before the Collectible details view is opened
        /// </summary>
        public static event EventHandler<CollectibleDto> CollectibleSelected;
        
        public Func<CollectibleDto, bool> CollectibleFilter { get; private set; }
        
        private void Awake()
        {
            if (!TryConfigureSingleton())
                return;
            
            Initialize();
        }
        
        /// <summary>
        /// This is the entry point for the Monaverse Modal
        /// Call this only after MonaverseModal is ready (After Awake).
        /// You may also listen to the Ready event before calling this
        /// An instance of the MonaverseModal must exist in the scene
        /// </summary>
        /// <param name="collectibleFilter">Optional filter for collectibles. This will determine which collectibles are compatible with your application</param>
        public static void Open(Func<CollectibleDto, bool> collectibleFilter = null)
        {
            if (!IsReady)
            {
                MonaDebug.LogError("[MonaverseModal] MonaverseModal is not ready yet.");
                return;
            }
            
            if (Instance.Modal.IsOpen)
            {
                Debug.LogWarning("[MonaverseModal] MonaverseModal is already open.");
                return;
            }
            
            
            const ViewType defaultView = ViewType.SelectWallet;
            var viewConfiguration = Instance._views.Find(x => x.viewType == defaultView);
            if (viewConfiguration == null)
            {
                Debug.LogError($"[MonaverseModal] No view found for {defaultView}");
                return;
            }
            
            Instance.CollectibleFilter = collectibleFilter;
            Instance.Modal.OpenView(viewConfiguration.view);
        }
        
        /// <summary>
        /// Forcefully closes the Monaverse Modal
        /// </summary>
        public static void Close()
        {
            if (!IsReady)
            {
                MonaDebug.LogError("[MonaverseModal] MonaverseModal is not ready yet.");
                return;
            }
            
            if (!Instance.Modal.IsOpen)
            {
                Debug.LogWarning("[MonaverseModal] MonaverseModal is already closed.");
                return;
            }
            
            Instance.Modal.CloseModal();
        }
        
        private static void Initialize()
        {
            Instance.Modal.Opened += (_, _) => ModalOpened?.Invoke(Instance, EventArgs.Empty);
            Instance.Modal.Closed += (_, _) => ModalClosed?.Invoke(Instance, EventArgs.Empty);
            
            IsReady = true;
            Ready?.Invoke(Instance, EventArgs.Empty);
        }
        
        private bool TryConfigureSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                return true;
            }

            Debug.LogError("[MonaverseModal] MonaverseModal already exists. Destroying...");
            Destroy(gameObject);
            return false;
        }
        
        internal static void TriggerImportCollectibleClicked(CollectibleDto collectibleDto)
        {
            ImportCollectibleClicked?.Invoke(Instance, collectibleDto);
        }
        
        internal static void TriggerPreviewCollectibleClicked(CollectibleDto collectibleDto)
        {
            if (PreviewCollectibleClicked == null)
            {
                Application.OpenURL(collectibleDto.GetMarketplaceUrl());
                return;
            }
            
            PreviewCollectibleClicked.Invoke(Instance, collectibleDto);
        }
        
        internal static void TriggerCollectiblesLoaded(List<CollectibleDto> collectibles)
        {
            CollectiblesLoaded?.Invoke(Instance, collectibles);
        }
        
        internal static void TriggerCollectibleSelected(CollectibleDto collectibleDto)
        {
            CollectibleSelected?.Invoke(Instance, collectibleDto);
        }
        
        [Serializable]
        public class ViewConfiguration
        {
            public MonaModalView view;
            public ViewType viewType;
        }
        
        public enum ViewType
        {
            SelectWallet = 1,
            ConnectingWallet = 2,
            Authorize = 3,
            Collectibles = 4,
            CollectiblesDetail = 5
        }
    }
}