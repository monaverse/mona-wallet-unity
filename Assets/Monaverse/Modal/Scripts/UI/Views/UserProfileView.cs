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

        public struct UserProfileParams
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
        }
        
        private string _username;

        private void Start()
        {
            _openWebProfileButton.onClick.AddListener(OnOpenWebProfileClicked);
            _logoutButton.onClick.AddListener(OnLogoutClicked);
            
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
            
            var userProfileParams = (UserProfileParams)options;
            
            _username = userProfileParams.Username;

            _usernameLabel.text = $"@{_username}";
            _emailLabel.text = userProfileParams.Email;
            _nameLabel.text = userProfileParams.Name;
            _initialsLabel.text = userProfileParams.Name.ToInitials();
        }
        
        private void OnOpenWebProfileClicked()
        {
            if(string.IsNullOrEmpty(_username))
                return;
            
            Application.OpenURL(MonaConstants.MonaversePages.ProfilePage(_username));
        }
        
        private void OnLogoutClicked()
        {
            MonaverseManager.Instance.SDK.Logout();
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