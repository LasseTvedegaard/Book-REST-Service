using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace Book_REST_Service.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase {
        private readonly ICRUD<Genre> _genreControl;
        private readonly ILogger<GenreController> _logger;

        public GenreController(ICRUD<Genre> genreControl, ILogger<GenreController>? logger = null) {
            _genreControl = genreControl;
            _logger = logger;
        }

        // GET: api/<GenreController>
        //[HttpGet]
        //public async Task<ActionResult<List<Genre>>> Get() {
        //    ActionResult<List<Genre>> foundReturn;
        //    List<Genre>? foundGenres = await _genreControl.GetAll();
        //    if (foundGenres != null) {
        //        foundReturn = Ok(foundGenres);
        //    } else {
        //        foundGenres = new List<Genre>();
        //        foundReturn = Ok(foundGenres);
        //    }
        //    return foundReturn;
        //}

        //// POST api/<GenreController>
        //[HttpPost]
        //public async Task<IActionResult> CreateGenre([FromBody] Genre genreToCreate) {
        //    IActionResult foundResult;
        //    int insertedId = await _genreControl.Create(genreToCreate);
        //    if (insertedId == -1) {
        //        foundResult = StatusCode(500);
        //    } else {
        //        genreToCreate.GenreId = insertedId;
        //        foundResult = CreatedAtAction(nameof(Get), new {id = insertedId}, genreToCreate);
        //    }
        //    return foundResult;
        //}

        //// PUT api/<GenreController>/5
        //[HttpPut("{genreId}")]
        //public async Task<IActionResult> UpdateGenre(int genreId, [FromBody] Genre genreToUpdate) {
        //    IActionResult foundResult;

        //    bool isUpdated = await _genreControl.Update(genreId, genreToUpdate);
        //    if (isUpdated) {
        //        foundResult = Ok();
        //    } else {
        //        foundResult = StatusCode(500);
        //    }
        //    return foundResult;
        //}

        //// DELETE api/<GenreController>/5
        //[HttpDelete("{genreId}")]
        //public async Task<IActionResult> DeleteGenre(int genreId) {
        //    IActionResult foundResult;

        //    if (genreId > 0) {
        //        bool isDeleted = await _genreControl.Delete(genreId);
        //        if (isDeleted) {
        //            foundResult = NoContent();
        //        } else {
        //            foundResult = StatusCode(500);
        //        }
        //    } else {
        //        foundResult = BadRequest();
        //    }
        //    return foundResult;
        //}
    }
}
