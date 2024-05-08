using System;
using NUnit.Framework;

namespace Monaverse.Api.Tests.Editor
{
    public class MonaApiClientTests
    {
        [Test]
        public void ClientInstanceTest()
        {
            var client = MonaApi.Init(Guid.NewGuid().ToString());
            Assert.NotNull(client);
        }

        [Test]
        public void AuthModuleInstanceTest()
        {
            var client = MonaApi.Init(Guid.NewGuid().ToString());

            Assert.NotNull(client);
            Assert.NotNull(client.Auth);
        }
    }
}