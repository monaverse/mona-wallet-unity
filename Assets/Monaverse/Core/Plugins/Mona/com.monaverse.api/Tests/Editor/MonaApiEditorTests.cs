using System;
using Monaverse.Api;
using NUnit.Framework;

public class MonaApiEditorTests
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
    
    [Test]
    public void CollectiblesModuleInstanceTest()
    {
        
        var client = MonaApi.Init(Guid.NewGuid().ToString());
        
        Assert.NotNull(client);
        Assert.NotNull(client.Collectibles);
    }
}