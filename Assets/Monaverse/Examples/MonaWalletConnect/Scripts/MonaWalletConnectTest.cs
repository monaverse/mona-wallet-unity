using System;
using System.Threading.Tasks;
using Monaverse.Api;
using Monaverse.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.Examples
{
    public class MonaWalletConnectTest : MonoBehaviour
    {
        [Header("Labels")]
        [SerializeField] private TMP_Text _connectButtonLabel;
        [SerializeField] private TMP_Text _resultLabel;
        
        [Header("Buttons")]
        [SerializeField] private Button _connectButton;
        [SerializeField] private Button _disconnectButton;
        [SerializeField] private Button _authorizeButton;
        [SerializeField] private Button _signOutButton;
        [SerializeField] private Button _postAuthorizeButton;
        
        [Header("States")]
        [Space] [SerializeField] private GameObject _dappButtons;
        [SerializeField] private GameObject _connectedState;
        [SerializeField] private GameObject _authorizedState;

        private enum WalletState
        {
            Disconnected,
            Connecting,
            Connected,
            Authorized
        }

        private void Start()
        {
            SetUIState(WalletState.Disconnected);
        }

        private async Task TryConnect()
        {
            try
            {
                SetUIState(WalletState.Connecting);

                await MonaverseManager.Instance.SDK.ConnectWallet();

                Debug.Log("[MonaWalletConnectTest] Connected!");

                SetUIState(WalletState.Connected);

                if (MonaverseManager.Instance.SDK.IsWalletAuthorized())
                {
                    SetUIState(WalletState.Authorized);
                    Debug.Log("[MonaWalletConnectTest] Wallet was already authorized!");
                }
            }
            catch (Exception exception)
            {
                Debug.Log("[MonaWalletConnectTest] failed: " + exception);
                SetUIState(WalletState.Disconnected);
            }
        }

        #region UI Click Events

        public async void OnConnectButton()
        {
            Debug.Log("[MonaWalletConnectTest] OnConnectButton");
            _connectButton.interactable = false;

            await TryConnect();
        }

        public async void OnDisconnectButton()
        {
            Debug.Log("[MonaWalletConnectTest] OnDisconnectButton");

            try
            {
                _disconnectButton.interactable = false;
                await MonaverseManager.Instance.SDK.Disconnect();
                SetUIState(WalletState.Disconnected);
            }
            catch (Exception e)
            {
                _disconnectButton.interactable = true;
                Debug.LogError("[MonaWalletConnectTest] Disconnect Exception: " + e);
            }
        }

        public async void OnAuthorizeWallet()
        {
            try
            {
                _authorizeButton.interactable = false; 
                Debug.Log("[MonaWalletConnectTest] OnAuthorizeWallet");
                
                _resultLabel.text = "Authorizing Wallet...";
            
                var authorizationResult = await MonaverseManager.Instance.SDK.AuthorizeWallet();

                var resultText = authorizationResult switch
                {
                    MonaWalletSDK.AuthorizationResult.WalletNotConnected => "Wallet Not Connected",
                    MonaWalletSDK.AuthorizationResult.Authorized => "Wallet Authorized",
                    MonaWalletSDK.AuthorizationResult.FailedAuthorizing => "Failed authorizing wallet",
                    MonaWalletSDK.AuthorizationResult.UserNotRegistered => "User not registered",
                    MonaWalletSDK.AuthorizationResult.FailedValidatingWallet => "Failed validating wallet",
                    MonaWalletSDK.AuthorizationResult.FailedSigningMessage => "Failed signing message",
                    MonaWalletSDK.AuthorizationResult.Error => "Unexpected error authorizing wallet",
                    _ => throw new ArgumentOutOfRangeException()
                };
            
                _authorizeButton.interactable = true;
            
                if(authorizationResult == MonaWalletSDK.AuthorizationResult.Authorized)
                    SetUIState(WalletState.Authorized);

                Debug.Log("[MonaWalletConnectTest] Authorization Result: " + resultText);
                _resultLabel.text = resultText;
            }
            catch (Exception exception)
            {
                _authorizeButton.interactable = true;
                Debug.LogError("[MonaWalletConnectTest] AuthorizeWallet Exception: " + exception.Message);
            }
        }
        
        public void OnSignOut()
        {
            try
            {
                Debug.Log("[MonaWalletConnectTest] OnSignOut");
                
                _resultLabel.text = "Signing out from the Monaverse...";
            
                _signOutButton.interactable = false; 

                MonaverseManager.Instance.SDK.ApiClient.ClearSession();
                
                _signOutButton.interactable = true;
            
                SetUIState(WalletState.Connected);
            }
            catch (Exception exception)
            {
                _authorizeButton.interactable = true;
                Debug.LogError("[MonaWalletConnectTest] SignOut Exception: " + exception.Message);
            }
        }

        public async void OnGetCollectibles()
        {
            if (!MonaverseManager.Instance.SDK.IsWalletAuthorized())
            {
                _resultLabel.text = "Wallet must be authorized first!";
                return;
            }

            _resultLabel.text = "Getting wallet collectibles...";

            var getCollectiblesResult = await MonaApi.ApiClient.Collectibles.GetWalletCollectibles();
            Debug.Log("[MonaWalletConnectTest] Collectibles: " + getCollectiblesResult);
            if (!getCollectiblesResult.IsSuccess)
            {
                _resultLabel.text = "Error: GetCollectibles failed: " + getCollectiblesResult.Message;
                return;
            }

            _resultLabel.text = "Success: wallet collectible count: " + getCollectiblesResult.Data.TotalCount;
        }

        #endregion

        private void SetUIState(WalletState state)
        {
            _dappButtons.SetActive(false);
            _connectedState.SetActive(false);
            _authorizedState.SetActive(false);
            
            switch (state)
            {
                case WalletState.Disconnected:
                    _connectButtonLabel.text = "Connect";
                    _connectButton.interactable = true;
                    _dappButtons.SetActive(false);
                    _connectedState.SetActive(false);
                    _disconnectButton.gameObject.SetActive(false);
                    _resultLabel.text = "Disconnected";
                    break;
                case WalletState.Connecting:
                    _connectButtonLabel.text = "Connecting...";
                    _connectButton.interactable = false;
                    break;
                case WalletState.Connected:
                    _connectButton.interactable = false;
                    _connectButtonLabel.text = "Connected";
                    _resultLabel.text = "Wallet Connected!";
                    _dappButtons.SetActive(true);
                    _connectedState.SetActive(true);
                    _disconnectButton.interactable = true;
                    _disconnectButton.gameObject.SetActive(true);
                    break;
                case WalletState.Authorized:
                    _dappButtons.SetActive(true);
                    _authorizedState.SetActive(true);
                    _resultLabel.text = "Wallet Authorized!";
                    break;
            }
        }
    }
}