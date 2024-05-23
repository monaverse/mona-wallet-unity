using System;
using System.Collections;
using System.Threading.Tasks;
using Monaverse.Core;
using Monaverse.UI.Components;
using TMPro;
using UnityEngine;

namespace Monaverse.UI.Views
{
    public class ConnectingWalletView : MonaModalView
    {
        [SerializeField] private TMP_Text _connectStatusText;
        [SerializeField] private MonaModalView _authorizeWalletView;
        
        public override async void Show(MonaModal modal, IEnumerator effectCoroutine, object options = null)
        {
            if (options == null)
            {
                modal.CloseView();
                return;
            }
            
            base.Show(modal, effectCoroutine, options);
            
            var monaWalletConnection = (MonaWalletConnection)options;
            await ConnectWallet(monaWalletConnection);
        }

        private async Task ConnectWallet(MonaWalletConnection options)
        {
            try
            {
                var walletName = options.MonaWalletProvider switch
                {
                    MonaWalletProvider.WalletConnect => "WalletConnect",
                    MonaWalletProvider.LocalWallet => "Local Wallet",
                    _ => throw new ArgumentOutOfRangeException("Unexpected WalletProvider: " + options.MonaWalletProvider)
                };

                parentModal.Header.Title = walletName;
                _connectStatusText.text = $"Connecting to {walletName}...";

                await MonaverseManager.Instance.SDK.ConnectWallet(options);
                parentModal.OpenView(_authorizeWalletView);
            }
            catch (Exception exception)
            {
                parentModal.CloseView();
                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Connection Aborted");
                MonaDebug.LogError("[ConnectingWalletView] ConnectWallet Exception: " + exception.Message);
            }
        }
    }
}