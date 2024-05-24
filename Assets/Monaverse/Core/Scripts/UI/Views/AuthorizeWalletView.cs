using System;
using System.Collections;
using System.Threading.Tasks;
using Monaverse.Core;
using Monaverse.Redcode.Awaiting;
using Monaverse.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.UI.Views
{
    public class AuthorizeWalletView : MonaModalView
    {
        [SerializeField] private TMP_Text _authorizeStatusText;
        [SerializeField] private TMP_Text _walletStatusText;
        [SerializeField] private TMP_Text _walletAddressText;
        [SerializeField] private Button _authorizeButton;
        [SerializeField] private GameObject _authorizedGroup;
        [SerializeField] private GameObject _unauthorizedGroup;

        [SerializeField] private MonaModalView _collectiblesView;
        
        public override async void Show(MonaModal modal, IEnumerator effectCoroutine, object options = null)
        {
            base.Show(modal, effectCoroutine, options);
            SetDefaults();
            await TryAuthorize();
        }

        private void Start()
        {
            MonaverseManager.Instance.SDK.Authorized += OnAuthorized;
            MonaverseManager.Instance.SDK.AuthorizationFailed += OnAuthorizationFailed;
            MonaverseManager.Instance.SDK.ConnectionErrored += OnConnectionErrored;
            MonaverseManager.Instance.SDK.SignMessageErrored += OnSignMessageErrored;
        }

        private void SetDefaults()
        {
            _walletStatusText.text = "Wallet Connected";
            _walletAddressText.text = "--";
            _authorizeStatusText.text = "Authorizing...";
            parentModal.Header.EnableBackButton(false);
            SetAuthorizedGroups(false);
            EnableAuthorizeButton(false);
        }

        private async Task TryAuthorize()
        {
            try
            {
                if (!await MonaverseManager.Instance.SDK.IsWalletConnected())
                {
                    _walletStatusText.text = "Wallet Disconnected";
                    _walletAddressText.text = "--";
                    _authorizeStatusText.text = "Lost connection to wallet...";
                    await new WaitForSeconds(2f);
                    parentModal.CloseView();
                    return;
                }
                
                _walletAddressText.text = await MonaverseManager.Instance.SDK.ActiveWallet.GetAddress();
                
                if (MonaverseManager.Instance.SDK.IsWalletAuthorized())
                {
                    OnAuthorized(this, EventArgs.Empty);
                    return;
                }
            
                _authorizeStatusText.text = "Authorizing your wallet...\n\n(Check your Wallet App)";
                await MonaverseManager.Instance.SDK.AuthorizeWallet();
            }
            catch (Exception exception)
            {
                Debug.LogError("[AuthorizeWalletView] AuthorizeWallet Exception: " + exception.Message);
                EnableAuthorizeButton(true);
                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Authorization Failed");
            }
        }

        #region Click Events

        public async void OnAuthorizeButton()
        {
            await TryAuthorize();
        }

        public async void OnDisconnectButton()
        {
            parentModal.CloseView();
            await MonaverseManager.Instance.SDK.Disconnect();
            parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Success, "Wallet Disconnected");
        }

        #endregion


        #region SDK Events

        private void OnSignMessageErrored(object sender, Exception exception)
        {
            Debug.LogError("[AuthorizeWalletView] OnSignMessageErrored: " + exception.Message);
            _authorizeStatusText.text = "Failed creating the signature. Make sure your Wallet App is open.";
            EnableAuthorizeButton(true);
            parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Signature Failed");
        }

        private void OnConnectionErrored(object sender, Exception exception)
        {
            Debug.LogError("[AuthorizeWalletView] OnConnectionErrored: " + exception.Message);
            _walletStatusText.text = "Wallet Disconnected";
            _walletAddressText.text = "--";
            _authorizeStatusText.text = "Lost connection to the wallet...";
            EnableAuthorizeButton(false);
        }

        private void OnAuthorizationFailed(object sender, MonaWalletSDK.AuthorizationResult authorizationResult)
        {
            Debug.LogError("[AuthorizeWalletView] OnAuthorizationFailed: " + authorizationResult);
            _authorizeStatusText.text = "Failed authorizing the wallet. Reason: " + authorizationResult;
            EnableAuthorizeButton(true);
            parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Authorization Failed");
        }

        private async void OnAuthorized(object sender, EventArgs e)
        {
            Debug.Log("[MonaWalletConnectTest.OnAuthorized]");
            EnableAuthorizeButton(false);
            SetAuthorizedGroups(true);
            parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Success, "Wallet Authorized");
            
            await new WaitForSeconds(2f);
            parentModal.OpenView(_collectiblesView);
        }
        
        #endregion
        
        private void EnableAuthorizeButton(bool isEnabled)
        {
            _authorizeButton.interactable = isEnabled;
            _authorizeButton.gameObject.SetActive(isEnabled);
        }
        
        private void SetAuthorizedGroups(bool isAuthorized)
        {
            _unauthorizedGroup.SetActive(!isAuthorized);
            _authorizedGroup.SetActive(isAuthorized);
        }
    }
}