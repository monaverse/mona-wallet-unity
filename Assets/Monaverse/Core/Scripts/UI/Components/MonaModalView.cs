using System.Collections;
using UnityEngine;

namespace Monaverse.UI.Components
{
    public class MonaModalView : MonoBehaviour
    {
        [Header("Scene References")] [SerializeField]
        private Canvas _canvas;
        [SerializeField] private string _title;
        [SerializeField] protected RectTransform rootTransform;

        protected MonaModal parentModal;

        public virtual string GetTitle() => _title;

        public virtual void Show(MonaModal modal, IEnumerator effectCoroutine, object options = null)
        {
            parentModal = modal;
            parentModal.Header.EnableBackButton(parentModal.ViewCount > 1);
            StartCoroutine(ShowAfterEffectRoutine(effectCoroutine));
        }

        public virtual void Hide()
        {
            _canvas.enabled = false;
        }
        
        protected virtual IEnumerator ShowAfterEffectRoutine(IEnumerator effectCoroutine)
        {
            yield return StartCoroutine(effectCoroutine);
            _canvas.enabled = true;
        }

        public float GetViewHeight()
        {
            return rootTransform.rect.height;
        }
    }
}