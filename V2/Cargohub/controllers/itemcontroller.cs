using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ServicesV2;

namespace ControllersV2;

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

    [HttpGet("/filter")]
    public ActionResult FilterPage([FromQuery] itemFilter tofilter = null, [FromQuery] string page = null, [FromQuery] string pageSize = null)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }
        var items = _itemService.GetAllItems();
        if (!tofilter.GetType().GetProperties().All(prop => prop.GetValue(tofilter) == null))
        {
            var filtereditems = items.AsQueryable();
            if (!string.IsNullOrEmpty(tofilter.code))
            {
                filtereditems = filtereditems.Where(_ => _.code == tofilter.code);
            }
            if (!string.IsNullOrEmpty(tofilter.commodity_code))
            {
                filtereditems = filtereditems.Where(_ => _.commodity_code == tofilter.commodity_code);
            }
            if (!string.IsNullOrEmpty(tofilter.model_number))
            {
                filtereditems = filtereditems.Where(_ => _.model_number == tofilter.model_number);
            }
            if (!string.IsNullOrEmpty(tofilter.upc_code))
            {
                filtereditems = filtereditems.Where(_ => _.upc_code == tofilter.upc_code);
            }
            if (tofilter.item_line.HasValue && tofilter.item_line.Value > 0)
            {
                filtereditems = filtereditems.Where(_ => _.item_line == tofilter.item_line.Value);
            }
            if (tofilter.item_type.HasValue && tofilter.item_type.Value > 0)
            {
                filtereditems = filtereditems.Where(_ => _.item_type == tofilter.item_type.Value);
            }
            int pageInt = int.TryParse(page, out int result) ? result : 1;
            int pageSizeInt = int.TryParse(pageSize, out int result1) ? result1 : filtereditems.Count();

            var filtereditemsCount = filtereditems.Count();
            var totalpages = (int)Math.Ceiling(filtereditemsCount / (double)pageSizeInt);

            var index = (pageInt - 1) * pageSizeInt;
            var pageItems = filtereditems.Skip(index).Take(pageSizeInt).ToList();

            var result2 = new
            {
                Page = pageInt,
                pagesize = pageSizeInt,
                TotalItems = filtereditemsCount,
                TotalPages = totalpages,
                Data = pageItems,
            };
            return Ok(result2);
        }
        else
        {
            int pageInt = int.TryParse(page, out int result) ? result : 1;
            int pageSizeInt = int.TryParse(pageSize, out int result1) ? result1 : 10;

            var itemsCount = items.Count();
            var totalpages = (int)Math.Ceiling(itemsCount / (double)pageSizeInt);

            var index = (pageInt - 1) * pageSizeInt;
            var pageItems = items.Skip(index).Take(pageSizeInt).ToList();

            var result2 = new
            {
                Page = pageInt,
                pagesize = pageSizeInt,
                TotalItems = itemsCount,
                TotalPages = totalpages,
                Data = pageItems,
            };

            return Ok(result2);
            }
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
