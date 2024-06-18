using System;
using Monaverse.Api.Modules.Auth.Requests;
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
        [SerializeField] private Button _verifyOtpButton;

        private string _emailAddress;
        
        private void Start()
        {
            _verifyOtpButton.onClick.AddListener(OnVerifyOtpButtonClicked);
        }
        
        protected override void OnOpened(object options = null)
        {
            _verifyOtpButton.interactable = true;
            _emailAddress = (string) options;
        }

        private async void OnVerifyOtpButtonClicked()
        {
            try
            {
                _verifyOtpButton.interactable = false;
                var result = await MonaverseManager.Instance.SDK.ApiClient.Auth
                    .VerifyOtp(new VerifyOtpRequest
                    {
                        Email = _emailAddress,
                        Otp = _otpInputField.text
                    });
                
                _verifyOtpButton.interactable = true;
                
                if (result.IsSuccess)
                {
                    parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Success, "Login Successful");
                    parentModal.OpenView(_getTokensView);
                    return;
                }

                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Failed signing in");
                MonaDebug.LogError("VerifyOtp Failed: " + result.Message);
            }
            catch (Exception exception)
            {
                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Error signing in");
                MonaDebug.LogException(exception);
            }
        }
    }
}