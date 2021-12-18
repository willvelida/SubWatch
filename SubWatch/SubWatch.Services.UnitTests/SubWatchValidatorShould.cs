using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using SubWatch.Common.Exceptions;
using SubWatch.Common.Models;
using SubWatch.Common.Request;
using SubWatch.Services.Validators;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SubWatch.Services.UnitTests
{
    public class SubWatchValidatorShould
    {
        private SubWatchValidator _serviceUnderTest;
        private Mock<HttpRequest> _mockHttpRequest;

        public SubWatchValidatorShould()
        {
            _serviceUnderTest = new SubWatchValidator();
            _mockHttpRequest = new Mock<HttpRequest>();
        }

        [Fact]
        public async Task ReturnValidSubscriptionRequestDto()
        {
            // Arrange
            var subscriptionDto = new SubscriptionRequestDto
            {
                Name = "Netflix",
                SubscriptionType = "Online",
                StartDate = "18/12/2021",
                RenewalCost = 14.99,
                RenewalFrequency = RenewalFrequency.Monthly
            };
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(subscriptionDto));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _mockHttpRequest.Setup(r => r.Body).Returns(memoryStream);

            // Act
            var expectedValidDto = await _serviceUnderTest.ValidateRequest(_mockHttpRequest.Object);

            // Assert
            using (new AssertionScope())
            {
                expectedValidDto.Name.Should().Be(subscriptionDto.Name);
                expectedValidDto.SubscriptionType.Should().Be(subscriptionDto.SubscriptionType);
                expectedValidDto.StartDate.Should().Be(subscriptionDto.StartDate);
                expectedValidDto.RenewalCost.Should().Be(subscriptionDto.RenewalCost);
                expectedValidDto.RenewalFrequency.Should().Be(subscriptionDto.RenewalFrequency);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task ThrowBadRequestExceptionWhenNameIsNullOrWhitespace(string name)
        {
            // Arrange
            var fixture = new Fixture();
            var subscriptionDto = fixture.Create<SubscriptionRequestDto>();
            subscriptionDto.Name = name;
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(subscriptionDto));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _mockHttpRequest.Setup(r => r.Body).Returns(memoryStream);

            // Act
            Func<Task> validatorAction = async () => await _serviceUnderTest.ValidateRequest(_mockHttpRequest.Object);

            // Assert
            await validatorAction.Should().ThrowAsync<BadRequestException>().WithMessage($"Subscription name cannot be null or empty");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task ThrowBadRequestExceptionWhenSubscriptionTypeIsNullOrWhitespace(string type)
        {
            // Arrange
            var fixture = new Fixture();
            var subscriptionDto = fixture.Create<SubscriptionRequestDto>();
            subscriptionDto.SubscriptionType = type;
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(subscriptionDto));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _mockHttpRequest.Setup(r => r.Body).Returns(memoryStream);

            // Act
            Func<Task> validatorAction = async () => await _serviceUnderTest.ValidateRequest(_mockHttpRequest.Object);

            // Assert
            await validatorAction.Should().ThrowAsync<BadRequestException>().WithMessage($"Subscription type cannot be null or empty");
        }
    }
}
