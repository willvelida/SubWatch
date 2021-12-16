using SubWatch.Common.Models;
using SubWatch.Common.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubWatch.Services.Interfaces
{
    public interface ISubscriptionHelper
    {
        double CalculateTotalCost(SubscriptionRequestDto subscriptionRequestDto);
        DateTime CalculateRenewalDate(Subscription subscription);
    }
}
