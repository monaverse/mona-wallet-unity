namespace Monaverse.Api.Modules.Auth.Responses
{
    public enum ValidateWalletResult
    {
        WalletIsValid,
        FailedGeneratingNonce,
        WalletIsNotRegistered,
        Error
    }
    
    public sealed record ValidateWalletResponse(ValidateWalletResult Result, 
        string SiweMessage = null,
        string ErrorMessage = null)
    {
        public bool IsValid => Result == ValidateWalletResult.WalletIsValid;
        public ValidateWalletResult Result { get; set; } = Result;
        public string SiweMessage { get; set; } = SiweMessage;
        public string ErrorMessage { get; set; } = ErrorMessage;
    }
}