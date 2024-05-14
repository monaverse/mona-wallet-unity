using UnityEngine;
using WalletConnectUnity.Core;
using WalletConnectSharp.Sign.Models.Engine;
using WalletConnectSharp.Sign.Models;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Monaverse.Redcode.Awaiting;
using WalletConnectUnity.Modal;

namespace Monaverse
{
    public class MonaWalletConnectUI : MonoBehaviour
    {
        public string[] SupportedMethods = new[] { "eth_sendTransaction", "personal_sign", "eth_signTypedData" };

        public static MonaWalletConnectUI Instance { get; private set; }

        private bool _connected;
        private Exception _exception;

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

        public virtual async Task Connect(BigInteger chainId)
        {
            _exception = null;
            
            await new WaitUntil(() => WalletConnectModal.IsReady || _exception != null);
            if (_exception != null)
            {
                //Clean up and close UI
                throw _exception;
            }
            
            var sessionResumed = await WalletConnect.Instance.TryResumeSessionAsync();
            if (sessionResumed)
            {
                MonaDebug.Log("[MonaWalletConnectUI] Session resumed");
                return;
            }
            
            _connected = false;

            WalletConnect.Instance.SessionConnected += OnSessionConnected;
            WalletConnect.Instance.SessionDisconnected += OnSessionDisconnected;
            WalletConnect.Instance.ActiveSessionChanged += OnActiveSessionChanged;
            WalletConnectModal.ModalClosed += OnModalClosed;

            WalletConnectModal.Open(new WalletConnectModalOptions
            {
                ConnectOptions = BuildConnectOptions(chainId)
            });

            await new WaitUntil(() => _connected || _exception != null);
            
            WalletConnect.Instance.SessionConnected -= OnSessionConnected;
            WalletConnect.Instance.SessionDisconnected -= OnSessionDisconnected;
            WalletConnect.Instance.ActiveSessionChanged -= OnActiveSessionChanged;
            WalletConnectModal.ModalClosed -= OnModalClosed;
            
            if (_exception != null && !_connected)
            {
                //Clean up and close UI
                throw _exception;
            }
        }

        private void OnModalClosed(object sender, EventArgs e)
        {
            if(_connected)
                return;
            
            Cancel();
        }

        public virtual void Cancel()
        {
            _exception = new UnityException("User cancelled");
        }

        private void OnSessionDisconnected(object sender, EventArgs e)
        {
            _connected = false;
            MonaDebug.Log($"[MonaWalletConnectUI] OnSessionDisconnected");
        }

        private void OnActiveSessionChanged(object sender, SessionStruct e)
        {
            if (string.IsNullOrEmpty(e.Topic))
                return;
            
            _connected = true;
            MonaDebug.Log($"[MonaWalletConnectUI] Session connected. Topic: {e.Topic}");
        }

        private void OnSessionConnected(object sender, SessionStruct e)
        {
            _connected = true;
            MonaDebug.Log($"[MonaWalletConnectUI] OnSessionConnected");
        }

        private ConnectOptions BuildConnectOptions(BigInteger chainId)
        {
            var requiredNamespaces = new RequiredNamespaces
            {
                {
                    "eip155", new ProposedNamespace
                    {
                        Methods = SupportedMethods,
                        Chains = new[]
                        {
                            $"eip155:{chainId}"
                        },
                        Events = new[]
                        {
                            "chainChanged",
                            "accountsChanged"
                        }
                    }
                }
            };
            
            return new ConnectOptions
            {
                RequiredNamespaces = requiredNamespaces,
            };
        }
    }
}