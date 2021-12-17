using SubWatch.Common.Models;
using SubWatch.Common.Request;

namespace SubWatch.Services.Interfaces
{
    public interface ISubscriptionHelper
    {
        double CalculateTotalCost(SubscriptionRequestDto subscriptionRequestDto);
        DateTime CalculateRenewalDate(Subscription subscription);
    }
}
