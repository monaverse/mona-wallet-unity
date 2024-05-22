using System;
using Monaverse.UI.Components;
using UnityEngine;

namespace Monaverse.Modal
{
    public sealed class MonaverseModal : MonoBehaviour
    {
        [field: SerializeField] private bool InitializeOnAwake { get; set; } = true;

        [field: SerializeField] private bool ResumeSessionOnInit { get; set; } = true;

        [field: SerializeField, Space] private MonaModal Modal { get; set; }
        
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
    }
}