using AutoMapper;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SubWatch.API;
using SubWatch.Common;
using SubWatch.Repository;
using SubWatch.Repository.Interfaces;
using SubWatch.Services;
using SubWatch.Services.Interfaces;
using SubWatch.Services.Mappers;
using SubWatch.Services.Validators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SubWatch.API
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);

            builder.Services.AddOptions<Settings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("Settings").Bind(settings);
                });

            builder.Services.AddAutoMapper(typeof(Startup));
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapSubscriptionRequestDtoToSubscription());
            });
            var mapper = mappingConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);

            builder.Services.AddSingleton(sp =>
            {
                IConfiguration config = sp.GetService<IConfiguration>();
                CosmosClientOptions cosmosClientOptions = new CosmosClientOptions
                {
                    MaxRetryAttemptsOnRateLimitedRequests = 3,
                    MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(60),
                    ApplicationRegion = "Australia East",
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    }
                };
                return new CosmosClient(config["CosmosDBConnectionString"], cosmosClientOptions);
            });
            builder.Services.AddTransient<ISubWatchValidator, SubWatchValidator>();
            builder.Services.AddTransient<ISubWatchRepository, SubWatchRepository>();
            builder.Services.AddTransient<ISubscriptionHelper, SubscriptionHelper>();
            builder.Services.AddTransient<ISubWatchService, SubWatchService>();
        }
    }
}
