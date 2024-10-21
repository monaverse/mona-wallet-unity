using System;
using Monaverse.Core;
using Monaverse.Core.Scripts.Utils;
using Monaverse.Modal.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.Modal.UI.Views
{
    public class GenerateOtpView : MonaModalView
    {
        [SerializeField] private MonaModalView _verifyOtpView;
        [SerializeField] private MonaModalView _userTokensView;
        [SerializeField] private MonaModalView _signupView;
        [SerializeField] private TMP_InputField _emailInputField;
        [SerializeField] private Button _generateOtpButton;
        [SerializeField] private Button _createAccountButton;

        private void Start()
        {
            _generateOtpButton.onClick.AddListener(OnGenerateOtpButtonClicked);
            _createAccountButton.onClick.AddListener(OnCreateAccountButtonClicked);
            _emailInputField.onValueChanged.AddListener(OnEmailInputValueChanged);
        }

        private void OnCreateAccountButtonClicked()
        {
            parentModal.OpenView(_signupView);
        }

        private void OnEmailInputValueChanged(string email)
        {
            _generateOtpButton.interactable = email.IsEmailValid();
        }

        protected override void OnOpened(object options = null)
        {
            if (MonaverseManager.Instance.SDK.IsAuthenticated())
            {
                parentModal.OpenView(_userTokensView);
                return;
            }
            
            _generateOtpButton.interactable = _emailInputField.text.IsEmailValid();
        }

        private async void OnGenerateOtpButtonClicked()
        {
            try
            {
                _generateOtpButton.interactable = false;

                var result = await MonaverseManager.Instance.SDK
                    .GenerateOneTimePassword(_emailInputField.text);

                _generateOtpButton.interactable = true;
                
                if (result)
                {
                    parentModal.OpenView(_verifyOtpView, parameters: _emailInputField.text);
                    return;
                }

                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Failed to generate OTP");
            }
            catch (Exception exception)
            {
                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Failed to generate OTP");
                MonaDebug.LogException(exception);
            }
        }
    }
}