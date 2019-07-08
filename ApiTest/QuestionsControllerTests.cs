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
    public class QuestionsControllerTests : IDisposable
    {
        private MockRepository mockRepository;

        private Mock<IQuestionBusiness> mockQuestionBusiness;
        private Mock<IUserBusiness> mockUserBusiness;
        private Mock<IOptions<PagingOptions>> mockOptions;

        public QuestionsControllerTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockQuestionBusiness = this.mockRepository.Create<IQuestionBusiness>();
            this.mockUserBusiness = this.mockRepository.Create<IUserBusiness>();
            this.mockOptions = this.mockRepository.Create<IOptions<PagingOptions>>();
        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private QuestionsController CreateQuestionsController()
        {
            // common setups
            this.mockOptions.Setup(r => r.Value).Returns(new PagingOptions() { Limit = 2, Offset = 2 });

            var controllerObj = new QuestionsController(
                this.mockQuestionBusiness.Object,
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
        public async Task GetAllQuestions_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateQuestionsController();

            List<Question> qstns = new List<Question>();
            qstns.Add(new Question() { Active = true, Description = "Desc 2", Id = Guid.NewGuid() });
            qstns.Add(new Question() { Active = true, Description = "Desc 1", Id = Guid.NewGuid() });
            PagedResults<Question> sampleResult = new PagedResults<Question>() { Items = qstns };

            mockQuestionBusiness.Setup(r => r.GetAllAsync(
                    It.IsAny<PagingOptions>(),
                    It.IsAny<SortOptions<Question, QuestionEntity>>(),
                    It.IsAny<SearchOptions<Question, QuestionEntity>>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(sampleResult);

            PagingOptions pagingOptions = new PagingOptions() { Limit = 1, Offset = 1 };
            SortOptions<Question, QuestionEntity> sortOptions = new SortOptions<Question, QuestionEntity>() { OrderBy = new string[] { "" } };
            SearchOptions<Question, QuestionEntity> searchOptions = new SearchOptions<Question, QuestionEntity>() { Search = new string[] { "" } };
            CancellationToken ct = new CancellationToken();

            // Act
            var result = await unitUnderTest.GetAllQuestions(
                pagingOptions,
                sortOptions,
                searchOptions,
                ct);

            // Assert
            Assert.True(result.Value != null);
        }

        [Fact]
        public async Task GetQuestionById_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateQuestionsController();

            mockQuestionBusiness.Setup(r => r.FindAsync(
                  It.IsAny<Guid>(),
                  It.IsAny<CancellationToken>())).ReturnsAsync(new Question() { });

            // Act
            var result = await unitUnderTest.GetQuestionById(
                new Guid(),
                new CancellationToken());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetQuestionById_StateUnderTest_ExpectedBehavior_NotFound()
        {
            // Arrange
            var unitUnderTest = this.CreateQuestionsController();

            mockQuestionBusiness.Setup(r => r.FindAsync(
                  It.IsAny<Guid>(),
                  It.IsAny<CancellationToken>())).ReturnsAsync((Question)null);

            // Act
            var result = await unitUnderTest.GetQuestionById(
                new Guid(),
                new CancellationToken());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateQuestion_StateUnderTest_ExpectedBehavior_Badresult()
        {
            // Arrange
            var unitUnderTest = this.CreateQuestionsController();

            // Act
            var result = await unitUnderTest.UpdateQuestion(
                new Guid(),
                new QuestionForm() { Id = Guid.NewGuid() });

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateQuestion_StateUnderTest_ExpectedBehavior_NullUser()
        {
            // Arrange
            var unitUnderTest = this.CreateQuestionsController();

            mockUserBusiness.Setup(r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((Guid?)null);

            Guid guid = Guid.NewGuid();
            // Act
            var result = await unitUnderTest.UpdateQuestion(
                guid,
                new QuestionForm() { Id = guid });

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateQuestion_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateQuestionsController();

            mockUserBusiness.Setup(r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new Guid());

            mockQuestionBusiness.Setup(r => r.UpdateAsync(It.IsAny<QuestionForm>(), It.IsAny<Guid>())).ReturnsAsync(new Question());

            Guid guid = Guid.NewGuid();
            // Act
            var result = await unitUnderTest.UpdateQuestion(
                guid,
                new QuestionForm() { Id = guid });

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
        /*
            [Fact]
            public async Task UpdateQuestion_StateUnderTest_ExpectedBehavior_Exception()
            {
                // Arrange
                var unitUnderTest = this.CreateQuestionsController();

                mockUserBusiness.Setup(r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new Guid());

                mockQuestionBusiness.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

                mockQuestionBusiness.Setup(r => r.UpdateAsync(
                    It.IsAny<QuestionForm>(),
                    It.IsAny<Guid>())
                    ).Throws(new DbUpdateConcurrencyException("sample", It.IsAny<IReadOnlyList<IUpdateEntry>>()));

                Guid guid = Guid.NewGuid();
                // Act
                var result = await unitUnderTest.UpdateQuestion(
                    guid,
                    new QuestionForm() { Id = guid });

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }*/

        [Fact]
        public async Task CreateQuestion_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateQuestionsController();

            var mockCtxtUrl = this.mockRepository.Create<IUrlHelper>();
            mockCtxtUrl.Setup(r => r.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("sample link");

            unitUnderTest.Url = mockCtxtUrl.Object;

            mockUserBusiness.Setup(r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(Guid.NewGuid());

            mockQuestionBusiness.Setup(r => r.AddAsync(It.IsAny<QuestionForm>(), It.IsAny<Guid>())).ReturnsAsync(Guid.NewGuid());

            // Act
            var result = await unitUnderTest.CreateQuestion(new QuestionForm());

            // Assert
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task CreateQuestion_StateUnderTest_ExpectedBehavior_Noresult()
        {
            // Arrange
            var unitUnderTest = this.CreateQuestionsController();

            mockUserBusiness.Setup(r => r.GetUserIdAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((Guid?)null);

            // Act
            var result = await unitUnderTest.CreateQuestion(new QuestionForm());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteQuestion_StateUnderTest_ExpectedBehavior_Noresult()
        {
            // Arrange
            var unitUnderTest = this.CreateQuestionsController();

            mockQuestionBusiness.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            // Act
            var result = await unitUnderTest.DeleteQuestion(new Guid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteQuestion_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateQuestionsController();

            mockQuestionBusiness.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            mockQuestionBusiness.Setup(r => r.RemoveAsync(It.IsAny<Guid>())).ReturnsAsync(new Question());

            // Act
            var result = await unitUnderTest.DeleteQuestion(new Guid());

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}
