using System;
using Monaverse.Modal.UI.Components;
using Monaverse.Modal.UI.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.Modal.UI.Views
{
    public class TokenDetailsView : MonaModalView
    {
        [Header("Scene References")]
        [SerializeField] private Image _collectibleImage;
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private TMP_Text _descriptionLabel;
        [SerializeField] private TMP_Text _priceLabel;
        [SerializeField] private TMP_Text _typeLabel;
        [SerializeField] private TMP_Text _artistLabel;
        [SerializeField] private TMP_Text _networkLabel;
        [SerializeField] private TMP_Text _tokenIdLabel;
        [SerializeField] private GameObject _communityTag;

        [Header("Buttons")]
        [SerializeField] private Button _importButton;
        [SerializeField] private Button _previewButton;
        
        private MonaRemoteSprite _remoteSprite;
        
        private Action _onImportClick;
        private Action _onPreviewClick;
        
        public struct CollectibleDetailsParams
        {
            public string title;
            public string imageUrl;
            public string typeText;
            public string artist;
            public string network;
            public int tokenId;
            public bool minted;
            public decimal price;
            public string priceCurrency;
            public string description;
            public Action onImportClick;
            public Action onPreviewClick;
            public bool canImport;
            public bool isCommunityToken;
        }

        private void Start()
        {
            _importButton.onClick.AddListener(OnImportClick);
            _previewButton.onClick.AddListener(OnPreviewClick);
        }

        private void OnPreviewClick()
        {
            _onPreviewClick?.Invoke();
        }

        private void OnImportClick()
        {
            _onImportClick?.Invoke();
        }

        public void Initialize(in CollectibleDetailsParams parameters)
        {
            _remoteSprite = MonaRemoteSpriteFactory.GetRemoteSprite(parameters.imageUrl);
            _remoteSprite.SubscribeImage(_collectibleImage);
            _collectibleImage.color = Color.white;
            
            //Set details
            _titleLabel.text = parameters.title;
            _descriptionLabel.text = parameters.description;
            _priceLabel.text = $"{parameters.price.GetFormattedNativePrice()} {parameters.priceCurrency}";
            _typeLabel.text = parameters.typeText;
            _artistLabel.text = parameters.artist;
            _networkLabel.text = parameters.network;
            _tokenIdLabel.text = parameters.tokenId.ToString();
            _communityTag.SetActive(parameters.isCommunityToken);
            
            //Set buttons
            _onImportClick = parameters.onImportClick;
            _onPreviewClick = parameters.onPreviewClick;

            //Determines compatibility with the application
            _importButton.interactable = parameters.canImport;
        }

        public override void Hide()
        {
            base.Hide();
            _remoteSprite?.UnsubscribeImage(_collectibleImage);
        }

        protected override void OnOpened(object options = null)
        {
            base.OnOpened(options);
            if (options == null)
            {
                MonaDebug.LogError("No options were passed to this view. Please pass in a CollectibleDetailsParams object.");
                parentModal.CloseView();
                return;
            }
            
            var collectibleDetailsParams = (CollectibleDetailsParams) options;
            Initialize(collectibleDetailsParams);
        }
    }
}