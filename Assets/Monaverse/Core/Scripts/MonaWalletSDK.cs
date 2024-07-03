using System;
using System.Collections.Generic;
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
using Monaverse.Core.Utils;

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
            
            /// <summary>
            /// 
            /// </summary>
            public SupportedChainId defaultChain;
        }
        
        public enum SupportedChainId
        {
            Ethereum = 1,
            Polygon = 137,
            Arbitrum = 42161,
            Optimism = 10,
            Base = 8453
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

            Session.SaveDefaultChainId((int)options.defaultChain);
            
            ApiClient.Session.OnClearSession += ()=>
            {
                Session.Clear();
                OnLoggedOut();
            };
            
            var currentSyncContext = SynchronizationContext.Current;
            if (currentSyncContext.GetType().FullName != "UnityEngine.UnitySynchronizationContext")
                throw new Exception(
                    $"[Monaverse] SynchronizationContext is not of type UnityEngine.UnitySynchronizationContext. Current type is <i>{currentSyncContext.GetType().FullName}</i>. Make sure to initialize the Monaverse SDK from the main thread.");
            UnitySyncContext = currentSyncContext;
        }

        /// <summary>
        /// Generates a one time password and sends it to the provided email
        /// If the user is not registered, an email asking for registration will be sent
        /// </summary>
        /// <param name="email"> The email to send the one time password to </param>
        /// <returns> true if the operation was successful </returns>
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
        
        /// <summary>
        /// Verifies a one time password
        /// if successful, the user will be logged in and the session will be saved
        /// Once the session is saved, the Authenticated event will be raised
        /// </summary>
        /// <param name="email"> The registered email </param>
        /// <param name="otp"> The one time password to verify </param>
        /// <returns> true if the operation was successful </returns>
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

        /// <summary>
        /// Gets the currently logged-in user
        /// If the user is not authenticated, an error will be raised
        /// </summary>
        /// <returns> The user response object </returns>
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
        
        /// <summary>
        /// Gets the user's tokens from the Monaverse API for a specific chain and wallet address
        /// The wallet address supplied must be owned by the user
        /// You can link your wallet address to your account in https://marketplace.monaverse.com/
        /// </summary>
        /// <param name="chainId"> The id of the chain to get the tokens for</param>
        /// <param name="address"> The wallet address to get the tokens for</param>
        /// <returns> The user's tokens for the specified chain and wallet address </returns>
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

        #region Helpers

        /// <summary>
        /// Returns a list of supported chain ids by the Monaverse API
        /// </summary>
        /// <returns></returns>
        public HashSet<int> GetSupportedChainIds()
        {
            return ChainHelper.SupportedChains();
        }
        
        /// <summary>
        /// Returns the name of the chain with the specified chain id
        /// </summary>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public string GetChainName(int chainId)
        {
            return ChainHelper.GetChainName(chainId);
        }

        #endregion
        
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