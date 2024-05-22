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
                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Success, "Wallet Connected");
                parentModal.OpenView(_authorizeWalletView);
            }
            catch (Exception exception)
            {
                MonaDebug.LogError("[MonaverseModal] SelectWalletConnect Exception: " + exception.Message);
            }
        }
    }
}