using AutoMapper;
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
        private readonly ISubscriptionHelper _subscriptionHelper;
        private readonly IMapper _mapper;
        private readonly ILogger<SubWatchService> _logger;

        public SubWatchService(
            ISubWatchRepository subWatchRepository,
            ISubscriptionHelper subscriptionHelper,
            IMapper mapper,
            ILogger<SubWatchService> logger)
        {
            _subWatchRepository = subWatchRepository;
            _subscriptionHelper = subscriptionHelper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task AddSubscripion(SubscriptionRequestDto subscriptionRequestDto)
        {
            _logger.LogInformation($"Entering {nameof(AddSubscripion)} method.");

            try
            {
                var subscription = _mapper.Map<Subscription>(subscriptionRequestDto);
                subscription.Id = Guid.NewGuid().ToString();
                subscription.RenewalDate = _subscriptionHelper.CalculateRenewalDate(subscription);
                subscription.TotalCost = _subscriptionHelper.CalculateTotalCost(subscriptionRequestDto);

                await _subWatchRepository.CreateSubscription(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(AddSubscripion)}: {ex.Message}");
                throw;
            }

            _logger.LogInformation($"Executed {nameof(AddSubscripion)} method");
        }
    }
}
