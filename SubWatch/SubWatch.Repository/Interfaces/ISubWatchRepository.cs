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
        Task DeleteSubscription(string subscriptionId);
        Task<List<Subscription>> GetAllSubscriptions();
        Task<Subscription> GetSubscription(string subscriptionId);
        Task UpdateSubscription(string subscriptionId, Subscription subscription);
    }
}
