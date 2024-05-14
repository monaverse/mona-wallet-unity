using System;
using System.Threading.Tasks;
using Monaverse.Api;
using Monaverse.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonaWalletConnectTest : MonoBehaviour
{
    [SerializeField] private Button _connectButton;
    [Space] [SerializeField] private GameObject _dappButtons;
    [SerializeField] private Button _disconnectButton;
    [SerializeField] private Button _postNonceButton;
    [SerializeField] private Button _postAuthorizeButton;
    [SerializeField] private TMP_Text _resultLabel;

    private void Start()
    {
        _resultLabel.text = "Initializing...";
        _connectButton.interactable = false;

        _ = TryConnect();
    }

    private async Task TryConnect()
    {
        try
        {
            _dappButtons.SetActive(false);

            await MonaverseManager.Instance.SDK.ConnectWallet();

            Debug.Log("[MonaWalletConnectTest] Connected!");

            EnableDappButtons();
            _resultLabel.text = "Connected!";
        }
        catch (Exception exception)
        {
            Debug.LogError("[MonaWalletConnectTest] Exception: " + exception);
            EnableConnectButton();
        }
    }


    #region UI Click Events

    public async void OnConnectButton()
    {
        Debug.Log("[MonaWalletConnectTest] OnConnectButton");
        _connectButton.interactable = false;

        await TryConnect();
    }

    public async void OnDisconnectButton()
    {
        Debug.Log("[MonaWalletConnectTest] OnDisconnectButton");

        try
        {
            _disconnectButton.interactable = false;
            await MonaverseManager.Instance.SDK.Disconnect();
            EnableConnectButton();
        }
        catch (Exception e)
        {
            _disconnectButton.interactable = true;
            Debug.LogError("[MonaWalletConnectTest] Disconnect Exception: " + e);
        }
    }

    public async void OnPostNonceButton()
    {
        Debug.Log("[MonaWalletConnectTest] OnPostNonceButton");
        if (!await MonaverseManager.Instance.SDK.IsWalletConnected())
        {
            _resultLabel.text = "Wallet not connected";
            return;
        }

        _resultLabel.text = $"Generating Nonce...";

        var address = await MonaverseManager.Instance.SDK.ActiveWallet.GetAddress();

        var postNonceResponse = await MonaApi.ApiClient.Auth.PostNonce(address);
        Debug.Log("[MonaWalletConnectTest] PostNonce Done!\nResponse: " + postNonceResponse);
        _resultLabel.text = $"Nonce: {postNonceResponse.Data?.Nonce}";
    }

    public async void OnAuthorizeWallet()
    {
        Debug.Log("[MonaWalletConnectTest] OnAuthorizeWallet");
        _resultLabel.text = "Authorizing Wallet...";

        var authorizationResult = await MonaverseManager.Instance.SDK.AuthorizeWallet();

        var resultText = authorizationResult switch
        {
            MonaWalletSDK.AuthorizationResult.WalletNotConnected => "Wallet Not Connected",
            MonaWalletSDK.AuthorizationResult.Authorized => "Wallet Authorized",
            MonaWalletSDK.AuthorizationResult.FailedAuthorizing => "Failed authorizing wallet",
            MonaWalletSDK.AuthorizationResult.UserNotRegistered => "User not registered",
            MonaWalletSDK.AuthorizationResult.FailedValidatingWallet => "Failed validating wallet",
            MonaWalletSDK.AuthorizationResult.FailedSigningMessage => "Failed signing message",
            MonaWalletSDK.AuthorizationResult.Error => "Unexpected error authorizing wallet",
            _ => throw new ArgumentOutOfRangeException()
        };

        Debug.Log("[MonaWalletConnectTest] Authorization Result: " + resultText);
        _resultLabel.text = resultText;
    }

    public async void OnGetCollectibles()
    {
        if (!MonaverseManager.Instance.SDK.IsWalletAuthorized())
        {
            _resultLabel.text = "Wallet must be authorized first!";
            return;
        }
        
        _resultLabel.text = "Getting Collectibles...";

        var getCollectiblesResult = await MonaApi.ApiClient.Collectibles.GetWalletCollectibles();
        Debug.Log("[MonaWalletConnectTest] Collectibles: " + getCollectiblesResult);
        if (!getCollectiblesResult.IsSuccess)
        {
            _resultLabel.text = "GetCollectibles Failed: " + getCollectiblesResult.Message;
            return;
        }

        _resultLabel.text = "Pulled Collectibles. Total Count: " + getCollectiblesResult.Data.TotalCount;
    }

    #endregion

    private void EnableConnectButton()
    {
        _connectButton.interactable = true;
        _connectButton.gameObject.SetActive(true);
        _dappButtons.gameObject.SetActive(false);
    }

    private void EnableDappButtons()
    {
        _disconnectButton.interactable = true;
        _dappButtons.SetActive(true);
        _connectButton.gameObject.SetActive(false);
    }
}