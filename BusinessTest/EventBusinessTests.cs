using AutoMapper;
using Business;
using Entities;
using Models;
using Moq;
using Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BusinessTest
{
    public class EventBusinessTests : IDisposable
    {
        private MockRepository mockRepository;

        private Mock<IEventService> mockEventService;
        private Mock<IEmailService> mockEmailService;
        private Mock<IConfigurationProvider> mockConfigurationProvider;
        private Mock<IMapper> mockIMapper;

        public EventBusinessTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockEventService = this.mockRepository.Create<IEventService>();
            this.mockEmailService = this.mockRepository.Create<IEmailService>();
            this.mockConfigurationProvider = this.mockRepository.Create<IConfigurationProvider>();
            this.mockIMapper = this.mockRepository.Create<IMapper>();
        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private EventBusiness CreateEventBusiness()
        {
            // common setups 
            this.mockConfigurationProvider.Setup(r => r.CreateMapper()).Returns(this.mockIMapper.Object);

            return new EventBusiness(
                this.mockEventService.Object,
                this.mockEmailService.Object,
                this.mockConfigurationProvider.Object);
        }

        [Fact]
        public async Task GetAllAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateEventBusiness();

            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Event, EventEntity> sortOptions = new SortOptions<Event, EventEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Event, EventEntity> searchOptions = new SearchOptions<Event, EventEntity>() { Search = new string[] { "" } };
            CancellationToken ct = new CancellationToken();

            this.mockEventService.Setup(r => r.GetAllAsync(pagingOptions,
                sortOptions,
                searchOptions,
                ct)).ReturnsAsync(new PagedResults<Event>());

            // Act
            var result = await unitUnderTest.GetAllAsync(
                pagingOptions,
                sortOptions,
                searchOptions,
                ct);

            // Assert
            Assert.IsType<PagedResults<Event>>(result);
        }

        [Fact]
        public async Task GetAllByByPocAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateEventBusiness();

            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Event, EventEntity> sortOptions = new SortOptions<Event, EventEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Event, EventEntity> searchOptions = new SearchOptions<Event, EventEntity>() { Search = new string[] { "" } };
            CancellationToken ct = new CancellationToken();
            long pocId = 56346;

            this.mockEventService.Setup(r => r.GetAllByByPocAsync(pagingOptions,
                sortOptions,
                searchOptions,
                pocId,
                ct)).ReturnsAsync(new PagedResults<Event>());

            // Act
            var result = await unitUnderTest.GetAllByByPocAsync(
                pagingOptions,
                sortOptions,
                searchOptions,
                pocId,
                ct);

            // Assert
            Assert.IsType<PagedResults<Event>>(result);
        }

        [Fact]
        public async Task FindAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateEventBusiness();
            Guid id = new Guid();
            CancellationToken ct = new CancellationToken();

            List<Feedback> fbL = new List<Feedback>();
            fbL.Add(new Feedback() { Rating = 4 });
            fbL.Add(new Feedback() { Rating = 3 });
            fbL.Add(new Feedback() { Rating = 2 });
            fbL.Add(new Feedback() { Rating = 1 });

            this.mockEventService.Setup(r => r.FindAsync(
                id,
                ct)).ReturnsAsync(
                new Event()
                {
                    Feedback = fbL
                });

            // Act
            var result = await unitUnderTest.FindAsync(
                id,
                ct);

            // Assert
            Assert.IsType<Event>(result);
        }

        [Fact]
        public async Task AddAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateEventBusiness();
            List<Event> eventSummaryDetails = new List<Event>();

            this.mockEventService.Setup(r => r.AddAsync(eventSummaryDetails)).ReturnsAsync(true);

            // Act
            var result = await unitUnderTest.AddAsync(
                eventSummaryDetails);

            // Assert
            Assert.True(result);
        }
        [Fact]
        public async Task GetAllForExcelAsync_StateUnderTest_ExpectedBehavior_POC0()
        {
            // Arrange
            var unitUnderTest = this.CreateEventBusiness();

            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Event, EventEntity> sortOptions = new SortOptions<Event, EventEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Event, EventEntity> searchOptions = new SearchOptions<Event, EventEntity>() { Search = new string[] { "" } };
            CancellationToken ct = new CancellationToken();
            long pocId = 0;

            this.mockEventService.Setup(r => r.GetAllAsync(pagingOptions,
                sortOptions,
                searchOptions,
                ct)).ReturnsAsync(new PagedResults<Event>());

            this.mockIMapper.Setup(r => r.Map<PagedResults<EventReport>>(It.IsAny<object>())).Returns(new PagedResults<EventReport>());

            // Act
            var result = await unitUnderTest.GetAllForExcelAsync(
                pagingOptions,
                sortOptions,
                searchOptions,
                pocId,
                ct);

            // Assert
            Assert.IsType<PagedResults<EventReport>>(result);
        }

        [Fact]
        public async Task GetAllForExcelAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateEventBusiness();

            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Event, EventEntity> sortOptions = new SortOptions<Event, EventEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Event, EventEntity> searchOptions = new SearchOptions<Event, EventEntity>() { Search = new string[] { "" } };
            CancellationToken ct = new CancellationToken();
            long pocId = 56346;

            this.mockEventService.Setup(r => r.GetAllByByPocAsync(pagingOptions,
                sortOptions,
                searchOptions,
                pocId,
                ct)).ReturnsAsync(new PagedResults<Event>());

            this.mockIMapper.Setup(r => r.Map<PagedResults<EventReport>>(It.IsAny<object>())).Returns(new PagedResults<EventReport>() { });

            // Act
            var result = await unitUnderTest.GetAllForExcelAsync(
                pagingOptions,
                sortOptions,
                searchOptions,
                pocId,
                ct);

            // Assert
            Assert.IsType<PagedResults<EventReport>>(result);
        }

        [Fact]
        public async Task CreateReminderEmailAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateEventBusiness();
            Guid eventId = new Guid();

            this.mockEventService.Setup(r => r.FindAsync(
               eventId,
               CancellationToken.None)).ReturnsAsync(new Event());

            List<Feedback> fbL = new List<Feedback>();
            fbL.Add(new Feedback() { Rating = 4 });
            fbL.Add(new Feedback() { Rating = 3 });
            fbL.Add(new Feedback() { Rating = 2 });
            fbL.Add(new Feedback() { Rating = 1 });

            this.mockEventService.Setup(r => r.FindAsync(
                eventId,
                new CancellationToken())).ReturnsAsync(
                new Event()
                {
                    Feedback = fbL
                });

            // Act
            var result = await unitUnderTest.CreateReminderEmailAsync(
                eventId);

            // Assert
            Assert.IsType<Event>(result);
        }
    }
}
