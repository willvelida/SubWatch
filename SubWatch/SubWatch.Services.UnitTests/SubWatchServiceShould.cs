using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using SubWatch.Common.Exceptions;
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
            subscriptionRequestDto.StartDate = "18/12/2021";
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
            subscriptionRequestDto.StartDate = "18/12/2021";
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

        [Fact]
        public async Task RetrieveSubscriptionSuccessfully()
        {
            // Arrange
            var fixture = new Fixture();
            var subscription = fixture.Create<Subscription>();

            _mockSubWatchRepository.Setup(repo => repo.GetSubscription(It.IsAny<string>())).ReturnsAsync(subscription);

            // Act
            var expectedResponse = await _serviceUnderTest.RetrieveSubscription(subscription.Id);

            // Assert
            using (new AssertionScope())
            {
                expectedResponse.Id.Should().Be(subscription.Id);
                expectedResponse.Name.Should().Be(subscription.Name);
                expectedResponse.SubscriptionType.Should().Be(subscription.SubscriptionType);
                expectedResponse.RenewalCost.Should().Be(subscription.RenewalCost);
                expectedResponse.RenewalDate.Should().Be(subscription.RenewalDate);
                expectedResponse.RenewalFrequency.Should().Be(subscription.RenewalFrequency);
            }
        }

        [Fact]
        public async Task ThrowNotFoundExceptionWhenRepositoryClassReturnsNull()
        {
            // Arrange
            _mockSubWatchRepository.Setup(repo => repo.GetSubscription(It.IsAny<string>())).Returns(Task.FromResult<Subscription>(null));

            // Act
            Func<Task> subWatchService = async () => await _serviceUnderTest.RetrieveSubscription("1");

            // Assert
            await subWatchService.Should().ThrowAsync<NotFoundException>().WithMessage($"No subscription with ID 1 exists!");
        }

        [Fact]
        public async Task SuccessfullyDeleteSubscription()
        {
            // Arrange
            _mockSubWatchRepository.Setup(repo => repo.DeleteSubscription(It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            Func<Task> subWatchService = async () => await _serviceUnderTest.DeleteSubscription("1");

            // Assert
            await subWatchService.Should().NotThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task ThrowNotFoundExceptionWhenRepositoryCantDelete()
        {
            // Arrange
            _mockSubWatchRepository.Setup(repo => repo.DeleteSubscription(It.IsAny<string>())).ThrowsAsync(new NotFoundException("No Subscription with ID: 1 found! Delete failed"));

            // Act
            Func<Task> subWatchService = async () => await _serviceUnderTest.DeleteSubscription("1");

            // Assert
            await subWatchService.Should().ThrowAsync<NotFoundException>().WithMessage($"No Subscription with ID: 1 found! Delete failed");
        }

        [Fact]
        public async Task ThrowBadRequestExceptionWhenUpdateRequestDtoIsInvalid()
        {
            // Arrange
            var fixture = new Fixture();
            var subscription = fixture.Create<Subscription>();
            var subscriptionRequestDto = fixture.Create<SubscriptionRequestDto>();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(subscriptionRequestDto));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _mockHttpRequest.Setup(r => r.Body).Returns(memoryStream);

            _mockSubWatchRepository.Setup(repo => repo.GetSubscription(It.IsAny<string>())).ReturnsAsync(subscription);

            _mockSubWatchValidator.Setup(validator => validator.ValidateRequest(It.IsAny<HttpRequest>())).ThrowsAsync(new BadRequestException("Oops!!"));

            // Act
            Func<Task> subWatchService = async () => await _serviceUnderTest.UpdateSubscription("1", _mockHttpRequest.Object);

            // Assert
            await subWatchService.Should().ThrowAsync<BadRequestException>().WithMessage($"Oops!!");
        }

        [Fact]
        public async Task ThrowNotFoundExceptionWhenSubscriptionToUpdateIsNull()
        {
            // Arrange
            var fixture = new Fixture();
            var subscription = fixture.Create<Subscription>();
            var subscriptionRequestDto = fixture.Create<SubscriptionRequestDto>();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(subscriptionRequestDto));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _mockHttpRequest.Setup(r => r.Body).Returns(memoryStream);

            _mockSubWatchRepository.Setup(repo => repo.GetSubscription(It.IsAny<string>())).Returns(Task.FromResult<Subscription>(null));

            // Act
            Func<Task> subWatchService = async () => await _serviceUnderTest.UpdateSubscription("1", _mockHttpRequest.Object);

            // Assert
            await subWatchService.Should().ThrowAsync<NotFoundException>().WithMessage($"Subscription with ID: 1 not found!");
        }
    }
}