using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using ServicesV2;

namespace ControllersV2
{
    [ApiController]
    [Route("api/v2/locations")]
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
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Analyst",
                                                                   "Logistics", "Sales" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();
            var WarehouseIDFromKey = (int)HttpContext.Items["WarehouseID"];

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                if (userRole == "Operative" || userRole == "Supervisor" || userRole == "Floor Manager" || userRole == "Inventory Manager")
                {
                    var LocationsForWarehouse = _locationService.GetLocationsByWarehouseId(WarehouseIDFromKey);
                    return Ok(LocationsForWarehouse);
                }
                return Unauthorized();
            }

            var locations = _locationService.GetAllLocations();
            return Ok(locations);
        }

        // GET: /locations/{id}
        [HttpGet("{id}")]
        public ActionResult<LocationCS> GetLocationById([FromRoute] int id)
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Supervisor", "Operative", "Analyst",
                                                                   "Logistics", "Sales" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }

            var location = _locationService.GetLocationById(id);
            if (location is null)
            {
                return NotFound();
            }
            return Ok(location);
        }

        // GET: /locations/warehouse/{warehouse_id}
        [HttpGet("warehouse/{warehouse_id}")]
        public ActionResult<IEnumerable<LocationCS>> GetLocationsByWarehouseId([FromRoute] int warehouse_id)
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Supervisor", "Operative", "Analyst",
                                                                   "Logistics", "Sales" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }

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
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                               "Floor Manager" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }

            if (location is null)
            {
                return BadRequest("Location is null.");
            }

            var createdLocation = _locationService.CreateLocation(location);

            return CreatedAtAction(nameof(GetLocationById), new { id = createdLocation.Id }, createdLocation);
        }

        // POST: /locations/multiple
        [HttpPost("multiple")]
        public ActionResult<IEnumerable<LocationCS>> CreateMultipleLocations([FromBody] List<LocationCS> newLocations)
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                               "Floor Manager" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }

            if (newLocations is null)
            {
                return BadRequest("Location data is null");
            }

            var createdLocations = _locationService.CreateMultipleLocations(newLocations);
            return StatusCode(StatusCodes.Status201Created, createdLocations);
        }

        // PUT: api/warehouse/5
        [HttpPut("{id}")]
        public ActionResult<LocationCS> UpdateLocation([FromRoute] int id, [FromBody] LocationCS newLocation)
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                               "Floor Manager" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }

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
        [HttpPatch("{id}/{property}")]
        public ActionResult<LocationCS> PatchLocation([FromRoute]int id, [FromRoute]string property, [FromBody]object newvalue){
            if(string.IsNullOrEmpty(property) || newvalue is null){
                return BadRequest("Missing inputs in request");
            }
            var result = _locationService.PatchLocation(id, property, newvalue);
            return Ok(result);
        }
        // DELETE: api/warehouse/5
        [HttpDelete("{id}")]
        public ActionResult DeleteLocation(int id)
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }

            var location = _locationService.GetLocationById(id);
            if (location == null)
            {
                return NotFound();
            }
            _locationService.DeleteLocation(id);
            return Ok();
        }

        [HttpDelete("batch")]
        public ActionResult DeleteLocations([FromBody] List<int> ids)
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }

            if (ids is null)
            {
                return NotFound();
            }
            _locationService.DeleteLocations(ids);
            return Ok("Locations deleted");
        }
    }
}
