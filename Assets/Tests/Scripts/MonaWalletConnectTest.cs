using System.Linq;
using Monaverse.Api;
using Monaverse.Api.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WalletConnectUnity.Core;
using WalletConnectUnity.Core.Evm;
using WalletConnectUnity.Modal;

public class MonaWalletConnectTest : MonoBehaviour
{
    [SerializeField] private Button _disconnectButton;
    [SerializeField] private Button _postNonceButton;
    [SerializeField] private Button _postAuthorizeButton;
    [SerializeField] private TMP_Text _resultLabel;
    
    private void Awake()
    {
        MonaApi.Init("fakeAppId");
        
        WalletConnect.Instance.ActiveSessionChanged += (_, @struct) =>
        {
            _disconnectButton.interactable = true;
            _postNonceButton.interactable = true;
            _postAuthorizeButton.interactable = true;

            _resultLabel.text = "Connected";
        };
    }
    
    public void OnDisconnectButton()
    {
        Debug.Log("[MonaWalletConnectTest] OnDisconnectButton");

        _disconnectButton.interactable = false;
        _postNonceButton.interactable = false;
        _postAuthorizeButton.interactable = false;

        WalletConnectModal.Disconnect();
    }

    public async void OnPostNonceButton()
    {
        Debug.Log("[MonaWalletConnectTest] OnPostNonceButton");
        _resultLabel.text = $"Generating Nonce...";

        var session = WalletConnect.Instance.ActiveSession;
        var address = WalletConnect.Instance.ActiveSession.CurrentAddress(session.Namespaces.Keys.FirstOrDefault())
            .Address;

        var postNonceResponse = await MonaApi.ApiClient.Auth.PostNonce(address);
        Debug.Log("[MonaWalletConnectTest] PostNonce Done!\nResponse: " + postNonceResponse);
        _resultLabel.text = $"Nonce: {postNonceResponse.Nonce}";
    }
    
    public async void OnAuthorizeWallet()
    {
        Debug.Log("[MonaWalletConnectTest] OnPostNonceButton");
        
        var session = WalletConnect.Instance.ActiveSession;
        var address = WalletConnect.Instance.ActiveSession.CurrentAddress(session.Namespaces.Keys.FirstOrDefault())
            .Address;
        
        address = address.ToChecksumAddress();

        _resultLabel.text = $"Validating Wallet {address}";
        
        var validateWalletAddressResponse = await MonaApi.ApiClient.Auth.ValidateWalletAddress(address);
        Debug.Log("[MonaWalletConnectTest] ValidateWallet Done!\nResponse: " + validateWalletAddressResponse);

        if (!validateWalletAddressResponse.IsSuccess)
        {
            _resultLabel.text = validateWalletAddressResponse.ErrorMessage;
            return;
        }

        _resultLabel.text = "Valid Wallet Address. Signing Message...";
        
        _postAuthorizeButton.interactable = validateWalletAddressResponse.IsSuccess;
        
        var data = new PersonalSign(validateWalletAddressResponse.SiweMessage, address);
        var signature = await WalletConnect.Instance.RequestAsync<PersonalSign, string>(data);
        
        _resultLabel.text = "Authorizing with Mona...";
        
        var authorizeResponse = await MonaApi.ApiClient.Auth.Authorize(validateWalletAddressResponse.SiweMessage, signature);
        Debug.Log("[MonaWalletConnectTest] Authorize Done!\nResponse: " + authorizeResponse);

        if (!authorizeResponse)
        {
            _resultLabel.text = "Authorization Failed";
            return;
        }
        
        _resultLabel.text = "Authorization Successful";
        
        var collectibles = await MonaApi.ApiClient.Collectibles.GetWalletCollectibles();
        Debug.Log("[MonaWalletConnectTest] Collectibles: " + collectibles);
        
        _resultLabel.text = "Pulled Collectibles. Total Count: " + collectibles.TotalCount;
    }
}
