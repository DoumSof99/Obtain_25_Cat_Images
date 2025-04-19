using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Obtain_25_Cat_Images.Interfaces;

namespace Obtain_25_Cat_Images.Controllers {
    public class CatsController(ICatService catService) : BaseApiController {
        
        [HttpPost("fetch")]
        public async Task<IActionResult> FetchAndSaveCats() {
            var result = await catService.FetchAndSaveCatsAsync();

            return result.IsSuccess 
                ? Ok(result.Value)
                : BadRequest(result.Errors[0].Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCatById(int id) {
            var result = await catService.GetCatByIdAsync(id);

            return result.IsSuccess 
                ? Ok(result.Value)
                : NotFound(result.Errors[0].Message);
        }

        [HttpGet]
        public async Task<IActionResult> GetCats([FromQuery] string? tag, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) {
            var result = await catService.GetCatsAsync(tag, page, pageSize);

            return result.IsSuccess
                ? Ok(result.Value)
                : NotFound(result.Errors[0].Message);
        }
    }
}
