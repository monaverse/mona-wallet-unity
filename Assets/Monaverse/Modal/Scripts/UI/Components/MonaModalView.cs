using System.Collections;
using Monaverse.Core;
using UnityEngine;

namespace Monaverse.Modal.UI.Components
{
    public class MonaModalView : MonoBehaviour
    {
        [Header("Scene References")] [SerializeField]
        private Canvas _canvas;
        [SerializeField] private string _title;
        [SerializeField] protected RectTransform rootTransform;

        protected MonaModal parentModal;
        protected MonaverseManager Manager => MonaverseManager.Instance;

        public virtual string GetTitle() => _title;
        public bool IsActive => _canvas.enabled;
        
        public virtual void Show(MonaModal modal, IEnumerator effectCoroutine, object options = null)
        {
            parentModal = modal;
            parentModal.Header.EnableBackButton(parentModal.ViewCount > 1);
            StartCoroutine(ShowAfterEffectRoutine(effectCoroutine, options));
        }
        
        protected virtual void OnOpened(object options = null) {}

        public virtual void Hide()
        {
            _canvas.enabled = false;
        }
        
        protected virtual IEnumerator ShowAfterEffectRoutine(IEnumerator effectCoroutine, object options)
        {
            yield return StartCoroutine(effectCoroutine);
            _canvas.enabled = true;
            OnOpened(options);
        }

        public float GetViewHeight()
        {
            return rootTransform.rect.height;
        }
    }
}