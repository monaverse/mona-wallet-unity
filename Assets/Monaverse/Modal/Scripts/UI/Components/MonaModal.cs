using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Monaverse.Redcode.Awaiting;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.Modal.UI.Components
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
        [SerializeField] private RectTransform _footerRectTransform;
        [field: SerializeField] public MonaModalHeader Header { get; private set; }
        [field: SerializeField] public MonaLoadingIcon LoadingIndicator { get; private set; }

        public bool IsOpen => _canvas.enabled;
        public event EventHandler Opened;
        public event EventHandler Closed;

        private readonly Stack<MonaModalView> _viewsStack = new();
        private readonly HashSet<MonaModalView> _cachedViews = new();
        private MonaModalDialog _currentDialog;
        private bool _hasGlobalBackground;
        private bool _resizingModal;

        public int ViewCount => _viewsStack.Count;

        private void Awake()
        {
            _hasGlobalBackground = _globalBackgroundCanvas != null;
            HandleConstantPhysicalSize();
        }

        private void HandleConstantPhysicalSize()
        {
            const float targetDPI = 160;

            // When using Game view instead of Device Simulator, you may want to change the target DPI for better scaling, e.g.:
// #if (UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS))
//             targetDPI = 96;
// #endif

            if (Screen.dpi != 0)
                _rootCanvasScaler.scaleFactor = Screen.dpi / targetDPI;
            else
                _rootCanvasScaler.scaleFactor = 1f;
        }

        public void OpenView(MonaModalView view, MonaModal modal = null, object parameters = null, bool removeSelf = false)
        {
            if (_viewsStack.Count == 0)
                EnableModal();

            if (_viewsStack.Count > 0)
            {
                if (removeSelf)
                {
                    var currentView = _viewsStack.Pop();
                    currentView.Hide();
                }
                
                //Loop through all views and hide them
                foreach (var existingView in _viewsStack)
                    existingView.Hide();
            }

            modal ??= this;
            CloseDialog();

            var resizeCoroutine = ResizeModalRoutine(view.GetViewHeight());
            _viewsStack.Push(view);
            view.Show(modal, resizeCoroutine, parameters);
            Header.Title = view.GetTitle();
            
            //Cache all existing views
            _cachedViews.Add(view);
        }
        
        public void OpenDialog(MonaModalDialog view, MonaModal modal = null, object parameters = null)
        {
            modal ??= this;
            view.Show(modal, parameters);
            _currentDialog = view;
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
        
        public void CloseDialog() => _currentDialog?.Hide();

        public void CloseModal()
        {
            if (_viewsStack.Count > 0)
            {
                var lastView = _viewsStack.Pop();
                lastView.Hide();
            }

            foreach (var cachedView in _cachedViews)
            {
                if(cachedView != null)
                    cachedView.OnModalClosed();
            }
            
            _cachedViews.Clear();
            _viewsStack.Clear();
            DisableModal();

            Closed?.Invoke(this, EventArgs.Empty);
        }

        public async Task CloseModalWithDelay(float delay)
        {
            await new WaitForSeconds(delay);
            CloseModal();
        }

        private IEnumerator ResizeModalRoutine(float targetHeight)
        {
            yield break;
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