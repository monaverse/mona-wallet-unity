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

- The SDK has been tested using Unity 2022 LTS. We highly recommend using 2022 LTS.
- The Newtonsoft DLLs are included as part of the Unity Package, feel free to deselect/remove them if you already have them installed as a dependency to avoid conflicts.

## Monaverse Modal

It's the simplest and most minimal way to interact with the 3D collectibles from your wallet, enabled the Monaverse platform.

### Usage
1. Install the SDK following the steps described above.
3. Drag and drop the [MonaverseModal](https://github.com/monaverse/mona-wallet-unity/tree/main/Assets/Monaverse/Modal/Prefabs) prefab to the first scene in your game.
4. Open the modal at any time after `Awake`

### Open Modal
From your code, use this to open the modal.

```c#
MonaverseModal.Open();
```

You may pass an optional filter function used to determine compatibility with your game.

```c#
//Example: Filter tokens by kind
MonaverseModal.Open(token => token.Kind == "erc1155");
```

### Modal Flow
The `MonaverseModal` will take the user through various views in order to authenticate with the Monaverse platform.
Once authentication is granted, the user will have access to their tokens and be able to interact with them from your application.

#### Generate One-Time Password (OTP)
To generate an OTP, users must provide their email address. The process varies based on their registration status:
- Registered Users: An OTP will be sent to their email.
- Unregistered Users: Registration instructions will be sent to their email.
![image](https://github.com/monaverse/mona-wallet-unity/assets/708754/adbd547b-efc5-429e-9163-c499685a9dee)


#### Verify One-Time Password (OTP)
Users must enter the OTP sent to their email in the previous step. Upon successful entry, the authentication process is completed, granting access to all authenticated APIs. The next screen, the Tokens View, will then be loaded.
![image](https://github.com/monaverse/mona-wallet-unity/assets/708754/e2198a5a-6152-49d3-915d-3210b15b4000)


#### Tokens View
From this view, the user can browse through their tokens.
As a developer, you may signal the SDK on how to identify which tokens are compatible with your application (See the `filter function` in the `Open Modal` section). Compatible items will have a checkmark on their left hand side.

![image](https://github.com/monaverse/mona-wallet-unity/assets/708754/91cc440c-c824-458b-8693-3e47c2b7ae80)


#### Token Detail View
Displays details of a selected token with `Preview` and `Import` actions available.
You must tell the SDK how to handle the `Import` action via events.
By default, the `Preview` action will open the respective webpage to a token's details page in the `Monaverse.com` marketplace

![image](https://github.com/monaverse/mona-wallet-unity/assets/708754/286e6ce3-49d3-4159-9c92-62f3db1c636d)


### Events
The `MonaverseModal` class exposes a set of events for you to handle optionally from your code.

- [Ready](https://github.com/monaverse/mona-wallet-unity/blob/545dacf71152f1ff4a399bd7143323263b025661/Assets/Monaverse/Modal/Scripts/MonaverseModal.cs#L32)
- [ModalOpened](https://github.com/monaverse/mona-wallet-unity/blob/545dacf71152f1ff4a399bd7143323263b025661/Assets/Monaverse/Modal/Scripts/MonaverseModal.cs#L37)
- [ModalClosed](https://github.com/monaverse/mona-wallet-unity/blob/545dacf71152f1ff4a399bd7143323263b025661/Assets/Monaverse/Modal/Scripts/MonaverseModal.cs#L42)
- [ImportTokenClicked](https://github.com/monaverse/mona-wallet-unity/blob/a460ed5147230c99b4418ef866e4e07c915f8a46/Assets/Monaverse/Modal/Scripts/MonaverseModal.cs#L54)
- [PreviewTokenClicked](https://github.com/monaverse/mona-wallet-unity/blob/a460ed5147230c99b4418ef866e4e07c915f8a46/Assets/Monaverse/Modal/Scripts/MonaverseModal.cs#L60)
- [TokensLoaded](https://github.com/monaverse/mona-wallet-unity/blob/a460ed5147230c99b4418ef866e4e07c915f8a46/Assets/Monaverse/Modal/Scripts/MonaverseModal.cs#L68)
- [TokenSelected](https://github.com/monaverse/mona-wallet-unity/blob/a460ed5147230c99b4418ef866e4e07c915f8a46/Assets/Monaverse/Modal/Scripts/MonaverseModal.cs#L76)

## Modal Example
This is a sample WebGL application demonstrating how to use the SDK and the `Modal` prefab to quickly connect users to their Monaverse account and Tokens. 
[Demo App](https://monaverse.github.io/mona-wallet-unity)

The [Source Code](https://github.com/monaverse/mona-wallet-unity/blob/1f2737da2ac75743e00a3a5349077f738f876aaa/Assets/Monaverse/Examples/MonaverseModal/Scripts/MonaverseModalExample.cs#L8) for this example is included in the SDK.

![image](https://github.com/monaverse/mona-wallet-unity/assets/708754/1688eaf3-e81a-4e85-adaf-8005f89e4e5d)







