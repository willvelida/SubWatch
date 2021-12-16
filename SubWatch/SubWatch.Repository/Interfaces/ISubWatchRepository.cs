using SubWatch.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubWatch.Repository.Interfaces
{
    public interface ISubWatchRepository
    {
        Task CreateSubscription(Subscription subscription);
        Task DeleteSubscription(string subscriptionId, string subscriptionType);
        Task<List<Subscription>> GetAllSubscriptions();
        Task<Subscription> GetSubscription(string subscriptionId, string subscriptionType);
        Task UpdateSubscription(string subscriptionId, Subscription subscription);
    }
}
