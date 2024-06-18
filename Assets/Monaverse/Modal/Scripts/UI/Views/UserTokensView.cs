using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Monaverse.Api.Modules.User.Responses;
using Monaverse.Core;
using Monaverse.Modal.UI.Components;
using TMPro;
using UnityEngine;

namespace Monaverse.Modal.UI.Views
{
    public class UserTokensView : MonaModalView
    {
        [SerializeField] private TMP_Dropdown _walletsDropdown;
        [SerializeField] private TMP_Dropdown _chainsDropdown;
        [SerializeField] private TMP_Text _usernameText;
        [SerializeField] private TMP_Text _emailText;

        private GetUserResponse _user;

        private readonly Dictionary<string, BigInteger> _chains = new()
        {
            { "Ethereum", 1 },
            { "Polygon", 137 },
            { "Arbitrum", 42161 },
            { "Optimism", 10 },
            { "Base", 8453 },
        };

        private void Start()
        {
            _chainsDropdown.options.Clear();
            foreach (var chain in _chains)
                _chainsDropdown.options.Add(new TMP_Dropdown.OptionData(chain.Key));

            _walletsDropdown.onValueChanged.AddListener(OnWalletsDropdownChanged);
            _chainsDropdown.onValueChanged.AddListener(OnChainsDropdownChanged);
        }

        protected override async void OnOpened(object options = null)
        {
            base.OnOpened(options);
            await GetUser();
        }

        private async Task GetUser()
        {
            try
            {
                var result = await MonaverseManager.Instance.SDK.ApiClient.User
                    .GetUser();

                if (!result.IsSuccess)
                {
                    parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Failed getting user");
                    MonaDebug.LogError("GetUser Failed: " + result.Message);
                    return;
                }

                _user = result.Data;
                _usernameText.text = $"@{_user.Username}";
                _emailText.text = _user.Email;
                _walletsDropdown.ClearOptions();
                _walletsDropdown.AddOptions(_user.Wallets);
                
                await GetUserTokens();
            }
            catch (Exception exception)
            {
                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Error getting user");
                MonaDebug.LogException(exception);
            }
        }

        private async Task GetUserTokens()
        {
            try
            {
                if (!_chains.TryGetValue(_chainsDropdown.options[_chainsDropdown.value].text, out var chain))
                {
                    parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Chain error");
                    return;
                }

                var wallet = _walletsDropdown.options[_walletsDropdown.value].text;

                if (string.IsNullOrEmpty(wallet))
                {
                    parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Wallet not found");
                    return;
                }

                var result = await MonaverseManager.Instance.SDK.ApiClient.User
                    .GetUserTokens(chainId: (int)chain,
                        address: wallet);
                
                
                if (!result.IsSuccess)
                {
                    parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Failed getting user tokens");
                    MonaDebug.LogError("GetUserTokens Failed: " + result.Message);
                    return;
                }   
                
                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Success, "Tokens pulled!");
            }
            catch (Exception exception)
            {
                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Error getting user");
                MonaDebug.LogException(exception);
            }
        }

        private async void OnChainsDropdownChanged(int chainIndex)
        {
            var chain = _chainsDropdown.options[chainIndex].text;
            Debug.Log($" {chain} - {_chains[chain]}");
            await GetUserTokens();
        }

        private async void OnWalletsDropdownChanged(int walletIndex)
        {
            var wallet = _walletsDropdown.options[walletIndex].text;
            Debug.Log(wallet);
            await GetUserTokens();
        }
    }
}