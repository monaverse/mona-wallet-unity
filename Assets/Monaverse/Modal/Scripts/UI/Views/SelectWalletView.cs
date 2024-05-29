using System;
using Monaverse.Core;
using Monaverse.Modal.UI.Components;
using UnityEngine;

namespace Monaverse.Modal.UI.Views
{
    public class SelectWalletView : MonaModalView
    {
        [SerializeField] private MonaModalView _connectingWalletView;
        [SerializeField] private MonaModalView _collectiblesView;

        protected override void OnOpened(object options = null)
        {
            if(MonaverseManager.Instance.SDK.IsWalletAuthorized())
                parentModal.OpenView(_collectiblesView);
        }

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