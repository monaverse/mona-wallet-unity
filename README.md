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

Alternatively, you can download the latest generated package from [Main Workflow](https://github.com/monaverse/mona-wallet-unity/actions/workflows/main.workflow.yml?query=branch%3Amain) workflow page. The file is available at the bottom of each workflow run as `monaverse-sdk-package`. Please note that this version may not be stable.

### Install package

Drag and drop the `.unitypackage` file into your Unity project or double click it. Feel free to deselect common packages like Newtonsoft if you already have them.

If there are no compiler errors in the console, you are all set!

**Important Notes:**

- The SDK has been tested using Unity 2022 LTS. We highly recommend using 2022 LTS.
- The Newtonsoft DLLs are included as part of the Unity Package, feel free to deselect/remove them if you already have them installed as a dependency to avoid conflicts.

## Monaverse Modal

It's the simplest and most minimal way to interact with the 3D tokens from your wallets, enabled by the Monaverse platform.

### Usage
1. Install the SDK following the steps described above.
2. Drag and drop the [MonaverseModal](https://github.com/monaverse/mona-wallet-unity/tree/main/Assets/Monaverse/Modal/Prefabs) prefab to the first scene in your game.
3. Enter your `monaApplicationId` in the `MonaverseManager` inspector. You can get on at [MonaStudio](https://studio.monaverse.com/)
4. Open the modal at any time after `Awake`

### Open Modal
From your code, use this to open the modal.

```c#
MonaverseModal.Open();
```

#### Modal Options
You can configure the behavior of the modal using optional parameters that control how the modal behaves when opened. This is done by passing a lambda expression to the Open method, which allows you to set various options.

- **LoadTokensView**: Set to false if you don't want to load the tokens UI after authentication.
_Default: true_

```csharp
MonaverseModal.Open(options =>
{
    options.LoadTokensView = false;
});
```


- **TokenFilter**: An optional filter function used to determine which tokens are compatible with your application.
_Default: null (No filtering)_


```c#
MonaverseModal.Open(options =>
{
    options.TokenFilter = token => token.Kind == "erc1155";
});
```

You can combine multiple options to customize the behavior further:
```csharp
MonaverseModal.Open(options =>
{
    options.LoadTokensView = false;
    options.TokenFilter = token => token.Kind == "erc1155";
}); 
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

## Leaderboards

Our SDK now supports leaderboards, allowing developers to integrate high-score tracking into their games. Below is a guide on how to enable and use this feature.

### Enabling Leaderboards

1. **Enable Leaderboards on the Studio Website**:  
   To enable the leaderboard feature, navigate to your application's page on the [MonaStudio](https://studio.monaverse.com/) site and click on `Enable Leaderboards`. Once enabled, additional settings will become available, allowing you to configure the `featured` leaderboard, which will be the default leaderboard displayed on our website.

2. **Obtain the SDK Secret**:  
   On the same application page, you'll find the `SDK Secret`, which is required to authenticate your `PostScore` requests from Unity.
    
   There are two ways to provide the `SDK Secret` in Unity:

    - Set it once at startup using:
      ```csharp
      MonaverseManager.Instance.SDK.SetSecret("YOUR_SDK_SECRET");
      ```

    - Optionally, pass it in each score submission call:
      ```csharp
      var result = await MonaverseManager.Instance.SDK
         .PostScore(score: 100,
                    topic: "Level 1",
                    sdkSecret: "YOUR_SDK_SECRET"); // Optional
      ```
      For security reasons, do not store the `SDK Secret` in serialized fields as it is less secure and can be compromised.

**IMPORTANT: Please do **not** use the `API Secret` in Unity or any other client, as it is intended for server-to-server communication only.**

### Leaderboard APIs

### GetTopScores

Retrieves the top scores from the leaderboard. You can customize the query to get scores from a specific topic, time period, or sorting order.

```csharp
 var result = await MonaverseManager.Instance.SDK
                .GetTopScores(
                    limit: 20,
                    offset: 0,
                    featured: false,
                    topic: "Level 1",
                    sortingOrder: "highest",
                    period: "all_time",
                    includeAllUserScores: false);
```

#### Parameters:
- **limit**: Maximum number of scores per page (default: 20)
- **offset**: Start position for scores (default: 0)
- **featured**: Whether to return scores for the featured leaderboard (ignores topic)
- **sorting**Order: Sorting order: "highest" or "lowest"
- **topic**: Topic of the leaderboard (ignored if featured is true)
- **period**: Time period: "all_time", "daily", "weekly", or "monthly"
- **startTime & endTime**: Custom date range for scores
- **includeAllUserScores**: If true, includes all scores for rank calculation

#### Returns:
The response is a `GetTopScoresResponse` object containing:

- Items: A list of TopScore records, each representing a score entry.
- Count: Total number of scores returned in this paginated response.

#### TopScore Object:

* Id: The unique ID of the score entry.
* User: A TopScoreUser object containing user details.
* Score: The score value achieved by the user.
* Topic: The leaderboard topic the score belongs to.
* CreatedAt: The date and time the score was recorded.
* Rank: The rank of the score in the leaderboard.

#### TopScoreUser Object:
* Username: The username of the player.
* Name: The player's display name.

### GetUserRank
Gets the rank of the authenticated user on a given leaderboard.

```csharp
var result = await MonaverseManager.Instance.SDK
                .GetUserRank(
                    featured: false,
                    topic: "Level 1",
                    sortingOrder: "highest",
                    period: "all_time",
                    includeAllUserScores: false);
```

#### Parameters:

- **featured**: If true, retrieves rank for the featured leaderboard set in the MonaStudio website
- **sortingOrder**: "highest" or "lowest"
- **topic**: Topic of the leaderboard (ignored if featured is true)
- **period**: Time period: "all_time", "daily", "weekly", or "monthly"
- **startTime & endTime**: Custom date range for ranking
- **includeAllUserScores**: Include all scores for rank calculation

#### Returns:
The response is a `GetUserRankResponse` object containing:
* Rank: The user's rank in the leaderboard.
* Score: The user's highest score for the given topic and time period.
* Topic: The topic the score was recorded for.
* CreatedAt: The date and time the score was recorded.


### GetAroundMeScores
Gets scores around the user's rank (Above and below the user) 

```csharp
var result = await MonaverseManager.Instance.SDK
                .GetAroundMeScores(
                    featured: false,
                    topic: "Level 1",
                    sortingOrder: "highest",
                    period: "all_time",
                    includeAllUserScores: false,
                    limit: 20);
```


#### Parameters:
- **featured**: If true, retrieves rank for the featured leaderboard
- **sortingOrder**: "highest" or "lowest"
- **topic**: Topic of the leaderboard (ignored if featured is true)
- **period**: Time period: "all_time", "daily", "weekly", or "monthly"
- **startTime & endTime**: Custom date range for ranking
- **includeAllUserScores**: Include all scores for rank calculation

#### Returns:
A list of `TopScore` objects representing scores around the user's rank. 
The `TopScore` object contains the same fields as described in the `GetTopScores` section.

### PostScore

Posts a score to the leaderboard. You can associate the score with a specific topic, or leave it ungrouped by not specifying a topic.

```csharp
var result = await MonaverseManager.Instance.SDK
                .PostScore(score: 20.5f, topic: "Level 1");
```

**_NOTE: The SDK secret is required to post scores, you must have set it either in the `SetSecret` method at startup or passed as an optional parameter in this call._**

```csharp
var result = await MonaverseManager.Instance.SDK
                .PostScore(score: 20.5f, 
                    topic: "Level 1",
                    sdkSecret: "YOUR_SDK_SECRET"); //Optionally, pass in the SDK secret if not set on startup
```

#### Parameters:

* **score**: The score value to post.
* **topic**: (Optional) The topic to which the score will be posted, such as "Level 1" or "Halloween Special". If no topic is provided, the score will be posted without a grouping.
* **sdkSecret**: (Optional) The SDK secret to use for authentication. You can pass it here or set it once at startup using SetSecret().

#### Returns:
An `ApiResult` object containing:
* IsSuccess: A boolean indicating whether the score was posted successfully.
* Message: A string with additional information about the result. If the request failed, this will contain the error message.


### Understanding Leaderboard Topics

In our leaderboard system, you can use **topics** to dynamically group scores, making it easy to create multiple leaderboards without explicitly setting up separate ones for each scenario.

#### What Are Topics?

A **topic** is a label you can associate with each score submission. Topics allow you to categorize or group scores under different contexts, such as specific levels, events, or game modes. For example, instead of creating a new leaderboard for every level in your game, you can simply pass a unique topic (e.g., `"Level 1"`, `"Level 2"`, `"Halloween Special"`, etc.) when posting a score. This flexibility allows you to easily manage thousands of different leaderboards dynamically without needing to configure each one manually.

#### How to Use Topics

- **Posting Scores**: When posting a score using the `PostScore` API, you can specify a topic to group that score under a particular label:
    ```csharp
    var result = await MonaverseManager.Instance.SDK
        .PostScore(score: _score, topic: "Level 1");
    ```

- **Retrieving Scores**: When fetching scores with the `GetTopScores`, `GetUserRank`, or `GetAroundMeScores` APIs, you can query based on the topic to get results specific to that grouping:
    ```csharp
    var topScores = await MonaverseManager.Instance.SDK
        .GetTopScores(topic: "Level 1");
    ```

#### Dynamic Leaderboards with Topics

By leveraging topics, you can create as many dynamic leaderboards as your game requires. Whether it’s for different levels, seasonal events, or special modes, simply passing a relevant topic when posting and retrieving scores allows you to effectively manage and retrieve high scores without needing to set up new leaderboards.

For instance, if your game has thousands of levels, you can track high scores for each level individually by using topics like `"Level 1"`, `"Level 2"`, and so on. There’s no need to manually configure a new leaderboard for each level—topics provide the flexibility to create as many leaderboards as needed, on the fly.

#### Important Notes:
- **Topics are case-sensitive**, so `"level 1"` and `"Level 1"` will be treated as two different topics. Ensure consistency in how you define and reference topics throughout your game.
- If no topic is provided when posting a score, the score will not be grouped under any specific topic and will be considered part of the general leaderboard.

By utilizing topics, you gain full control over how scores are organized, allowing your game to scale seamlessly without added complexity.
