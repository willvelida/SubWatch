using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SubWatch.Common;
using SubWatch.Common.Exceptions;
using SubWatch.Common.Models;
using SubWatch.Repository.Interfaces;

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

                await _subwatchContainer.CreateItemAsync(subscription, new PartitionKey(subscription.Id), itemRequestOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task DeleteSubscription(string subscriptionId)
        {
            _logger.LogInformation($"Entering {nameof(DeleteSubscription)} method");

            try
            {
                ItemResponse<Subscription> itemResponse = await _subwatchContainer.DeleteItemAsync<Subscription>(subscriptionId, new PartitionKey(subscriptionId));
            }
            catch (CosmosException cex) when (cex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new NotFoundException($"No Subscription with ID: {subscriptionId} found! Delete failed");
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

        public async Task<Subscription> GetSubscription(string subscriptionId)
        {
            _logger.LogInformation($"Entering {GetSubscription} method");

            try
            {
                ItemResponse<Subscription> itemResponse = await _subwatchContainer.ReadItemAsync<Subscription>(subscriptionId, new PartitionKey(subscriptionId));

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

                await _subwatchContainer.ReplaceItemAsync(subscription, subscriptionId, new PartitionKey(subscriptionId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
