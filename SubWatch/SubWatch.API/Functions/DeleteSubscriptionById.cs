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
    public class DeleteSubscriptionById
    {
        private readonly ISubWatchService _subWatchService;
        private readonly ILogger<DeleteSubscriptionById> _logger;

        public DeleteSubscriptionById(ISubWatchService subWatchService, ILogger<DeleteSubscriptionById> logger)
        {
            _subWatchService = subWatchService;
            _logger = logger;
        }

        [FunctionName(nameof(DeleteSubscriptionById))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Subscription/{subscriptionId}")] HttpRequest req,
            string subscriptionId)
        {
            try
            {
                _logger.LogInformation($"Processing DELETE Request: Subscription");

                await _subWatchService.DeleteSubscription(subscriptionId);

                return new CustomRequestObjectResult(null, StatusCodes.Status204NoContent);
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
                _logger.LogError($"Exception thrown in {nameof(DeleteSubscriptionById)}: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
