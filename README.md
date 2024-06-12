# Mona Wallet Unity

[![Build Package](https://github.com/monaverse/mona-wallet-unity/actions/workflows/main.workflow.yml/badge.svg?branch=main)](https://github.com/monaverse/mona-wallet-unity/actions/workflows/main.workflow.yml?query=branch%3Amain)


Enable your users to connect their Web3 wallets to the open metaverse, allowing them to import and interact with their 3D NFTs within your Unity-based games and applications using our SDK.

### Prerequisites
- Unity 2022.3.17f or above.
- IL2CPP managed code stripping level: Minimal (or lower).

### Supported Platforms
- Android
- iOS
- macOS
- Windows
- WebGL

## Getting Started

The Mona Wallet Unity SDK is distributed as a [Unity Package](https://docs.unity3d.com/Manual/PackagesList.html).

### Download Package

You can download the latest official `.unitypackage` from the [Releases](https://github.com/monaverse/mona-wallet-unity/releases) page.

Alternatively, you can download the latest generated package from [Build Packages](https://github.com/monaverse/mona-wallet-unity/actions/workflows/build-sdk-package.yml?query=branch%3Amain) workflow page. The file is available at the bottom of each workflow run as `monaverse-sdk-package`. Please note that this version may not be stable.

### Install package

Drag and drop the `.unitypackage` file into your Unity project or double click it. Feel free to deselect common packages like Newtonsoft if you already have them.

If there are no compiler errors in the console, you are all set!

**Important Notes:**

- If you have an existing Web3 integration in your project such as `Thirdweb`, `WalletConnect`, etc. Please try using our [API-only](https://github.com/monaverse/mona-wallet-unity/tree/main/Assets/Monaverse/Core/Plugins/Mona/com.monaverse.api) package instead.
- The SDK has been tested using Unity 2022 LTS. We highly recommend using 2022 LTS.
- The Newtonsoft and Nethereums DLLs are included as part of the Unity Package, feel free to deselect/remove them if you already have them installed as a dependency to avoid conflicts.


## Monaverse Modal

It's the simplest and most minimal way to interact with the 3D collectibles from your wallet, enabled the Monaverse platform.

### Usage
1. Install the SDK following the steps described above.
2. Add a WalletConnect ProjectId to the `MonaverseManager` component. 
   3. If you don't have a WalletConnect ProjectID, you can create one in [WalletConnect Cloud](https://cloud.walletconnect.com/).
3. Drag and drop the [MonaverseModal](https://github.com/monaverse/mona-wallet-unity/tree/main/Assets/Monaverse/Modal/Prefabs) prefab to the first scene in your game.
4. Open the modal at any time after `Awake`

### Open Modal
From your code, use this to open the modal.

```c#
MonaverseModal.Open();
```

You may pass an optional filter function used to determine compatibility with your game.

```c#
//Example: Filter collectibles by artist
MonaverseModal.Open(collectible => collectible.Artist == "<insert artist here>");
```

### Modal Flow
The `MonaverseModal` will take the user through various views in order to connect and authorize their wallet with the Monaverse platform.
Once authorization is granted, the user will have access to their collectibles and be able to interact with them from your application.

#### Connect Your Wallet
The user may select the wallet provider of their choice via Wallet Connect
![image](https://github.com/monaverse/mona-wallet-unity/assets/708754/b71ce3ef-2259-48f0-a03d-317a9c7aa832)

![image](https://github.com/monaverse/mona-wallet-unity/assets/708754/d7d308eb-ffd4-47c8-be7d-f741bd9d1b75)

#### Authorize Your Wallet
Once connected, the SDK will try to authorize the user's wallet with the Monaverse platform.
Note: The user must be registered at [Monaverse.com](https://monaverse.com)

![image](https://github.com/monaverse/mona-wallet-unity/assets/708754/b3b802a7-009a-4b36-81a4-18bccc2f8c6f)

![image](https://github.com/monaverse/mona-wallet-unity/assets/708754/3cb757fc-2047-47b3-819c-5e9e7e6dbcdf)

#### Collectibles View
From this view, the user can browse through their collectibles.
As a developer, you may signal the SDK on how to identify which collectibles are compatible with your application (See the `filter function` in the `Open Modal` section). Compatible items will have a checkmark on their left hand side.

![image](https://github.com/monaverse/mona-wallet-unity/assets/708754/1eedee72-1d62-4333-8a89-4a9499207e8f)

#### Collectible Detail View
Displays details of a selected collectible with `Preview` and `Import` actions available.
You must tell the SDK how to handle the `Import` action via events.
By default, the `Preview` action will open the respective webpage to a collectible's details page in the `Monaverse.com` marketplace

![image](https://github.com/monaverse/mona-wallet-unity/assets/708754/286e6ce3-49d3-4159-9c92-62f3db1c636d)


### Events
The `MonaverseModal` class exposes a set of events for you to handle optionally from your code.

- [Ready](https://github.com/monaverse/mona-wallet-unity/blob/545dacf71152f1ff4a399bd7143323263b025661/Assets/Monaverse/Modal/Scripts/MonaverseModal.cs#L32)
- [ModalOpened](https://github.com/monaverse/mona-wallet-unity/blob/545dacf71152f1ff4a399bd7143323263b025661/Assets/Monaverse/Modal/Scripts/MonaverseModal.cs#L37)
- [ModalClosed](https://github.com/monaverse/mona-wallet-unity/blob/545dacf71152f1ff4a399bd7143323263b025661/Assets/Monaverse/Modal/Scripts/MonaverseModal.cs#L42)
- [ImportCollectibleClicked](https://github.com/monaverse/mona-wallet-unity/blob/545dacf71152f1ff4a399bd7143323263b025661/Assets/Monaverse/Modal/Scripts/MonaverseModal.cs#L50)
- [PreviewCollectibleClicked](https://github.com/monaverse/mona-wallet-unity/blob/545dacf71152f1ff4a399bd7143323263b025661/Assets/Monaverse/Modal/Scripts/MonaverseModal.cs#L55)
- [CollectiblesLoaded](https://github.com/monaverse/mona-wallet-unity/blob/545dacf71152f1ff4a399bd7143323263b025661/Assets/Monaverse/Modal/Scripts/MonaverseModal.cs#L63)
- [CollectibleSelected](https://github.com/monaverse/mona-wallet-unity/blob/545dacf71152f1ff4a399bd7143323263b025661/Assets/Monaverse/Modal/Scripts/MonaverseModal.cs#L71)








