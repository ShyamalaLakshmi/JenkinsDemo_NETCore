using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Api.Controllers;
using Api.Infrastructure;
using Business;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Models;
using Moq;
using Services;
using Xunit;

namespace ApiTest
{
    public class EventsControllerTests : IDisposable
    {
        private MockRepository mockRepository;

        private Mock<IEventBusiness> mockEventBusiness;
        private Mock<IUserBusiness> mockUserBusiness;
        private Mock<IAuthorizationService> mockAuthorizationService;
        private Mock<IRazorPartialToStringRenderer> mockRazorPartialToStringRenderer;
        private Mock<IEmailService> mockEmailService;
        private Mock<IOptions<PagingOptions>> mockOptions;
        private Mock<IConfiguration> mockConfiguration;

        public EventsControllerTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockEventBusiness = this.mockRepository.Create<IEventBusiness>();
            this.mockUserBusiness = this.mockRepository.Create<IUserBusiness>();
            this.mockAuthorizationService = this.mockRepository.Create<IAuthorizationService>();
            this.mockRazorPartialToStringRenderer = this.mockRepository.Create<IRazorPartialToStringRenderer>();
            this.mockEmailService = this.mockRepository.Create<IEmailService>();
            this.mockOptions = this.mockRepository.Create<IOptions<PagingOptions>>();
            this.mockConfiguration = this.mockRepository.Create<IConfiguration>();
        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private EventsController CreateEventsController()
        {
            // common setups
            this.mockOptions.Setup(r => r.Value).Returns(new PagingOptions() { Limit = 2, Offset = 2 });
             
            var controllerObj = new EventsController(
                this.mockEventBusiness.Object,
                this.mockUserBusiness.Object,
                this.mockAuthorizationService.Object,
                this.mockRazorPartialToStringRenderer.Object,
                this.mockEmailService.Object,
                this.mockConfiguration.Object,
                this.mockOptions.Object);

            //this.mockRepository.Create<ControllerContext>();
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
        public async Task GetAllEvents_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateEventsController();
            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Event, EventEntity> sortOptions = new SortOptions<Event, EventEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Event, EventEntity> searchOptions = new SearchOptions<Event, EventEntity>() { Search = new string[] { "" } };
            CancellationToken ct = new CancellationToken();

            this.mockAuthorizationService.Setup(
                r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                ).ReturnsAsync(AuthorizationResult.Success());

            List<Event> events = new List<Event>();
            events.Add(new Event()
            {
                Id = new Guid(),
                EventName = "Test",
                CreatedAt = DateTime.Now
            });
            events.Add(new Event()
            {
                Id = new Guid(),
                EventName = "Test 1",
                CreatedAt = DateTime.Now
            });
            PagedResults<Event> sampleResult = new PagedResults<Event>()
            {
                Items = events
            };

            this.mockEventBusiness.Setup(r => r.GetAllAsync(
                It.IsAny<PagingOptions>(),
                It.IsAny<SortOptions<Event, EventEntity>>(),
                It.IsAny<SearchOptions<Event, EventEntity>>(),
                It.IsAny<CancellationToken>())
                ).ReturnsAsync(sampleResult);

            // Act
            var result = await unitUnderTest.GetAllEvents(
                pagingOptions,
                sortOptions,
                searchOptions,
                ct);

            // Assert 
            Assert.True(result.Value != null);
        }

        [Fact]
        public async Task GetAllEvents_StateUnderTest_ExpectedBehavior_UnAuthorized()
        {
            // Arrange
            var unitUnderTest = this.CreateEventsController();
            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Event, EventEntity> sortOptions = new SortOptions<Event, EventEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Event, EventEntity> searchOptions = new SearchOptions<Event, EventEntity>() { Search = new string[] { "" } };
            CancellationToken ct = new CancellationToken();

            this.mockAuthorizationService.Setup(
                r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                ).ReturnsAsync(AuthorizationResult.Failed());

            List<Event> events = new List<Event>();
            events.Add(new Event()
            {
                Id = new Guid(),
                EventName = "Test",
                CreatedAt = DateTime.Now
            });
            events.Add(new Event()
            {
                Id = new Guid(),
                EventName = "Test 1",
                CreatedAt = DateTime.Now
            });
            PagedResults<Event> sampleResult = new PagedResults<Event>()
            {
                Items = events
            };

            this.mockUserBusiness.Setup(r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new Guid());

            this.mockEventBusiness.Setup(r => r.GetAllByByPocAsync(
                It.IsAny<PagingOptions>(),
                It.IsAny<SortOptions<Event, EventEntity>>(),
                It.IsAny<SearchOptions<Event, EventEntity>>(),
                 It.IsAny<long>(),
                It.IsAny<CancellationToken>())
                ).ReturnsAsync(sampleResult);

            // Act
            var result = await unitUnderTest.GetAllEvents(
                pagingOptions,
                sortOptions,
                searchOptions,
                ct);

            // Assert 
            Assert.True(result.Value != null);
        }

        [Fact]
        public async Task GetAllEventsExcel_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateEventsController();

