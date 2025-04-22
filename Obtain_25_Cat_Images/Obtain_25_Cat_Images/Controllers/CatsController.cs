using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Obtain_25_Cat_Images.DTOs;
using Obtain_25_Cat_Images.Interfaces;

namespace Obtain_25_Cat_Images.Controllers {
    public class CatsController(ICatService catService) : BaseApiController {
      
        /// <summary>
        /// Fetches 25 cat images with breed information and stores them in the database.
        /// </summary>
        /// <remarks>
        /// This endpoint contacts TheCatAPI (https://thecatapi.com), extracts image + temperament data,
        /// and stores cats with tag relations in SQL Server.
        /// </remarks>
        /// <returns>List of newly saved cat records</returns>
        /// <response code="200">Returns the list of cats</response>
        /// <response code="500">If the external API fails or saving to the DB fails</response>
        [HttpPost("fetch")]
        [ProducesResponseType(typeof(List<CatResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FetchAndSaveCats() {
            var result = await catService.FetchAndSaveCatsAsync();

            return result.IsSuccess 
                ? Ok(result.Value)
                : BadRequest(result.Errors[0].Message);
        }

        /// <summary>
        /// Retrieves a single cat by its database ID.
        /// </summary>
        /// <param name="id">The internal database ID of the cat.</param>
        /// <returns>
        /// </returns>
        /// <response code="200">Returns the cat if found</response>
        /// <response code="404">Returns a 404 if the cat is not found</response>
        /// <response code="400">Returns a 400 if there was a request error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CatResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCatById(int id) {
            var result = await catService.GetCatByIdAsync(id);

            return result.IsSuccess 
                ? Ok(result.Value)
                : NotFound(result.Errors[0].Message);
        }

        /// <summary>
        /// Returns a list of cats with optional tag filter and pagination.
        /// </summary>
        /// <param name="tag">Optional tag name to filter cats by.</param>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">Number of results per page.</param>
        /// <returns>A paginated list of cats.</returns>
        /// <response code="200">Returns the list of cats</response>
        /// <response code="404">No cats found</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCats([FromQuery] string? tag, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) {
            var result = await catService.GetCatsAsync(tag, page, pageSize);

            return result.IsSuccess
                ? Ok(result.Value)
                : NotFound(result.Errors[0].Message);
        }
    }
}
