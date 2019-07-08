using Business;
using Models;
using Moq;
using Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BusinessTest
{
    public class ParticipantBusinessTests : IDisposable
    {
        private MockRepository mockRepository;

        private Mock<IParticipantService> mockParticipantService;

        public ParticipantBusinessTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockParticipantService = this.mockRepository.Create<IParticipantService>();
        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private ParticipantBusiness CreateParticipantBusiness()
        {
            return new ParticipantBusiness(
                this.mockParticipantService.Object);
        }

        [Fact]
        public async Task FindAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateParticipantBusiness();
            Guid id = new Guid();
            CancellationToken ct = new CancellationToken();

            this.mockParticipantService.Setup(r => r.FindAsync(
                id,
                ct)).ReturnsAsync(new Participant() { });

            // Act
            var result = await unitUnderTest.FindAsync(
                id,
                ct);

            // Assert
            Assert.IsType<Participant>(result);
        }
    }
}
