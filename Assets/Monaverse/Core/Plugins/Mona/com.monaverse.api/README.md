# Monaverse API

The Unity3D Monaverse API Package is designed to facilitate interoperability across the open metaverse by allowing Web3 metaverse and game platforms to empower their users to import 3D NFTs from their Web3 wallets into Unity-based games or applications. 

As this is only the `API module`, projects must have an existing Web3 implementation for wallet connectivity and authentication. 
(i.e. MetaMask, WalletConnect, Thirdweb, or similar integrations)

### Important Notice

> If your project lacks a wallet integration, please use the full Monaverse SDK, available at the root of this repository, to integrate these features fully.

## Prerequisites

- Unity 2022.3.17f1 or above
- Existing Web3 wallet integration for connecting and message signing
- Nethereum Dlls _**(Ignore if this Nethereum is already included with your Wallet Integration)**_
  - If after importing the API package there are compiler errors related to missing `Nethereum` assemblies, please add the Nethereum reference to your `manifest.json`. 
  - i.e. `"com.nethereum.unity": "https://github.com/Nethereum/Nethereum.Unity.git#4.19.1"`

## Usage

### Initializing the API
Start by initializing the Monaverse API client using your application ID. You can obtain one at [monaverse.com](https://monaverse.com).

```csharp
// Initialize the Monaverse API client
MonaApi.Init("your-application-id");
```

### Authenticating the User
Authentication involves verifying the wallet address and signing a message with your wallet provider.

```csharp
// Step 1: Verify Wallet Address
// Obtain the wallet address via your existing Web3 wallet integration
string walletAddress = "user_wallet_address";

// Validate the connected wallet with Mona
var validateWalletResult = await MonaApi.ApiClient.Auth.ValidateWallet(walletAddress);

// Check if the wallet validation request succeeded
if(!validateWalletResult.IsSuccess)
{
    Debug.LogError($"There was a problem validating your wallet: {validateWalletResult.Message}");
    return;
}

//If the wallet is not registered, ask the user to sign up at https://monaverse.com
if (validateWalletResult.Data.Result == ValidateWalletResult.WalletIsNotRegistered)
{
    Debug.LogError("Wallet is not registered. Please sign up at https://monaverse.com");
    return;
}

// Step 2: Sign the Message and Authorize
// Sign the SiweMessage using your Web3 wallet integration
// This is the equivalent to a Wallet Personal Sign
var signature = await walletSigner.Sign(validateWalletResult.Data.SiweMessage);

// Authorize with Monaverse using the signed message and the SiweMessage
var authorizationResult = await MonaApi.ApiClient.Auth.Authorize(signature, validateWalletResult.Data.SiweMessage);

if (!authorizationResult.IsSuccess)
{
    Debug.LogError($"Authorization failed.{authorizationResult.Message}");
    return;
}

//Ready to call authorized APIs
```

### Get Collectibles [Authorized]
Once the user is successfully authorized, you can call the API to retrieve the 3D NFTs associated with their wallet.

_**Note**: You must Authorize the user before calling this endpoint_
```csharp
// Fetch the 3D NFTs stored in the user's wallet
var getWalletCollectiblesResult = await MonaApi.ApiClient.Collectibles.GetWalletCollectibles();
if (!getWalletCollectiblesResult.IsSuccess)
{
    Debug.LogError($"GetWalletCollectibles failed.{getWalletCollectiblesResult.Message}");
    return;
}


Debug.Log("GetWalletCollectibles: " + getWalletCollectiblesResult.Data);
```

### Get Collectibles By Id [Authorized]
Once the user is successfully authorized, you can call the API to retrieve the 3D NFTs associated with their wallet.

_**Note**: You must Authorize the user before calling this endpoint_
```csharp
// Fetch the 3D Collectible by Id
var collectibleId = "collectible-unique-id";
var getWalletCollectibleByIdResult = await MonaApi.ApiClient.Collectibles.GetWalletCollectibleById(collectibleId);
if (!getWalletCollectibleByIdResult.IsSuccess)
{
    Debug.LogError($"GetWalletCollectibleById failed.{getWalletCollectibleByIdResult.Message}");
    return;
}

Debug.Log("GetWalletCollectibleById: " + getWalletCollectibleByIdResult.Data);
```