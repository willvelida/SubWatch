using Microsoft.AspNetCore.Http;
using SubWatch.Common.Models;

namespace SubWatch.Services.Interfaces
{
    public interface ISubWatchService
    {
        Task AddSubscripion(HttpRequest httpRequest);
        Task<Subscription> RetrieveSubscription(string subscriptionId);
        Task DeleteSubscription(string subscriptionId);
        Task UpdateSubscription(string subscriptionId, HttpRequest httpRequest);
        Task<List<Subscription>> GetAllSubscriptions();
    }
}
