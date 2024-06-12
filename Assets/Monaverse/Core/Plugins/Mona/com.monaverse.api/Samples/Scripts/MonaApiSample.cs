using Monaverse.Api.Modules.Auth.Requests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monaverse.Api.Samples
{
    public class MonaApiSample : MonoBehaviour
    {
        [SerializeField] private string _applicationId;
        
        [Header("Scene References")]
        [SerializeField] private TMP_InputField _emailInputField; 
        [SerializeField] private TMP_InputField _otpInputField; 
        [SerializeField] private TMP_InputField _refreshTokenInputField; 
        [SerializeField] private TMP_InputField _accessTokenInputField; 
        [SerializeField] private TMP_Text _generateOtpResultText;
        [SerializeField] private TMP_Text _verifyOtpResultText;
        [SerializeField] private TMP_Text _refreshTokenResultText;

        [SerializeField] private Button _generateOtpButton;
        [SerializeField] private Button _verifyOtpButton;
        [SerializeField] private Button _refreshTokenButton;

        private IMonaApiClient _monaverseApi;
        
        private void Start()
        {

            _monaverseApi = MonaApi.Init(_applicationId);
            
            _generateOtpButton.onClick.AddListener(GenerateOtp);
            _verifyOtpButton.onClick.AddListener(VerifyOtp);
            _refreshTokenButton.onClick.AddListener(RefreshToken);
        }
        
        private async void GenerateOtp()
        {
            var request = new GenerateOtpRequest
            {
                Email = _emailInputField.text
            };
            var result = await _monaverseApi.Auth.GenerateOtp(request);
            _generateOtpResultText.text = result.IsSuccess? "Success " : "Failed " + result.Message;
        }
        
        private async void VerifyOtp()
        {
            var request = new VerifyOtpRequest
            {
                Email = _emailInputField.text,
                Otp = _otpInputField.text
            };
            var result = await _monaverseApi.Auth.VerifyOtp(request);
            _verifyOtpResultText.text = result.IsSuccess? "Success " : "Failed " + result.Message;

            if (!result.IsSuccess) return;
            
            _accessTokenInputField.text = result.Data.Access;
            _refreshTokenInputField.text = result.Data.Refresh;
        }
        
        private async void RefreshToken()
        {
            var request = new RefreshTokenRequest
            {
                Refresh = _refreshTokenInputField.text
            };
            var result = await _monaverseApi.Auth.RefreshToken(request);
            _refreshTokenResultText.text = result.IsSuccess? "Success " : "Failed " + result.Message;
            
            if (!result.IsSuccess) return;
            
            _accessTokenInputField.text = result.Data.Access;
            _refreshTokenInputField.text = result.Data.Refresh;
        }
    }
}