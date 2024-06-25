using System;
using Monaverse.Api.Modules.User.Dtos;
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

        public void SetCollectible(TokenDto collectible, Action onClick = null)
        {
            _onClick = onClick;
            _remoteSprite?.UnsubscribeImage(_image);
            
            _remoteSprite = MonaRemoteSpriteFactory.GetRemoteSprite(collectible.Image);
            _remoteSprite.SubscribeImage(_image);
            _title.text = collectible.Name;
        }
    }
}