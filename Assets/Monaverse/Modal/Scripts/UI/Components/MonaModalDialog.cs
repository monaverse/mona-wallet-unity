using System.Collections;
using UnityEngine;

namespace Monaverse.Modal.UI.Components
{
    public class MonaModalDialog : MonoBehaviour
    {
        [Header("Scene References")] [SerializeField]
        private Canvas _canvas;
        [SerializeField] protected RectTransform rootTransform;
        
        public bool IsActive => _canvas.enabled;

        
        protected MonaModal parentModal;

        public virtual void Show(MonaModal modal, object options = null)
        {
            parentModal = modal;
            _canvas.enabled = true;
            OnOpened(options);
        }
        
        protected virtual void OnOpened(object options = null) {}

        public virtual void Hide()
        {
            _canvas.enabled = false;
        }
    }
}