using System;
using System.Threading.Tasks;
using Monaverse.Core;
using Nethereum.Signer;
using Nethereum.Web3.Accounts;
using Nethereum.Web3;

namespace Monaverse.Wallets
{
    public sealed class MonaLocalWallet : IMonaWallet
    {
        private readonly string _privateKey;
        private readonly MonaWalletProvider _walletProvider;
        private Account _account;
        private Web3 _web3;

        public MonaLocalWallet(string privateKey = null)
        {
            _privateKey = privateKey;
            if (string.IsNullOrEmpty(privateKey))
            {
                var generatedKey = EthECKey.GenerateKey();
                var bytes = generatedKey.GetPrivateKeyAsBytes();
                _privateKey = BitConverter.ToString(bytes).Replace("-", "");
            }

            _walletProvider = MonaWalletProvider.LocalWallet;
        }

        public async Task<string> Connect(MonaWalletConnection monaWalletConnection)
        {
            _account = new Account(_privateKey);
            _web3 = new Web3(_account);
            return await GetAddress();
        }

        public Task<bool> Disconnect()
        {
            _web3 = null;
            _account = null;

            return Task.FromResult(true);
        }

        public async Task<string> GetAddress()
        {
            if (!await IsConnected())
                return null;

            return _account.Address;
        }

        public async Task<string> SignMessage(string message)
        {
            if (!await IsConnected())
                return null;

            var signer = new EthereumMessageSigner();

            var signature = signer.EncodeUTF8AndSign(message, new EthECKey(_account.PrivateKey));

            if (string.IsNullOrEmpty(signature))
            {
                MonaDebug.LogError("Signature is null or empty.");
                return null;
            }

            return signature;
        }

        public Task<bool> IsConnected() => Task.FromResult(_web3 != null);

        public MonaWalletProvider GetProvider()=> _walletProvider;
    }
}