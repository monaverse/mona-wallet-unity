using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Monaverse.Api.Modules.Collectibles.Dtos;
using Monaverse.Core;
using Monaverse.Core.Utils;
using Monaverse.Redcode.Awaiting;
using Monaverse.Modal.UI.Components;
using Monaverse.Modal.UI.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.Modal.UI.Views
{
    public class CollectiblesView : MonaModalView
    {
        [Header("Options")]
        [SerializeField] private bool _closeOnLoaded;
        
        [Header("Scene References")]
        [SerializeField] private RectTransform _parent;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private List<MonaListItem> _cardsPool = new();
        [SerializeField] private GameObject _noItemsFound;
        [SerializeField] private MonaWalletInfo _walletInfo;
        [SerializeField] private CollectibleDetailsView _collectiblesDetailsView;
        [SerializeField] private GameObject _loadingAnimator;
        
        [Header("Asset References")] [SerializeField]
        private MonaListItem _cardPrefab;

        [SerializeField] private int _countPerPage = 12;
        [SerializeField, Range(0.01f, 0.9f)]
        private float _loadThreshold = 0.5f;
        private readonly Dictionary<string, MonaRemoteSprite> _sprites = new();
        
        private List<CollectibleDto> _collectibles;
        private int _totalCount;

        private bool _isPageLoading = false;
        private int _maxCount;
        private int _nextPageToLoad = 1;
        private int _countPerPageRealtime = 0;
        private int _usedCardsCount = 0;
        private bool _reachedMaxItemCount = false;
        
        private void Start()
        {
            MonaverseManager.Instance.SDK.Disconnected += OnDisconnected;
        }

        public override void Show(MonaModal modal, IEnumerator effectCoroutine, object options = null)
        {
            base.Show(modal, effectCoroutine, options);
            _countPerPageRealtime = _countPerPage;
        }
        
        protected override async void OnOpened(object options = null)
        {
            _walletInfo.Show();
            await LoadNextPage();
        }
        
        public override void Hide()
        {
            base.Hide();

            StopAllCoroutines();

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
            _nextPageToLoad = 1;
            _isPageLoading = false;
            _reachedMaxItemCount = false;
        }
        
        private void FixedUpdate()
        {
            if (IsActive &&
                !_isPageLoading &&
                !_reachedMaxItemCount &&
                _scrollRect.verticalNormalizedPosition < _loadThreshold)
                _ = LoadNextPage();
        }
        
        private async Task LoadNextPage()
        {
            _isPageLoading = true;

            if (_maxCount != -1)
            {
                if (_nextPageToLoad * _countPerPageRealtime > _maxCount)
                {
                    _countPerPageRealtime = _maxCount - _usedCardsCount;
                    _reachedMaxItemCount = true;
                }
            }

            //Load cache if any
            if (_collectibles is { Count: > 0 })
            {
                await RefreshView(_collectibles);
            }
            else
            {
                _loadingAnimator.SetActive(true);
            }

            var getWalletCollectiblesResult = await MonaverseManager.Instance.SDK.ApiClient.Collectibles.GetWalletCollectibles();
            _loadingAnimator.SetActive(false);
            
            if(!IsActive)
                return;
            
            if (!getWalletCollectiblesResult.IsSuccess)
            {
                MonaDebug.LogError($"[CollectiblesView] Failed to get collectibles: {getWalletCollectiblesResult.Message}");
                return;
            }

            _totalCount = getWalletCollectiblesResult.Data.TotalCount;
            var collectiblePageCount = getWalletCollectiblesResult.Data.Data.Count;
            
            if (_maxCount == -1)
            {
                _maxCount = getWalletCollectiblesResult.Data.TotalCount;

                if (_nextPageToLoad * _countPerPageRealtime > _maxCount)
                {
                    _countPerPageRealtime = _maxCount - _usedCardsCount;
                    _reachedMaxItemCount = true;
                }
            }

            var collectibles = getWalletCollectiblesResult.Data.Data;
            await RefreshView(collectibles);
            
            //Filter using the CollectibleFilter
            MonaverseModal.TriggerCollectiblesLoaded(collectibles.GetFilteredCollectibles());

            _collectibles = collectibles;
            _usedCardsCount += collectiblePageCount;
            _nextPageToLoad++;

            _isPageLoading = false;

            if (_closeOnLoaded)
            {
                await new WaitForSeconds(2f);
                parentModal.CloseModal();
            }
        }
        
        public async Task RefreshView(List<CollectibleDto> collectibles)
        {
            parentModal.Header.Title = $"Collectibles ({_totalCount})";
            
            if (collectibles.Count > _cardsPool.Count - _usedCardsCount)
                await IncreaseCardsPoolSize(collectibles.Count + _usedCardsCount);
            
            //Sort using the CollectibleFilter if any
            if(MonaverseModal.Instance.CollectibleFilter != null)
                collectibles = collectibles.OrderBy(i=> i.CanBeImported() ? 0 : 1).ToList();

            for (var i = 0; i < collectibles.Count; i++)
            {
                var collectible = collectibles[i];
                var monaListItem = _cardsPool[i + _usedCardsCount];
                var imageUrl = collectible.GetImageUrl();
                var sprite = GetSprite(imageUrl);
                var canBeImported = collectible.CanBeImported();
                
                //configure details view
                var collectibleDetailsParams = new CollectibleDetailsView.CollectibleDetailsParams
                {
                    title = collectible.Title,
                    imageUrl = imageUrl,
                    typeText = collectible.Type,
                    description = collectible.Description,
                    tokenId = collectible.Nft.TokenId,
                    artist = collectible.Artist,
                    minted = collectible.Minted,
                    network = collectible.Nft.Network,
                    price = collectible.Price,
                    onImportClick = () =>
                    {
                        parentModal.CloseModal();
                        MonaverseModal.TriggerImportCollectibleClicked(collectible);
                    },
                    onPreviewClick = () =>
                    {
                        MonaverseModal.TriggerPreviewCollectibleClicked(collectible);
                    },
                    canImport = canBeImported
                };

                //configure list item
                monaListItem.Initialize(new MonaListItem.ListItemParams
                {
                    title = collectible.Title,
                    remoteSprite = sprite,
                    onClick = () =>
                    {
                        MonaverseModal.TriggerCollectibleSelected(collectible);
                        parentModal.OpenView(_collectiblesDetailsView, parameters: collectibleDetailsParams);
                    },
                    isInstalled = false,
                    isSupported = canBeImported
                });
            }
        }
        
        private MonaRemoteSprite GetSprite(string collectibleImageUrl)
        {
            if (_sprites.TryGetValue(collectibleImageUrl, out var sprite))
                return sprite;

            sprite = MonaRemoteSpriteFactory.GetRemoteSprite(collectibleImageUrl);
            _sprites.Add(collectibleImageUrl, sprite);
            return sprite;
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
        
        private void OnDisconnected(object sender, EventArgs e)
        {
            _collectibles = null;
            _totalCount = 0;
            
            if(!IsActive)
                return;
            
            parentModal.CloseView();
            parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Wallet Disconnected");
        }
    }
}