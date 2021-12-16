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
            throw new NotImplementedException();
        }

        public async Task DeleteSubscription(string subscriptionId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Subscription>> GetAllSubscriptions()
        {
            throw new NotImplementedException();
        }

        public async Task<Subscription> GetSubscription(string subscriptionId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateSubscription(string subscriptionId, Subscription subscription)
        {
            throw new NotImplementedException();
        }
    }
}
