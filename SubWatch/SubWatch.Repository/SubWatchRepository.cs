using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SubWatch.Common;
using SubWatch.Common.Models;
using SubWatch.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubWatch.Repository
{
    public class SubWatchRepository : ISubWatchRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _subwatchContainer;
        private readonly Settings _settings;
        private readonly ILogger<SubWatchRepository> _logger;

        public SubWatchRepository(CosmosClient cosmosClient, IOptions<Settings> options, ILogger<SubWatchRepository> logger)
        {
            _settings = options.Value;
            _cosmosClient = cosmosClient;
            _subwatchContainer = _cosmosClient.GetContainer(_settings.DatabaseName, _settings.ContainerName);
            _logger = logger;
        }

        public async Task CreateSubscription(Subscription subscription)
        {
            _logger.LogInformation($"Entering {nameof(CreateSubscription)} method");

            try
            {
                ItemRequestOptions itemRequestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false
                };

                await _subwatchContainer.CreateItemAsync(subscription, new PartitionKey(subscription.SubscriptionType), itemRequestOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task DeleteSubscription(string subscriptionId, string subscriptionType)
        {
            _logger.LogInformation($"Entering {nameof(DeleteSubscription)} method");

            try
            {
                await _subwatchContainer.DeleteItemAsync<Subscription>(subscriptionId, new PartitionKey(subscriptionType));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<List<Subscription>> GetAllSubscriptions()
        {
            _logger.LogInformation($"Entering {nameof(GetAllSubscriptions)} method");

            try
            {
                List<Subscription> subscriptions = new List<Subscription>();

                QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c");

                FeedIterator<Subscription> feedIterator = _subwatchContainer.GetItemQueryIterator<Subscription>(queryDefinition);

                while (feedIterator.HasMoreResults)
                {
                    FeedResponse<Subscription> response = await feedIterator.ReadNextAsync();
                    subscriptions.AddRange(response);
                }

                return subscriptions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<Subscription> GetSubscription(string subscriptionId, string subscriptionType)
        {
            _logger.LogInformation($"Entering {GetSubscription} method");

            try
            {
                ItemResponse<Subscription> itemResponse = await _subwatchContainer.ReadItemAsync<Subscription>(subscriptionId, new PartitionKey(subscriptionType));

                return itemResponse.Resource;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task UpdateSubscription(string subscriptionId, Subscription subscription)
        {
            _logger.LogInformation($"Entering {UpdateSubscription} method");

            try
            {
                ItemRequestOptions itemRequestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false
                };

                await _subwatchContainer.ReplaceItemAsync<Subscription>(subscription, subscriptionId, new PartitionKey(subscription.SubscriptionType));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
