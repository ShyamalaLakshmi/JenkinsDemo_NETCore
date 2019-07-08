using Api.Controllers;
using Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Moq;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ApiTest
{
    public class DashboardControllerTests : IDisposable
    {
        private MockRepository mockRepository;

        private Mock<IDashboardBusiness> mockDashboardBusiness;
        private Mock<IUserBusiness> mockUserBusiness;
        private Mock<IAuthorizationService> mockAuthorizationService;

        public DashboardControllerTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockDashboardBusiness = this.mockRepository.Create<IDashboardBusiness>();
            this.mockUserBusiness = this.mockRepository.Create<IUserBusiness>();
            this.mockAuthorizationService = this.mockRepository.Create<IAuthorizationService>();
        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private DashboardController CreateDashboardController()
        {
            var controllerObj = new DashboardController(
                this.mockDashboardBusiness.Object,
                this.mockUserBusiness.Object,
                this.mockAuthorizationService.Object);

            //this.mockRepository.Create<ControllerContext>();
            controllerObj.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "123")
                    }, "someAuthTypeName")),
                }
            };

            return controllerObj;
        }

        [Fact]
        public async Task GetStats_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateDashboardController();
            CancellationToken ct = new CancellationToken() { };

            this.mockAuthorizationService.Setup(
                r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                ).ReturnsAsync(AuthorizationResult.Success());

            this.mockDashboardBusiness.Setup(
                r => r.GetStatsAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())
                ).ReturnsAsync(
                new Dashboard()
                {
                    LivesImpacted = 2,
                    TotalEvents = 2
                }
            );
            
            // Act
            var result = await unitUnderTest.GetStats(ct);

            // Assert
            Assert.IsType<Dashboard>(result.Value);
        }

        [Fact]
        public async Task GetStats_StateUnderTest_ExpectedBehavior_UnAuthorised()
        {
            // Arrange
            var unitUnderTest = this.CreateDashboardController();
            CancellationToken ct = new CancellationToken() { };

            this.mockAuthorizationService.Setup(
                r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                ).ReturnsAsync(AuthorizationResult.Failed());

            this.mockUserBusiness.Setup(
                r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())
                ).ReturnsAsync(new Guid());

            this.mockDashboardBusiness.Setup(
                r => r.GetStatsAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())
                ).ReturnsAsync(
                new Dashboard()
                {
                    LivesImpacted = 2,
                    TotalEvents = 2
                }
            );
            
            // Act
            var result = await unitUnderTest.GetStats(ct);

            // Assert
            Assert.IsType<Dashboard>(result.Value);
        }
    }
}
