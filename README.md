# Mona Wallet Unity

[![Build Packages](https://github.com/monaverse/mona-wallet-unity/actions/workflows/build-sdk-package.yml/badge.svg?branch=main)](https://github.com/monaverse/mona-wallet-unity/actions/workflows/build-sdk-package.yml?query=branch%3Amain)


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
- The Newtonsoft and Nethereums DLLs are included as part of the Unity Package, feel free to deselect them if you already have them installed as a dependency to avoid conflicts.


## Usage

