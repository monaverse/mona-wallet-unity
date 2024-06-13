using System;
using System.Linq;
using Monaverse.Api.Logging;
using Monaverse.Api.MonaHttpClient;
using Monaverse.Api.MonaHttpClient.Request;
using NUnit.Framework;

namespace Monaverse.Api.Tests.Runtime
{
    public class ApiClientTests
    {
        [Test]
        public async void VerifyApplicationId()
        {
            //Arrange
            var applicationId = Guid.NewGuid().ToString();
            var logger = new UnityMonaApiLogger(ApiLogLevel.Info);
            var client = new UnityAsyncHttpClient(logger);
            var request = new MonaHttpRequest("https://localhost", RequestMethod.Post);

            //Act
            var response = await client.SendAsync(request);

            //Assert
            var header = response.HttpRequest.Headers.FirstOrDefault(x => x.Key == "X-Mona-Application-Id");
            Assert.NotNull(header);
            Assert.AreEqual(applicationId, header.Value);
        }
    }
}