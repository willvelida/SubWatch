using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SubWatch.Common.Exceptions;
using SubWatch.Common.Request;
using SubWatch.Common.Response;
using SubWatch.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace SubWatch.API.Functions
{
    public class CreateSubscription
    {
        private readonly ISubWatchService _subWatchService;
        private readonly ILogger<CreateSubscription> _logger;

        public CreateSubscription(
            ISubWatchService subWatchService,
            ILogger<CreateSubscription> logger)
        {
            _subWatchService = subWatchService;
            _logger = logger;
        }

        [FunctionName(nameof(CreateSubscription))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Subscription")] HttpRequest req)
        {
            try
            {
                await _subWatchService.AddSubscripion(req);

                return new CustomRequestObjectResult(null, StatusCodes.Status201Created);
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
                _logger.LogError($"Exception thrown in {nameof(CreateSubscription)}: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
