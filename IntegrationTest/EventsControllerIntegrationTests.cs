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
    public class EventsControllerIntegrationTests
    {
        private readonly HttpClient _client;

        public EventsControllerIntegrationTests()
        {
            var server = new TestServer(new WebHostBuilder().UseEnvironment("Testing").UseStartup<Startup>())
            {
                // Added as api is having required https enabled
                BaseAddress = new Uri("https://localhost")
            };

            _client = server.CreateClient();
        }

        [TestMethod]
        public void AdminCredentialsMustReturnAllEventsTest()
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

            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/Events");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = _client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            var eventsStatusResponse = JsonConvert.DeserializeObject<Collection<EventReport>>(
                response.Content.ReadAsStringAsync().Result);

            //Assert
            Assert.AreNotEqual(8, eventsStatusResponse.Value.Length);
        }

        [TestMethod]
        public void PocCredentialsMustReturnOnlyPocEventsTest()
        {
            //Arrange
            var data = new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "username", "696752@cognizant.com" },
                { "password", "Poc@123" }
            };
            var secretContent = new FormUrlEncodedContent(data);

            //Act
            var tokenResponse = _client.PostAsync("/api/token/", secretContent).Result;
            tokenResponse.EnsureSuccessStatusCode();

            var token = JsonConvert.DeserializeObject<OpenIdConnectResponse>(
                tokenResponse.Content.ReadAsStringAsync().Result).AccessToken;

            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/Events");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = _client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            var eventsStatusResponse = JsonConvert.DeserializeObject<Collection<EventReport>>(
                response.Content.ReadAsStringAsync().Result);

            //Assert
            Assert.AreNotEqual(2, eventsStatusResponse.Value.Length);
        }

        [TestMethod]
        public void EventsReportDownloadMustReturnEventsExcelFileTest()
        {
            //Arrange
            var data = new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "username", "696752@cognizant.com" },
                { "password", "Poc@123" }
            };
            var secretContent = new FormUrlEncodedContent(data);

            //Act
            var tokenResponse = _client.PostAsync("/api/token/", secretContent).Result;
            tokenResponse.EnsureSuccessStatusCode();

            var token = JsonConvert.DeserializeObject<OpenIdConnectResponse>(
                tokenResponse.Content.ReadAsStringAsync().Result).AccessToken;

            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/Events/true");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = _client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            //Assert
            Assert.AreEqual("Grid.xlsx", response.Content.Headers.ContentDisposition.FileName);
        }

        [TestMethod]
        public void GetEventByIdMustReturnOnlyOneEvent()
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

            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/Events/B6FC0079-8FE2-468D-80E0-10F927DD69A9");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = _client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            var eventsStatusResponse = JsonConvert.DeserializeObject<Event>(
                response.Content.ReadAsStringAsync().Result);

            //Assert
            Assert.AreEqual("TEACHING", eventsStatusResponse.EventName);
        }

    }
}
