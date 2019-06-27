using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.AdvertApi.Dto;
using WebAdvert.AdvertApi.Services;

namespace WebAdvert.AdvertApi.Controllers
{
    [ApiController]
    [Route("adverts/v1")]
    public class AdvertsController : Controller
    {
        private readonly IAdvertStorageService _advertStorageService;

        public AdvertsController(IAdvertStorageService advertStorageService)
        {
            _advertStorageService = advertStorageService;
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
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            return Ok();
        }
    }
}