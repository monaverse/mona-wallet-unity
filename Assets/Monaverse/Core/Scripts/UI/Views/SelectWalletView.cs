using System;
using Monaverse.Core;
using Monaverse.UI.Components;
using UnityEngine;

namespace Monaverse.UI.Views
{
    public class SelectWalletView : MonaModalView
    {
        [SerializeField] private MonaModalView _connectingWalletView;
        
        public void SelectWalletConnect()
        {
            try
            {
                parentModal.OpenView(_connectingWalletView, parameters: new MonaWalletConnection
                {
                    ChainId = 1,
                    MonaWalletProvider = MonaWalletProvider.WalletConnect
                });
            }
            catch (Exception exception)
            {
                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Something went wrong");
                MonaDebug.LogError("[MonaverseModal] SelectWalletConnect Exception: " + exception.Message);
            }
        }
    }
}