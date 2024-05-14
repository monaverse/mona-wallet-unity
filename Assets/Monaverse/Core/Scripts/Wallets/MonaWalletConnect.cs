using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Monaverse.Core;
using Monaverse.Redcode.Awaiting;
using Monaverse.Wallets.Common;
using UnityEngine;
using WalletConnectSharp.Sign.Models;
using WalletConnectUnity.Core;
using WalletConnectUnity.Core.Evm;

namespace Monaverse.Wallets
{
    public sealed class MonaWalletConnect : IMonaWallet
    {
        private readonly string _walletConnectProjectId;
        private KeyValuePair<string, Namespace> _namespace;

        public MonaWalletConnect(string walletConnectProjectId)
        {
            _walletConnectProjectId = walletConnectProjectId;
        }

        public async Task<string> Connect(MonaWalletConnection monaWalletConnection)
        {
            if (MonaWalletConnectUI.Instance == null)
            {
                GameObject.Instantiate(MonaverseManager.Instance.WalletConnectPrefab);
                await new WaitForSeconds(0.5f);
            }
            
            await MonaWalletConnectUI.Instance.Connect(monaWalletConnection.ChainId);
            _namespace = WalletConnect.Instance.ActiveSession.Namespaces.First();

            return await GetAddress();
        }

        public async Task<bool> Disconnect()
        {
            try
            {
                await WalletConnect.Instance.DisconnectAsync();
                return true;
            }
            catch (Exception e)
            {
                MonaDebug.LogWarning($"Error disconnecting WalletConnect: {e.Message}");
                return false;
            }
        }

        public Task<string> GetAddress()
        {
            var ethAccs = new [] { WalletConnect.Instance.ActiveSession.CurrentAddress(_namespace.Key).Address };
            var addy = ethAccs[0];
            if (addy != null)
                addy = addy.ToChecksumAddress();
            return Task.FromResult(addy);
        }

        public async Task<string> SignMessage(string message)
        {
            var address = await GetAddress();
            var data = new PersonalSign(message, address);
            var signature = await WalletConnect.Instance.RequestAsync<PersonalSign, string>(data);
            return signature;
        }

        public Task<bool> IsConnected()
        {
            return Task.FromResult(WalletConnect.Instance.IsConnected);
        }

        public MonaWalletProvider GetProvider() => MonaWalletProvider.WalletConnect;
    }
}