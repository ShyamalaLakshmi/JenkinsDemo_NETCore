using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Api.Controllers;
using Business;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Moq;
using Services;
using Xunit;

namespace ApiTest
{
    public class UsersControllerTests : IDisposable
    {
        private MockRepository mockRepository;

        private Mock<IUserBusiness> mockUserBusiness;
        private Mock<IOptions<PagingOptions>> mockOptions;
        private Mock<IAuthorizationService> mockAuthorizationService;

        public UsersControllerTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            var mockuserService = this.mockRepository.Create<IUserService>();
            this.mockUserBusiness = this.mockRepository.Create<IUserBusiness>();
            this.mockOptions = this.mockRepository.Create<IOptions<PagingOptions>>();
            this.mockAuthorizationService = this.mockRepository.Create<IAuthorizationService>();

        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private UsersController CreateUsersController()
        {
            // common setups
            this.mockOptions.Setup(r => r.Value).Returns(new PagingOptions() { Limit = 2, Offset = 2 });

            var controllerObj = new UsersController(
                 this.mockUserBusiness.Object,
                 this.mockOptions.Object,
                 this.mockAuthorizationService.Object);

            //this.mockRepository.Create<ControllerContext>();
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

            return controllerObj;
        }
        [Fact]
        public async Task GetVisibleUsers_StateUnderTest_ExpectedBehavior()
        {
            // setup all needed values
            {
                this.mockAuthorizationService.Setup(
                    r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                    ).ReturnsAsync(AuthorizationResult.Success());

                List<User> users = new List<User>();
                users.Add(new User()
                {
                    Id = new Guid(),
                    FirstName = "Test",
                    LastName = "T",
                    CreatedAt = DateTime.Now,
                    Email = "sample"
                });
                users.Add(new User()
                {
                    Id = new Guid(),
                    FirstName = "Test 1",
                    LastName = "T",
                    CreatedAt = DateTime.Now,
                    Email = "sample 1"
                });
                PagedResults<User> sampleResult = new PagedResults<User>()
                {
                    Items = users
                };

                this.mockUserBusiness.Setup(r => r.GetUsersAsync(
                    It.IsAny<PagingOptions>(),
                    It.IsAny<SortOptions<User, UserEntity>>(),
                    It.IsAny<SearchOptions<User, UserEntity>>())
                    ).ReturnsAsync(sampleResult);
            }

            // Arrange
            var unitUnderTest = this.CreateUsersController();
            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<User, UserEntity> sortOptions = new SortOptions<User, UserEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<User, UserEntity> searchOptions = new SearchOptions<User, UserEntity>() { Search = new string[] { "" } };

            // Act
            var result = await unitUnderTest.GetVisibleUsers(
                pagingOptions,
                sortOptions,
                searchOptions);

            // Assert
            Assert.True(result.Value != null);
        }

