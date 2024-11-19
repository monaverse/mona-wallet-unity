using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.Modal.UI.Components
{
    public class MonaListItem : MonoBehaviour
    {
        [SerializeField] private Button _defaultViewButton;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _price;
        [SerializeField] private TMP_Text _supply;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _iconBorder;
        [SerializeField] private GameObject _defaultViewObject;
        [SerializeField] private Color _defaultBorderColor;
        [SerializeField] private GameObject _communityTag;

        public bool IsCommunityToken { get; set; }
        
        private MonaModalView _targetView;
        private object _targetViewParameters;
        private Action _onClick;
        private MonaRemoteSprite _remoteSprite;
        private bool _initialized;

        protected void Awake()
        {
            _defaultViewButton.onClick.AddListener(OnClick);
        }
        
        public void Initialize(in ListItemParams parameters)
        {
            if (_initialized)
            {
                ResetDefaults();
            }

            _title.text = parameters.title;
            _price.text = parameters.price;
            _supply.text = parameters.supply;
            _onClick = parameters.onClick;
            _remoteSprite = parameters.remoteSprite;
            IsCommunityToken = parameters.IsCommunityToken;
            _communityTag.SetActive(IsCommunityToken);
            _defaultViewButton.interactable = parameters.IsInteractable;

            _iconBorder.color = parameters.borderColor == default
                ? _defaultBorderColor
                : parameters.borderColor;

            if (parameters.remoteSprite == null)
            {
                _icon.sprite = parameters.sprite;
                _icon.color = Color.white;
            }
            else
            {
                parameters.remoteSprite.SubscribeImage(_icon);
            }

            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            if (parameters.isInstalled)
                EnableDefaultView();
            
            _initialized = true;
        }
        
        public void OnClick()
        {
            _onClick?.Invoke();
        }
        
        private void EnableDefaultView()
        {
            _defaultViewObject.SetActive(true);
        }
        
        public void ResetDefaults()
        {
            _remoteSprite?.UnsubscribeImage(_icon);
            _icon.color = new Color(1, 1, 1, 0.1f);
            _title.text = string.Empty;
            _defaultViewObject.SetActive(false);
        }
        
        public struct ListItemParams
        {
            public MonaRemoteSprite remoteSprite;
            public Sprite sprite; // used if remoteSprite is null
            public string title;
            public Action onClick;
            public Color borderColor;
            public bool isInstalled;
            public bool IsCommunityToken;
            public string price;
            public string supply;
            public bool IsInteractable;
        }
    }
}