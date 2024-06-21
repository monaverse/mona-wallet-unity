using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Monaverse.Api;
using Monaverse.Api.Configuration;
using Monaverse.Api.Logging;
using Monaverse.Api.Modules.Auth.Requests;
using Monaverse.Api.Modules.Common;
using Monaverse.Api.Modules.User.Responses;
using Monaverse.Api.Options;

namespace Monaverse.Core
{
    /// <summary>
    /// The entry point for the Monaverse Wallet SDK
    /// </summary>
    public sealed class MonaWalletSDK
    {
        public struct SDKOptions
        {
            /// <summary>
            /// Monaverse Application ID.
            /// </summary>
            public string applicationId;
            
            /// <summary>
            /// The Monaverse API environment to use.
            /// </summary>
            public ApiEnvironment apiEnvironment;

            /// <summary>
            /// Whether to show the sdk debug logs
            /// </summary>
            public bool showDebugLogs;
        }
        
        public SDKOptions Options { get; private set; }
        public IMonaApiClient ApiClient { get; internal set; }
        public MonaverseSession Session { get; private set; }
        
        public static SynchronizationContext UnitySyncContext { get; private set; }
        
        /// <summary>
        /// Event raised when the user is logged out
        /// </summary>
        public event EventHandler LoggedOut;

        /// <summary>
        /// Event raised when the user is authenticated with the Monaverse API
        /// </summary>
        public event EventHandler Authenticated;
        
        /// <summary>
        /// Event raised when the user's wallet is not authorized
        /// An authorization result is passed as a parameter
        /// </summary>
        public event EventHandler<string> AuthenticationFailed;

        
        public MonaWalletSDK(SDKOptions options)
        {
            Options = options;
            ApiClient = MonaApi.Init(new DefaultApiOptions
            {
                ApplicationId = options.applicationId,
                Environment = options.apiEnvironment,
                LogLevel = options.showDebugLogs? ApiLogLevel.Info : ApiLogLevel.Off
            });
            
            Session = new MonaverseSession(ApiClient.Session.AccessToken, ApiClient.Session.RefreshToken);
            Session.Load();
            
            var currentSyncContext = SynchronizationContext.Current;
            if (currentSyncContext.GetType().FullName != "UnityEngine.UnitySynchronizationContext")
                throw new Exception(
                    $"[Monaverse] SynchronizationContext is not of type UnityEngine.UnitySynchronizationContext. Current type is <i>{currentSyncContext.GetType().FullName}</i>. Make sure to initialize the Monaverse SDK from the main thread.");
            UnitySyncContext = currentSyncContext;
        }

        public async Task<bool> GenerateOneTimePassword(string email)
        {
            try
            {
                var result = await ApiClient.Auth
                    .GenerateOtp(new GenerateOtpRequest
                    {
                        Email = email
                    });
                
                return result.IsSuccess;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return false;
            }
        }
        
        public async Task<bool> VerifyOneTimePassword(string email, string otp)
        {
            try
            {
                var result = await ApiClient.Auth
                    .VerifyOtp(new VerifyOtpRequest
                    {
                        Email = email,
                        Otp = otp
                    });

                if (!result.IsSuccess)
                {
                    OnAuthenticationFailed(result.Message);
                    return false;
                }
                
                Session.SaveSession(accessToken: result.Data.Access, 
                    refreshToken: result.Data.Refresh,
                    emailAddress: email);
                
                OnAuthenticated();
                
                return true;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                OnAuthenticationFailed(exception.Message);
                return false;
            }
        }

        public async Task<ApiResult<GetUserResponse>> GetUser()
        {
            try
            {
                if (!IsAuthenticated())
                    return ApiResult<GetUserResponse>.Failed("Not authenticated");   
                
                var result = await ApiClient.User
                    .GetUser();

                if (!result.IsSuccess) return result;
                
                Session.Wallets = result.Data.Wallets.ToHashSet();
                Session.SaveSessionEmail(result.Data.Email);

                return result;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult<GetUserResponse>.Failed(exception.Message);
            }
        }
        
        public async Task<ApiResult<GetUserTokensResponse>> GetUserTokens(int chainId, string address)
        {
            try
            {
                if (!IsAuthenticated())
                    return ApiResult<GetUserTokensResponse>.Failed("Not authenticated");                    
                
                var result = await ApiClient.User
                    .GetUserTokens(chainId: chainId,
                        address: address);

               //TODO: Do some caching here
                
                return result;
            }
            catch (Exception exception)
            {
                MonaDebug.LogException(exception);
                return ApiResult<GetUserTokensResponse>.Failed(exception.Message);
            }
        }

        /// <summary>
        /// Logs the user out of the Monaverse API
        /// It clears any stored credentials
        /// </summary>
        public void Logout()
        {
            //Clear session
            ApiClient.Session.ClearSession();
            Session.Clear();
            
            OnLoggedOut();
        }
        
        /// <summary>
        /// Returns true if the there is an active session with the Monaverse API.
        /// This must be true before you can call any authenticated API endpoints
        /// </summary>
        /// <returns></returns>
        public bool IsAuthenticated()
        {
            return Session.IsAuthenticated;
        }

        #region Events Handlers

        private void OnAuthenticated()
        {
            UnitySyncContext.Post(_ =>
            {
                Authenticated?.Invoke(this, EventArgs.Empty);
            }, null);
        }
        
        private void OnLoggedOut()
        {
            UnitySyncContext.Post(_ =>
            {
                LoggedOut?.Invoke(this, EventArgs.Empty);
            }, null);
        }

        private void OnAuthenticationFailed(string errorMessage)
        {
            UnitySyncContext.Post(_ =>
            {
                AuthenticationFailed?.Invoke(this, errorMessage);
            }, null);
        }

        #endregion
    }
}