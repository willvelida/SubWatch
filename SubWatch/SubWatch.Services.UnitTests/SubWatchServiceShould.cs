using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using SubWatch.Common.Models;
using SubWatch.Common.Request;
using SubWatch.Repository.Interfaces;
using SubWatch.Services.Interfaces;
using SubWatch.Services.Mappers;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SubWatch.Services.UnitTests
{
    public class SubWatchServiceShould
    {
        private Mock<ISubWatchValidator> _mockSubWatchValidator;
        private Mock<ISubWatchRepository> _mockSubWatchRepository;
        private Mock<ISubscriptionHelper> _mockSubscriptionHelper;
        private Mock<ILogger<SubWatchService>> _mockLogger;
        private Mock<HttpRequest> _mockHttpRequest;
        private IMapper _mapper;
        private SubWatchService _serviceUnderTest;

        public SubWatchServiceShould()
        {
            _mockSubWatchValidator = new Mock<ISubWatchValidator>();
            _mockSubWatchRepository = new Mock<ISubWatchRepository>();
            _mockSubscriptionHelper = new Mock<ISubscriptionHelper>();
            _mockLogger = new Mock<ILogger<SubWatchService>>();
            _mockHttpRequest = new Mock<HttpRequest>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapSubscriptionRequestDtoToSubscription());
            });
            var mapper = config.CreateMapper();
            _mapper = mapper;
            _serviceUnderTest = new SubWatchService(
                _mockSubWatchValidator.Object,
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
            subscriptionRequestDto.StartDate = new DateTime(2021, 12, 18);
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(subscriptionRequestDto));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _mockHttpRequest.Setup(r => r.Body).Returns(memoryStream);

            _mockSubWatchValidator.Setup(validator => validator.ValidateRequest(It.IsAny<HttpRequest>())).ReturnsAsync(subscriptionRequestDto);

            // Act
            Func<Task> subWatchService = async () => await _serviceUnderTest.AddSubscripion(_mockHttpRequest.Object);

            // Assert
            await subWatchService.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task ThrowExceptionWhenRepositoryCallFails()
        {
            // Arrange
            var fixture = new Fixture();
            var subscriptionRequestDto = fixture.Create<SubscriptionRequestDto>();
            subscriptionRequestDto.StartDate = new DateTime(2021, 12, 18);
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(subscriptionRequestDto));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _mockHttpRequest.Setup(r => r.Body).Returns(memoryStream);

            _mockSubWatchValidator.Setup(validator => validator.ValidateRequest(It.IsAny<HttpRequest>())).ReturnsAsync(subscriptionRequestDto);

            _mockSubWatchRepository.Setup(repo => repo.CreateSubscription(It.IsAny<Subscription>())).ThrowsAsync(new Exception("Exception thrown in AddSubscription: Oops!"));

            // Act
            Func<Task> subWatchService = async () => await _serviceUnderTest.AddSubscripion(_mockHttpRequest.Object);

            // Assert
            await subWatchService.Should().ThrowAsync<Exception>().WithMessage($"Exception thrown in AddSubscription: Oops!");
        }
    }
}