            this.mockAuthorizationService.Setup(
                r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                ).ReturnsAsync(AuthorizationResult.Success());

            PagedResults<EventReport> sampleResult = new PagedResults<EventReport>
            {
                Items = Enumerable.Empty<EventReport>()
            };

            this.mockEventBusiness.Setup(r => r.GetAllForExcelAsync(
                It.IsAny<PagingOptions>(),
                It.IsAny<SortOptions<Event, EventEntity>>(),
                It.IsAny<SearchOptions<Event, EventEntity>>(),
                It.IsAny<long>(),
                It.IsAny<CancellationToken>())
                ).ReturnsAsync(sampleResult);

            bool report = false;
            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Event, EventEntity> sortOptions = new SortOptions<Event, EventEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Event, EventEntity> searchOptions = new SearchOptions<Event, EventEntity>() { Search = new string[] { "" } };
            CancellationToken ct = new CancellationToken();

            // Act
            var result = await unitUnderTest.GetAllEventsExcel(
                report,
                pagingOptions,
                sortOptions,
                searchOptions,
                ct);

            // Assert
            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async Task GetAllEventsExcel_StateUnderTest_ExpectedBehavior_UnauthorizedUser()
        {
            // Arrange
            var unitUnderTest = this.CreateEventsController();

            this.mockAuthorizationService.Setup(
                r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                ).ReturnsAsync(AuthorizationResult.Failed());

            PagedResults<EventReport> sampleResult = new PagedResults<EventReport>
            {
                Items = Enumerable.Empty<EventReport>()
            };

            this.mockUserBusiness.Setup(r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new Guid());

            this.mockEventBusiness.Setup(r => r.GetAllForExcelAsync(
                It.IsAny<PagingOptions>(),
                It.IsAny<SortOptions<Event, EventEntity>>(),
                It.IsAny<SearchOptions<Event, EventEntity>>(),
                It.IsAny<long>(),
                It.IsAny<CancellationToken>())
                ).ReturnsAsync(sampleResult);

            bool report = false;
            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Event, EventEntity> sortOptions = new SortOptions<Event, EventEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Event, EventEntity> searchOptions = new SearchOptions<Event, EventEntity>() { Search = new string[] { "" } };
            CancellationToken ct = new CancellationToken();

            // Act
            var result = await unitUnderTest.GetAllEventsExcel(
                report,
                pagingOptions,
                sortOptions,
                searchOptions,
                ct);

            // Assert
            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async Task GetEventById_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateEventsController();

            this.mockEventBusiness.Setup(r => r.FindAsync(
              It.IsAny<Guid>(),
              It.IsAny<CancellationToken>())
              ).ReturnsAsync(new Event() { });

            // Act
            var result = await unitUnderTest.GetEventById(
                new Guid(),
                new CancellationToken());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetEventById_StateUnderTest_ExpectedBehavior_NotFound()
        {
            // Arrange
            var unitUnderTest = this.CreateEventsController();

            this.mockEventBusiness.Setup(r => r.FindAsync(
              It.IsAny<Guid>(),
              It.IsAny<CancellationToken>())
              ).ReturnsAsync((Event)null);

            // Act
            var result = await unitUnderTest.GetEventById(
                new Guid(),
                new CancellationToken());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateReminderEmail_StateUnderTest_ExpectedBehavior_NotFound()
        {
            // Arrange
            var unitUnderTest = this.CreateEventsController();

            this.mockUserBusiness.Setup(r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((Guid?)null);

            // Act
            var result = await unitUnderTest.CreateReminderEmail(new ReminderEmailForm());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateReminderEmail_StateUnderTest_ExpectedBehavior()
        {
            var mockIConfigurationSection = this.mockRepository.Create<IConfigurationSection>();
            mockIConfigurationSection.Setup(r => r.Value).Returns("http://localhost:4200");

            this.mockConfiguration.Setup(r => r.GetSection(It.IsAny<string>())).Returns(mockIConfigurationSection.Object);

            // Arrange
            var unitUnderTest = this.CreateEventsController();

            this.mockUserBusiness.Setup(r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new Guid());

            this.mockRazorPartialToStringRenderer.Setup(r => r.RenderPartialToStringAsync(
               It.IsAny<string>(),
               It.IsAny<FeedbackRequestEmailTemplateModel>())
           ).ReturnsAsync("<html><b>sample amail</b></html>");

            this.mockEmailService.Setup(r => r.SendAsync(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>())
           ).Returns(Task.CompletedTask);

            List<Participant> prtcpnts = new List<Participant>();
            prtcpnts.Add(new Participant()
            {
                IsFeedbackReceived = false,
                Attended = true,
                NotAttended = false,
                EmployeeName = "EmployeeName",
                EmployeeId = "emp100"
            });
            var sampOutput = new Event()
            {
                EventName = "EventName",
                EventDate = DateTime.Now,
                Participant = prtcpnts
            };
            this.mockEventBusiness.Setup(r => r.FindAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(sampOutput);

            // Act
            var result = await unitUnderTest.CreateReminderEmail(new ReminderEmailForm());

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}
