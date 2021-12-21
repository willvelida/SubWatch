using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SubWatch.Common.Exceptions;
using SubWatch.Common.Models;
using SubWatch.Repository.Interfaces;
using SubWatch.Services.Interfaces;

namespace SubWatch.Services
{
    public class SubWatchService : ISubWatchService
    {
        private readonly ISubWatchValidator _subWatchValidator;
        private readonly ISubWatchRepository _subWatchRepository;
        private readonly ISubscriptionHelper _subscriptionHelper;
        private readonly IMapper _mapper;
        private readonly ILogger<SubWatchService> _logger;

        public SubWatchService(
            ISubWatchValidator subWatchValidator,
            ISubWatchRepository subWatchRepository,
            ISubscriptionHelper subscriptionHelper,
            IMapper mapper,
            ILogger<SubWatchService> logger)
        {
            _subWatchValidator = subWatchValidator;
            _subWatchRepository = subWatchRepository;
            _subscriptionHelper = subscriptionHelper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task AddSubscripion(HttpRequest httpRequest)
        {
            _logger.LogInformation($"Entering {nameof(AddSubscripion)} method.");

            try
            {
                var subscriptionRequestDto = await _subWatchValidator.ValidateRequest(httpRequest);
                var subscription = _mapper.Map<Subscription>(subscriptionRequestDto);                
                subscription.Id = Guid.NewGuid().ToString();
                subscription.StartDate = subscriptionRequestDto.StartDate;
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

        public async Task<Subscription> RetrieveSubscription(string subscriptionId)
        {
            _logger.LogInformation($"Entering {nameof(RetrieveSubscription)} method.");

            var subscription = await _subWatchRepository.GetSubscription(subscriptionId);

            if (subscription is null)
                throw new NotFoundException($"No subscription with ID {subscriptionId} exists!");

            _logger.LogInformation($"Executed {nameof(RetrieveSubscription)} method.");

            return subscription;
        }
    }
}
