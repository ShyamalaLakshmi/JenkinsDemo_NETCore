using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Controllers;
using Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Moq;
using Xunit;

namespace ApiTest
{
    public class UserinfoControllerTests : IDisposable
    {
        private MockRepository mockRepository;

        private Mock<IUserBusiness> mockUserBusiness;

        private Mock<IUrlHelper> mockCtxtUrl;

        public UserinfoControllerTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockUserBusiness = this.mockRepository.Create<IUserBusiness>();

            this.mockCtxtUrl = this.mockRepository.Create<IUrlHelper>();
        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private UserinfoController CreateUserinfoController()
        {
           var controllerObj =  new UserinfoController(this.mockUserBusiness.Object);
            
            controllerObj.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "Admin123")
                    }, "someAuthTypeName"))
                }                
            };

            controllerObj.Url = mockCtxtUrl.Object;

            return controllerObj;
        }

        [Fact]
        public async Task Userinfo_StateUnderTest_ExpectedBehavior_InvalidUser()
        {
            // Arrange
            var unitUnderTest = this.CreateUserinfoController();

            Models.User user = null;
            this.mockUserBusiness.Setup(r => r.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            // Act
            var result = await unitUnderTest.Userinfo();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Userinfo_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateUserinfoController();

            mockCtxtUrl.Setup(r => r.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("sample link");

            Models.User user = new Models.User()
            {
                FirstName = "Sample",
                Email = "email",
                Id = Guid.NewGuid()
            };
             
            this.mockUserBusiness.Setup(r => r.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            this.mockUserBusiness.Setup(r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(Guid.NewGuid());
             
            // Act
            var result = await unitUnderTest.Userinfo();

            // Assert
            Assert.IsType<UserinfoResponse>(result.Value);
        }
    }
}
