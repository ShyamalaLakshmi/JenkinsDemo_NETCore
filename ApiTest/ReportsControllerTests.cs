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
using Xunit;

namespace ApiTest
{
    public class ReportsControllerTests : IDisposable
    {
        private MockRepository mockRepository;

        private Mock<IReportBusiness> mockReportBusiness;
        private Mock<IUserBusiness> mockUserBusiness;
        private Mock<IAuthorizationService> mockAuthorizationService;
        private Mock<IOptions<PagingOptions>> mockOptions;

        public ReportsControllerTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockReportBusiness = this.mockRepository.Create<IReportBusiness>();
            this.mockUserBusiness = this.mockRepository.Create<IUserBusiness>();
            this.mockAuthorizationService = this.mockRepository.Create<IAuthorizationService>();
            this.mockOptions = this.mockRepository.Create<IOptions<PagingOptions>>();
        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private ReportsController CreateReportsController()
        {
            // common setups
            this.mockOptions.Setup(r => r.Value).Returns(new PagingOptions() { Limit = 2, Offset = 2 });

            var controllerObj = new ReportsController(
                this.mockReportBusiness.Object,
                this.mockUserBusiness.Object,
                this.mockAuthorizationService.Object,
                this.mockOptions.Object);

            controllerObj.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "123@Admin")
                    }, "someAuthTypeName"))
                }
            };

            return controllerObj;
        }

        [Fact]
        public async Task GetAllEventStatus_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateReportsController();

            this.mockAuthorizationService.Setup(
                r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                ).ReturnsAsync(AuthorizationResult.Success());

            List<EventStatus> evsts = new List<EventStatus>();
            evsts.Add(new EventStatus() { EventDate = DateTime.Now, EventName = "Desc 2", EventId = "1" });
            evsts.Add(new EventStatus() { EventDate = DateTime.Now, EventName = "Desc 1", EventId = "2" });
            PagedResults<EventStatus> sampleResult = new PagedResults<EventStatus>() { Items = evsts };

            mockReportBusiness.Setup(r => r.GetAllAsync(
                    It.IsAny<PagingOptions>(),
                    It.IsAny<SortOptions<Event, EventEntity>>(),
                    It.IsAny<SearchOptions<Event, EventEntity>>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(sampleResult);

            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Event, EventEntity> sortOptions = new SortOptions<Event, EventEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Event, EventEntity> searchOptions = new SearchOptions<Event, EventEntity>() { Search = new string[] { "" } };
            CancellationToken ct = new CancellationToken();

            // Act
            var result = await unitUnderTest.GetAllEventStatus(
                pagingOptions,
                sortOptions,
                searchOptions,
                ct);

            // Assert
            Assert.True(result.Value != null);
        }

        [Fact]
        public async Task GetAllEventStatus_StateUnderTest_ExpectedBehavior_UnAuth()
        {
            // Arrange
            var unitUnderTest = this.CreateReportsController();

            this.mockAuthorizationService.Setup(
                r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                ).ReturnsAsync(AuthorizationResult.Failed());

            this.mockUserBusiness.Setup(r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new Guid());

            List<EventStatus> evsts = new List<EventStatus>();
            evsts.Add(new EventStatus() { EventDate = DateTime.Now, EventName = "Desc 2", EventId = "1" });
            evsts.Add(new EventStatus() { EventDate = DateTime.Now, EventName = "Desc 1", EventId = "2" });
            PagedResults<EventStatus> sampleResult = new PagedResults<EventStatus>() { Items = evsts };

            mockReportBusiness.Setup(r => r.GetAllByPocAsync(
                    It.IsAny<PagingOptions>(),
                    It.IsAny<SortOptions<Event, EventEntity>>(),
                    It.IsAny<SearchOptions<Event, EventEntity>>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(sampleResult);

            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Event, EventEntity> sortOptions = new SortOptions<Event, EventEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Event, EventEntity> searchOptions = new SearchOptions<Event, EventEntity>() { Search = new string[] { "" } };
            CancellationToken ct = new CancellationToken();

            // Act
            var result = await unitUnderTest.GetAllEventStatus(
                pagingOptions,
                sortOptions,
                searchOptions,
                ct);

            // Assert
            Assert.True(result.Value != null);
        }

        [Fact]
        public async Task GetAllEventStatusReport_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateReportsController();

            this.mockAuthorizationService.Setup(
                r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                ).ReturnsAsync(AuthorizationResult.Success());

            List<EventStatusReport> evsts = new List<EventStatusReport>();
            evsts.Add(new EventStatusReport() { EventDate = DateTime.Now, EventName = "Desc 2", EventId = "1" });
            evsts.Add(new EventStatusReport() { EventDate = DateTime.Now, EventName = "Desc 1", EventId = "2" });
            PagedResults<EventStatusReport> sampleResult = new PagedResults<EventStatusReport>() { Items = evsts };

            mockReportBusiness.Setup(r => r.GetAllReportAsync(
                    It.IsAny<PagingOptions>(),
                    It.IsAny<SortOptions<Event, EventEntity>>(),
                    It.IsAny<SearchOptions<Event, EventEntity>>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(sampleResult);

            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Event, EventEntity> sortOptions = new SortOptions<Event, EventEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Event, EventEntity> searchOptions = new SearchOptions<Event, EventEntity>() { Search = new string[] { "" } };
            CancellationToken ct = new CancellationToken();
            bool report = false;

            // Act
            var result = await unitUnderTest.GetAllEventStatusReport(
                pagingOptions,
                sortOptions,
                searchOptions,
                report,
                ct);

            // Assert
            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async Task GetAllEventStatusReport_StateUnderTest_ExpectedBehavior_Unauth()
        {
            // Arrange
            var unitUnderTest = this.CreateReportsController();

            this.mockAuthorizationService.Setup(
                r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                ).ReturnsAsync(AuthorizationResult.Failed());

            this.mockUserBusiness.Setup(r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new Guid());

            List<EventStatusReport> evsts = new List<EventStatusReport>();
            evsts.Add(new EventStatusReport() { EventDate = DateTime.Now, EventName = "Desc 2", EventId = "1" });
            evsts.Add(new EventStatusReport() { EventDate = DateTime.Now, EventName = "Desc 1", EventId = "2" });
            PagedResults<EventStatusReport> sampleResult = new PagedResults<EventStatusReport>() { Items = evsts };

            mockReportBusiness.Setup(r => r.GetAllReportByPocAsync(
                    It.IsAny<PagingOptions>(),
                    It.IsAny<SortOptions<Event, EventEntity>>(),
                    It.IsAny<SearchOptions<Event, EventEntity>>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(sampleResult);

            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Event, EventEntity> sortOptions = new SortOptions<Event, EventEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Event, EventEntity> searchOptions = new SearchOptions<Event, EventEntity>() { Search = new string[] { "" } };
            CancellationToken ct = new CancellationToken();
            bool report = false;

            // Act
            var result = await unitUnderTest.GetAllEventStatusReport(
                pagingOptions,
                sortOptions,
                searchOptions,
                report,
                ct);

            // Assert
            Assert.IsType<FileContentResult>(result);
        }
    }
}
