namespace Monaverse.Api.Extensions
{
    public static class StringExtensions
    {
        public static string ToChecksumAddress(this string address)
        {
            return Nethereum.Util.AddressUtil.Current.ConvertToChecksumAddress(address);
        }
    }
}