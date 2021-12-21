using Microsoft.AspNetCore.Http;
using SubWatch.Common.Models;

namespace SubWatch.Services.Interfaces
{
    public interface ISubWatchService
    {
        Task AddSubscripion(HttpRequest httpRequest);
        Task<Subscription> RetrieveSubscription(string subscriptionId);
    }
}
