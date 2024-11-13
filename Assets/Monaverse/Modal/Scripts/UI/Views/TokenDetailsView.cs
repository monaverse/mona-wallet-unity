using System;
using System.Collections.Generic;
using Monaverse.Modal.UI.Components;
using Monaverse.Modal.UI.Extensions;
using Monaverse.Modal.UI.Views.Elements;
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
        [SerializeField] private TMP_Text _supplyLabel;
        [SerializeField] private GameObject _communityTag;
        [SerializeField] private TokenAttributeViewElement _attributeTemplate;
        [SerializeField] private RectTransform _contentRectTransform;

        [Header("Buttons")]
        [SerializeField] private Button _importButton;
        [SerializeField] private Button _previewButton;
        
        private MonaRemoteSprite _remoteSprite;
        
        private Action _onImportClick;
        private Action _onPreviewClick;
        
        private List<TokenAttributeViewElement> _attributes = new();
        
        public class CollectibleDetailsParams
        {
            public string title;
            public string imageUrl;
            public string typeText;
            public string artist;
            public string network;
            public int tokenId;
            public int supply;
            public bool minted;
            public decimal price;
            public string priceCurrency;
            public string description;
            public Action onImportClick;
            public Action onPreviewClick;
            public bool canImport;
            public bool isCommunityToken;
            public List<(string Key, string Value, string Rarity)> attributes = new();
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
            _supplyLabel.text = parameters.supply.ToString();
            _communityTag.SetActive(parameters.isCommunityToken);
            
            //Set buttons
            _onImportClick = parameters.onImportClick;
            _onPreviewClick = parameters.onPreviewClick;

            //Determines compatibility with the application
            _importButton.interactable = parameters.canImport;

            foreach (var attributeViewElement in _attributes)
                Destroy(attributeViewElement.gameObject);
            
            _attributes.Clear();
            
            foreach (var attribute in parameters.attributes)
            {
                var attributeViewElement = Instantiate(_attributeTemplate, _attributeTemplate.transform.parent);
                attributeViewElement.gameObject.SetActive(true);
                attributeViewElement.Set(attribute.Key, attribute.Value, attribute.Rarity);
                _attributes.Add(attributeViewElement);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRectTransform);
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