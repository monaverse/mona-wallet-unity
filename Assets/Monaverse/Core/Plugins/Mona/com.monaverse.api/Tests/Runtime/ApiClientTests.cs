using System;
using System.Linq;
using Monaverse.Api.Logging;
using Monaverse.Api.MonaHttpClient.Request;
using NUnit.Framework;
using UnityEngine;

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
            var client = new MonaApiHttpClient(logger, applicationId);
            var request = new MonaHttpRequest("https://localhost", RequestMethod.Post);

            //Act
            var response = await client.SendAsync(request);

            //Assert
            var header = response.HttpRequest.Headers.FirstOrDefault(x => x.Key == "X-Mona-Application-Id");
            Assert.NotNull(header);
            Assert.AreEqual(applicationId, header.Value);
        }

        [Test]
        public async void VerifyAccessToken()
        {
            //Arrange
            var applicationId = Guid.NewGuid().ToString();
            var logger = new UnityMonaApiLogger(ApiLogLevel.Info);
            var client = new MonaApiHttpClient(logger, applicationId);
            var accessToken = Guid.NewGuid().ToString();
            client.AccessToken = accessToken;
            var request = new MonaHttpRequest("https://localhost", RequestMethod.Post);
            Debug.Log("Before");

            //Act
            await client.SendAsync(request);

            Debug.Log("After");
            //Assert
            var header = request.Headers.FirstOrDefault(x => x.Key == "Authorization");
            Assert.NotNull(header);
            Assert.AreEqual("Bearer " + accessToken, header.Value);
        }
    }
}