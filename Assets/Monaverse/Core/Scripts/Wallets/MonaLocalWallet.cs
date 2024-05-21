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
            try
            {
                _account = new Account(_privateKey);
                _web3 = new Web3(_account);
            
                var address = await GetAddress();
                Connected?.Invoke(this, address);
                return address;
            }
            catch (Exception exception)
            {
                ConnectionErrored?.Invoke(this, exception);
                throw;
            }
        }

        public Task<bool> Disconnect()
        {
            _web3 = null;
            _account = null;

            Disconnected?.Invoke(this, EventArgs.Empty);
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
            try
            {
                if (!await IsConnected())
                    throw new Exception("Wallet not connected");

                var signer = new EthereumMessageSigner();
                return signer.EncodeUTF8AndSign(message, new EthECKey(_account.PrivateKey));
            }
            catch (Exception exception)
            {
                SignMessageErrored?.Invoke(this, exception);
                throw;
            }
        }

        public Task<bool> IsConnected() => Task.FromResult(_web3 != null);

        public MonaWalletProvider GetProvider()=> _walletProvider;
        public event EventHandler<string> Connected;
        public event EventHandler Disconnected;
        public event EventHandler<Exception> ConnectionErrored;
        public event EventHandler<Exception> SignMessageErrored;
    }
}