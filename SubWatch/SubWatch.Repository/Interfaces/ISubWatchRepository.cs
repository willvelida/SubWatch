using SubWatch.Common.Models;

namespace SubWatch.Repository.Interfaces
{
    public interface ISubWatchRepository
    {
        Task CreateSubscription(Subscription subscription);
        Task DeleteSubscription(string subscriptionId);
        Task<List<Subscription>> GetAllSubscriptions();
        Task<Subscription> GetSubscription(string subscriptionId);
        Task UpdateSubscription(string subscriptionId, Subscription subscription);
    }
}
