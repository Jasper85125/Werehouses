using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using Services;

namespace Controllers
{
    [ApiController]
    [Route("/locations")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        // GET: /locations
        [HttpGet()]
        public ActionResult<IEnumerable<LocationCS>> GetAllLocations()
        {
            var locations = _locationService.GetAllLocations();
            return Ok(locations);
        }

        // GET: /locations/{id}
        [HttpGet("{id}")]
        public ActionResult<LocationCS> GetLocationById([FromRoute]int id)
        {
            var location = _locationService.GetLocationById(id);
            if (location is null)
            {
                return NotFound();
            }
            return Ok(location);
        }
        // GET: /locations/warehouse/{warehouse_id}
        [HttpGet("warehouse/{warehouse_id}")]
        public ActionResult<IEnumerable<LocationCS>> GetLocationsByWarehouseId([FromRoute]int warehouse_id)
        {
            var locations = _locationService.GetLocationsByWarehouseId(warehouse_id);
            if (locations is null)
            {
                return NotFound();
            }
            return Ok(locations);
        }

        // POST: /locations
        [HttpPost()]
        public ActionResult<LocationCS> CreateLocation([FromBody] LocationCS location)
        {
            if (location is null)
            {
                return BadRequest("Location is null.");
            }

            var createdLocation = _locationService.CreateLocation(location);

            return CreatedAtAction(nameof(GetLocationById), new { id = createdLocation.Id }, createdLocation);
        }

        // PUT: api/warehouse/5
        [HttpPut("{id}")]
        public ActionResult<LocationCS> UpdateLocation([FromRoute]int id, [FromBody] LocationCS newLocation)
        {
            if (newLocation is null)
            {
                return BadRequest("Location is null.");
            }
            var updatedLocation = _locationService.UpdateLocation(newLocation, id);
            if (updatedLocation is null)
            {
                return BadRequest("No location found with that id");
            }
            return Ok(updatedLocation);
        }

        // DELETE: api/warehouse/5
        [HttpDelete("{id}")]
        public ActionResult DeleteLocation(int id)
        {
            var location = _locationService.GetLocationById(id);
            if (location == null)
            {
                return NotFound();
            }
        _locationService.DeleteLocation(id);
        return Ok();
        }
        [HttpDelete("batch")]
        public ActionResult DeleteLocations([FromBody]List<int> ids){
            if(ids is null){
                return NotFound();
            }
            _locationService.DeleteLocations(ids);
            return Ok("Locations deleted");
        }
    }
}