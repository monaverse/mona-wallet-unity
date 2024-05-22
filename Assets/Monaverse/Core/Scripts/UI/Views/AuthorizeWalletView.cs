using System;
using System.Collections;
using System.Threading.Tasks;
using Monaverse.Core;
using Monaverse.Redcode.Awaiting;
using Monaverse.UI.Components;
using TMPro;
using UnityEngine;

namespace Monaverse.UI.Views
{
    public class AuthorizeWalletView : MonaModalView
    {
        [SerializeField] private TMP_Text _authorizeStatusText;
        [SerializeField] private TMP_Text _walletAddressText;

        public override async void Show(MonaModal modal, IEnumerator effectCoroutine, object options = null)
        {
            base.Show(modal, effectCoroutine, options);
            await SetWalletInformation();
        }

        private async Task SetWalletInformation()
        {
            if (!await MonaverseManager.Instance.SDK.IsWalletConnected())
            {
                _walletAddressText.text = "Wallet lost connection";
                await new WaitForSeconds(3f);
                parentModal.CloseView();
                return;
            }
            _walletAddressText.text = await MonaverseManager.Instance.SDK.ActiveWallet.GetAddress();
        }

        public async void AuthorizeWallet()
        {
            try
            {
                _authorizeStatusText.text = "Authorizing your wallet...";
                var result = await MonaverseManager.Instance.SDK.AuthorizeWallet(); 
                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Success, "Wallet Authorized");
            }
            catch (Exception exception)
            {
                MonaDebug.LogError("[MonaverseModal] AuthorizeWallet Exception: " + exception.Message);
            }
        }
    }
}