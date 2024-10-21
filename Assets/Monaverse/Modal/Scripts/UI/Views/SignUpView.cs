using System;
using Monaverse.Core.Scripts.Utils;
using Monaverse.Modal.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.Modal.UI.Views
{
    public class SignUpView : MonaModalView
    {
        [SerializeField] private MonaModalView _verifyOtpView;
        [SerializeField] private TMP_InputField _emailInputField;
        [SerializeField] private TMP_InputField _usernameInputField;
        [SerializeField] private TMP_InputField _nameInputField;
        [SerializeField] private Button _signUpButton;
        [SerializeField] private Button _signInButton;

        private void Start()
        {
            _signUpButton.onClick.AddListener(OnSignUpButtonClicked);
            _signInButton.onClick.AddListener(OnSignInButtonClicked);
            
            _emailInputField.onValueChanged.AddListener(OnEmailInputValueChanged);
            _usernameInputField.onValueChanged.AddListener(OnUsernameInputValueChanged);
            _nameInputField.onValueChanged.AddListener(OnNameInputValueChanged);
        }

        protected override void OnOpened(object options = null)
        {
            ValidateForm();
        }

        private async void OnSignUpButtonClicked()
        {
            try
            {
                _signUpButton.interactable = false;

                var result = await Manager.SDK
                    .SignUp(email: _emailInputField.text,
                        username: _usernameInputField.text,
                        name: _nameInputField.text);

                _signUpButton.interactable = true;

                if (result.IsSuccess)
                {
                    ResetForm();
                    parentModal.OpenView(_verifyOtpView, parameters: _emailInputField.text, removeSelf: true);
                    return;
                }

                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, 
                    result.StatusCode == 409 ? "Email or username already exists" : "Signup error");

                MonaDebug.LogError($"SignUp Failed: {result.StatusCode} {result.Message}");
            }
            catch (Exception exception)
            {
                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Signup failed");
                MonaDebug.LogException(exception);
            }
        }
        
        private void OnSignInButtonClicked() => parentModal.CloseView();
        
        private void ResetForm()
        {
            _emailInputField.text = "";
            _usernameInputField.text = "";
            _nameInputField.text = "";
        }

        private void ValidateForm()
        {
            _signUpButton.interactable = _emailInputField.text.IsEmailValid()
                                         && _usernameInputField.text.Length > 0
                                         && _nameInputField.text.Length > 0;
        }

        private void OnEmailInputValueChanged(string arg0) => ValidateForm();
        private void OnUsernameInputValueChanged(string arg0)=> ValidateForm();
        private void OnNameInputValueChanged(string arg0)=> ValidateForm();
    }
}