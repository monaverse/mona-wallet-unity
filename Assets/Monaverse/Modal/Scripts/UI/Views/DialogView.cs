using System;
using Monaverse.Modal.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.Modal.UI.Views
{
    public sealed class DialogView : MonaModalDialog
    {
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private TMP_Text _descriptionLabel;
        [SerializeField] private TMP_Text _confirmActionLabel;
        [SerializeField] private TMP_Text _cancelActionLabel;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private string _defaultConfirmButtonTitle = "Confirm";
        [SerializeField] private string _defaultCancelButtonTitle = "Cancel";
        
        private Action _onConfirm;
        private Action _onCancel;
        
        public struct DialogParams
        {
            public string Title { get; set; }
            public string Message { get; set; }
            public string ConfirmButtonTitle { get; set; }
            public string CancelButtonTitle { get; set; }
            public Action OnConfirm { get; set; }
            public Action OnCancel { get; set; }
        }
        
        private void Start()
        {
            _confirmButton.onClick.AddListener(OnConfirmClick);
            _cancelButton.onClick.AddListener(OnCancelClick);
            Reset();
        }
        
        protected override void OnOpened(object options = null)
        {
            base.OnOpened(options);
            if (options == null)
            {
                MonaDebug.LogError($"No options were passed to this view. Please pass in a {nameof(DialogParams)} object.");
                parentModal.CloseView();
                return;
            }
            
            var dialogParams = (DialogParams)options;
            
            _titleLabel.text = dialogParams.Title;
            _descriptionLabel.text = dialogParams.Message;
            _confirmActionLabel.text = dialogParams.ConfirmButtonTitle ?? _defaultConfirmButtonTitle;
            _cancelActionLabel.text = dialogParams.CancelButtonTitle ?? _defaultCancelButtonTitle;
            _onConfirm = dialogParams.OnConfirm;
            _onCancel = dialogParams.OnCancel;
        }

        public override void Hide()
        {
            base.Hide();
            Reset();
        }

        private void Reset()
        {
            _titleLabel.text = string.Empty;
            _descriptionLabel.text = string.Empty;
            _confirmActionLabel.text = _defaultConfirmButtonTitle;
            _cancelActionLabel.text = _defaultCancelButtonTitle;
            _onConfirm = null;
            _onCancel = null;
        }

        private void OnCancelClick()
        {
            _onCancel?.Invoke();
            parentModal.CloseDialog();
        }

        private void OnConfirmClick()
        {
            _onConfirm?.Invoke();
            parentModal.CloseDialog();
        }
    }
}