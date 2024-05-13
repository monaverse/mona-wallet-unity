using System;
using System.Numerics;
using System.Threading.Tasks;
using Monaverse.Api;
using Monaverse.Api.Configuration;
using Monaverse.Api.Modules.Auth.Responses;
using Monaverse.Core.Scripts.Wallets.Common;
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
            ApiClient = MonaApi.Init(options.applicationId);
        }
        
        public async Task<string> Connect(MonaWalletConnection monaWalletConnection)
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
            if (ActiveWallet != null)
                await ActiveWallet.Disconnect();
            else
                MonaDebug.LogWarning("No active wallet detected, unable to disconnect.");
        }

        /// <summary>
        /// Checks if a wallet is connected.
        /// </summary>
        /// <returns>True if a wallet is connected, false otherwise.</returns>
        public async Task<bool> IsConnected()
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
        public bool IsAuthorized()
        {
            return ApiClient.IsAuthorized();
        }

        public async Task<AuthorizationResult> AuthorizeWallet()
        {
            try
            {
                //Check if wallet is connected
                if (!await IsConnected())
                {
                    MonaDebug.LogError("Wallet is not connected");
                    return AuthorizationResult.WalletNotConnected;
                }
                
                //Get wallet address
                var address = await ActiveWallet.GetAddress();
                
                var validateWalletResponse = await ApiClient.Auth.ValidateWallet(address);
                if (!validateWalletResponse.IsSuccess)
                {
                    MonaDebug.LogError(validateWalletResponse.Message);
                    return AuthorizationResult.FailedValidatingWallet;
                }
                
                //Check if wallet is registered in Monaverse.com
                if (validateWalletResponse.Data.Result == ValidateWalletResult.WalletIsNotRegistered)
                {
                    MonaDebug.LogError(validateWalletResponse.Message);
                    return AuthorizationResult.UserNotRegistered;
                }
                
                //if wallet is not valid at this point, there must be an error generating nonce
                if (validateWalletResponse.Data.Result != ValidateWalletResult.WalletIsValid)
                {
                    MonaDebug.LogError(validateWalletResponse.Message);
                    return AuthorizationResult.FailedValidatingWallet;
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