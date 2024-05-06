namespace Monaverse.Api.Modules.Auth.Responses
{
    public enum ValidateWalletResult
    {
        WalletIsValid,
        FailedGeneratingNonce,
        WalletIsNotRegistered,
        Error
    }
    
    public sealed record ValidateWalletAddressResponse(ValidateWalletResult Result, 
        string SiweMessage = null,
        string ErrorMessage = null)
    {
        public bool IsSuccess => Result == ValidateWalletResult.WalletIsValid;
        public ValidateWalletResult Result { get; set; } = Result;
        public string SiweMessage { get; set; } = SiweMessage;
        public string ErrorMessage { get; set; } = ErrorMessage;
    }
}