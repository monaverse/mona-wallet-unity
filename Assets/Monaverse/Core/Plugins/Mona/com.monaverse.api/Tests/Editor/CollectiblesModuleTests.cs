using System;
using System.Threading.Tasks;
using Monaverse.Api.Logging;
using Monaverse.Api.MonaHttpClient;
using Monaverse.Api.MonaHttpClient.Request;
using Monaverse.Api.MonaHttpClient.Response;
using Monaverse.Api.Options;
using Moq;
using NUnit.Framework;

namespace Monaverse.Api.Tests.Editor
{
    public class CollectiblesModuleTests
    {
        [Test]
        public void CollectiblesModuleInstanceTest()
        {
            var client = MonaApi.Init(Guid.NewGuid().ToString());

            Assert.NotNull(client);
            Assert.NotNull(client.Collectibles);
        }
        
        
        [Test]
        public void GetCollectibles()
        {
            Task.Run(async () =>
            {
                var mockMonaHttpClient = new Mock<IMonaHttpClient>();

                var mockMonaHttpResponse = new Mock<IMonaHttpResponse>();
                mockMonaHttpResponse.Setup(i => i.IsSuccess).Returns(true);
                mockMonaHttpResponse.Setup(i =>
                        i.GetResponseString(null))
                    .Returns(SampleDataHelper.GetWalletCollectiblesResponse);

                mockMonaHttpClient.Setup(i =>
                        i.SendAsync(It.IsAny<IMonaHttpRequest>()))
                    .ReturnsAsync(mockMonaHttpResponse.Object);

                var monaApiClient = MonaApi.Init(new DefaultApiOptions(), new UnityMonaApiLogger(ApiLogLevel.Info), mockMonaHttpClient.Object);
                var getWalletCollectiblesResponse = await monaApiClient.Collectibles.GetWalletCollectibles();

                Assert.NotNull(getWalletCollectiblesResponse);
                Assert.NotNull(getWalletCollectiblesResponse.Data);
                Assert.AreEqual(1, getWalletCollectiblesResponse.Data.TotalCount);
            });
        }
        
        [Test]
        public void GetCollectibleById()
        {
            Task.Run(async () =>
            {
                var mockMonaHttpClient = new Mock<IMonaHttpClient>();
                const string collectibleId = "bgsDpasdriLhk";

                var mockMonaHttpResponse = new Mock<IMonaHttpResponse>();
                mockMonaHttpResponse.Setup(i => i.IsSuccess).Returns(true);
                mockMonaHttpResponse.Setup(i =>
                        i.GetResponseString(null))
                    .Returns(SampleDataHelper.GetWalletCollectibleByIdResponse);

                mockMonaHttpClient.Setup(i =>
                        i.SendAsync(It.IsAny<IMonaHttpRequest>()))
                    .ReturnsAsync(mockMonaHttpResponse.Object);

                var monaApiClient = MonaApi.Init(new DefaultApiOptions(), new UnityMonaApiLogger(ApiLogLevel.Info), mockMonaHttpClient.Object);
                var walletCollectibleById = await monaApiClient.Collectibles.GetWalletCollectibleById(collectibleId);

                Assert.NotNull(walletCollectibleById);
                Assert.AreEqual(collectibleId, walletCollectibleById.Data.Id);
            });
        }
    }
}