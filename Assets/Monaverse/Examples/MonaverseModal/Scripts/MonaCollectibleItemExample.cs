using System;
using Monaverse.Api.Modules.Collectibles.Dtos;
using Monaverse.Core.Utils;
using Monaverse.Modal.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.Examples
{
    public class MonaCollectibleItemExample : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Button _viewButton;
        
        private MonaRemoteSprite _remoteSprite;
        private Action _onClick;

        private void Start()
        {
            if(_viewButton != null)
                _viewButton.onClick.AddListener(OnClickButton);
        }

        private void OnClickButton()
        {
            _onClick?.Invoke();
        }

        public void SetCollectible(CollectibleDto collectible, Action onClick = null)
        {
            _onClick = onClick;
            _remoteSprite?.UnsubscribeImage(_image);
            
            _remoteSprite = MonaRemoteSpriteFactory.GetRemoteSprite(collectible.GetImageUrl());
            _remoteSprite.SubscribeImage(_image);
            _title.text = collectible.Title;
        }
    }
}