using System;
using Monaverse.Core;
using Monaverse.Modal.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.Modal.UI.Views
{
    public sealed class VerifyOtpView : MonaModalView
    {
        [SerializeField] private MonaModalView _getTokensView;
        [SerializeField] private TMP_InputField _otpInputField;
        [SerializeField] private TMP_Text _emailText;
        [SerializeField] private Button _verifyOtpButton;

        private string _emailAddress;
        
        private void Start()
        {
            _verifyOtpButton.onClick.AddListener(OnVerifyOtpButtonClicked);
            _otpInputField.onValueChanged.AddListener(OnOtpInputValueChanged);
        }

        private void OnOtpInputValueChanged(string otpInput)
        {
            _verifyOtpButton.interactable = !string.IsNullOrEmpty(otpInput);
        }

        protected override void OnOpened(object options = null)
        {
            if (options == null)
            {
                parentModal.CloseView();
                return;
            }
            
            _verifyOtpButton.interactable = false;
            _emailAddress = (string) options;
            _emailText.text = _emailAddress;
            _otpInputField.text = string.Empty;
        }

        private async void OnVerifyOtpButtonClicked()
        {
            try
            {
                _verifyOtpButton.interactable = false;
                var result = await MonaverseManager.Instance.SDK
                    .VerifyOneTimePassword(_emailAddress, _otpInputField.text);
                
                _verifyOtpButton.interactable = true;
                
                if (result)
                {
                    parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Success, "Login Successful");
                    parentModal.OpenView(_getTokensView);
                    return;
                }

                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Failed signing in");
            }
            catch (Exception exception)
            {
                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Error signing in");
                MonaDebug.LogException(exception);
            }
        }
    }
}