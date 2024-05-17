# Monaverse API

The Unity3D Monaverse API Package is designed to facilitate interoperability across the open metaverse by allowing Web3 metaverse and game platforms to empower their users to import 3D NFTs from their Web3 wallets into Unity-based games or applications. 

This package only includes the `API` module, so projects must already have an existing Web3 implementation for wallet connectivity and authentication (e.g., MetaMask, WalletConnect, Thirdweb, or similar integrations).

### Important Notice

> If your project lacks a wallet integration, please use the full Monaverse SDK, available at the root of this repository, to fully integrate these features.

### Download Package

You can download the latest official `com.monaverse.api.unitypackage` from the [Releases](https://github.com/monaverse/mona-wallet-unity/releases) page.

Alternatively, you can download the latest generated package from [Build Packages](https://github.com/monaverse/mona-wallet-unity/actions/workflows/build-sdk-package.yml?query=branch%3Amain) workflow page. The file is available at the bottom of each workflow run as `monaverse-sdk-package`. Please note that this version may not be stable.

## Prerequisites

- Unity 2022.3.17f1 or above
- Existing Web3 wallet integration for connecting and message signing

### Nethereum dependency
> This package depends on [Nethereum](https://github.com/Nethereum/Nethereum.Unity#4.19.1).
>- If you are using the distributed `.unitypackage`, Nethereum will be included.
>- There is chance your existing Wallet Integration already includes Nethereum. Feel free to remove ours if there are conflicting DLLs.
>- If there are compiler errors related to missing `Nethereum` assemblies, you will need to manually add Nethereum via git or directly importing their DLLs

## Usage

### Initializing the API
Start by initializing the Monaverse API client using your application ID. You can obtain one at [monaverse.com](https://monaverse.com).

```csharp
// Initialize the Monaverse API client
MonaApi.Init("your-application-id");
```

### Authorizing the User
Authorizing involves verifying the wallet address and signing a SIWE message with the user's Web3 wallet.

#### Prerequisites: 
- The user must have an existing account with [Monaverse](https://monaverse.com/) 
- You will need your existing Web3 wallet integration in order to connect to the user's wallet and have them sign the SIWE messages.

```csharp
// Step 1: Verify Wallet Address
// Obtain the wallet address via your existing Web3 wallet integration (WalletConnect, Thirdweb, MetaMask, etc)
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

// Step 2: Sign the Message and Authorize (This step needs a Web3 wallet integration)
// Sign the SiweMessage using your Web3 wallet integration
// This is the equivalent to a Wallet Personal Sign
// Example of signing a message with WalletConnect:
// var data = new PersonalSign(siweMessage, walletAddress);
// var signature = await WalletConnect.Instance.RequestAsync<PersonalSign, string>(data);

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
Once authorized, your application can retrieve a paginated list of all the 3D collectibles available in a user's Web3 wallet to be imported and interacted with within your game or application.

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
Call this API to retrieve a 3D Collectible by `ID` from the authorized user's wallet.

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