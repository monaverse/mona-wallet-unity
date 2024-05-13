namespace Monaverse.Wallets.Common
{
    public static class WalletExtensions
    {
        public static string ToChecksumAddress(this string address)
        {
            return Nethereum.Util.AddressUtil.Current.ConvertToChecksumAddress(address);
        }
    }
}