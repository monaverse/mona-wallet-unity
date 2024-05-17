using System;
using System.Numerics;
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

        public MonaWalletSDK(SDKOptions options)
        {
            Options = options;
            ApiClient = MonaApi.Init(new DefaultApiOptions
            {
                ApplicationId = options.applicationId,
                Environment = options.apiEnvironment,
                LogLevel = options.showDebugLogs? ApiLogLevel.Info : ApiLogLevel.Off
            });
        }
        
        public Task<string> ConnectWallet()
        {
            return ConnectWallet(new MonaWalletConnection
            {
                ChainId = 1,
                MonaWalletProvider = MonaWalletProvider.WalletConnect
            });
        }
        
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
            
            await ActiveWallet.Connect(monaWalletConnection);
            
            var address = await ActiveWallet.GetAddress();
            
            MonaDebug.Log($"Connected wallet {monaWalletConnection.MonaWalletProvider} with address {address} on chain {ChainId}");

            return address;
        }
        
        
        /// <summary>
        /// Disconnects the user's wallet.
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
        /// Returns true if the there is an active session with the Monaverse
        /// </summary>
        /// <returns></returns>
        public bool IsWalletAuthorized()
        {
            return ApiClient.IsAuthorized();
        }

        public async Task<AuthorizationResult> AuthorizeWallet()
        {
            try
            {
                //Check if wallet is connected
                if (!await IsWalletConnected())
                {
                    MonaDebug.LogError("Wallet is not connected");
                    return AuthorizationResult.WalletNotConnected;
                }
                
                //Get wallet address
                var address = await ActiveWallet.GetAddress();
                
                var validateWalletResponse = await ApiClient.Auth.ValidateWallet(address);
                if (!validateWalletResponse.IsSuccess)
                {
                    MonaDebug.LogError("Failed validating wallet: " + validateWalletResponse.Message);

                    return validateWalletResponse.Data.Result switch
                    {
                        ValidateWalletResult.FailedGeneratingNonce => AuthorizationResult.FailedValidatingWallet,
                        ValidateWalletResult.WalletIsNotRegistered => AuthorizationResult.UserNotRegistered,
                        ValidateWalletResult.Error => AuthorizationResult.Error,
                        ValidateWalletResult.WalletIsValid => throw new UnityException("Unexpected ValidateWalletResult: WalletIsValid"),
                        _ => throw new ArgumentOutOfRangeException(" Unexpected ValidateWalletResult: " + validateWalletResponse.Data.Result)
                    };
                }
                
                //Sign message with the user's active wallet provider
                var signature = await ActiveWallet.SignMessage(validateWalletResponse.Data.SiweMessage);
                if (string.IsNullOrEmpty(signature))
                {
                    MonaDebug.LogError($"Failed signing message with {ActiveWallet.GetProvider().ToString()}");
                    return AuthorizationResult.FailedSigningMessage;
                }
                
                //Authorize wallet
                var authorizationResult = await ApiClient.Auth.Authorize(signature, validateWalletResponse.Data.SiweMessage);
                if (!authorizationResult.IsSuccess)
                {
                    MonaDebug.LogError($"Failed authorizing wallet {authorizationResult.Message}");
                    return AuthorizationResult.FailedAuthorizing;
                }
                
                return AuthorizationResult.Authorized;
            }
            catch (Exception exception)
            {
               MonaDebug.LogException(exception);
               return AuthorizationResult.Error;
            }
        }
    }
}