using System;
using System.Collections.Generic;
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
    public class TaskControllerTests : IDisposable
    {
        private MockRepository mockRepository;

        private Mock<MockHostingEnvironment> mockHostingEnvironment;
        private Mock<IEventBusiness> mockEventBusiness;
        private Mock<IReportBusiness> mockReportBusiness;
        private Mock<IRazorPartialToStringRenderer> mockRazorPartialToStringRenderer;
        private Mock<IEmailService> mockEmailService;
        private Mock<IAuthorizationService> mockAuthorizationService;
        private Mock<IUserBusiness> mockUserBusiness;
        private Mock<IOptions<PagingOptions>> mockOptions;
        private Mock<IConfiguration> mockConfiguration;

        public TaskControllerTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockHostingEnvironment = this.mockRepository.Create<MockHostingEnvironment>();
            this.mockEventBusiness = this.mockRepository.Create<IEventBusiness>();
            this.mockReportBusiness = this.mockRepository.Create<IReportBusiness>();
            this.mockRazorPartialToStringRenderer = this.mockRepository.Create<IRazorPartialToStringRenderer>();
            this.mockEmailService = this.mockRepository.Create<IEmailService>();
            this.mockAuthorizationService = this.mockRepository.Create<IAuthorizationService>();
            this.mockUserBusiness = this.mockRepository.Create<IUserBusiness>();
            this.mockOptions = this.mockRepository.Create<IOptions<PagingOptions>>();
            this.mockConfiguration = this.mockRepository.Create<IConfiguration>();
        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private TaskController CreateTaskController()
        {
            // common setups
            this.mockOptions.Setup(r => r.Value).Returns(new PagingOptions() { Limit = 2, Offset = 2 });

            var controllerObj = new TaskController(
                this.mockHostingEnvironment.Object,
                this.mockEventBusiness.Object,
                this.mockReportBusiness.Object,
                this.mockRazorPartialToStringRenderer.Object,
                this.mockEmailService.Object,
                this.mockAuthorizationService.Object,
                this.mockUserBusiness.Object,
                this.mockConfiguration.Object,
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
        public async Task SendReminderEmailForAllEvents_StateUnderTest_ExpectedBehavior()
        {
            var mockIConfigurationSection = this.mockRepository.Create<IConfigurationSection>();
            mockIConfigurationSection.Setup(r => r.Value).Returns("http://localhost:4200");

            this.mockConfiguration.Setup(r => r.GetSection(It.IsAny<string>())).Returns(mockIConfigurationSection.Object);

            // Arrange
            var unitUnderTest = this.CreateTaskController();

            List<Participant> prtcpnts = new List<Participant>();
            prtcpnts.Add(new Participant()
            {
                IsFeedbackReceived = false,
                Attended = true,
                NotAttended = false,
                EmployeeName = "EmployeeName",
                EmployeeId = "emp100"
            });
            List<Event> evsts = new List<Event>();
            evsts.Add(new Event()
            {
                EventDate = DateTime.Now,
                EventName = "Desc 2",
                EventId = "1",
                Participant = prtcpnts
            });
            PagedResults<Event> sampleResult = new PagedResults<Event>() { Items = evsts };

            this.mockEventBusiness.Setup(r => r.GetAllAsync(
                    It.IsAny<PagingOptions>(),
                    It.IsAny<SortOptions<Event, EventEntity>>(),
                    It.IsAny<SearchOptions<Event, EventEntity>>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(sampleResult);

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

            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Event, EventEntity> sortOptions = new SortOptions<Event, EventEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Event, EventEntity> searchOptions = new SearchOptions<Event, EventEntity>() { Search = new string[] { "" } };
            CancellationToken ct = new CancellationToken();

            // Act
            var result = await unitUnderTest.SendReminderEmailForAllEvents(
                pagingOptions,
                sortOptions,
                searchOptions,
                ct);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task EmailEventStatusReport_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateTaskController();

            //this.mockHostingEnvironment.SetupProperty(r => r.WebRootPath, GetApplicationRoot());

            this.mockAuthorizationService.Setup(
                r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                ).ReturnsAsync(AuthorizationResult.Success());

            List<EventStatusReport> evsts = new List<EventStatusReport>();
            evsts.Add(new EventStatusReport()
            {
                EventDate = DateTime.Now,
                EventName = "Desc 2",
                EventId = "1"
            });
            PagedResults<EventStatusReport> sampleResult = new PagedResults<EventStatusReport>() { Items = evsts };

            this.mockReportBusiness.Setup(r => r.GetAllReportAsync(
                It.IsAny<PagingOptions>(),
                It.IsAny<SortOptions<Event, EventEntity>>(),
                It.IsAny<SearchOptions<Event, EventEntity>>(),
                It.IsAny<CancellationToken>())
                ).ReturnsAsync(sampleResult);

            this.mockEmailService.Setup(r => r.SendAsync(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>())
           ).Returns(Task.CompletedTask);

            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Event, EventEntity> sortOptions = new SortOptions<Event, EventEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Event, EventEntity> searchOptions = new SearchOptions<Event, EventEntity>() { Search = new string[] { "" } };
            ReportEmailForm form = new ReportEmailForm() { Email = "sample" };

            // Act
            var result = await unitUnderTest.EmailEventStatusReport(
                pagingOptions,
                sortOptions,
                searchOptions,
                form);

            // delete created file after save
            // string dirPath = this.mockHostingEnvironment.Object.WebRootPath + "/EventRatingReport";
            //  Directory.Delete(dirPath);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task EmailEventStatusReport_StateUnderTest_ExpectedBehavior_NoAuth()
        {
            // Arrange
            var unitUnderTest = this.CreateTaskController();

            //this.mockHostingEnvironment.SetupProperty(r => r.WebRootPath, GetApplicationRoot());

            this.mockAuthorizationService.Setup(
                r => r.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>(), It.IsAny<string>())
                ).ReturnsAsync(AuthorizationResult.Failed());

            this.mockUserBusiness.Setup(
                r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())
                ).ReturnsAsync(new Guid());

            List<EventStatusReport> evsts = new List<EventStatusReport>();
            evsts.Add(new EventStatusReport()
            {
                EventDate = DateTime.Now,
                EventName = "Desc 2",
                EventId = "1"
            });
            PagedResults<EventStatusReport> sampleResult = new PagedResults<EventStatusReport>() { Items = evsts };

            this.mockReportBusiness.Setup(r => r.GetAllReportByPocAsync(
                It.IsAny<PagingOptions>(),
                It.IsAny<SortOptions<Event, EventEntity>>(),
                It.IsAny<SearchOptions<Event, EventEntity>>(),
                It.IsAny<long>(),
                It.IsAny<CancellationToken>())
                ).ReturnsAsync(sampleResult);

            this.mockEmailService.Setup(r => r.SendAsync(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>())
           ).Returns(Task.CompletedTask);

            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Event, EventEntity> sortOptions = new SortOptions<Event, EventEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Event, EventEntity> searchOptions = new SearchOptions<Event, EventEntity>() { Search = new string[] { "" } };
            ReportEmailForm form = new ReportEmailForm() { Email = "sample" };

            // Act
            var result = await unitUnderTest.EmailEventStatusReport(
                pagingOptions,
                sortOptions,
                searchOptions,
                form);

            // delete created file after save
            // string dirPath = this.mockHostingEnvironment.Object.WebRootPath + "/EventRatingReport";
            //  Directory.Delete(dirPath);

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}
