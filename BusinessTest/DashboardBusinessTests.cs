using AutoMapper;
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
    public class DashboardBusinessTests : IDisposable
    {
        private MockRepository mockRepository;

        private Mock<IDashboardService> mockDashboardService;
        private Mock<IConfigurationProvider> mockConfigurationProvider;
        private Mock<IMapper> mockIMapper;

        public DashboardBusinessTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockDashboardService = this.mockRepository.Create<IDashboardService>();
            this.mockConfigurationProvider = this.mockRepository.Create<IConfigurationProvider>();
            this.mockIMapper = this.mockRepository.Create<IMapper>();
        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private DashboardBusiness CreateDashboardBusiness()
        {
            // common setups 
            this.mockConfigurationProvider.Setup(r => r.CreateMapper()).Returns(this.mockIMapper.Object);

            return new DashboardBusiness(
                this.mockDashboardService.Object,
                this.mockConfigurationProvider.Object);
        }

        [Fact]
        public async Task GetStatsAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = this.CreateDashboardBusiness();
            long id = 63434;
            CancellationToken ct = new CancellationToken();

            Event[] outp = new Event[] { };
            this.mockDashboardService.Setup(r=>r.GetStatsAsync(id, ct)).ReturnsAsync(outp);

            // Act
            var result = await unitUnderTest.GetStatsAsync(
                id,
                ct);

            // Assert
            Assert.IsType<Dashboard>(result);
        }
    }
}
