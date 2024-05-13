using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Monaverse.Core.Scripts.Wallets.Common;
using Monaverse.Wallets.Common;
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
            //TODO: Open WalletConnect Modal and await for connection result
            _namespace = WalletConnect.Instance.ActiveSession.Namespaces.First();

            return await GetAddress();
        }

        public Task<bool> Disconnect()
        {
            throw new System.NotImplementedException();
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