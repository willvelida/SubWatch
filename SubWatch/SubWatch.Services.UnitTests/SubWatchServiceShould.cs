using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SubWatch.Common.Models;
using SubWatch.Common.Request;
using SubWatch.Repository.Interfaces;
using Xunit;

namespace SubWatch.Services.UnitTests
{
    public class SubWatchServiceShould
    {
        private Mock<ISubWatchRepository> _mockSubWatchRepository;
        private Mock<ILogger<SubWatchService>> _mockLogger;
        private SubWatchService _serviceUnderTest;

        public SubWatchServiceShould()
        {
            _mockSubWatchRepository = new Mock<ISubWatchRepository>();
            _mockLogger = new Mock<ILogger<SubWatchService>>();
            _serviceUnderTest = new SubWatchService(_mockSubWatchRepository.Object, _mockLogger.Object);
        }

        [Theory]
        [InlineData(RenewalFrequency.Weekly)]
        [InlineData(RenewalFrequency.Fortnightly)]
        [InlineData(RenewalFrequency.Monthly)]
        [InlineData(RenewalFrequency.Yearly)]
        [InlineData(RenewalFrequency.Quarterly)]
        [InlineData(RenewalFrequency.HalfYearly)]
        public void CalculateTotalCostCorrectly(RenewalFrequency renewalFrequency)
        {
            // Arrange
            var fixture = new Fixture();
            var subscriptionRequestDto = fixture.Create<SubscriptionRequestDto>();
            subscriptionRequestDto.RenewalCost = 10.00;
            subscriptionRequestDto.RenewalFrequency = renewalFrequency;

            // Act
            var expectedCost = _serviceUnderTest.CalculateTotalCost(subscriptionRequestDto);

            // Assert
            switch (renewalFrequency)
            {
                case RenewalFrequency.Weekly:
                    Assert.Equal(520.00, expectedCost);
                    break;
                case RenewalFrequency.Fortnightly:
                    Assert.Equal(260.00, expectedCost);
                    break;
                case RenewalFrequency.Monthly:
                    Assert.Equal(120.00, expectedCost);
                    break;
                case RenewalFrequency.Quarterly:
                    Assert.Equal(40.00, expectedCost);
                    break;
                case RenewalFrequency.HalfYearly:
                    Assert.Equal(20.00, expectedCost);
                    break;
                default:
                    Assert.Equal(10.00, expectedCost);
                    break;
            }
        }
    }
}