        [Fact]
        public async Task GetVisibleUsers_StateUnderTest_ExpectedBehavior_ForSelf()
        {
            // setup all needed values
            {
                this.mockAuthorizationService.Setup(
                    r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                    ).ReturnsAsync(AuthorizationResult.Failed());

                User user = new User()
                {
                    Id = new Guid(),
                    FirstName = "Admin123",
                    LastName = "T",
                    CreatedAt = DateTime.Now,
                    Email = "sample"
                };

                this.mockUserBusiness.Setup(r => r.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            }

            // Arrange
            var unitUnderTest = this.CreateUsersController();
            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<User, UserEntity> sortOptions = new SortOptions<User, UserEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<User, UserEntity> searchOptions = new SearchOptions<User, UserEntity>() { Search = new string[] { "" } };

            // Act
            var result = await unitUnderTest.GetVisibleUsers(
                pagingOptions,
                sortOptions,
                searchOptions);

            // Assert
            Assert.True(result.Value != null);
        }

        [Fact]
        public async Task GetUsersByRole_StateUnderTest_ExpectedBehavior()
        {
            // setup all needed values
            {
                this.mockAuthorizationService.Setup(
                    r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                    ).ReturnsAsync(AuthorizationResult.Success());

                List<User> users = new List<User>();
                users.Add(new User()
                {
                    Id = new Guid(),
                    FirstName = "Test",
                    LastName = "T",
                    CreatedAt = DateTime.Now,
                    Email = "sample"
                });
                users.Add(new User()
                {
                    Id = new Guid(),
                    FirstName = "Test 1",
                    LastName = "T",
                    CreatedAt = DateTime.Now,
                    Email = "sample 1"
                });
                PagedResults<User> sampleResult = new PagedResults<User>()
                {
                    Items = users
                };

                this.mockUserBusiness.Setup(r => r.GetUsersByRoleAsync(
                    It.IsAny<PagingOptions>(),
                    It.IsAny<SortOptions<User, UserEntity>>(),
                    It.IsAny<SearchOptions<User, UserEntity>>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                    )).ReturnsAsync(sampleResult);
            }

            // Arrange
            var unitUnderTest = this.CreateUsersController();
            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<User, UserEntity> sortOptions = new SortOptions<User, UserEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<User, UserEntity> searchOptions = new SearchOptions<User, UserEntity>() { Search = new string[] { "" } };
            CancellationToken ct = new CancellationToken(true) { };

            // Act
            var result = await unitUnderTest.GetUsersByRole(
                pagingOptions,
                sortOptions,
                searchOptions,
                ct);

            // Assert
            Assert.IsType<ActionResult<PagedCollection<User>>>(result);
        }

        [Fact]
        public async Task GetUserById_StateUnderTest_ExpectedBehavior_InvalidUser()
        {
            // setup all needed values
            {
                Guid? myGuidVar = null;
                this.mockUserBusiness.Setup(r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(myGuidVar);
            }

            // Arrange
            var unitUnderTest = this.CreateUsersController();

            // Act
            var result = await unitUnderTest.GetUserById(new Guid());

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetUserById_StateUnderTest_ExpectedBehavior_SameUser()
        {
            Guid myGuidVar = new Guid("9034b908-dc1f-41ad-b3f1-af416757753e");
            // setup all needed values
            {
                this.mockUserBusiness.Setup(r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(myGuidVar);

                this.mockUserBusiness.Setup(r => r.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User()
                {
                    Id = myGuidVar,
                    FirstName = "Test",
                    LastName = "T",
                    CreatedAt = DateTime.Now,
                    Email = "sample"
                });
            }

            // Arrange
            var unitUnderTest = this.CreateUsersController();

            // Act
            var result = await unitUnderTest.GetUserById(myGuidVar);

            // Assert
            Assert.True(result.Value.Id == myGuidVar);
        }

        [Fact]
        public async Task GetUserById_StateUnderTest_ExpectedBehavior_NoAccessRole()
        {
            // setup all needed values
            {
                this.mockAuthorizationService.Setup(
                    r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                    ).ReturnsAsync(AuthorizationResult.Failed());

                this.mockUserBusiness.Setup(r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new Guid());
            }
            // Arrange
            var unitUnderTest = this.CreateUsersController();

            // Act
            Guid myGuidVar = new Guid("9034b908-dc1f-41ad-b3f1-af416757753e");
            var result = await unitUnderTest.GetUserById(myGuidVar);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetUserById_StateUnderTest_ExpectedBehavior_NonExistUser()
        {
            // setup all needed values
            {
                this.mockAuthorizationService.Setup(
                    r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                    ).ReturnsAsync(AuthorizationResult.Success());

                this.mockUserBusiness.Setup(r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new Guid());

                User usvar = null;
                this.mockUserBusiness.Setup(r => r.GetUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync(usvar);
            }
            // Arrange
            var unitUnderTest = this.CreateUsersController();

            // Act            
            Guid myGuidVar = new Guid("9034b908-dc1f-41ad-b3f1-af416757753e");
            var result = await unitUnderTest.GetUserById(myGuidVar);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetUserById_StateUnderTest_ExpectedBehavior()
        {
            Guid myGuidVar = new Guid("9034b908-dc1f-41ad-b3f1-af416757753e");
            // setup all needed values
            {
                this.mockAuthorizationService.Setup(
                    r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                    ).ReturnsAsync(AuthorizationResult.Success());

                this.mockUserBusiness.Setup(r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new Guid());

                User usvar = new User()
                {
                    FirstName = "Sample",
                    LastName = "T",
                    Id = myGuidVar
                };
                this.mockUserBusiness.Setup(r => r.GetUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync(usvar);
            }
            // Arrange
            var unitUnderTest = this.CreateUsersController();

            // Act            
            var result = await unitUnderTest.GetUserById(myGuidVar);

            // Assert
            Assert.True(result.Value.Id == myGuidVar);
        }

        //[Fact]
        //public async Task RegisterUser_StateUnderTest_ExpectedBehavior_Success()
        //{
        //    RegisterForm input = new RegisterForm();
        //    // setup all needed values
        //    {
        //        (bool Succeeded, string ErrorMessage) retVal = (true, "");
        //        this.mockUserBusiness.Setup(r => r.CreateUserAsync(input)).ReturnsAsync(retVal);

        //    }

        //    // Arrange
        //    var unitUnderTest = this.CreateUsersController();

        //    // Act
        //    var result = await unitUnderTest.RegisterUser(input);

        //    // Assert
        //    Assert.True(result == null);
        //    //Assert.IsType<CreatedResult>(result);
        //}

        [Fact]
        public async Task RegisterUser_StateUnderTest_ExpectedBehavior_Fail()
        {
            RegisterForm input = new RegisterForm();

            // setup all needed values
            {
                (bool Succeeded, string ErrorMessage) retVal = (false, "");
                this.mockUserBusiness.Setup(r => r.CreateUserAsync(input)).ReturnsAsync(retVal);
            }

            // Arrange
            var unitUnderTest = this.CreateUsersController();

            // Act
            var result = await unitUnderTest.RegisterUser(input);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateRole_StateUnderTest_ExpectedBehavior_NoUser()
        {
            // Arrange
            var unitUnderTest = this.CreateUsersController();
            RoleForm input = new RoleForm()
            {
                Email = "Sample",
                Add = true
            };

            // setup all needed values 
            User outp = null;
            this.mockUserBusiness.Setup(r => r.GetUserByEmailAsync(input.Email)).ReturnsAsync(outp);

            // Act
            var result = await unitUnderTest.UpdateRole(input);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateRole_StateUnderTest_ExpectedBehavior_Add()
        {
            // Arrange
            var unitUnderTest = this.CreateUsersController();
            RoleForm role = new RoleForm()
            {
                Email = "sample",
                Add = true
            };
            User userobj = new User()
            {
                Email = "sample",
                FirstName = "SampleName",
                Id = new Guid()
            };

            // setup all needed values 
            this.mockUserBusiness.Setup(r => r.GetUserByEmailAsync(role.Email)).ReturnsAsync(userobj);

            (bool Succeeded, string ErrorMessage) retVal = (false, "");
            this.mockUserBusiness.Setup(r => r.AddToRoleAsync(userobj.Id, "Pmo")).ReturnsAsync(retVal);

            // Act
            var result = await unitUnderTest.UpdateRole(role);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateRole_StateUnderTest_ExpectedBehavior_Remove()
        {
            // Arrange
            var unitUnderTest = this.CreateUsersController();
            RoleForm role = new RoleForm()
            {
                Email = "sample",
                Add = false
            };
            User userobj = new User()
            {
                Email = "sample",
                FirstName = "SampleName",
                Id = new Guid()
            };

            // setup all needed values 
            this.mockUserBusiness.Setup(r => r.GetUserByEmailAsync(role.Email)).ReturnsAsync(userobj);

            (bool Succeeded, string ErrorMessage) retVal = (false, "");
            this.mockUserBusiness.Setup(r => r.RemoveFromRoleAsync(userobj.Id, "Pmo")).ReturnsAsync(retVal);

            // Act
            var result = await unitUnderTest.UpdateRole(role);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
