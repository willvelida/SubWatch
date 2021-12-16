using Microsoft.Extensions.Logging;
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

        public double CalculateTotalCost(SubscriptionRequestDto subscriptionRequestDto)
        {
            var renewalFrequency = 0;

            switch (subscriptionRequestDto.RenewalFrequency)
            {
                case Common.Models.RenewalFrequency.Weekly:
                    renewalFrequency = 52;
                    break;
                case Common.Models.RenewalFrequency.Fortnightly:
                    renewalFrequency = 26;
                    break;
                case Common.Models.RenewalFrequency.Monthly:
                    renewalFrequency = 12;
                    break;
                case Common.Models.RenewalFrequency.Quarterly:
                    renewalFrequency = 4;
                    break;
                case Common.Models.RenewalFrequency.HalfYearly:
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
