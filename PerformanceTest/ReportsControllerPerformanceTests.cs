using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace PerformanceTest
{
    [TestClass]
    public class ReportsControllerPerformanceTests
    {
        private const int Expected = 500;

        [TestMethod]
        public void AdminCredentialsMustReturnAllEventsStatsTest()
        {
            //Arrange
            var allResponseTimes = new List<(DateTime Start, DateTime End)>();

            var data = new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "username", "admin@outreach.com" },
                { "password", "Admin@123" }
            };

            var secretContent = new FormUrlEncodedContent(data);

            //Act
            for (var i = 0; i < 100; i++)
            {
                using (var client = new HttpClient())
                {
                    var start = DateTime.Now;

                    var tokenResponse = client.PostAsync("http://localhost:8091/api/token/", secretContent).Result;
                    //tokenResponse.EnsureSuccessStatusCode();

                    if (!tokenResponse.StatusCode.Equals(HttpStatusCode.BadRequest))
                    {
                        var token = JsonConvert.DeserializeObject<OpenIdConnectResponse>(
                        tokenResponse.Content.ReadAsStringAsync().Result).AccessToken;

                        var request = new HttpRequestMessage(new HttpMethod("GET"), "http://localhost:8091/api/reports");
                        request.Headers.Add("Authorization", $"Bearer {token}");

                        var response = client.SendAsync(request).Result;
                        //response.EnsureSuccessStatusCode();
                    }

                    var end = DateTime.Now;

                    allResponseTimes.Add((start, end));
                }
            }

            //Assert
            var actual = (int)allResponseTimes.Select(r => (r.End - r.Start).TotalMilliseconds).Average();
            Assert.IsTrue(actual <= Expected, $"Expected total milliseconds of less than or equal to {Expected} but was {actual}.");
        }

        [TestMethod]
        public void PocCredentialsMustReturnOnlyPocEventsStatsTest()
        {
            //Arrange
            var allResponseTimes = new List<(DateTime Start, DateTime End)>();

            var data = new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "username", "696752@cognizant.com" },
                { "password", "Poc@123" }
            };

            var secretContent = new FormUrlEncodedContent(data);

            //Act
            for (var i = 0; i < 100; i++)
            {
                using (var client = new HttpClient())
                {
                    var start = DateTime.Now;

                    var tokenResponse = client.PostAsync("http://localhost:8091/api/token/", secretContent).Result;
                    //tokenResponse.EnsureSuccessStatusCode();

                    if (!tokenResponse.StatusCode.Equals(HttpStatusCode.BadRequest))
                    {
                        var token = JsonConvert.DeserializeObject<OpenIdConnectResponse>(
                            tokenResponse.Content.ReadAsStringAsync().Result).AccessToken;

                        var request =
                            new HttpRequestMessage(new HttpMethod("GET"), "http://localhost:8091/api/reports");
                        request.Headers.Add("Authorization", $"Bearer {token}");

                        var response = client.SendAsync(request).Result;
                        //response.EnsureSuccessStatusCode();
                    }

                    var end = DateTime.Now;

                    allResponseTimes.Add((start, end));
                }
            }

            //Assert
            var actual = (int)allResponseTimes.Select(r => (r.End - r.Start).TotalMilliseconds).Average();
            Assert.IsTrue(actual <= Expected, $"Expected total milliseconds of less than or equal to {Expected} but was {actual}.");
        }

        [TestMethod]
        public void AdminCredentialsWithSearchCityMustReturnOnlyEventsStatsWithThatCityTest()
        {
            //Arrange
            var allResponseTimes = new List<(DateTime Start, DateTime End)>();

            var data = new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "username", "696752@cognizant.com" },
                { "password", "Poc@123" }
            };

            var secretContent = new FormUrlEncodedContent(data);

            //Act
            for (var i = 0; i < 100; i++)
            {
                using (var client = new HttpClient())
                {
                    var start = DateTime.Now;

                    var tokenResponse = client.PostAsync("http://localhost:8091/api/token/", secretContent).Result;
                    //tokenResponse.EnsureSuccessStatusCode();

                    if (!tokenResponse.StatusCode.Equals(HttpStatusCode.BadRequest))
                    {
                        var token = JsonConvert.DeserializeObject<OpenIdConnectResponse>(
                            tokenResponse.Content.ReadAsStringAsync().Result).AccessToken;

                        var request = new HttpRequestMessage(new HttpMethod("GET"),
                            "http://localhost:8091/api/reports?search=baselocation%20eq%20Pune");
                        request.Headers.Add("Authorization", $"Bearer {token}");

                        var response = client.SendAsync(request).Result;
                        //response.EnsureSuccessStatusCode();
                    }

                    var end = DateTime.Now;

                    allResponseTimes.Add((start, end));
                }
            }

            //Assert
            var actual = (int)allResponseTimes.Select(r => (r.End - r.Start).TotalMilliseconds).Average();
            Assert.IsTrue(actual <= Expected, $"Expected total milliseconds of less than or equal to {Expected} but was {actual}.");
        }

        [TestMethod]
        public void StatusReportDownloadMustReturnEventStatsExcelFileTest()
        {
            //Arrange
            var allResponseTimes = new List<(DateTime Start, DateTime End)>();

            var data = new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "username", "696752@cognizant.com" },
                { "password", "Poc@123" }
            };

            var secretContent = new FormUrlEncodedContent(data);

            //Act
            for (var i = 0; i < 100; i++)
            {
                using (var client = new HttpClient())
                {
                    var start = DateTime.Now;

                    var tokenResponse = client.PostAsync("http://localhost:8091/api/token/", secretContent).Result;
                    //tokenResponse.EnsureSuccessStatusCode();

                    if (!tokenResponse.StatusCode.Equals(HttpStatusCode.BadRequest))
                    {
                        var token = JsonConvert.DeserializeObject<OpenIdConnectResponse>(
                            tokenResponse.Content.ReadAsStringAsync().Result).AccessToken;

                        var request = new HttpRequestMessage(new HttpMethod("GET"),
                            "http://localhost:8091/api/reports/true");
                        request.Headers.Add("Authorization", $"Bearer {token}");

                        var response = client.SendAsync(request).Result;
                        //response.EnsureSuccessStatusCode();
                    }

                    var end = DateTime.Now;

                    allResponseTimes.Add((start, end));
                }
            }

            //Assert
            var actual = (int)allResponseTimes.Select(r => (r.End - r.Start).TotalMilliseconds).Average();
            Assert.IsTrue(actual <= Expected, $"Expected total milliseconds of less than or equal to {Expected} but was {actual}.");
        }
    }
}
