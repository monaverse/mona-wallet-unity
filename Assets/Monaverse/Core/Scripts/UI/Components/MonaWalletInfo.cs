using System;
using Monaverse.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.UI.Components
{
    public class MonaWalletInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text _walletStatusText;
        [SerializeField] private TMP_Text _walletAddressText;
        [SerializeField] private Button _disconnectButton;
        
        private void Start()
        {
            MonaverseManager.Instance.SDK.Connected += OnConnected;
            MonaverseManager.Instance.SDK.Disconnected += OnDisconnected;
            MonaverseManager.Instance.SDK.ConnectionErrored += OnConnectionErrored;
            _disconnectButton.onClick.AddListener(OnDisconnectButton);
        }

        private void OnConnectionErrored(object sender, Exception e)
        {
            SetWalletInfo("Connection Error", "--");
            _disconnectButton.interactable = false;
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            SetWalletInfo("Wallet Disconnected", "--");
        }

        private void OnConnected(object sender, string address)
        {
            SetWalletInfo("Wallet Connected", address);
        }
        
        public async void Show()
        {
            var session = MonaverseManager.Instance.SDK.Session;
            if(session.IsActive)
                SetWalletInfo("Wallet Connected", session.WalletAddress);
            else
                SetWalletInfo("Wallet Disconnected", "--");
        }

        public void SetWalletInfo(string status, string address)
        {
            _disconnectButton.interactable = true;
            _walletStatusText.text = status;
            _walletAddressText.text = address;
        }
        
        public async void OnDisconnectButton()
        {
            try
            {
                _disconnectButton.interactable = false;
                await MonaverseManager.Instance.SDK.Disconnect();
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
            }
            
            _disconnectButton.interactable = true;
        }
    }
}