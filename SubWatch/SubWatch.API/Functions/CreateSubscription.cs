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
using SubWatch.Common.Exceptions;
using SubWatch.Common.Response;

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
                string messageRequest = await new StreamReader(req.Body).ReadToEndAsync();
                var subscriptionRequestDto = JsonConvert.DeserializeObject<SubscriptionRequestDto>(messageRequest);

                await _subWatchService.AddSubscripion(subscriptionRequestDto);

                return new CreatedResult(nameof(CreateSubscription), subscriptionRequestDto);
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
