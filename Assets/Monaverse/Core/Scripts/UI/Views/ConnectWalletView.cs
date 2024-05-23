using System;
using Monaverse.Core;
using Monaverse.UI.Components;
using UnityEngine;

namespace Monaverse.UI.Views
{
    public class ConnectWalletView : MonaModalView
    {
        [SerializeField] private MonaModalView _authorizeWalletView;
        
        public async void SelectWalletConnect()
        {
            try
            {
                await MonaverseManager.Instance.SDK.ConnectWallet();
                parentModal.OpenView(_authorizeWalletView);
                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Success, "Wallet Connected");
            }
            catch (Exception exception)
            {
                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Wallet Connection Failed");
                MonaDebug.LogError("[MonaverseModal] SelectWalletConnect Exception: " + exception.Message);
            }
        }
    }
}