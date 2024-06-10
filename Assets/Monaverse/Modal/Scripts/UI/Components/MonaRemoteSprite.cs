using System;
using System.Collections;
using System.Collections.Generic;
using Monaverse.Core.Utils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Monaverse.Modal.UI.Components
{
    public static class MonaRemoteSpriteFactory
    {
        private static readonly Dictionary<string, object> UriSpritesMap = new();

        public static MonaRemoteSprite GetRemoteSprite(string uri)
        {
            if(string.IsNullOrEmpty(uri))
                throw new ArgumentNullException(nameof(uri));
            
            if (UriSpritesMap.TryGetValue(uri, out var spriteObj) && spriteObj is MonaRemoteSprite sprite)
            {
                return sprite;
            }

            var newSprite = new MonaRemoteSprite(uri);
            UriSpritesMap[uri] = newSprite;
            return newSprite;
        }
    }
    
    
    public class MonaRemoteSprite
    {
        private readonly string _uri;
        private bool _isLoading;
        private Sprite _sprite;
        private bool _isLoaded;
        private readonly HashSet<Image> _subscribedImages = new();
        
        internal MonaRemoteSprite(string uri)
        {
            _uri = uri;
        }

        public void SubscribeImage(Image image)
        {
            if (!_isLoaded && !_isLoading)
                MonaUnityEventsDispatcher.Instance.StartCoroutine(LoadRemoteSprite());

            if (_isLoaded)
            {
                SetImage(image);
            }
            else
            {
               //Implement loading indicator
            }

            _subscribedImages.Add(image);
        }

        public void UnsubscribeImage(Image image)
        {
            image.sprite = null;
            _subscribedImages.Remove(image);
        }

        private void SetImage(Image image)
        {
            image.sprite = _sprite;
            image.color = Color.white;
        }
        
        private IEnumerator LoadRemoteSprite()
        {
            _isLoading = true;

            using (var uwr = UnityWebRequestTexture.GetTexture(_uri))
            {
                uwr.SetRequestHeader("accept", "image/jpeg,image/png");

                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to load remote sprite from {_uri}: {uwr.error}");
                }
                else
                {
                    // While UnityWebRequest creates texture in the background (on other thread), some finishing work is done on main thread.
                    // Skipping a few frames here to let Unity finish its work to reduce CPU spikes.
                    for (var i = 0; i < 5; i++)
                        yield return null;

                    var texture = DownloadHandlerTexture.GetContent(uwr);
                    var rect = new Rect(0.0f, 0.0f, texture.width, texture.height);
                    _sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100.0f);
                    _isLoaded = true;

                    foreach (var image in _subscribedImages)
                        SetImage(image);
                }
            }

            _isLoading = false;
        }
    }
}