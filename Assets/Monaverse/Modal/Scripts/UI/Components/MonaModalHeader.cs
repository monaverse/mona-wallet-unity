using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Monaverse.Modal.UI.Components
{
    public class MonaModalHeader : MonoBehaviour
    {
        [field: SerializeField] public MonaSnackbar Snackbar { get; private set; }
        [field: SerializeField] public RectTransform RectTransform { get; private set; }
        [field: SerializeField] private TMP_Text TitleText { get; set; }
        [field: SerializeField] private Button LeftButton { get; set; }
        [field: SerializeField] private Image LeftButtonImage { get; set; }
        [field: SerializeField] private Button RightButton { get; set; }
        [field: SerializeField, Space] private MonaModal Modal { get; set; }

        public float Height => RectTransform.rect.height;

        private Sprite _leftButtonDefaultSprite;

        public string Title
        {
            get => TitleText.text;
            set => TitleText.text = value;
        }

        private void Awake()
        {
            Assert.IsNotNull(RectTransform, $"Missing {nameof(RectTransform)} reference in {name}");
            Assert.IsNotNull(TitleText, $"Missing {nameof(TitleText)} reference in {name}");
            Assert.IsNotNull(LeftButton, $"Missing {nameof(LeftButton)} reference in {name}");
            Assert.IsNotNull(RightButton, $"Missing {nameof(RightButton)} reference in {name}");

            Assert.IsNotNull(Modal, $"Missing {nameof(Modal)} reference in {name}");

            LeftButton.onClick.AddListener(OnLeftButtonClicked);
            RightButton.onClick.AddListener(OnRightButtonClicked);

            _leftButtonDefaultSprite = LeftButtonImage.sprite;
        }

        private void OnLeftButtonClicked()
        {
            Modal.CloseView();
        }

        private void OnRightButtonClicked()
        {
            Modal.CloseModal();
        }
        
        public void EnableBackButton(bool isEnabled) => LeftButton.gameObject.SetActive(isEnabled);
    }
}