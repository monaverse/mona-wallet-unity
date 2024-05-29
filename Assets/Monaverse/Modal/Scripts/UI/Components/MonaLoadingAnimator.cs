using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.Modal.UI.Components
{
    public class MonaLoadingAnimator : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private Color _fromColor;
        [SerializeField] private Color _toColor;
        
        private void Start()
        {
            _image.color = _fromColor;
        }

        //Rotate the image
        private void Update()
        {
            //Lerp the image alpha
            _image.color = Color.Lerp(_fromColor, _toColor, Mathf.PingPong(Time.time * _speed, 1));;
        }
    }
}