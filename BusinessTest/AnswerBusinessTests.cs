using AutoMapper;
using Business;
using Microsoft.Extensions.Options;
using Models;
using Moq;
using Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BusinessTest
{
    public class AnswerBusinessTests : IDisposable
    {
        private MockRepository mockRepository;

        private Mock<IAnswerService> mockAnswerService;
        private Mock<IConfigurationProvider> mockConfigurationProvider;
        private Mock<IOptions<PagingOptions>> mockOptions;
        private Mock<IMapper> mockIMapper;

        public AnswerBusinessTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockAnswerService = this.mockRepository.Create<IAnswerService>();
            this.mockConfigurationProvider = this.mockRepository.Create<IConfigurationProvider>();
            this.mockOptions = this.mockRepository.Create<IOptions<PagingOptions>>();
            this.mockIMapper = this.mockRepository.Create<IMapper>();
        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private AnswerBusiness CreateAnswerBusiness()
        {
            // common setups
            this.mockOptions.Setup(r => r.Value).Returns(new PagingOptions() { Limit = 1, Offset = 1 });
            
            this.mockConfigurationProvider.Setup(r => r.CreateMapper()).Returns(this.mockIMapper.Object);

            return new AnswerBusiness(
                this.mockAnswerService.Object,
                this.mockConfigurationProvider.Object,
                this.mockOptions.Object);
        }

        [Fact]
        public async Task FindAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateAnswerBusiness();

            Guid id = new Guid();
            CancellationToken ct = new CancellationToken();

            this.mockAnswerService.Setup(r => r.FindAsync(id, ct)).ReturnsAsync(new Answer() { });

            // Act
            var result = await unitUnderTest.FindAsync(
                id,
                ct);

            // Assert
            Assert.IsType<Answer>(result);
        }
    }
}
