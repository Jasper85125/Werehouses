using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using ServicesV2;
using System.Reflection.Metadata.Ecma335;

namespace ControllersV2;
public class inventoryFilter
{
    public int Id { get; set; }
    public string? item_id { get; set; }
    public int LocationsCount {get; set;}
    public int total_on_hand { get; set; }
    public int total_expected { get; set; }
    public int total_ordered { get; set; }
    public int total_allocated { get; set; }
    public int total_available { get; set; }
}
[ApiController]
[Route("api/v2/inventories")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    private readonly ILocationService _locationService;
    public InventoryController(IInventoryService inventoryService, ILocationService locationService)
    {
        _inventoryService = inventoryService;
        _locationService = locationService;
    }
    

    // GET: /inventories
    // [HttpGet()]
    // public ActionResult<IEnumerable<InventoryCS>> GetAllInventories()
    // {
    //     var userRole = HttpContext.Items["UserRole"]?.ToString();
    //     if (!HttpContext.Items.TryGetValue("WarehouseID", out var warehouseIdObj) || !(warehouseIdObj is string warehouseID))
    //     {
    //         return BadRequest("WarehouseID is missing or invalid.");
    //     }

    //     var allowedRoles = new List<string> { "Admin", "Warehouse Manager", "Inventory Manager", "Floor Manager", "Sales", "Analyst", "Logistics" };
    //     if (string.IsNullOrEmpty(userRole) || !allowedRoles.Contains(userRole))
    //     {
    //         if (userRole == "Operative" || userRole == "Supervisor")
    //         {

    //             var warehouseid = warehouseID.Split(',').Select(int.Parse).ToList();
    //             // get location from the inventories and then look in the location for the warehouse_id
    //             // location can be found in the data/locations.json file and the location variable is the same as the id in the json file. the file has a variable called warehouse id
    //             // use the locationservice to get all the locations and then filter the locations
                
    //             var locations = _locationService.GetAllLocations();
    //             var filteredLocations = locations.Where(location => warehouseid.Contains(location.warehouse_id)).ToList();


    //             var locationsByWarehouse = filteredLocations.GroupBy(location => location.warehouse_id);

    //             var locationIds = filteredLocations.Select(location => location.Id).ToList();

    //             var inventoriesByLocation = _inventoryService.GetInventoriesByLocationId(locationIds);

    //             var warehouseInventoryList = locationsByWarehouse
    //                 .Select(group => group
    //                     .SelectMany(location => inventoriesByLocation
    //                         .Where(inventory => inventory.Locations.Any(loc => loc == location.Id)))
    //                     .ToList())
    //                 .ToList();

    //             return Ok(warehouseInventoryList);
    //         }
    //         else
    //         {
    //             return Unauthorized();
    //         }
    //     }
    //     var inventoriesall = _inventoryService.GetAllInventories();
    //     return Ok(inventoriesall);
    // }

    [HttpGet()]
    public ActionResult<PaginationCS<InventoryCS>> GetAllInventories(
        [FromQuery] inventoryFilter tofilter, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        List<string> listOfAllowedRoles = new List<string>()
        { "Admin", "Warehouse Manager", "Inventory Manager", "Floor Manager", "Sales", "Analyst", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        // Get all inventories
        var inventories = _inventoryService.GetAllInventories();
        var query = inventories.AsQueryable();

        //filter inventories
        if (tofilter != null)
        {
            if (tofilter.Id != 0)
            {
                query = query.Where(x => x.Id == tofilter.Id);
            }
            if (tofilter.item_id != null)
            {
                query = query.Where(x => x.item_id == tofilter.item_id);
            }
            if (tofilter.LocationsCount != 0)
            {
                query = query.Where(x => x.Locations.Count >= tofilter.LocationsCount);
            }
            if (tofilter.total_on_hand != 0)
            {
                query = query.Where(x => x.total_on_hand == tofilter.total_on_hand);
            }
            if (tofilter.total_expected != 0)
            {
                query = query.Where(x => x.total_expected == tofilter.total_expected);
            }
            if (tofilter.total_ordered != 0)
            {
                query = query.Where(x => x.total_ordered == tofilter.total_ordered);
            }
            if (tofilter.total_allocated != 0)
            {
                query = query.Where(x => x.total_allocated == tofilter.total_allocated);
            }
            if (tofilter.total_available != 0)
            {
                query = query.Where(x => x.total_available == tofilter.total_available);
            }
        }
        

        // Get the filtered count
        int filteredInventoriesCount = query.Count();

        // Pagination logic
        int totalPages = (int)Math.Ceiling(filteredInventoriesCount / (double)pageSize);
        var pagedInventories = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        // Return paginated and filtered result
        var result = new PaginationCS<InventoryCS>()
        {
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            Data = pagedInventories
        };

        return Ok(result);
    }

    // GET: /inventories/{id}
    [HttpGet("{id}")]
    public ActionResult<InventoryCS> GetInventoryById([FromRoute] int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var inventory = _inventoryService.GetInventoryById(id);
        if (inventory is null)
        {
            return NotFound();
        }
        return Ok(inventory);
    }

    // GET: /inventories/total/{item_id}
    // (GET) The system must allow users to fetch inventory totals for a specific item. the total is the total_on_hand + total_allocated it must return either a 200 or 404 status code and the sum of the total_on_hand and total_allocated for the item.
    [HttpGet("total/{item_id}")]
    public ActionResult<int> GetInventoryByItemId([FromRoute] string item_id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        // (GET) The system must allow users to fetch inventory totals for a specific item. the total is the total_on_hand + total_allocated it must return either a 200 or 404 status code and the sum of the total_on_hand and total_allocated for the item.
        var inventory = _inventoryService.GetInventoriesForItem(item_id);
        if (inventory is null)
        {
            return NotFound();
        }
        var inventory_total = inventory.total_on_hand + inventory.total_allocated;
        return Ok(inventory_total);

    }

    // POST: /inventories
    [HttpPost()]
    public ActionResult<InventoryCS> CreateInventory([FromBody] InventoryCS inventory)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (inventory is null)
        {
            return BadRequest("Inventory is null");
        }
        var newInventory = _inventoryService.CreateInventory(inventory);
        return CreatedAtAction(nameof(GetInventoryById), new { id = newInventory.Id }, newInventory);
    }

    // POST: /inventories/multiple
    [HttpPost("multiple")]
    public ActionResult<InventoryCS> CreateMultipleInventories([FromBody] List<InventoryCS> newInventory)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newInventory is null)
        {
            return BadRequest("Inventory data is null");
        }

        var createdInventories = _inventoryService.CreateMultipleInventories(newInventory);
        return StatusCode(StatusCodes.Status201Created, createdInventories);
    }

    // PUT: api/warehouse/5
    [HttpPut("{id}")]
    public ActionResult<InventoryCS> UpdateInventoryById(int id, [FromBody] InventoryCS value)
    {
        // Replace with your logic
        //make use of your update inventory by id service
        //include a check if value is null
        // if(value is null || value.created_at == default || value.updated_at == default 
        // || string.IsNullOrWhiteSpace(value.description) || value.item_id is null || value.item_reference is null 
        // || value.Locations is null){
        //     return BadRequest("request is invalid/contains invalid values");
        // }
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (value is null)
        {
            return BadRequest("request is invalid/contains invalid values");
        }
        var inventory = _inventoryService.UpdateInventoryById(id, value);
        return Ok(inventory);
    }
    

    // DELETE: api/warehouse/5
    [HttpDelete("{id}")]
    public ActionResult DeleteInventory(int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var existingInventory = _inventoryService.GetInventoryById(id);
        if (existingInventory is null)
        {
            return NotFound();
        }
        _inventoryService.DeleteInventory(id);
        return Ok();
    }
    [HttpDelete("batch")]
    public ActionResult DeleteInventories([FromBody] List<int> ids)
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
        _inventoryService.DeleteInventories(ids);
        return Ok("inventories deleted");
    }

    //PATCH: /inventories/{id}
    [HttpPatch("{id}")]
    public ActionResult<InventoryCS> PatchInventory(int id, [FromQuery] string property, [FromBody] object newvalue)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (property is null || newvalue is null)
        {
            return BadRequest("property or new value is null");
        }
        var patchedInventory = _inventoryService.PatchInventory(id, property, newvalue);
        return Ok(patchedInventory);
    }
}
