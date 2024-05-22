using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.UI.Components
{
    public class MonaModal : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Canvas _canvas;

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectTransform _rootRectTransform;
        [SerializeField] private CanvasScaler _rootCanvasScaler;
        [SerializeField] private Canvas _globalBackgroundCanvas;
        [SerializeField] private Image _modalMaskImage;
        [SerializeField] private Image _modalBorderImage;
        [field: SerializeField] public MonaModalHeader Header { get; private set; }
        
        [Header("Settings")]
        [SerializeField, Range(0, 1)] private float _mobileMaxHeightPercent = 0.8f;

        public bool IsOpen => _canvas.enabled;
        public event EventHandler Opened;
        public event EventHandler Closed;
        
        private readonly Stack<MonaModalView> _viewsStack = new();
        private bool _hasGlobalBackground;
        private bool _resizingModal;

        public void OpenView(MonaModalView view, MonaModal modal = null, object parameters = null)
        {
            if (_viewsStack.Count == 0)
                EnableModal();

            if (_viewsStack.Count > 0)
                _viewsStack.Peek().Hide();

            modal ??= this;


            var resizeCoroutine = ResizeModalRoutine(view.GetViewHeight());
            _viewsStack.Push(view);
            view.Show(modal, resizeCoroutine, parameters);

            Header.Title = view.GetTitle();
        }

        public void CloseView()
        {
            if (_viewsStack.Count <= 0) return;

            var currentView = _viewsStack.Pop();
            currentView.Hide();

            if (_viewsStack.Count > 0)
            {
                var nextView = _viewsStack.Peek();
                Header.Title = nextView.GetTitle();
                var resizeCoroutine = ResizeModalRoutine(nextView.GetViewHeight());
                nextView.Show(this, resizeCoroutine);
            }
            else
            {
                DisableModal();
            }
        }

        public void CloseModal()
        {
            if (_viewsStack.Count > 0)
            {
                var lastView = _viewsStack.Pop();
                lastView.Hide();
            }

            _viewsStack.Clear();
            DisableModal();

            Closed?.Invoke(this, EventArgs.Empty);
        }
        
        public IEnumerator ResizeModalRoutine(float targetHeight)
        {
            if (_resizingModal) yield break;
            _resizingModal = true;

            targetHeight = targetHeight + Header.Height + 12;

#if UNITY_ANDROID || UNITY_IOS
            if (DeviceUtils.GetDeviceType() == DeviceType.Phone)
                targetHeight += 8;
#endif

            var rootTransformSizeDelta = _rectTransform.sizeDelta;
            var originalHeight = rootTransformSizeDelta.y;
            var elapsedTime = 0f;
            var duration = .25f; // TODO: serialize this

            targetHeight = Mathf.Min(targetHeight, _rootRectTransform.sizeDelta.y * _mobileMaxHeightPercent);

            while (elapsedTime < duration)
            {
                var lerp = Mathf.Lerp(originalHeight, targetHeight, elapsedTime / duration);
                _rectTransform.sizeDelta = new Vector2(rootTransformSizeDelta.x, lerp);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _rectTransform.sizeDelta = new Vector2(rootTransformSizeDelta.x, targetHeight);
            _resizingModal = false;
        }
        
        private void EnableModal()
        {
            _canvas.enabled = true;

            if (_hasGlobalBackground)
                _globalBackgroundCanvas.enabled = true;

            Opened?.Invoke(this, EventArgs.Empty);
        }
        
        private void DisableModal()
        {
            _canvas.enabled = false;

            if (_hasGlobalBackground)
                _globalBackgroundCanvas.enabled = false;
        }
    }
}