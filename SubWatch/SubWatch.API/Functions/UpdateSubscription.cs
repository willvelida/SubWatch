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
    public class UpdateSubscription
    {
        private readonly ISubWatchService _subWatchService;
        private readonly ILogger<UpdateSubscription> _logger;

        public UpdateSubscription(ISubWatchService subWatchService, ILogger<UpdateSubscription> logger)
        {
            _subWatchService = subWatchService;
            _logger = logger;
        }

        [FunctionName(nameof(UpdateSubscription))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Subscription/{subscriptionId}")] HttpRequest req,
            string subscriptionId)
        {
            try
            {
                _logger.LogInformation($"Processing PUT Request: Subscription");

                await _subWatchService.UpdateSubscription(subscriptionId, req);

                return new CustomRequestObjectResult(null, StatusCodes.Status204NoContent);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex.Message);
                BaseResponse errorResponse = new BaseResponse();
                errorResponse.SetErrorMessage(ex.Message);
                return new CustomRequestObjectResult(errorResponse, StatusCodes.Status404NotFound);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex.Message);
                BaseResponse errorResponse = new BaseResponse();
                errorResponse.SetErrorMessage(ex.Message);
                return new CustomRequestObjectResult(errorResponse, StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(UpdateSubscription)}: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
