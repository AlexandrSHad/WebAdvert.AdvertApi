using Amazon.SimpleNotificationService;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.AdvertApi.Dto;
using WebAdvert.AdvertApi.Services;
using Microsoft.Extensions.Configuration;
using WebAdvert.AdvertApi.Dto.Messages;
using Newtonsoft.Json;

namespace WebAdvert.AdvertApi.Controllers
{
    [ApiController]
    [Route("adverts/v1")]
    public class AdvertsController : Controller
    {
        private readonly IAdvertStorageService _advertStorageService;
        private readonly IConfiguration _configuration;

        public AdvertsController(IAdvertStorageService advertStorageService, IConfiguration configuration)
        {
            _advertStorageService = advertStorageService;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("create")]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(CreateAdvertResponseDto), 201)]
        public async Task<IActionResult> Create(AdvertDto model)
        {
            var advertId = await _advertStorageService.AddAsync(model);

            var result = new CreateAdvertResponseDto { Id = advertId };

            return Created($"adverts/v1/{result.Id}", result);
        }

        [HttpPut]
        [Route("confirm")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Confirm(ConfirmAdvertDto model)
        {
            // TODO: use messaging (lecture 15)
            try
            {
                await _advertStorageService.ConfirmAsync(model);
                await RaiseAdvertConfirmedMessageAsync(model);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            return Ok();
        }

        // TODO: move this method to service
        private async Task RaiseAdvertConfirmedMessageAsync(ConfirmAdvertDto confirmModel)
        {
            var topicArn = _configuration.GetValue<string>("TopicArn");

            // To get the title we fetch the advert from db,
            // or in another way you can pass Title with ConfirmAdvertDto
            var advert = await _advertStorageService.GetByIdAsync(confirmModel.Id);

            if (advert == null)
            {
                throw new KeyNotFoundException($"Record with Id: {confirmModel.Id} was not found.");
            }

            using (var snsClient = new AmazonSimpleNotificationServiceClient())
            {
                var message = new AdvertConfirmedMessage
                {
                    Id = confirmModel.Id,
                    Title = advert.Title
                };

                var messageJson = JsonConvert.SerializeObject(message);

                await snsClient.PublishAsync(topicArn, messageJson);
            }
        }
    }
}