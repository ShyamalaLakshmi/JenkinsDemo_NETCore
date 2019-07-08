using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace PerformanceTest
{
    [TestClass]
    public class TokenControllerPerformanceTests
    {
        private const int Expected = 500;

        [TestMethod]
        public void AuthorizationWithInValidGrantMustReturnUnsupportedGrantTest()
        {
            //Arrange
            var allResponseTimes = new List<(DateTime Start, DateTime End)>();

            var data = new Dictionary<string, string>()
            {
                { "grant_type", "cookie" },
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

                    var response = client.PostAsync("http://localhost:8091/api/token/", secretContent).Result;

                    var end = DateTime.Now;

                    allResponseTimes.Add((start, end));
                }
            }

            //Assert
            var actual = (int)allResponseTimes.Select(r => (r.End - r.Start).TotalMilliseconds).Average();
            Assert.IsTrue(actual <= Expected, $"Expected total milliseconds of less than or equal to {Expected} but was {actual}.");
        }

        [TestMethod]
        public void AuthorizationWithInvalidCredentialsMustReturnInvalidGrantTest()
        {
            //Arrange
            var allResponseTimes = new List<(DateTime Start, DateTime End)>();

            var data = new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "username", "admin@outreach.com" },
                { "password", "Admin@1234" }
            };

            var secretContent = new FormUrlEncodedContent(data);

            //Act
            for (var i = 0; i < 100; i++)
            {
                using (var client = new HttpClient())
                {
                    var start = DateTime.Now;

                    var response = client.PostAsync("http://localhost:8091/api/token/", secretContent).Result;

                    var end = DateTime.Now;

                    allResponseTimes.Add((start, end));
                }
            }

            //Assert
            var actual = (int)allResponseTimes.Select(r => (r.End - r.Start).TotalMilliseconds).Average();
            Assert.IsTrue(actual <= Expected, $"Expected total milliseconds of less than or equal to {Expected} but was {actual}.");
        }

        [TestMethod]
        public void AuthorizationWithValidCredentialsMustReturnTokenTest()
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

                    var response = client.PostAsync("http://localhost:8091/api/token/", secretContent).Result;

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
