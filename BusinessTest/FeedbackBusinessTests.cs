using AutoMapper;
using Business;
using Entities;
using Microsoft.Extensions.Options;
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
    public class FeedbackBusinessTests : IDisposable
    {
        private MockRepository mockRepository;

        private Mock<IFeedbackService> mockFeedbackService;
        private Mock<IUserService> mockUserService;
        private Mock<IQuestionService> mockQuestionService;
        private Mock<IEmailService> mockEmailService;
        private Mock<IAnswerService> mockAnswerService;
        private Mock<IParticipantService> mockParticipantService;
        private Mock<IConfigurationProvider> mockConfigurationProvider;
        private Mock<IOptions<PagingOptions>> mockOptions;
        private Mock<IMapper> mockIMapper;

        public FeedbackBusinessTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockFeedbackService = this.mockRepository.Create<IFeedbackService>();
            this.mockUserService = this.mockRepository.Create<IUserService>();
            this.mockQuestionService = this.mockRepository.Create<IQuestionService>();
            this.mockEmailService = this.mockRepository.Create<IEmailService>();
            this.mockAnswerService = this.mockRepository.Create<IAnswerService>();
            this.mockParticipantService = this.mockRepository.Create<IParticipantService>();
            this.mockConfigurationProvider = this.mockRepository.Create<IConfigurationProvider>();
            this.mockOptions = this.mockRepository.Create<IOptions<PagingOptions>>();
            this.mockIMapper = this.mockRepository.Create<IMapper>();
        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private FeedbackBusiness CreateFeedbackBusiness()
        {
            // common setups 
            this.mockOptions.Setup(r => r.Value).Returns(new PagingOptions() { Limit = 1, Offset = 1 });
            this.mockConfigurationProvider.Setup(r => r.CreateMapper()).Returns(this.mockIMapper.Object);

            return new FeedbackBusiness(
                this.mockFeedbackService.Object,
                this.mockUserService.Object,
                this.mockQuestionService.Object,
                this.mockEmailService.Object,
                this.mockAnswerService.Object,
                this.mockParticipantService.Object,
                this.mockConfigurationProvider.Object,
                this.mockOptions.Object);
        }

        [Fact]
        public async Task GetAllByFeedbackTypeAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateFeedbackBusiness();
            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Question, QuestionEntity> sortOptions = new SortOptions<Question, QuestionEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Question, QuestionEntity> searchOptions = new SearchOptions<Question, QuestionEntity>() { Search = new string[] { "" } };
            CancellationToken ct = new CancellationToken();
            string feedbackType = "test";

            this.mockFeedbackService.Setup(r => r.GetAllByFeedbackTypeAsync(pagingOptions,
                sortOptions,
                searchOptions,
                feedbackType,
                ct)).ReturnsAsync(new PagedResults<Question>());

            // Act
            var result = await unitUnderTest.GetAllByFeedbackTypeAsync(
                pagingOptions,
                sortOptions,
                searchOptions,
                feedbackType,
                ct);

            // Assert
            Assert.IsType<PagedResults<Question>>(result);
        }
        /*
        [Fact]
        public async Task AddAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateFeedbackBusiness();

            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Question, QuestionEntity> sortOptions = new SortOptions<Question, QuestionEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Question, QuestionEntity> searchOptions = new SearchOptions<Question, QuestionEntity>() { Search = new string[] { "" } };

            var guidS = Guid.NewGuid();
            var guidSA = Guid.NewGuid();
            List<ReasonForm> rfL = new List<ReasonForm>();
            rfL.Add(new ReasonForm() { QuestionId = guidS, AnswerId = guidSA.ToString() });
            FeedbackForm feedback = new FeedbackForm() { FeedbackType = "test", Reason = rfL, ParticipantId = Guid.NewGuid() };
            List<Answer> aL = new List<Answer>();
            aL.Add(new Answer() { Id = guidSA, Description = "1" });
            List<Question> qL = new List<Question>();
            qL.Add(new Question() { Id = guidS, CustomQuestion = true, Answers = aL, FreeTextQuestion = true, Description = "sample like improved" });
            var sampleRes = new PagedResults<Question>() { Items = qL, TotalSize = 1 };

            this.mockFeedbackService.Setup(r => r.GetAllByFeedbackTypeAsync(
                pagingOptions,
                sortOptions,
                searchOptions,
                feedback.FeedbackType,
                It.IsAny<CancellationToken>())).ReturnsAsync(sampleRes);

            this.mockFeedbackService.Setup(r => r.AddAsync(
               It.IsAny<Feedback>(),
               It.IsAny<Guid>())).ReturnsAsync(new Guid());

            var pnt = new Participant() { Id = new Guid(), EmployeeId = "tests" };
            this.mockParticipantService.Setup(r => r.FindAsync(
               feedback.ParticipantId,
               It.IsAny<CancellationToken>())).ReturnsAsync(pnt);

            this.mockParticipantService.Setup(r => r.UpdateAsync(It.IsAny<Participant>())).ReturnsAsync(new Participant() { });

            this.mockEmailService.Setup(r => r.SendAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
                ));//.Returns(It.IsAny<Task>());

            // Act
            var res = unitUnderTest.AddAsync(
                pagingOptions,
                sortOptions,
                searchOptions,
                feedback);

            // Assert
            await Assert.IsType<Task>(res);
        }
        */
    }
}
