using Monaverse.Api.Configuration;
using UnityEngine;

namespace Monaverse.Core
{
    public class MonaverseManager : MonoBehaviour
    {
        [Tooltip("Whether the SDK should initialize on awake or not")]
        [SerializeField] private bool _initializeOnAwake;
        
        [Tooltip(" Monaverse Application ID.")]
        public string monaApplicationId = null;

        [Tooltip("WalletConnect Project ID (https://cloud.walletconnect.com/app)")]
        public string walletConnectProjectId = null;
        
        [Tooltip("The Monaverse API environment to use")]
        public ApiEnvironment apiEnvironment = ApiEnvironment.Staging;
        
        [Tooltip("Whether to show the sdk debug logs")]
        public bool showDebugLogs = false;

        [Tooltip("Instantiates the WalletConnect SDK for Native platforms.")]
        public GameObject WalletConnectPrefab;
        public static MonaverseManager Instance { get; private set; }
        public MonaWalletSDK SDK { get; private set; }


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.LogWarning("Two MonaverseManager instances were found, removing this one.");
                Destroy(gameObject);
                return;
            }

            if (_initializeOnAwake)
                Initialize();
        }
        
        public static void Initialize()
        {
            if (Instance == null)
            {
                MonaDebug.LogError("A MonaverseManager component must be attached to a GameObject in a scene");
                return;
            }
            
            var sdkOptions = new MonaWalletSDK.SDKOptions
            {
                applicationId = Instance.monaApplicationId,
                walletConnectProjectId = Instance.walletConnectProjectId,
                apiEnvironment = Instance.apiEnvironment
            };

            MonaDebug.IsEnabled = Instance.showDebugLogs;
            Instance.SDK = new MonaWalletSDK(sdkOptions);
        }
    }
}