using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using SubWatch.Common.Models;
using SubWatch.Common.Request;
using SubWatch.Services.Mappers;
using System;
using Xunit;

namespace SubWatch.Services.UnitTests
{
    public class MapSubscriptionRequestDtoToSubscriptionShould
    {
        [Fact]
        public void MapRequestDtoToSubscriptionSuccessfully()
        {
            // Arrange
            var fixture = new Fixture();
            var requestDto = fixture.Create<SubscriptionRequestDto>();
            requestDto.StartDate = "18/12/2021";

            var mappingConfiguration = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapSubscriptionRequestDtoToSubscription());
            });

            var mapper = mappingConfiguration.CreateMapper();

            // Act
            var subscription = mapper.Map<Subscription>(requestDto);

            // Assert
            using (new AssertionScope())
            {
                subscription.Name.Should().Be(requestDto.Name);
                subscription.SubscriptionType.Should().Be(requestDto.SubscriptionType);
                subscription.RenewalCost.Should().Be(requestDto.RenewalCost);
                subscription.RenewalFrequency.Should().Be(requestDto.RenewalFrequency);
            }
        }
    }
}
