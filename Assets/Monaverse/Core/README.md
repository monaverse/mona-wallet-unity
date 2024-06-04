### Monaverse Manager
Follow this guide if you want to use your own UI.
Otherwise, follow the guide at the root of this repository on how to use our easy-to-use Modal prefab instead.

All you need is a `MonaverseManager` prefab present in your scene.

In your project window, navigate to `Assets > Monaverse > Core > Prefabs > MonaverseManager` and drag the prefab into your scene.

Make sure you set the following fields:
- `Mona Application Id`: You can get one from [Monaverse](https://Monaverse.com)
- `WalletConnectProjectId`: Your project ID for WalletConnect. Get one at [Wallet Connect](https://cloud.walletconnect.com/sign-up)

## Usage

To access the SDK, you only need to include a `MonaverseManager` prefab instance in your scene.

The `MonaverseManager` will guide you through the following steps to access a user's 3D collectibles stored in their Web3 wallet:

- Initialize SDK
- Connect Wallet
- Authorize Wallet
- Get Collectibles

### Initialize SDK

If the `initializeOnAwake` toggle is checked in the `MonaverseManager` MonoBehavior, initialization will be done automatically. Please do not call any SDK functions from within the `Awake` function.

![image](https://github.com/monaverse/mona-wallet-unity/assets/708754/20456f7f-ac09-48d5-bcd6-cb0856e7ae74)

Alternatively, if you prefer to initialize manually from code, you can uncheck the toggle and call:

```C#
MonaverseManager.Initialize();
```

### Connect Wallet
By default, the SDK uses `WalletConnect` with a ready-to-use projectId. Alternatively, you can configure it with your own `WalletConnectProjectId` and set it in the `MonaverseManager` inspector.

```C#
// Connect to the user's Web3 wallet via WalletConnect
var address = await MonaverseManager.Instance.SDK.ConnectWallet();
```

Calling the line above will open `WalletConnect`'s modal, allowing you to choose your wallet provider.
Once connected to your app, the asynchronous function will return the selected account address.

![image](https://github.com/monaverse/mona-wallet-unity/assets/708754/348bffeb-6565-4c46-a0eb-7c6ec6416160)

### Authorize Wallet
Once connected, it's time to authorize the wallet with the `Monaverse` platform. Note that the user must have an existing account at [Monaverse](https://monaverse.com/).

The following code will attempt to authorize the currently connected wallet. You must connect a wallet before calling this.

```C#
var authorizationResult = await MonaverseManager.Instance.SDK.AuthorizeWallet();
```

The `AuthorizeWallet` function returns an enum result with the following options:

```C#
public enum AuthorizationResult
{
    WalletNotConnected,
    FailedValidatingWallet,
    UserNotRegistered,
    FailedSigningMessage,
    FailedAuthorizing,
    Authorized,
    Error
}
```

If the result is `Authorized`, your application is all set to pull the authenticated user 3D collectibles from their Web3 wallet.

### Get Collectibles [Authorized]
Once authorized, your application can retrieve a paginated list of all the 3D collectibles available in a user's Web3 wallet to be imported and interacted with within your game or application.

To get all collectibles for the authenticated user, call:

```C#
var getCollectiblesResult = await MonaApi.ApiClient.Collectibles.GetWalletCollectibles();
if (!getCollectiblesResult.IsSuccess)
{
    Debug.Log("There was an error pulling collectibles: " + getCollectiblesResult.Message);
    return;
}
```

The collectibles result `Data` is of type `GetWalletCollectiblesResponse` with the following properties:

```C#
  public List<CollectibleDto> Data { get; set; }
  public int TotalCount { get; set; }
  public int? NextPageKey { get; set; }
```

Here's an example of the `CollectibleDto` as JSON:

```JSON
{
  "id": "bgsDpasdriLhk",
  "type": "Avatar",
  "checked": false,
  "minted": false,
  "nsfw": false,
  "promoted": false,
  "__v": 11,
  "activeVersion": 0,
  "artist": "John Doe",
  "description": "Test",
  "creator": "0xe42c4fe879955cEa16380e9dAf9E7b6B48126E93",
  "slug": "grifter-squid-goerli",
  "views": 0,
  "accessibility": {
    "accessLevel": "Public",
    "accessibleAt": null,
    "accessibleUntil": null
  },
  "_updated_at": "2024-05-03T18:36:56.258Z",
  "_created_at": "2024-05-03T18:32:35.784Z",
  "_deleted_at": null,
  "collectionId": null,
  "hidden": false,
  "image": "bafasdak2ulwymssqfsaeadls65nt7xxftzh3c3npe7snkmhmfqxqoyu",
  "lastSaleEventId": null,
  "lastSalePrice": null,
  "owner": "0xe42c4fe879955cEa16380e9dAf9E7b6B48126E93",
  "owners": [
    {
      "address": "0xe42c4fe879955cEa16380e9dAf9E7b6B48126E93",
      "amount": 1
    }
  ],
  "parentId": null,
  "price": null,
  "properties": [],
  "traits": {
    "Genre": "Cyber Punk",
    "Season": "Spring"
  },
  "subCollectionId": null,
  "title": "Grifter Squaddie Goerli",
  "nft": {
    "contract": null,
    "ipfsUrl": "bafybeig3tvmgazsdfgadfgsdfm7qrtq2undmaxpsy7xnc7m6flzlgffy",
    "network": null,
    "tokenHash": null,
    "tokenId": null,
    "tokenUri": null,
    "transactionId": null,
    "tokenStandard": null,
    "amount": null
  },
  "creatorId": null,
  "documentId": null,
  "versions": [
    {
      "asset": "https://cdn-staging.mona.gallery/sdfe5433-suet-d9ik-rrgl-fsdfrww.vrm"
    }
  ]
}
```

## Examples

The [MonaWalletConnect](https://github.com/monaverse/mona-wallet-unity/tree/main/Assets/Monaverse/Examples/MonaWalletConnect) sample scene will demonstrate how to wire actions to the UI and provide a complete workflow from connection to authorization.
