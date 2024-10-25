using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Monaverse.Api.Modules.User.Dtos;
using Monaverse.Api.Modules.User.Responses;
using Monaverse.Core;
using Monaverse.Core.Scripts.Utils;
using Monaverse.Core.Utils;
using Monaverse.Modal.UI.Components;
using Monaverse.Modal.UI.Extensions;
using Monaverse.Redcode.Awaiting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.Modal.UI.Views
{
    public class UserTokensView : MonaModalView
    {
        [Header("Options")]
        [SerializeField] private bool _closeOnLoaded;

        [Header("Scene References")]
        [SerializeField] private TMP_Dropdown _walletsDropdown;

        [SerializeField] private TMP_Dropdown _chainsDropdown;
        [SerializeField] private TMP_Text _usernameText;
        [SerializeField] private RectTransform _parent;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private List<MonaListItem> _cardsPool = new();
        [SerializeField] private GameObject _noItemsFound;
        [SerializeField] private TokenDetailsView _tokensDetailsView;
        [SerializeField] private MonaModalView _userProfileView;
        [SerializeField] private GameObject _loadingAnimator;
        [SerializeField] private Button _marketplaceButton;
        [SerializeField] private Button _profileButton;

        [Header("Asset References")] [SerializeField]
        private MonaListItem _cardPrefab;

        [SerializeField, Range(0.01f, 0.9f)]
        private float _loadThreshold = 0.2f;

        private readonly Dictionary<string, MonaRemoteSprite> _sprites = new();

        private List<TokenDto> _tokensCache = new();
        private string _continuationToken;
        private bool _isPageLoading = false;
        private int _usedCardsCount = 0;

        private GetUserResponse _user;

        private void Start()
        {
            _chainsDropdown.options.Clear();
            foreach (var chain in MonaverseManager.Instance.SDK.GetSupportedChainIds())
                _chainsDropdown.options.Add(
                    new TMP_Dropdown.OptionData(MonaverseManager.Instance.SDK.GetChainName(chain)));

            var defaultChainId = MonaverseManager.Instance.SDK.Session.DefaultChainId;
            if (defaultChainId > 0)
            {
                var defaultChainIndex = _chainsDropdown.options
                    .FindIndex(x =>
                        x.text == MonaverseManager.Instance.SDK.GetChainName(defaultChainId));

                if (defaultChainId != -1)
                    _chainsDropdown.value = defaultChainIndex;
            }

            _walletsDropdown.onValueChanged.AddListener(OnWalletsDropdownChanged);
            _chainsDropdown.onValueChanged.AddListener(OnChainsDropdownChanged);
            _marketplaceButton.onClick.AddListener(OnMarketplaceClicked);
            _profileButton.onClick.AddListener(OnProfileClicked);

            //Disable for iOS
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                _marketplaceButton.gameObject.SetActive(false);

            MonaverseManager.Instance.SDK.LoggedOut += OnLoggedOut;
        }

        protected override async void OnOpened(object options = null)
        {
            base.OnOpened(options);
            parentModal.Header.EnableBackButton(false);
            
            await GetUser();
            UpdateHeader();
            MonaverseModal.TriggerTokensViewOpened(_tokensCache.GetFilteredTokens());
        }

        public override void Hide()
        {
            base.Hide();
            StopAllCoroutines();
        }

        private void ResetCards(bool flushCache = false)
        {
            if(_cardsPool.Count == 0)
                return;
            
            for (var i = _cardsPool.Count - 1; i >= 0; i--)
            {
                var card = _cardsPool[i];
                if (card)
                {
                    card.ResetDefaults();
                    card.gameObject.SetActive(false);
                }
                else
                {
                    _cardsPool.RemoveAt(i);
                }
            }

            _usedCardsCount = 0;
            _isPageLoading = false;
            _continuationToken = null;

            if (flushCache)
                _tokensCache.Clear();
        }

        private void FixedUpdate()
        {
            if (IsActive &&
                !_isPageLoading &&
                !string.IsNullOrEmpty(_continuationToken) &&
                _scrollRect.verticalNormalizedPosition < _loadThreshold)
                _ = LoadPage();
        }

        private async Task LoadPage()
        {
            _isPageLoading = true;
            _loadingAnimator.SetActive(true);

            var getUserTokensResponse = await GetUserTokens();
            _loadingAnimator.SetActive(false);

            if (!IsActive)
                return;

            if (getUserTokensResponse == null)
                return;

            _noItemsFound.SetActive(getUserTokensResponse.Tokens.Count == 0);

            _continuationToken = getUserTokensResponse.Continuation;

            var tokens = getUserTokensResponse.Tokens;
            await RefreshView(tokens);

            //Filter using the CollectibleFilter
            MonaverseModal.TriggerTokensLoaded(tokens.GetFilteredTokens());

            _tokensCache.AddRange(tokens);
            _usedCardsCount += getUserTokensResponse.Tokens.Count;

            _isPageLoading = false;
            
            UpdateHeader();

            if (_closeOnLoaded)
            {
                await new WaitForSeconds(2f);
                parentModal.CloseModal();
            }
        }

        private async Task RefreshView(IReadOnlyList<TokenDto> tokens)
        {
            if (tokens.Count > _cardsPool.Count - _usedCardsCount)
                await IncreaseCardsPoolSize(tokens.Count + _usedCardsCount);

            //Sort using the TokenFilter if any
            if (MonaverseModal.Instance.Options.TokenFilter != null)
                tokens = tokens.OrderBy(i => i.CanBeImported() ? 0 : 1).ToList();

            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                var monaListItem = _cardsPool[i + _usedCardsCount];
                var sprite = token.GetImageRemoteSprite();
                var canBeImported = token.CanBeImported();

                //configure details view
                var collectibleDetailsParams = new TokenDetailsView.CollectibleDetailsParams
                {
                    title = token.Name,
                    imageUrl = token.Image,
                    typeText = token.Kind,
                    description = token.Description,
                    tokenId = int.TryParse(token.TokenId, out var tokenId) ? tokenId : 0,
                    artist = token.Collection.Name,
                    minted = true,
                    network = ChainHelper.GetChainName((int)token.ChainId),
                    price = (float)token.GetNativePrice(),
                    priceCurrency = token.GetCurrency(),
                    onImportClick = () =>
                    {
                        parentModal.CloseModal();
                        MonaverseModal.TriggerImportTokenClicked(token);
                    },
                    onPreviewClick = () => { MonaverseModal.TriggerPreviewCollectibleClicked(token); },
                    canImport = canBeImported
                };

                //configure list item
                monaListItem.Initialize(new MonaListItem.ListItemParams
                {
                    title = token.Name,
                    remoteSprite = sprite,
                    onClick = () =>
                    {
                        MonaverseModal.TriggerTokenSelected(token);
                        parentModal.OpenView(_tokensDetailsView, parameters: collectibleDetailsParams);
                    },
                    isInstalled = false,
                    isSupported = canBeImported
                });
            }
        }

        private async Task IncreaseCardsPoolSize(int newSize)
        {
            if (newSize <= _cardsPool.Count)
                throw new ArgumentException("New size must be greater than current size");

            var oldSize = _cardsPool.Count;
            _cardsPool.AddRange(new MonaListItem[newSize - oldSize]);

            for (var i = oldSize; i < newSize; i++)
            {
                var card = Instantiate(_cardPrefab, _parent);
                _cardsPool[i] = card;

                // After every 3 new cards, wait for a frame to reduce lag
                if ((i - oldSize + 1) % 3 == 0)
                    await new WaitForEndOfFrame();
            }
        }

        private async Task GetUser()
        {
            // Don't get user if we already have one
            if(_user != null)
                return;
            
            try
            {
                var result = await MonaverseManager.Instance.SDK.GetUser();

                if (!result.IsSuccess)
                {
                    parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Failed getting user");
                    MonaDebug.LogError("GetUser Failed: " + result.Message);
                    return;
                }

                _user = result.Data;
                _usernameText.text = $"@{_user.Username}";
                _walletsDropdown.ClearOptions();
                _walletsDropdown.AddOptions(_user.Wallets);

                await LoadPage();
            }
            catch (Exception exception)
            {
                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Error getting user");
                MonaDebug.LogException(exception);
            }
        }

        private async Task<GetUserTokensResponse> GetUserTokens()
        {
            try
            {
                var chainId = ChainHelper.GetChainId(_chainsDropdown.options[_chainsDropdown.value].text);
                if (chainId == 0)
                {
                    parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Chain not supported");
                    return null;
                }

                //Check if selected wallet exists in dropdown
                if (_walletsDropdown.options.Count <= _walletsDropdown.value)
                {
                    parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "No wallets linked");
                    return null;
                }
                
                var wallet = _walletsDropdown.options[_walletsDropdown.value].text;

                if (string.IsNullOrEmpty(wallet))
                {
                    parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Wallet not found");
                    return null;
                }

                var result = await MonaverseManager.Instance.SDK
                    .GetUserTokens(chainId: chainId,
                        address: wallet,
                        continuation: _continuationToken);

                if (!result.IsSuccess)
                {
                    parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Failed getting user tokens");
                    MonaDebug.LogError("GetUserTokens Failed: " + result.Message);
                    return result.Data;
                }

                if (result.Data.Tokens.Count == 0)
                {
                    parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Info, "No tokens found");
                    return result.Data;
                }

                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Success, $"{result.Data.Tokens.Count} tokens found");
                return result.Data;
            }
            catch (Exception exception)
            {
                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Error getting user");
                MonaDebug.LogException(exception);
                return null;
            }
        }

        private void UpdateHeader()
        {
            var chain = _chainsDropdown.options[_chainsDropdown.value].text;
            parentModal.Header.Title = $"Your {chain} Tokens ({_tokensCache?.Count})";
        }

        private async void OnChainsDropdownChanged(int chainIndex)
        {
            ResetCards(true);
            await LoadPage();
        }

        private async void OnWalletsDropdownChanged(int walletIndex)
        {
            ResetCards(true);
            await LoadPage();
        }

        private void OnMarketplaceClicked()
        {
            Application.OpenURL(MonaConstants.MonaversePages.Marketplace);
        }
        
        private void OnProfileClicked()
        {
            parentModal.OpenView(_userProfileView, parameters: new UserProfileView.UserProfileParams
            {
                Email = _user.Email,
                Name = _user.Name,
                Username = _user.Username
            });
        }

        private void OnLoggedOut(object sender, EventArgs e)
        {
            _user = null;
            ResetCards(true);

            if (!IsActive)
                return;

            parentModal.CloseView();
            parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Logged Out");
        }
    }
}