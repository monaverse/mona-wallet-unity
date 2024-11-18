using System;
using System.Collections.Generic;
using Monaverse.Api.Modules.User.Dtos;
using Monaverse.Core.Scripts.Utils;
using Monaverse.Modal.UI.Components;
using Monaverse.Modal.UI.Views;
using UnityEngine;

namespace Monaverse.Modal
{
    public sealed class MonaverseModal : MonoBehaviour
    {
        [field: SerializeField] private bool InitializeOnAwake { get; set; } = true;

        [field: SerializeField] private bool ResumeSessionOnInit { get; set; } = true;

        [field: SerializeField, Space] private MonaModal Modal { get; set; }

        private List<MonaModalView> _views;
        private List<MonaModalDialog> _dialogs;
        private MonaModalView _defaultView;
        private MonaModalDialog _defaultDialogView;

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
        public static event EventHandler<TokenDto> ImportTokenClicked;

        /// <summary>
        /// This will override the default behavior of the preview button
        /// By default, it will open the respective URL in the browser
        /// </summary>
        public static event EventHandler<TokenDto> PreviewTokenClicked;

        /// <summary>
        /// Called when a set of tokens are loaded or updated from the Monaverse API
        /// Only tokens compatible with your application will be passed in this event
        /// By default, all tokens are loaded
        /// For custom compatibility, pass an optional filter function in the Open method 
        /// </summary>
        public static event EventHandler<List<TokenDto>> TokensLoaded;

        /// <summary>
        /// Called when the MonaverseModal Tokens view is opened
        /// This is a UI only event.
        /// By default, all tokens are loaded
        /// For custom compatibility, pass an optional filter function in the Open method 
        /// </summary>
        public static event EventHandler<List<TokenDto>> TokensViewOpened;

        /// <summary>
        /// Called when a collectible is selected in the Collectibles view
        /// This is a UI only event.
        /// Essentially, this is called when a collectible item is clicked on the Collectible view
        /// and before the Collectible details view is opened
        /// </summary>
        public static event EventHandler<TokenDto> TokenSelected;

        public ModalOptions Options { get; private set; }

        public class ModalOptions
        {
            /// <summary>
            /// Set to false if you don't want to load the tokens UI after authentication.
            /// Defaults to true
            /// </summary>
            public bool LoadTokensView { get; set; } = true;

            /// <summary>
            /// Optional filter for tokens. This will determine which tokens are compatible with your application
            /// </summary>
            public Func<TokenDto, bool> TokenFilter { get; set; } = null;

            /// <summary>
            /// If true, the cache will be flushed before opening the modal. Defaults to false.
            /// This is useful if you want to force a refresh of the tokens loaded in the modal before calling Open
            /// </summary>
            public bool FlushCache { get; set; } = false;
        }

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
        /// <param name="options"> Optional parameters for the Monaverse Modal</param>
        public static void Open(Action<ModalOptions> options = null)
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

            if (Instance._defaultView == null)
                Instance._defaultView = Instance._views.Find(x => x is GenerateOtpView);

            if (Instance._defaultDialogView == null)
                Instance._defaultDialogView = Instance._dialogs.Find(x => x is DialogView);

            if (Instance._defaultView == null)
            {
                Debug.LogError($"[MonaverseModal] No view found for {nameof(GenerateOtpView)}");
                return;
            }

            //Apply options
            Instance.Options = new ModalOptions();
            options?.Invoke(Instance.Options);

            //Flush cache if requested
            if (Instance.Options.FlushCache)
            {
                foreach (var view in Instance._views)
                    view.FlushCache();
            }

            Instance.Modal.OpenView(Instance._defaultView);
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

            //Find and reference all existing modal views and dialogs in children
            Instance._views = new List<MonaModalView>();
            Instance._dialogs = new List<MonaModalDialog>();
            foreach (Transform child in Instance.Modal.transform)
            {
                var views = child.GetComponentsInChildren<MonaModalView>();

                foreach (var view in views)
                {
                    if (view == null) continue;
                    Instance._views.Add(view);
                }

                var dialogs = child.GetComponentsInChildren<MonaModalDialog>();

                foreach (var dialog in dialogs)
                {
                    if (dialog == null) continue;
                    Instance._dialogs.Add(dialog);
                }
            }

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

        internal static void TriggerImportTokenClicked(TokenDto tokenDto)
        {
            ImportTokenClicked?.Invoke(Instance, tokenDto);
        }

        internal static void TriggerPreviewCollectibleClicked(TokenDto tokenDto)
        {
            if (PreviewTokenClicked == null)
            {
                Application.OpenURL(tokenDto.GetMarketplaceUrl());
                return;
            }

            PreviewTokenClicked.Invoke(Instance, tokenDto);
        }

        internal static void TriggerTokensLoaded(List<TokenDto> tokens)
        {
            TokensLoaded?.Invoke(Instance, tokens);
        }

        internal static void TriggerTokensViewOpened(List<TokenDto> tokens)
        {
            TokensViewOpened?.Invoke(Instance, tokens);
        }

        internal static void TriggerTokenSelected(TokenDto tokenDto)
        {
            TokenSelected?.Invoke(Instance, tokenDto);
        }

        internal static void ShowDialog(string title,
            string message,
            Action onConfirm = null,
            Action onCancel = null,
            string confirmText = null,
            string cancelText = null)
        {
            Instance.Modal.OpenDialog(Instance._defaultDialogView, parameters: new DialogView.DialogParams
            {
                Title = title,
                Message = message,
                ConfirmButtonTitle = confirmText,
                CancelButtonTitle = cancelText,
                OnConfirm = onConfirm,
                OnCancel = onCancel
            });
        }
    }
}