using System;
using System.Collections;
using System.Threading.Tasks;
using Monaverse.Core;
using Monaverse.Redcode.Awaiting;
using Monaverse.Modal.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.Modal.UI.Views
{
    public class AuthorizeWalletView : MonaModalView
    {
        [SerializeField] private MonaWalletInfo _walletInfo;
        [SerializeField] private TMP_Text _authorizeStatusText;
        [SerializeField] private Button _authorizeButton;
        [SerializeField] private GameObject _authorizedGroup;
        [SerializeField] private GameObject _unauthorizedGroup;

        [SerializeField] private MonaModalView _collectiblesView;
        
        public override void Show(MonaModal modal, IEnumerator effectCoroutine, object options = null)
        {
            base.Show(modal, effectCoroutine, options);
            SetDefaults();
        }
        
        protected override async void OnOpened(object options = null)
        {
            await TryAuthorize();
        }

        private void Start()
        {
            MonaverseManager.Instance.SDK.Authorized += OnAuthorized;
            MonaverseManager.Instance.SDK.AuthorizationFailed += OnAuthorizationFailed;
            MonaverseManager.Instance.SDK.ConnectionErrored += OnConnectionErrored;
            MonaverseManager.Instance.SDK.SignMessageErrored += OnSignMessageErrored;
            MonaverseManager.Instance.SDK.Disconnected += OnDisconnected;
        }

        private void SetDefaults()
        {
            _authorizeStatusText.text = "Authorizing...";
            parentModal.Header.EnableBackButton(false);
            SetAuthorizedGroups(false);
            EnableAuthorizeButton(false);
            _walletInfo.Show();
        }

        private async Task TryAuthorize()
        {
            try
            {
                if (!await MonaverseManager.Instance.SDK.IsWalletConnected())
                {
                    parentModal.CloseView();
                    return;
                }
                
                if (MonaverseManager.Instance.SDK.IsWalletAuthorized())
                {
                    OnAuthorized(this, EventArgs.Empty);
                    return;
                }
            
                _authorizeStatusText.text = "Authorizing your wallet...";
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

        #endregion

        #region SDK Events

        
        private void OnDisconnected(object sender, EventArgs e)
        {
            if(!IsActive)
                return;
            
            parentModal.CloseView();
            parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Success, "Wallet Disconnected");
        }
        
        private void OnSignMessageErrored(object sender, Exception exception)
        {
            if(!IsActive)
                return;
            
            Debug.LogError("[AuthorizeWalletView] OnSignMessageErrored: " + exception.Message);
            _authorizeStatusText.text = "Failed creating the signature. Make sure your Wallet App is open.";
            EnableAuthorizeButton(true);
            parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Signature Failed");
        }

        private void OnConnectionErrored(object sender, Exception exception)
        {
            if(!IsActive)
                return;
            
            Debug.LogError("[AuthorizeWalletView] OnConnectionErrored: " + exception.Message);
            _authorizeStatusText.text = "Lost connection to the wallet...";
            EnableAuthorizeButton(false);
        }

        private void OnAuthorizationFailed(object sender, MonaWalletSDK.AuthorizationResult authorizationResult)
        {
            if(!IsActive)
                return;
            
            Debug.LogError("[AuthorizeWalletView] OnAuthorizationFailed: " + authorizationResult);
            _authorizeStatusText.text = "Failed authorizing the wallet. Reason: " + authorizationResult;
            EnableAuthorizeButton(true);
            parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Authorization Failed");
        }

        private async void OnAuthorized(object sender, EventArgs e)
        {
            if(!IsActive)
                return;
            
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