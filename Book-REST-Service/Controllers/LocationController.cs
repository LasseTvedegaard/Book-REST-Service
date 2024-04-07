using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace Book_REST_Service.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase {
        private readonly ICRUD<Location> _locationControl;
        private readonly ILogger<LocationController> _logger;

        public LocationController(ICRUD<Location> locationControl, ILogger<LocationController>? logger = null) {
            _locationControl = locationControl;
            _logger = logger;
        }

        // GET: api/<LocationController>
        //[HttpGet]
        //public async Task<ActionResult<List<Location>>> Get() {
        //    ActionResult<List<Location>> foundReturn;
        //    List<Location>? foundLocations = await _locationControl.GetAll();
        //    if (foundLocations != null) {
        //        foundReturn = Ok(foundLocations);
        //    } else {
        //        foundLocations = new List<Location>();
        //        foundReturn = Ok(foundLocations);
        //    }
        //    return foundReturn;
        //}

        //// POST api/<locationController>
        //[HttpPost]
        //public async Task<IActionResult> CreateGenre([FromBody] Location locationToCreate) {
        //    IActionResult foundResult;
        //    int insertedId = await _locationControl.Create(locationToCreate);
        //    if (insertedId == -1) {
        //        foundResult = StatusCode(500);
        //    } else {
        //        locationToCreate.LocationId = insertedId;
        //        foundResult = CreatedAtAction(nameof(Get), new {id = insertedId}, locationToCreate);
        //    }
        //    return foundResult;
        //}

        //// PUT api/<locationController>/5
        //[HttpPut("{locationId}")]
        //public async Task<IActionResult> UpdateLocation(int locationId, [FromBody] Location locationToUpdate) {
        //    IActionResult foundResult;

        //    bool isUpdated = await _locationControl.Update(locationId, locationToUpdate);
        //    if (isUpdated) {
        //        foundResult = Ok();
        //    } else {
        //        foundResult = StatusCode(500);
        //    }
        //    return foundResult;
        //}

        //// DELETE api/<locationController>/5
        //[HttpDelete("{locationId}")]
        //public async Task<IActionResult> Deletelocation(int locationId) {
        //    IActionResult foundResult;

        //    if (locationId > 0) { 
        //        bool isDeleted = await _locationControl.Delete(locationId);
        //        if (isDeleted) {
        //            foundResult= NoContent();
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
