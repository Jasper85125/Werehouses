using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ServicesV2;

namespace ControllersV2;

public class itemFilter()
{
    public string? code { get; set; }
    public string? upc_code { get; set; }
    public string? model_number { get; set; }
    public string? commodity_code { get; set; }
    public int? item_line { get; set; } // Changed to nullable
    public int? item_type { get; set; } // Changed to nullable
    public int? item_group { get; set; } // Changed to nullable
    public int? unit_purchase_quantity { get; set; } // Changed to nullable
    public int? unit_order_quantity { get; set; } // Changed to nullable
    public int? pack_order_quantity { get; set; } // Changed to nullable
    public int? supplier_id { get; set; } // Changed to nullable
    public string? supplier_code { get; set; }
}
[Route("api/v2/items")]
[ApiController]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;
    private readonly IInventoryService _inventoryService;
    private readonly ILocationService _locationService;

    // Constructor to initialize the ItemController with an IItemService instance
    public ItemController(IItemService itemService, IInventoryService inventoryService, ILocationService locationService)
    {
        _itemService = itemService;
        _inventoryService = inventoryService;
        _locationService = locationService;
    }

    [HttpGet()]
    public ActionResult<PaginationCS<ItemCS>> GetAllItems(
        [FromQuery] itemFilter tofilter,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        if (!HttpContext.Items.TryGetValue("WarehouseID", out var warehouseIdObj) || !(warehouseIdObj is string warehouseID))
        {
            return BadRequest("WarehouseID is missing or invalid.");
        }

        var allowedRoles = new List<string> { "Admin", "Warehouse Manager", "Inventory Manager", "Floor Manager", "Sales", "Analyst", "Logistics" };
        if (string.IsNullOrEmpty(userRole) || !allowedRoles.Contains(userRole))
        {
            if (userRole == "Operative" || userRole == "Supervisor")
            {
                // Parse the warehouse IDs from the input
                var warehouseIds = warehouseID.Split(',').Select(int.Parse).ToList();

                // Get all locations connected to the user's warehouses
                var locations = _locationService.GetAllLocations();
                var filteredLocations = locations.Where(location => warehouseIds.Contains(location.warehouse_id)).ToList();

                if (!filteredLocations.Any())
                {
                    return NotFound("No locations found for the specified warehouses.");
                }

                // Extract location IDs for inventory lookup
                var locationIds = filteredLocations.Select(location => location.Id).ToList();

                // Get all inventories connected to the filtered locations
                var inventoriesByLocation = _inventoryService.GetInventoriesByLocationId(locationIds);

                if (!inventoriesByLocation.Any())
                {
                    return NotFound("No inventories found for the specified locations.");
                }

                // Get all items
                var allItems = _itemService.GetAllItems();

                // Filter items that exist in the inventories connected to the user's warehouses
                var filteredItems = allItems.Where(item => inventoriesByLocation.Any(inventory => inventory.item_id == item.uid)).ToList();

                if (!filteredItems.Any())
                {
                    return NotFound("No items found for the specified warehouses.");
                }

                return Ok(filteredItems);
            }
            
            else
            {
                return Unauthorized();
            }
        }

        if (tofilter == null)
        {
            tofilter = new itemFilter();
        }

        var items = _itemService.GetAllItems();
        var query = items.AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(tofilter.code))
        {
            query = query.Where(item => item.code == tofilter.code);
        }
        if (!string.IsNullOrEmpty(tofilter.commodity_code))
        {
            query = query.Where(item => item.commodity_code == tofilter.commodity_code);
        }
        if (!string.IsNullOrEmpty(tofilter.upc_code))
        {
            query = query.Where(item => item.upc_code == tofilter.upc_code);
        }
        if (!string.IsNullOrEmpty(tofilter.model_number))
        {
            query = query.Where(item => item.model_number == tofilter.model_number);
        }
        if (tofilter.item_line.HasValue)
        {
            query = query.Where(item => item.item_line == tofilter.item_line.Value);
        }
        if (tofilter.item_group.HasValue)
        {
            query = query.Where(item => item.item_group == tofilter.item_group.Value);
        }
        if (tofilter.item_type.HasValue)
        {
            query = query.Where(item => item.item_type == tofilter.item_type.Value);
        }

        // Get the filtered count
        int filteredItemsCount = query.Count();

        // Calculate the total number of pages
        int totalPages = (int)Math.Ceiling(filteredItemsCount / (double)pageSize);

        if (page == 0)
        {
            page = totalPages;
        }
        page = Math.Max(1, Math.Min(page, totalPages));

        // Fetch the items for the requested page
        var pagedItems = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        // Return the paginated and filtered result
        var result = new PaginationCS<ItemCS>()
        {
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            Data = pagedItems
        };

        return Ok(result);
    }
    

    //GET: generate a report of a specific item by its uid it wil recieve a list of u and do it for all of the give items. it contains short_description unit_purchase_quantity unit_order_quantity created_at updated_at. it should save the report in a file.
    [HttpGet("report")]
    public ActionResult GenerateReport([FromBody] List<string> uids)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (uids is null || uids.Count == 0)
        {
            return BadRequest("No items to generate report for.");
        }

        _itemService.GenerateReport(uids);
        return Ok();
    }


    // GET: items/5
    // Retrieves an item by its unique identifier (uid)
    [HttpGet("{uid}")]
    public ActionResult<ItemCS> GetByUid(string uid)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var item = _itemService.GetItemById(uid);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    // GET: /items/{item_id}/inventory
    [HttpGet("{uid}/inventory")]
    public ActionResult<InventoryCS> GetInventoriesForItem([FromRoute] string uid)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var inventory = _inventoryService.GetInventoriesForItem(uid);
        if (inventory is null)
        {
            return NotFound("No inventory found for item with the given uid.");
        }
        return Ok(inventory);
    }

    // POST: items
    [HttpPost()]
    public ActionResult<ItemCS> CreateItem([FromBody] ItemCS newItem)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }
        if (newItem == null)
        {
            return BadRequest("Item is null.");
        }

        var createdItem = _itemService.CreateItem(newItem);
        return CreatedAtAction(nameof(GetByUid), new { uid = createdItem.uid }, createdItem);
    }

    // POST: /items/multiple
    [HttpPost("multiple")]
    public ActionResult<ItemCS> CreateMultipleItems([FromBody] List<ItemCS> newItems)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newItems is null)
        {
            return BadRequest("Item data is null");
        }

        var createdItems = _itemService.CreateMultipleItems(newItems);
        return StatusCode(StatusCodes.Status201Created, createdItems);
    }

    // PUT: items/5
    [HttpPut("{uid}")]
    public ActionResult<ItemCS> UpdateItem(string uid, [FromBody] ItemCS updatedItem)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (updatedItem == null)
        {
            return BadRequest("Item is null.");
        }

        var existingItem = _itemService.GetItemById(uid);
        if (existingItem == null)
        {
            return NotFound();
        }

        var updatedItemResult = _itemService.UpdateItem(uid, updatedItem);
        return Ok(updatedItemResult);
    }
    // change the value of one property in an item object
    [HttpPatch("{uid}/{property}")]
    public ActionResult<ItemCS> PatchItem([FromRoute] string uid, [FromRoute] string property, [FromBody] object newvalue)
    {
        if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(property) || newvalue is null)
        {
            return BadRequest("Error in request");
        }
        var result = _itemService.PatchItem(uid, property, newvalue);
        return Ok(result);
    }
    [HttpDelete("{uid}")]
    public ActionResult DeleteItem(string uid)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var existingItem = _itemService.GetItemById(uid);
        if (existingItem == null)
        {
            return NotFound();
        }

        _itemService.DeleteItem(uid);
        return Ok();
    }

    // Delete: item/multiple/{id}
    // Deletes multiple items by their ids it can be used to delete multiple items at once so a list input is expected
    [HttpDelete("multiple")]
    public ActionResult DeleteMultipleItems([FromBody] List<string> uids)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (uids == null || uids.Count == 0)
        {
            return BadRequest("No items to delete.");
        }

        _itemService.DeleteItems(uids);
        return Ok();
    }
}
