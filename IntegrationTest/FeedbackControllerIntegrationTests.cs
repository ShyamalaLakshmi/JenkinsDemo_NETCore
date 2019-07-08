using Api;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace IntegrationTest
{
    [TestClass]
    public class FeedbackControllerIntegrationTests
    {
        private readonly HttpClient _client;

        public FeedbackControllerIntegrationTests()
        {
            var server = new TestServer(new WebHostBuilder().UseEnvironment("Testing").UseStartup<Startup>())
            {
                // Added as api is having required https enabled
                BaseAddress = new Uri("https://localhost")
            };

            _client = server.CreateClient();
        }

        [TestMethod]
        public void MustReturnParticipatedQuestionsTest()
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

            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/Feedback/participated");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = _client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            var participatedQuestionsResponse = JsonConvert.DeserializeObject<QuestionResponse>(
                response.Content.ReadAsStringAsync().Result);

            //Assert
            Assert.AreEqual(3, participatedQuestionsResponse.Value.Count(x => x.FeedbackType.Equals("participated")));
        }

        [TestMethod]
        public void MustReturnUnregisteredQuestionsTest()
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

            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/Feedback/unregistered");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = _client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            var unregisteredQuestionsResponse = JsonConvert.DeserializeObject<QuestionResponse>(
                response.Content.ReadAsStringAsync().Result);

            //Assert
            Assert.AreEqual(1, unregisteredQuestionsResponse.Value.Length);
            Assert.AreEqual("unregistered", unregisteredQuestionsResponse.Value.Select(x => x.FeedbackType).SingleOrDefault());
        }

        [TestMethod]
        public void MustReturnNotParticipatedQuestionsTest()
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

            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/Feedback/notparticipated");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = _client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            var unattendedQuestionsResponse = JsonConvert.DeserializeObject<QuestionResponse>(
                response.Content.ReadAsStringAsync().Result);

            //Assert
            Assert.AreEqual(1, unattendedQuestionsResponse.Value.Length);
            Assert.AreEqual("notparticipated", unattendedQuestionsResponse.Value.Select(x => x.FeedbackType).SingleOrDefault());
        }
    }
}
