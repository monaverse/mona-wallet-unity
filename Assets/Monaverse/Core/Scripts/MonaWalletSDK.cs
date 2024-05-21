using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Monaverse.Api;
using Monaverse.Api.Configuration;
using Monaverse.Api.Logging;
using Monaverse.Api.Modules.Auth.Responses;
using Monaverse.Api.Options;
using Monaverse.Wallets;
using UnityEngine;

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
            /// WalletConnect Project ID (https://cloud.walletconnect.com/app).
            /// </summary>
            public string walletConnectProjectId;
            
            /// <summary>
            /// The Monaverse API environment to use.
            /// </summary>
            public ApiEnvironment apiEnvironment;

            /// <summary>
            /// Whether to show the sdk debug logs
            /// </summary>
            public bool showDebugLogs;
        }
        
        public enum AuthorizationResult
        {
            WalletNotConnected,
            FailedValidatingWallet,
            UserNotRegistered,
            FailedSigningMessage,
            FailedAuthorizing,
            Authorized,
            Error
        }
        
        public SDKOptions Options { get; private set; }
        public BigInteger ChainId { get; private set; }
        public IMonaWallet ActiveWallet { get; private set; }
        public IMonaApiClient ApiClient { get; internal set; }
        public static SynchronizationContext UnitySyncContext { get; private set; }
        
        
        /// <summary>
        ///  Event raised when the user's wallet is connected
        ///  The wallet address is passed as a parameter
        /// </summary>
        public event EventHandler<string> Connected;
        
        /// <summary>
        /// Event raised when there is an error while connecting the user's wallet
        /// An exception is passed as a parameter
        /// </summary>
        public event EventHandler<Exception> ConnectionErrored;
        
        /// <summary>
        /// Event raised when the user's wallet is disconnected
        /// </summary>
        public event EventHandler Disconnected;
        
        /// <summary>
        /// Event raised when the user's wallet is authorized
        /// </summary>
        public event EventHandler Authorized;
        
        /// <summary>
        /// Event raised when the user's wallet is not authorized
        /// An authorization result is passed as a parameter
        /// </summary>
        public event EventHandler<AuthorizationResult> AuthorizationFailed;
        
        /// <summary>
        /// Event raised when there is an error while signing a message with the user's wallet
        /// An exception is passed as a parameter
        /// </summary>
        public event EventHandler<Exception> SignMessageErrored;
        
        
        public MonaWalletSDK(SDKOptions options)
        {
            Options = options;
            ApiClient = MonaApi.Init(new DefaultApiOptions
            {
                ApplicationId = options.applicationId,
                Environment = options.apiEnvironment,
                LogLevel = options.showDebugLogs? ApiLogLevel.Info : ApiLogLevel.Off
            });
            
            var currentSyncContext = SynchronizationContext.Current;
            if (currentSyncContext.GetType().FullName != "UnityEngine.UnitySynchronizationContext")
                throw new Exception(
                    $"[Monaverse] SynchronizationContext is not of type UnityEngine.UnitySynchronizationContext. Current type is <i>{currentSyncContext.GetType().FullName}</i>. Make sure to initialize the Monaverse SDK from the main thread.");
            UnitySyncContext = currentSyncContext;
        }
        
        /// <summary>
        /// Connects a user's Web3 wallet via WalletConnect.
        /// This will open the WalletConnect UI modal and let the user connect their Web3 wallet
        /// </summary>
        /// <returns> A task that completes when the wallet connection is complete.
        /// If successful, the task returns the address of the connected wallet.</returns>
        public Task<string> ConnectWallet()
        {
            return ConnectWallet(new MonaWalletConnection
            {
                ChainId = 1,
                MonaWalletProvider = MonaWalletProvider.WalletConnect
            });
        }
        
        /// <summary>
        /// Connects a user's Web3 wallet via a given wallet provider.
        /// </summary>
        /// <param name="monaWalletConnection">The wallet provider and optional parameters.</param>
        /// <returns>A task that completes when the wallet connection is complete.
        /// If successful, the task returns the address of the connected wallet.</returns>
        /// <exception cref="UnityException">Thrown if the wallet provider is not supported on this platform.</exception>
        public async Task<string> ConnectWallet(MonaWalletConnection monaWalletConnection)
        {
            ChainId = monaWalletConnection.ChainId;
            
            switch(monaWalletConnection.MonaWalletProvider) 
            {
                case MonaWalletProvider.LocalWallet:
                    ActiveWallet = new MonaLocalWallet();
                    break;
                case MonaWalletProvider.WalletConnect:
                    if (string.IsNullOrEmpty(Options.walletConnectProjectId))
                        throw new UnityException("Wallet connect project id is required for wallet connect connection method!");
                    ActiveWallet = new MonaWalletConnect(Options.walletConnectProjectId);
                    break;
                default: throw new UnityException($"{monaWalletConnection.MonaWalletProvider} not supported on this platform");
            }
            
            ActiveWallet.Connected += OnConnected;
            ActiveWallet.ConnectionErrored += OnConnectionErrored;
            ActiveWallet.Disconnected += OnDisconnected;
            ActiveWallet.SignMessageErrored += OnSignMessageErrored;
            
            var address = await ActiveWallet.Connect(monaWalletConnection);
            
            MonaDebug.Log($"Connected wallet {monaWalletConnection.MonaWalletProvider} with address {address} on chain {ChainId}");

            return address;
        }

        /// <summary>
        /// Disconnects the user's wallet and clears any active authorization
        /// </summary>
        public async Task Disconnect()
        {
            //Disconnect wallet
            if (ActiveWallet != null)
                await ActiveWallet.Disconnect();
            else
                MonaDebug.LogWarning("No active wallet detected, unable to disconnect.");
            
            //Clear session
            ApiClient.ClearSession();
        }

        /// <summary>
        /// Checks if a wallet is connected.
        /// </summary>
        /// <returns>True if a wallet is connected, false otherwise.</returns>
        public async Task<bool> IsWalletConnected()
        {
            if (ActiveWallet == null)
                return false;
            
            try
            {
                return await ActiveWallet.IsConnected();
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Returns true if the there is an active session with the Monaverse API.
        /// The session will remain authorized for 24 hours since the last time the user authorized their wallet.
        /// The session can be cleared using the Disconnect method
        /// The session can be cleared using the ApiClient.ClearSession method
        /// This must be true before you can call any authorized API endpoints
        /// </summary>
        /// <returns>True if the session is authorized, false otherwise.</returns>
        public bool IsWalletAuthorized()
        {
            return ApiClient.IsAuthorized();
        }

        /// <summary>
        /// This will authorize the user's wallet with the Monaverse platform.
        /// Before calling this, make sure:
        /// - The user is registered at Monaverse.com. You can validate if the user is registered using the ValidateWallet API endpoint.
        /// - The user has connected their wallet.
        /// </summary>
        /// <returns>AuthorizationResult enum with the following values:
        /// <see cref="AuthorizationResult"/>
        /// - WalletNotConnected: The user's wallet is not connected
        /// - FailedValidatingWallet: Failed validating the user's wallet
        /// - UserNotRegistered: The user is not registered
        /// - FailedSigningMessage: Failed signing the SIWE message
        /// - FailedAuthorizing: Failed authorizing the user's wallet with the Monaverse API
        /// - Authorized: The user's wallet is authorized and ready to use
        /// - Error: An unknown error occurred
        /// </returns>
        /// <exception cref="UnityException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<AuthorizationResult> AuthorizeWallet()
        {
            try
            {
                //Check if wallet is connected
                if (!await IsWalletConnected())
                {
                    MonaDebug.LogError("Wallet is not connected");
                    OnAuthorizationFailed(AuthorizationResult.WalletNotConnected);
                    return AuthorizationResult.WalletNotConnected;
                }
                
                //Get wallet address
                var address = await ActiveWallet.GetAddress();
                
                var validateWalletResponse = await ApiClient.Auth.ValidateWallet(address);
                if (!validateWalletResponse.IsSuccess)
                {
                    MonaDebug.LogError("Failed validating wallet: " + validateWalletResponse.Message);

                    
                    var result = validateWalletResponse.Data.Result switch
                    {
                        ValidateWalletResult.FailedGeneratingNonce => AuthorizationResult.FailedValidatingWallet,
                        ValidateWalletResult.WalletIsNotRegistered => AuthorizationResult.UserNotRegistered,
                        ValidateWalletResult.Error => AuthorizationResult.Error,
                        ValidateWalletResult.WalletIsValid => throw new UnityException("Unexpected ValidateWalletResult: WalletIsValid"),
                        _ => throw new ArgumentOutOfRangeException(" Unexpected ValidateWalletResult: " + validateWalletResponse.Data.Result)
                    };

                    OnAuthorizationFailed(result);
                    return result;
                }
                
                //Sign message with the user's active wallet provider
                var signature = await ActiveWallet.SignMessage(validateWalletResponse.Data.SiweMessage);
                if (string.IsNullOrEmpty(signature))
                {
                    MonaDebug.LogError($"Failed signing message with {ActiveWallet.GetProvider().ToString()}");
                    OnAuthorizationFailed(AuthorizationResult.FailedSigningMessage);
                    return AuthorizationResult.FailedSigningMessage;
                }
                
                //Authorize wallet
                var authorizationResult = await ApiClient.Auth.Authorize(signature, validateWalletResponse.Data.SiweMessage);
                if (!authorizationResult.IsSuccess)
                {
                    MonaDebug.LogError($"Failed authorizing wallet {authorizationResult.Message}");
                    OnAuthorizationFailed(AuthorizationResult.FailedAuthorizing);
                    return AuthorizationResult.FailedAuthorizing;
                }
                
                OnAuthorized();
                
                return AuthorizationResult.Authorized;
            }
            catch (Exception exception)
            {
               MonaDebug.LogException(exception);
               OnAuthorizationFailed(AuthorizationResult.Error);
               return AuthorizationResult.Error;
            }
        }


        #region Events Handlers

        private void OnConnected(object sender, string connectedEvent)
        {
            UnitySyncContext.Post(_ =>
            {
                Connected?.Invoke(this, connectedEvent);
            }, null);
        }
        
        private void OnDisconnected(object sender, EventArgs e)
        {
            UnitySyncContext.Post(_ =>
            {
                Disconnected?.Invoke(this, e);
            }, null);
        }
        
        private void OnAuthorized()
        {
            UnitySyncContext.Post(_ =>
            {
                Authorized?.Invoke(this, EventArgs.Empty);
            }, null);
        }

        private void OnAuthorizationFailed(AuthorizationResult authorizationResult)
        {
            UnitySyncContext.Post(_ =>
            {
                AuthorizationFailed?.Invoke(this, authorizationResult);
            }, null);
        }
        
        private void OnSignMessageErrored(object sender, Exception exception)
        {
            UnitySyncContext.Post(_ =>
            {
                SignMessageErrored?.Invoke(this, exception);
            }, null);
        }

        private void OnConnectionErrored(object sender, Exception exception)
        {
            UnitySyncContext.Post(_ =>
            {
                ConnectionErrored?.Invoke(this, exception);
            }, null);
        }

        #endregion
    }
}