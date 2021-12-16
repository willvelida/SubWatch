using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SubWatch.Common.Models;
using SubWatch.Common.Request;
using SubWatch.Repository.Interfaces;
using SubWatch.Services.Interfaces;
using SubWatch.Services.Mappers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SubWatch.Services.UnitTests
{
    public class SubWatchServiceShould
    {
        private Mock<ISubWatchRepository> _mockSubWatchRepository;
        private Mock<ISubscriptionHelper> _mockSubscriptionHelper;
        private Mock<ILogger<SubWatchService>> _mockLogger;
        private IMapper _mapper;
        private SubWatchService _serviceUnderTest;

        public SubWatchServiceShould()
        {
            _mockSubWatchRepository = new Mock<ISubWatchRepository>();
            _mockSubscriptionHelper = new Mock<ISubscriptionHelper>();
            _mockLogger = new Mock<ILogger<SubWatchService>>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapSubscriptionRequestDtoToSubscription());
            });
            var mapper = config.CreateMapper();
            _mapper = mapper;
            _serviceUnderTest = new SubWatchService(
                _mockSubWatchRepository.Object,
                _mockSubscriptionHelper.Object,
                _mapper,
                _mockLogger.Object);
        }

        [Fact]
        public async Task SuccessfullyAddSubscription()
        {
            // Arrange
            var fixture = new Fixture();
            var subscriptionRequestDto = fixture.Create<SubscriptionRequestDto>();

            // Act
            Func<Task> subWatchService = async () => await _serviceUnderTest.AddSubscripion(subscriptionRequestDto);

            // Assert
            await subWatchService.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task ThrowExceptionWhenRepositoryCallFails()
        {
            // Arrange
            var fixture = new Fixture();
            var subscriptionRequestDto = fixture.Create<SubscriptionRequestDto>();

            _mockSubWatchRepository.Setup(repo => repo.CreateSubscription(It.IsAny<Subscription>())).ThrowsAsync(new Exception("Exception thrown in AddSubscription: Oops!"));

            // Act
            Func<Task> subWatchService = async () => await _serviceUnderTest.AddSubscripion(subscriptionRequestDto);

            // Assert
            await subWatchService.Should().ThrowAsync<Exception>().WithMessage($"Exception thrown in AddSubscription: Oops!");
        }
    }
}