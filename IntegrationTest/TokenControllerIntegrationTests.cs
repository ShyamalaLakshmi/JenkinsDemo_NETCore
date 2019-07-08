using Api;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace IntegrationTest
{
    [TestClass]
    public class TokenControllerIntegrationTests
    {
        private readonly HttpClient _client;

        public TokenControllerIntegrationTests()
        {
            var server = new TestServer(new WebHostBuilder().UseEnvironment("Testing").UseStartup<Startup>())
            {
                // Added as api is having required https enabled
                BaseAddress = new Uri("https://localhost")
            };

            _client = server.CreateClient();
        }

        [TestMethod]
        public void AuthorizationWithInValidGrantMustReturnUnsupportedGrantTest()
        {
            //Arrange
            var data = new Dictionary<string, string>()
            {
                { "grant_type", "cookie" },
                { "username", "admin@outreach.com" },
                { "password", "Admin@123" }
            };
            var secretContent = new FormUrlEncodedContent(data);

            //Act
            var response = _client.PostAsync("/api/token/", secretContent).Result;

            var tokenResponse = JsonConvert.DeserializeObject<ApiError>(
                response.Content.ReadAsStringAsync().Result);

            //Assert
            Assert.IsNull(tokenResponse.Message);
        }

        [TestMethod]
        public void AuthorizationWithInvalidCredentialsMustReturnInvalidGrantTest()
        {
            //Arrange
            var data = new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "username", "admin@outreach.com" },
                { "password", "Admin@1234" }
            };
            var secretContent = new FormUrlEncodedContent(data);

            //Act
            var response = _client.PostAsync("/api/token/", secretContent).Result;

            var tokenResponse = JsonConvert.DeserializeObject<ApiError>(
                response.Content.ReadAsStringAsync().Result);

            //Assert
            Assert.AreEqual(OpenIdConnectConstants.Errors.InvalidGrant, tokenResponse.Message);
        }

        [TestMethod]
        public void AuthorizationWithValidCredentialsMustReturnTokenTest()
        {
            //Arrange
            var data = new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "username", "admin@outreach.com" },
                { "password", "Admin@123" }
            };
            var secretContent = new FormUrlEncodedContent(data);

            //Act
            var response = _client.PostAsync("/api/token/", secretContent).Result;

            response.EnsureSuccessStatusCode();

            var tokenResponse = JsonConvert.DeserializeObject<OpenIdConnectResponse>(
                response.Content.ReadAsStringAsync().Result);

            //Assert
            Assert.IsNotNull(tokenResponse.AccessToken);
        }
    }
}
