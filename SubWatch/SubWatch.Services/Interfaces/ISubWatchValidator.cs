using Microsoft.AspNetCore.Http;
using SubWatch.Common.Request;

namespace SubWatch.Services.Interfaces
{
    public interface ISubWatchValidator
    {
        Task<SubscriptionRequestDto> ValidateRequest(HttpRequest httpRequest);
        void ValidateSubscriptionId(string subscriptionId);
    }
}
