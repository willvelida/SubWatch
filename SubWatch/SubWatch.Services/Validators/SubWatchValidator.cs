using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SubWatch.Common.Exceptions;
using SubWatch.Common.Request;
using SubWatch.Services.Interfaces;

namespace SubWatch.Services.Validators
{
    public class SubWatchValidator : ISubWatchValidator
    {
        public async Task<SubscriptionRequestDto> ValidateRequest(HttpRequest httpRequest)
        {
            string messageRequest = await new StreamReader(httpRequest.Body).ReadToEndAsync();
            var subscriptionRequestDto = JsonConvert.DeserializeObject<SubscriptionRequestDto>(messageRequest);

            if (string.IsNullOrWhiteSpace(subscriptionRequestDto.Name))
                throw new BadRequestException("Subscription name cannot be null or empty");

            if (string.IsNullOrWhiteSpace(subscriptionRequestDto.SubscriptionType))
                throw new BadRequestException("Subscription type cannot be null or empty");

            if (string.IsNullOrWhiteSpace(subscriptionRequestDto.StartDate))
                throw new BadRequestException("StartDate cannot be null or empty");

            return subscriptionRequestDto;
        }

        public void ValidateSubscriptionId(string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
                throw new BadRequestException("Subscription Id cannot be null or empty");

            bool isIdValid = Guid.TryParse(subscriptionId, out var id);
            if (isIdValid is false)
                throw new BadRequestException("Subscription Id is invalid!");
        }
    }
}
