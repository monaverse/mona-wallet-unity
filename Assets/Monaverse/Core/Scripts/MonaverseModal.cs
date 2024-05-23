using System;
using System.Collections.Generic;
using Monaverse.Core;
using Monaverse.UI.Components;
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
        
        public static bool IsReady { get; private set; }

        public static event EventHandler Ready;
        public static event EventHandler ModalOpened;
        public static event EventHandler ModalClosed;
        
        private void Awake()
        {
            if (!TryConfigureSingleton())
                return;
            
            Initialize();
        }
        
        public static void Open(ViewType view = ViewType.Connect)
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
            
            var viewConfiguration = Instance._views.Find(x => x.viewType == view);
            if (viewConfiguration == null)
            {
                Debug.LogError($"[MonaverseModal] No view found for {view}");
                return;
            }
            
            Instance.Modal.OpenView(viewConfiguration.view);
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
        
        [Serializable]
        public class ViewConfiguration
        {
            public MonaModalView view;
            public ViewType viewType;
        }
        
        public enum ViewType
        {
            Connect = 1,
            Authorize = 2,
            Collectibles = 3
        }
    }
}