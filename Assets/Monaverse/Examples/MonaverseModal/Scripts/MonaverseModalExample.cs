using Monaverse.Modal;
using UnityEngine;

namespace Monaverse.Examples
{
    public class MonaverseModalExample : MonoBehaviour
    {
        private void Start()
        {
            OpenModal();
        }
        
        [ContextMenu("Open Modal")]
        public void OpenModal()
        {
            MonaverseModal.Open();
        }
    }
}