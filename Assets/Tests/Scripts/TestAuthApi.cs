using Monaverse.Api;
using Nethereum.Signer;
using Nethereum.Web3.Accounts;
using UnityEngine;

public class TestAuthApi : MonoBehaviour
{
    [SerializeField] private string _privateKey;
    
    [ContextMenu("Test Authorize")]
    public async void TestAuthorize()
    {
        if(!Application.isPlaying)
        {
            Debug.LogError("Must be in play mode to test");
            return;
        }
        
        Debug.Log("Authorizing...");

        if (string.IsNullOrEmpty(_privateKey))
        {
            Debug.LogError("Missing private key");
            return;
        }

        //Create an EVM local wallet on the fly
        var account = new Account(_privateKey);
        var walletAddress = account.Address;
        
        //Initialize the Monaverse API client
        var monaApiClient = MonaApi.Init("FakeAppId");

        //Validate the wallet address
        var validateWalletAddress = await monaApiClient.Auth.ValidateWalletAddress(walletAddress);
        Debug.Log("ValidateWalletAddress: " + validateWalletAddress);
        
        if(!validateWalletAddress.IsSuccess)
            return;
        
        //Sign the message with the local wallet
        var signer = new EthereumMessageSigner();
        var signature = signer.EncodeUTF8AndSign(validateWalletAddress.SiweMessage, new EthECKey(_privateKey));
        
        //Authorize the wallet
        var authorize = await monaApiClient.Auth.Authorize(signature, validateWalletAddress.SiweMessage);
        Debug.Log("Authorize: " + authorize);}
}
