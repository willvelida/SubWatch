using Microsoft.Extensions.Logging;
using SubWatch.Common.Models;
using SubWatch.Common.Request;
using SubWatch.Repository.Interfaces;
using SubWatch.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubWatch.Services
{
    public class SubWatchService : ISubWatchService
    {
        private readonly ISubWatchRepository _subWatchRepository;
        private readonly ILogger<SubWatchService> _logger;

        public SubWatchService(ISubWatchRepository subWatchRepository, ILogger<SubWatchService> logger)
        {
            _subWatchRepository = subWatchRepository;
            _logger = logger;
        }

        public DateTime CalculateRenewDate(Subscription subscription)
        {
            switch (subscription.RenewalFrequency)
            {
                case RenewalFrequency.Weekly:
                    subscription.RenewalDate = subscription.RenewalDate.AddDays(7);
                    break;
                case RenewalFrequency.Fortnightly:
                    subscription.RenewalDate = subscription.RenewalDate.AddDays(14);
                    break;
                case RenewalFrequency.Monthly:
                    subscription.RenewalDate = subscription.RenewalDate.AddMonths(1);
                    break;
                case RenewalFrequency.Quarterly:
                    subscription.RenewalDate = subscription.RenewalDate.AddMonths(3);
                    break;
                case RenewalFrequency.HalfYearly:
                    subscription.RenewalDate = subscription.RenewalDate.AddMonths(6);
                    break;
                default:
                    subscription.RenewalDate = subscription.RenewalDate.AddYears(1);
                    break;
            }

            return subscription.RenewalDate;
        }

        public double CalculateTotalCost(SubscriptionRequestDto subscriptionRequestDto)
        {
            var renewalFrequency = 0;

            switch (subscriptionRequestDto.RenewalFrequency)
            {
                case RenewalFrequency.Weekly:
                    renewalFrequency = 52;
                    break;
                case RenewalFrequency.Fortnightly:
                    renewalFrequency = 26;
                    break;
                case RenewalFrequency.Monthly:
                    renewalFrequency = 12;
                    break;
                case RenewalFrequency.Quarterly:
                    renewalFrequency = 4;
                    break;
                case RenewalFrequency.HalfYearly:
                    renewalFrequency = 2;
                    break;
                default:
                    renewalFrequency = 1;
                    break;
            }

            var totalCost = subscriptionRequestDto.RenewalCost * renewalFrequency;

            return totalCost;
        }
    }
}
