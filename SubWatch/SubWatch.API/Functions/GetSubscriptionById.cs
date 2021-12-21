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
using SubWatch.Common.Exceptions;
using SubWatch.Common.Response;
using SubWatch.Common.Request;

namespace SubWatch.API.Functions
{
    public class GetSubscriptionById
    {
        private readonly ISubWatchService _subWatchService;
        private readonly ILogger<GetSubscriptionById> _logger;

        public GetSubscriptionById(ISubWatchService subWatchService, ILogger<GetSubscriptionById> logger)
        {
            _subWatchService = subWatchService;
            _logger = logger;
        }

        [FunctionName(nameof(GetSubscriptionById))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Subscription/{subscriptionId}")] HttpRequest req,
            string subscriptionId)
        {
            try
            {
                _logger.LogInformation($"Processing GET Request: Subscription");

                var subscription = await _subWatchService.RetrieveSubscription(subscriptionId);

                return new OkObjectResult(subscription);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex.Message);
                BaseResponse errorResponse = new BaseResponse();
                errorResponse.SetErrorMessage(ex.Message);
                return new CustomRequestObjectResult(errorResponse, StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetSubscriptionById)}: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
