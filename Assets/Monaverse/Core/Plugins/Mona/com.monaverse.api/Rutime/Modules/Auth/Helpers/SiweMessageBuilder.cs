using Nethereum.Siwe.Core;

namespace Monaverse.Api.Modules.Auth
{
    public static class SiweMessageBuilder
    {
        public static string BuildMessage(string domain,
            string address,
            string nonce)
        {
            var siweMessage = new SiweMessage
            {
                Domain = domain,
                Address = address,
                Nonce = nonce,
                Version = "1",
                ChainId = "1",
                Statement = "Sign in with Ethereum to the app.",
                Uri = $"https://{domain}",
            };

            siweMessage.SetIssuedAtNow();

            return SiweMessageStringBuilder.BuildMessage(siweMessage);
        }
    }
}