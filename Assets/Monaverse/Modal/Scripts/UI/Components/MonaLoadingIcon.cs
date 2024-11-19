using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.Modal.UI.Components
{
    [RequireComponent(typeof(Image))]
    public class MonaLoadingIcon : MonoBehaviour
    {
        // Rotation speed in degrees per second
        [SerializeField] private float _rotationSpeed = 300f;

        // Flag to control rotation
        [SerializeField] private bool _isRotating = false;
        
        // Reference to optional Button component
        [SerializeField] private Button _linkedButton;

        private Image _iconRenderer;
        private void Start()
        {
            _iconRenderer = GetComponent<Image>();
            UpdateVisibility();

            if (_linkedButton != null)
            {
                _linkedButton.onClick.AddListener(OnButtonClicked);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_isRotating)
            {
                return;
            }

            var rotationAmount = _rotationSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, -rotationAmount);
            
            // Automatically stop rotation if the button becomes interactable
            if (_linkedButton != null && _linkedButton.interactable && _isRotating)
            {
                SetRotation(false);
            }
        }
        
        // Public method to start or stop the rotation
        public void SetRotation(bool shouldRotate)
        {
            _isRotating = shouldRotate;
            UpdateVisibility();
        }
        
        public void Play() => SetRotation(true);
        public void Pause() => SetRotation(false);

        // Update the visibility of the icon based on the rotation state
        private void UpdateVisibility()
        {
            if (_iconRenderer != null)
                _iconRenderer.enabled = _isRotating;
        }
        
        // Method called when the linked button is clicked
        private void OnButtonClicked()
        {
            if (_linkedButton != null)
                SetRotation(true);
        }
    }
}