using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Api.Controllers;
using Business;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Moq;
using Xunit;

namespace ApiTest
{
    public class FeedbackControllerTests : IDisposable
    {
        private MockRepository mockRepository;

        private Mock<IFeedbackBusiness> mockFeedbackBusiness;
        private Mock<IUserBusiness> mockUserBusiness;
        private Mock<IOptions<PagingOptions>> mockOptions;

        public FeedbackControllerTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockFeedbackBusiness = this.mockRepository.Create<IFeedbackBusiness>();
            this.mockUserBusiness = this.mockRepository.Create<IUserBusiness>();
            this.mockOptions = this.mockRepository.Create<IOptions<PagingOptions>>();
        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private FeedbackController CreateFeedbackController()
        {
            // common setups
            this.mockOptions.Setup(r => r.Value).Returns(new PagingOptions() { Limit = 2, Offset = 2 });

            var controllerObj = new FeedbackController(
                this.mockFeedbackBusiness.Object,
                this.mockUserBusiness.Object,
                this.mockOptions.Object);

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
        public async Task GetAllQuestionsByFeedbackType_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateFeedbackController();

            List<Question> qstns = new List<Question>();
            qstns.Add(new Question() { Active = true, Description = "Desc 2", Id = Guid.NewGuid() });
            qstns.Add(new Question() { Active = true, Description = "Desc 1", Id = Guid.NewGuid() });
            PagedResults<Question> sampleResult = new PagedResults<Question>() { Items = qstns };

            mockFeedbackBusiness.Setup(r => r.GetAllByFeedbackTypeAsync(
                    It.IsAny<PagingOptions>(),
                    It.IsAny<SortOptions<Question, QuestionEntity>>(),
                    It.IsAny<SearchOptions<Question, QuestionEntity>>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(sampleResult);

            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Question, QuestionEntity> sortOptions = new SortOptions<Question, QuestionEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Question, QuestionEntity> searchOptions = new SearchOptions<Question, QuestionEntity>() { Search = new string[] { "" } };
            string feedbackType = "";
            CancellationToken ct = new CancellationToken();

            // Act
            var result = await unitUnderTest.GetAllQuestionsByFeedbackType(
                pagingOptions,
                sortOptions,
                searchOptions,
                feedbackType,
                ct);

            // Assert
            Assert.True(result.Value != null);
        }

        [Fact]
        public async Task CreateFeedback_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateFeedbackController();

            mockFeedbackBusiness.Setup(r => r.AddAsync(
                    It.IsAny<PagingOptions>(),
                    It.IsAny<SortOptions<Question, QuestionEntity>>(),
                    It.IsAny<SearchOptions<Question, QuestionEntity>>(),
                    It.IsAny<FeedbackForm>())).Returns(Task.CompletedTask);

            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Question, QuestionEntity> sortOptions = new SortOptions<Question, QuestionEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Question, QuestionEntity> searchOptions = new SearchOptions<Question, QuestionEntity>() { Search = new string[] { "" } };
            FeedbackForm feedback = new FeedbackForm();

            // Act
            var result = await unitUnderTest.CreateFeedback(
                pagingOptions,
                sortOptions,
                searchOptions,
                feedback);

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}
