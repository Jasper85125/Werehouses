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

        // POST: /locations
        [HttpPost]
        public async Task CreateLocation([FromBody] LocationCS location)
        {
            
        }

        // PUT: api/warehouse/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            // Replace with your logic
        }

        // DELETE: api/warehouse/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            // Replace with your logic
        }
    }
}