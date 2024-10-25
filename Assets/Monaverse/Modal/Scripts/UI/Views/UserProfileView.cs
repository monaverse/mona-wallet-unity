using System;
using Monaverse.Core;
using Monaverse.Modal.UI.Components;
using Monaverse.Modal.UI.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.Modal.UI.Views
{
    public sealed class UserProfileView : MonaModalView
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text _usernameLabel;

        [SerializeField] private TMP_Text _emailLabel;
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _initialsLabel;
        [SerializeField] private Button _openWebProfileButton;
        [SerializeField] private Button _logoutButton;
        [SerializeField] private Button _deleteAccountButton;

        public struct UserProfileParams
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
        }

        private UserProfileParams _userProfileParams;

        private void Start()
        {
            _openWebProfileButton.onClick.AddListener(OnOpenWebProfileClicked);
            _logoutButton.onClick.AddListener(OnLogoutClicked);
            _deleteAccountButton.onClick.AddListener(OnDeleteAccountClicked);

            MonaverseManager.Instance.SDK.LoggedOut += OnLoggedOut;
        }

        protected override void OnOpened(object options = null)
        {
            base.OnOpened(options);

            if (options == null)
            {
                MonaDebug.LogError($"No options were passed to this view. Please pass in a {nameof(UserProfileParams)} object.");
                parentModal.CloseView();
                return;
            }

            _userProfileParams = (UserProfileParams)options;

            _usernameLabel.text = $"@{_userProfileParams.Username}";
            _emailLabel.text = _userProfileParams.Email;
            _nameLabel.text = _userProfileParams.Name;
            _initialsLabel.text = _userProfileParams.Name.ToInitials();
        }

        private void OnOpenWebProfileClicked()
        {
            Application.OpenURL(MonaConstants.MonaversePages.ProfilePage(_userProfileParams.Username));
        }

        private void OnLogoutClicked()
        {
            MonaverseManager.Instance.SDK.Logout();
        }

        private void OnDeleteAccountClicked()
        {
            MonaverseModal.ShowDialog(title: "Confirm Account Deletion",
                message: "Your account will be deactivated immediately and scheduled for permanent deletion within 30 days.\n\nYou won't be able to sign in again. This action cannot be undone.",
                onConfirm: async () =>
                {
                    var result = await MonaverseManager.Instance.SDK.ApiClient.User
                        .DeleteAccount();

                    if (!result.IsSuccess)
                    {
                        MonaDebug.LogError("Failed deleting account: " + result.Message);
                        return;
                    }
                    
                    MonaDebug.Log("Account marked for deletion");
                    MonaverseManager.Instance.SDK.Logout();
                    parentModal.CloseModal();
                });
        }

        private void OnLoggedOut(object sender, EventArgs e)
        {
            if (!IsActive)
                return;

            parentModal.CloseModal();
            parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Logged Out");
        }
    }
}