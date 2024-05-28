using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Monaverse.Core;
using Monaverse.Core.Utils;
using Monaverse.Redcode.Awaiting;
using Monaverse.Modal.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.Modal.UI.Views
{
    public class CollectiblesView : MonaModalView
    {
        [Header("Scene References")]
        [SerializeField] private RectTransform _parent;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private List<MonaListItem> _cardsPool = new();
        [SerializeField] private GameObject _noItemsFound;
        [SerializeField] private MonaWalletInfo _walletInfo;
        
        [Header("Asset References")] [SerializeField]
        private MonaListItem _cardPrefab;

        [SerializeField] private int _countPerPage = 12;
        [SerializeField, Range(0.01f, 0.9f)]
        private float _loadThreshold = 0.5f;
        private readonly Dictionary<string, MonaRemoteSprite> _sprites = new();

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

        public override async void Show(MonaModal modal, IEnumerator effectCoroutine, object options = null)
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

            var result = await MonaverseManager.Instance.SDK.ApiClient.Collectibles.GetWalletCollectibles();

            if (!result.IsSuccess)
            {
                MonaDebug.LogError($"[CollectiblesView] Failed to get collectibles: {result.Message}");
                return;
            }

            var collectiblesTotalCount = result.Data.TotalCount;
            var collectiblePageCount = result.Data.Data.Count;
            
            parentModal.Header.Title = $"Collectibles ({collectiblesTotalCount})";
            
            _noItemsFound.SetActive(collectiblesTotalCount == 0);

            if (_maxCount == -1)
            {
                _maxCount = result.Data.TotalCount;

                if (_nextPageToLoad * _countPerPageRealtime > _maxCount)
                {
                    _countPerPageRealtime = _maxCount - _usedCardsCount;
                    _reachedMaxItemCount = true;
                }
            }

            var collectibles = result.Data.Data;

            if (collectiblePageCount > _cardsPool.Count - _usedCardsCount)
                await IncreaseCardsPoolSize(collectiblePageCount + _usedCardsCount);

            for (var i = 0; i < collectiblePageCount; i++)
            {
                var collectible = collectibles[i];
                var card = _cardsPool[i + _usedCardsCount];
                var sprite = GetSprite(collectible.GetImageUrl());

                card.Initialize(new MonaListItem.ListItemParams
                {
                    title = collectible.Title,
                    remoteSprite = sprite,
                    onClick = () =>
                    {
                        Debug.Log("Clicked " + collectible.Title);
                    },
                    isInstalled = false
                });
            }

            _usedCardsCount += collectiblePageCount;
            _nextPageToLoad++;

            _isPageLoading = false;
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
            if(!IsActive)
                return;
            
            parentModal.CloseView();
            parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Success, "Wallet Disconnected");
        }
    }
}