using Api;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace IntegrationTest
{
    [TestClass]
    public class QuestionsControllerIntegrationTests
    {
        private readonly HttpClient _client;

        public QuestionsControllerIntegrationTests()
        {
            var server = new TestServer(new WebHostBuilder().UseEnvironment("Testing").UseStartup<Startup>())
            {
                // Added as api is having required https enabled
                BaseAddress = new Uri("https://localhost")
            };

            _client = server.CreateClient();
        }

        [TestMethod]
        public void AdminCredentialsMustReturnAllQuestionsTest()
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

            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/Questions");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = _client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            var eventsStatusResponse = JsonConvert.DeserializeObject<Collection<Question>>(
                response.Content.ReadAsStringAsync().Result);

            //Assert
            Assert.AreEqual(5, eventsStatusResponse.Value.Length);
        }

        [TestMethod]
        public void PmoCredentialsMustReturnForbiddenTest()
        {
            //Arrange
            var data = new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "username", "123456@cognizant.com" },
                { "password", "Pmo@123" }
            };
            var secretContent = new FormUrlEncodedContent(data);

            //Act
            var tokenResponse = _client.PostAsync("/api/token/", secretContent).Result;
            tokenResponse.EnsureSuccessStatusCode();

            var token = JsonConvert.DeserializeObject<OpenIdConnectResponse>(
                tokenResponse.Content.ReadAsStringAsync().Result).AccessToken;

            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/Questions");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = _client.SendAsync(request).Result;

            //Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [TestMethod]
        public void GetQuestionByIdMustReturnOnlyOneQuestion()
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

            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/Questions/ee2b83be-91db-4de5-8122-35a9c9195975");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = _client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            var questionResponse = JsonConvert.DeserializeObject<Question>(
                response.Content.ReadAsStringAsync().Result);

            //Assert
            Assert.AreEqual("Hey there, Please share your feedback for unregistering from the event ?", questionResponse.Description);
            Assert.AreEqual(6, questionResponse.Answers.Count());
        }

    }
}

