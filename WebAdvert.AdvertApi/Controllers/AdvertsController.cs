using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.AdvertApi.Models;
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
        [ProducesResponseType(typeof(CreateAdvertResponse), 201)]
        public async Task<IActionResult> Create(AdvertModel model)
        {
            var advertId = await _advertStorageService.AddAsync(model);

            var result = new CreateAdvertResponse { Id = advertId };

            return Created($"adverts/v1/{result.Id}", result);
        }

        [HttpPut]
        [Route("confirm")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Confirm(ConfirmAdvertModel model)
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