using UnityEngine;
using UnityEngine.UI;
using WalletConnectUnity.Core;
using WalletConnectSharp.Sign.Models.Engine;
using WalletConnectSharp.Sign.Models;
using System;
using System.Collections.Generic;
using WalletConnectUnity.Modal;

namespace Monaverse
{
    public class WalletConnectUI : MonoBehaviour
    {
        [SerializeField] private Button _continueButton;
        [Space] [SerializeField] private GameObject _dappButtons;

        public static WalletConnectUI Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Application.targetFrameRate = Screen.currentResolution.refreshRate;

            // When WalletConnectModal is ready, enable buttons and subscribe to other events.
            // WalletConnectModal.SignClient can be null if WalletConnectModal is not ready.
            if (WalletConnectModal.IsReady)
            {
                var connected = WalletConnect.Instance.IsConnected;
                InitialiseDapp(connected);
            }
            else
            {
                WalletConnectModal.Ready += (_, args) => { InitialiseDapp(args.SessionResumed); };
            }
        }

        private void InitialiseDapp(bool connected)
        {
            if (connected)
                EnableDappButtons();
            else
                EnableContinueButton();
            
            // Invoked after wallet connected
            WalletConnect.Instance.ActiveSessionChanged += (_, @struct) =>
            {
                if (string.IsNullOrEmpty(@struct.Topic))
                    return;

                Debug.Log($"[WalletConnectModalSample] Session connected. Topic: {@struct.Topic}");
                EnableDappButtons();
            };

            // Invoked after wallet disconnected
            WalletConnect.Instance.SessionDisconnected += (_, _) =>
            {
                Debug.Log($"[WalletConnectModalSample] Session deleted.");
                EnableContinueButton();
            };
        }

        private void EnableContinueButton()
        {
            _dappButtons.SetActive(false);
            _continueButton.gameObject.SetActive(true);
        }

        private void EnableDappButtons()
        {
            _dappButtons.SetActive(true);
            _continueButton.gameObject.SetActive(false);
        }

        public void OnContinueButton()
        {
            var options = new WalletConnectModalOptions
            {
                ConnectOptions = BuildConnectOptions()
            };

            WalletConnectModal.Open(options);
        }

        private ConnectOptions BuildConnectOptions()
        {
            // Using optional namespaces. Wallet will approve only chains it supports.
            var optionalNamespaces = new Dictionary<string, ProposedNamespace>();
            var methods = new[]
            {
                "wallet_switchEthereumChain",
                "wallet_addEthereumChain",
                "eth_sendTransaction",
                "personal_sign"
            };

            var events = new[]
            {
                "chainChanged", "accountsChanged"
            };

            var chainIds = new[] { ChainConstants.Chains.Ethereum.ChainId };

            optionalNamespaces.Add(ChainConstants.Namespaces.Evm, new ProposedNamespace
            {
                Chains = chainIds,
                Events = events,
                Methods = methods
            });

            if (optionalNamespaces.Count == 0)
                throw new InvalidOperationException("No chains selected");

            return new ConnectOptions
            {
                OptionalNamespaces = optionalNamespaces
            };
        }
    }
}