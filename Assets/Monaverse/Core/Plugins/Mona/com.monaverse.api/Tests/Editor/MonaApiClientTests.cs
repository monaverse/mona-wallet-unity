using System;
using System.IO;
using Monaverse.Api.Modules.Ai.Requests;
using Newtonsoft.Json;
using NUnit.Framework;
using Monaverse.Api.Modules.Ai.Responses;

namespace Monaverse.Api.Tests.Editor
{
    public class MonaApiClientTests
    {
        private const string TEST_DATA_PATH = "Assets/Monaverse/Core/Plugins/Mona/com.monaverse.api/Tests/TestData";

        private string LoadJsonFromFile(string filename)
        {
            var path = Path.Combine(TEST_DATA_PATH, filename);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Test data file not found: {path}");
            }
            return File.ReadAllText(path);
        }

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
        public void GetGenerationRequestResponse_DeserializationTest()
        {
            // Load test JSON from file
            var json = LoadJsonFromFile("generation_request_response.json");
            
            // Attempt to deserialize
            var response = JsonConvert.DeserializeObject<GetGenerationRequestResponse>(json);
            
            // Verify deserialization
            Assert.NotNull(response);
            Assert.NotNull(response.OutputAsset);
            Assert.NotNull(response.InputAsset);
            Assert.NotNull(response.Uuid);
            // Add more specific assertions based on your test data
        }

        [Test]
        public void GetUserQuotaResponse_DeserializationTest()
        {
            var json = LoadJsonFromFile("user_quota_response.json");
            var response = JsonConvert.DeserializeObject<GetQuotaResponse>(json);
            
            Assert.NotNull(response);
            Assert.NotNull(response.QuotasRaw);
            Assert.Greater(response.QuotasRaw.Count, 0);
            // Add more specific assertions
        }

        [Test]
        public void CreateImageTo3dRequestResponse_DeserializationTest()
        {
            var json = LoadJsonFromFile("create_image_to_3d_response.json");
            var response = JsonConvert.DeserializeObject<CreateImageTo3dRequestResponse>(json);
            
            Assert.NotNull(response);
            Assert.NotNull(response.InputAsset);
            Assert.NotNull(response.Uuid);
            Assert.NotNull(response.Status);
        }

        [Test]
        public void CreateTextToImageRequestResponse_DeserializationTest()
        {
            var json = LoadJsonFromFile("create_text_to_image_response.json");
            var response = JsonConvert.DeserializeObject<CreateTextToImageRequestResponse>(json);
            
            Assert.NotNull(response);
            Assert.NotNull(response.Uuid);
            Assert.NotNull(response.Status);
        }

        [Test]
        public void GetAssetByIdResponse_DeserializationTest()
        {
            var json = LoadJsonFromFile("get_asset_by_id_response.json");
            var response = JsonConvert.DeserializeObject<GetAssetResponse>(json);
            
            Assert.NotNull(response);
            Assert.NotNull(response.Creator);
            Assert.NotNull(response.Uuid);
            Assert.NotNull(response.AssetType);
            Assert.NotNull(response.Url);
        }

        [Test]
        public void GetAssetsByUserResponse_DeserializationTest()
        {
            var json = LoadJsonFromFile("get_assets_by_user_response.json");
            var response = JsonConvert.DeserializeObject<GetAssetsResponse>(json);
            
            Assert.NotNull(response);
            Assert.NotNull(response.Items);
            Assert.Greater(response.Count, 0);
            Assert.That(response.Items.Count, Is.EqualTo(response.Count));
        }

        [Test]
        public void GetRequestsByUserResponse_DeserializationTest()
        {
            var json = LoadJsonFromFile("get_requests_by_user_response.json");
            var response = JsonConvert.DeserializeObject<GetGenerationRequestsResponse>(json);
            
            Assert.NotNull(response);
            Assert.NotNull(response.Items);
            Assert.Greater(response.Count, 0);
            Assert.That(response.Items.Count, Is.EqualTo(response.Count));
        }

        [Test]
        public void CreateImageTo3DRequest_SerializationTest()
        {
            var request = new CreateImageTo3dRequestRequest
            {
                ImageId = "K2GTGmCHMXTqEf9MH48nk4",
            };
            
            var serialized = JsonConvert.SerializeObject(request, Formatting.Indented);
            var expected = LoadJsonFromFile("create_image_to_3d_request.json");
            
            // Normalize both JSON strings to handle different whitespace/formatting
            var serializedObj = JsonConvert.DeserializeObject(serialized);
            var expectedObj = JsonConvert.DeserializeObject(expected);
            
            var normalizedSerialized = JsonConvert.SerializeObject(serializedObj);
            var normalizedExpected = JsonConvert.SerializeObject(expectedObj);
            
            Assert.That(normalizedSerialized, Is.EqualTo(normalizedExpected));
        }

        [Test]
        public void CreateTextToImageRequest_SerializationTest()
        {
            var request = new CreateTextToImageRequestRequest
            {
                Prompt = "a bright pink stuffed bunny rabbit"
            };
            
            var serialized = JsonConvert.SerializeObject(request, Formatting.Indented);
            var expected = LoadJsonFromFile("create_text_to_image_request.json");
            
            // Normalize both JSON strings to handle different whitespace/formatting
            var serializedObj = JsonConvert.DeserializeObject(serialized);
            var expectedObj = JsonConvert.DeserializeObject(expected);
            
            var normalizedSerialized = JsonConvert.SerializeObject(serializedObj);
            var normalizedExpected = JsonConvert.SerializeObject(expectedObj);
            
            Assert.That(normalizedSerialized, Is.EqualTo(normalizedExpected));
        }
    }
}