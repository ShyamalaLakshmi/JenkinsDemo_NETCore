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
    public class UsersControllerIntegrationTests
    {
        private readonly HttpClient _client;

        public UsersControllerIntegrationTests()
        {
            var server = new TestServer(new WebHostBuilder().UseEnvironment("Testing").UseStartup<Startup>())
            {
                // Added as api is having required https enabled
                BaseAddress = new Uri("https://localhost")
            };

            _client = server.CreateClient();
        }

        [TestMethod]
        public void AdminCredentialsMustReturnAllUsersTest()
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
            var tokenResponse = _client.PostAsync("/api/token/", secretContent).Result;
            tokenResponse.EnsureSuccessStatusCode();

            var token = JsonConvert.DeserializeObject<OpenIdConnectResponse>(
                tokenResponse.Content.ReadAsStringAsync().Result).AccessToken;

            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/Users");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = _client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            var usersResponse = JsonConvert.DeserializeObject<UsersResponse>(
                response.Content.ReadAsStringAsync().Result);

            //Assert
            Assert.AreEqual(5, usersResponse.Value.Length);
        }

        [TestMethod]
        public void GetUserByRoleMustReturnRespectiveUserTest()
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
            var tokenResponse = _client.PostAsync("/api/token/", secretContent).Result;
            tokenResponse.EnsureSuccessStatusCode();

            var token = JsonConvert.DeserializeObject<OpenIdConnectResponse>(
                tokenResponse.Content.ReadAsStringAsync().Result).AccessToken;

            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/Users/Pmo");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = _client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            var usersResponse = JsonConvert.DeserializeObject<UsersResponse>(
                response.Content.ReadAsStringAsync().Result);

            //Assert
            Assert.AreEqual(1, usersResponse.Value.Length);
        }
    }
}
