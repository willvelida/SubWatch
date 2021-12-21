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

        public async Task DeleteSubscription(string subscriptionId)
        {
            _logger.LogInformation($"Entering {nameof(DeleteSubscription)} method.");

            try
            {
                _subWatchValidator.ValidateSubscriptionId(subscriptionId);

                await _subWatchRepository.DeleteSubscription(subscriptionId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(DeleteSubscription)}: {ex.Message}");
                throw;
            }

            _logger.LogInformation($"Executed {nameof(DeleteSubscription)} method.");
        }

        public async Task<List<Subscription>> GetAllSubscriptions()
        {
            _logger.LogInformation($"Entering {nameof(GetAllSubscriptions)} method.");

            var subscriptions = new List<Subscription>();

            subscriptions = await _subWatchRepository.GetAllSubscriptions();

            _logger.LogInformation($"Executed {nameof(GetAllSubscriptions)} method.");

            return subscriptions;
        }

        public async Task<Subscription> RetrieveSubscription(string subscriptionId)
        {
            _logger.LogInformation($"Entering {nameof(RetrieveSubscription)} method.");

            _subWatchValidator.ValidateSubscriptionId(subscriptionId);

            var subscription = await _subWatchRepository.GetSubscription(subscriptionId);

            if (subscription is null)
                throw new NotFoundException($"No subscription with ID {subscriptionId} exists!");

            _logger.LogInformation($"Executed {nameof(RetrieveSubscription)} method.");

            return subscription;
        }

        public async Task UpdateSubscription(string subscriptionId, HttpRequest httpRequest)
        {
            _logger.LogInformation($"Entering {nameof(UpdateSubscription)} method.");

            try
            {
                _subWatchValidator.ValidateSubscriptionId(subscriptionId);

                var subscriptionToUpdate = await _subWatchRepository.GetSubscription(subscriptionId);

                if (subscriptionToUpdate is null)
                    throw new NotFoundException($"Subscription with ID: {subscriptionId} not found!");

                var subscriptionUpdateRequestDto = await _subWatchValidator.ValidateRequest(httpRequest);

                subscriptionToUpdate.Name = subscriptionUpdateRequestDto.Name;
                subscriptionToUpdate.SubscriptionType = subscriptionUpdateRequestDto.SubscriptionType;
                subscriptionToUpdate.RenewalCost = subscriptionUpdateRequestDto.RenewalCost;
                subscriptionToUpdate.StartDate = subscriptionUpdateRequestDto.StartDate;
                subscriptionToUpdate.RenewalFrequency = subscriptionUpdateRequestDto.RenewalFrequency;

                subscriptionToUpdate.RenewalDate = _subscriptionHelper.CalculateRenewalDate(subscriptionToUpdate);
                subscriptionToUpdate.TotalCost = _subscriptionHelper.CalculateTotalCost(subscriptionUpdateRequestDto);

                await _subWatchRepository.UpdateSubscription(subscriptionId, subscriptionToUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(UpdateSubscription)}: {ex.Message}");
                throw;
            }

            _logger.LogInformation($"Executed {nameof(UpdateSubscription)} method.");
        }
    }
}
