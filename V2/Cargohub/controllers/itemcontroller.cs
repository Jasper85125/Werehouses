using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ServicesV2;

namespace ControllersV2;
public class Pageination()
{
    public int Page {get; set;}
    public int PageSize {get;set;}
    public int TotItems {get;set;}
    public List<ItemCS>? Data {get; set;}
}
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

    // Constructor to initialize the ItemController with an IItemService instance
    public ItemController(IItemService itemService, IInventoryService inventoryService)
    {
        _itemService = itemService;
        _inventoryService = inventoryService;
    }
    
    // GET: items
    // Retrieves all items
    [HttpGet("page")]
    public ActionResult<IEnumerable<ItemCS>> GetAllItems([FromQuery] itemFilter tofilter, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        List<string> listOfAllowedRoles = new List<string>()
        { "Admin", "Warehouse Manager", "Inventory Manager", "Floor Manager", "Sales", "Analyst", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var items = _itemService.GetAllItems();
        var itemsquery = items.AsQueryable();
        if (tofilter.GetType().GetProperties().All(prop => prop.GetValue(tofilter) != null))
        {
            var itemsToFilter = items.AsQueryable();
            if (!string.IsNullOrEmpty(tofilter.code))
            {
                itemsToFilter = itemsToFilter.Where(_ => _.code == tofilter.code);
            }
            if (!string.IsNullOrEmpty(tofilter.commodity_code))
            {
                itemsToFilter = itemsToFilter.Where(_ => _.commodity_code == tofilter.commodity_code);
            }
            if (!string.IsNullOrEmpty(tofilter.upc_code))
            {
                itemsToFilter = itemsToFilter.Where(_ => _.upc_code == tofilter.upc_code);
            }
            if (!string.IsNullOrEmpty(tofilter.model_number))
            {
                itemsToFilter = itemsToFilter.Where(_ => _.model_number == tofilter.model_number);
            }
            if (tofilter.item_line.HasValue && tofilter.item_line > 0)
            {
                itemsToFilter = itemsToFilter.Where(_ => _.item_line == tofilter.item_line);
            }
            if (tofilter.item_group.HasValue && tofilter.item_group > 0)
            {
                itemsToFilter = itemsToFilter.Where(_ => _.item_group == tofilter.item_group);
            }
            if (tofilter.item_type.HasValue && tofilter.item_type > 0)
            {
                itemsToFilter = itemsToFilter.Where(_ => _.item_type == tofilter.item_type);
            }
            var filtereditemsCount = itemsToFilter.Count();
            int totalPages = (int)Math.Ceiling(filtereditemsCount / (double)pageSize);

            var index1 = (page - 1) * pageSize;
            var filteredpageItems = itemsToFilter.Skip(index1).Take(pageSize).ToList();

            var result1 = new Pageination(){ Page=page, PageSize=pageSize, TotItems=totalPages, Data=filteredpageItems};
            return Ok(result1);
        }
        int itemsCount = items.Count();
        int pagetotal = (int)Math.Ceiling(itemsCount / (double)pageSize);

        var index = (page - 1) * pageSize;
        var pageItems = itemsquery.Skip(index).Take(pageSize).ToList();
        var result2 = new
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = itemsCount,
            TotalPages = pagetotal,
            Data = pageItems
        };
        return Ok(result2);
    }

    // GET: items
    // Retrieves all items
    [HttpGet()]
    public ActionResult<IEnumerable<ItemCS>> GetAllItems()
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var items = _itemService.GetAllItems();
        return Ok(items);
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
