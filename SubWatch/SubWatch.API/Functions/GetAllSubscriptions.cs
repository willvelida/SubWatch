using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SubWatch.Services.Interfaces;
using SubWatch.Common.Request;

namespace SubWatch.API.Functions
{
    public class GetAllSubscriptions
    {
        private readonly ISubWatchService _subWatchService;
        private readonly ILogger<GetAllSubscriptions> _logger;

        public GetAllSubscriptions(ISubWatchService subWatchService, ILogger<GetAllSubscriptions> logger)
        {
            _subWatchService = subWatchService;
            _logger = logger;
        }

        [FunctionName(nameof(GetAllSubscriptions))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", Route = "Subscriptions")] HttpRequest req)
        {
            try
            {
                _logger.LogInformation($"Processing GET Request: Subscription");

                var subscriptions = await _subWatchService.GetAllSubscriptions();

                return new OkObjectResult(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetAllSubscriptions)}: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
