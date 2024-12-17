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
    //filter for get all items
    [HttpGet("searchterm?={term}")]
    public ActionResult<List<ItemCS>> Filter([FromRoute] string? searchterm){
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }
        if(!string.IsNullOrWhiteSpace(searchterm.ToString())){
            List<ItemCS> items = _itemService.GetAllItems();
            var filtered = items.Where(_=>_.code.Contains(searchterm.ToString())|| 
            _.commodity_code.Contains(searchterm.ToString())|| 
            // _.item_group == (int)searchterm ||
            // _.item_line == (int)searchterm ||
            // _.item_type == (int)searchterm ||
            _.model_number.Contains(searchterm.ToString())||
            // _.pack_order_quantity == (int)searchterm ||
            _.supplier_code.Contains(searchterm.ToString())||
            // _.supplier_id == (int)searchterm ||
            _.supplier_part_number.Contains(searchterm.ToString())||
            _.uid == searchterm.ToString() ||
            // _.unit_order_quantity == (int)searchterm ||
            // _.unit_purchase_quantity == (int)searchterm ||
            _.upc_code.Contains(searchterm.ToString())).OrderBy(_=>_.updated_at).ToList();
            return Ok(filtered);
        }
        return BadRequest();
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
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales"};
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
    public ActionResult<ItemCS> PatchItem([FromRoute] string uid, [FromRoute] string property, [FromBody] object newvalue){
        if(string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(property) || newvalue is null){
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
