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
    public class DashboardControllerIntegrationTests
    {
        private readonly HttpClient _client;

        public DashboardControllerIntegrationTests()
        {
            var server = new TestServer(new WebHostBuilder().UseEnvironment("Testing").UseStartup<Startup>())
            {
                // Added as api is having required https enabled
                BaseAddress = new Uri("https://localhost")
            };

            _client = server.CreateClient();
        }

        [TestMethod]
        public void AuthorizationWithAdminCredentialsMustReturnAdminStatsTest()
        {
            //Arrange
            var data = new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "username", "admin@outreach.com" },
                { "password", "Admin@123" }
            };
            var secretContent = new FormUrlEncodedContent(data);

            var dashboard = new Dashboard
            {
                TotalEvents = 8,
                LivesImpacted = 877,
                TotalVolunteers = 56,
                TotalParticipants = 6
            };

            //Act
            var tokenResponse = _client.PostAsync("/api/token/", secretContent).Result;
            tokenResponse.EnsureSuccessStatusCode();

            var token = JsonConvert.DeserializeObject<OpenIdConnectResponse>(
                tokenResponse.Content.ReadAsStringAsync().Result).AccessToken;

            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/dashboard/getstats");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = _client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            var dashboardResponse = JsonConvert.DeserializeObject<Dashboard>(
                response.Content.ReadAsStringAsync().Result);

            //Assert
            Assert.AreNotSame(dashboard.TotalEvents, dashboardResponse.TotalEvents);
            Assert.AreNotSame(dashboard.TotalVolunteers, dashboardResponse.TotalVolunteers);
            Assert.AreNotSame(dashboard.LivesImpacted, dashboardResponse.LivesImpacted);
            Assert.AreNotSame(dashboard.TotalParticipants, dashboardResponse.TotalParticipants);
        }

        [TestMethod]
        public void AuthorizationWithPocCredentialsMustReturnPocStatsTest()
        {
            //Arrange
            var data = new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "username", "696752@cognizant.com" },
                { "password", "Poc@123" }
            };
            var secretContent = new FormUrlEncodedContent(data);

            var dashboard = new Dashboard
            {
                TotalEvents = 2,
                LivesImpacted = 800,
                TotalVolunteers = 25,
                TotalParticipants = 2
            };

            //Act
            var tokenResponse = _client.PostAsync("/api/token/", secretContent).Result;
            tokenResponse.EnsureSuccessStatusCode();

            var token = JsonConvert.DeserializeObject<OpenIdConnectResponse>(
                tokenResponse.Content.ReadAsStringAsync().Result).AccessToken;

            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/dashboard/getstats");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = _client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            var dashboardResponse = JsonConvert.DeserializeObject<Dashboard>(
                response.Content.ReadAsStringAsync().Result);

            //Assert
            Assert.AreNotEqual(dashboard.TotalEvents, dashboardResponse.TotalEvents);
            Assert.AreNotEqual(dashboard.TotalVolunteers, dashboardResponse.TotalVolunteers);
            Assert.AreNotEqual(dashboard.LivesImpacted, dashboardResponse.LivesImpacted);
            Assert.AreNotEqual(dashboard.TotalParticipants, dashboardResponse.TotalParticipants);
        }
    }
}
