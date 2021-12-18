using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
