using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Services;

namespace Controllers;

[Route("items")]
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
    [HttpGet()]
    public ActionResult<IEnumerable<ItemCS>> GetAllItems()
    {
        var items = _itemService.GetAllItems();
        return Ok(items);
    }

    // GET: items/5
    // Retrieves an item by its unique identifier (uid)
    [HttpGet("{uid}")]
    public ActionResult<ItemCS> GetByUid(string uid)
    {
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
        if (newItem == null)
        {
            return BadRequest("Item is null.");
        }

        var createdItem = _itemService.CreateItem(newItem);
        return CreatedAtAction(nameof(GetByUid), new { uid = createdItem.uid }, createdItem);
    }

    // PUT: items/5
    [HttpPut("{uid}")]
    public ActionResult<ItemCS> UpdateItem(string uid, [FromBody] ItemCS updatedItem)
    {
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
    [HttpDelete("{uid}")]
    public ActionResult DeleteItem(string uid)
    {
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
        if (uids == null || uids.Count == 0)
        {
            return BadRequest("No items to delete.");
        }

        _itemService.DeleteItems(uids);
        return Ok();
    } 

    // POST: items/multiple
    // Creates multiple items at once by providing a list of items in the request body
    [HttpPost("multiple")]
    public ActionResult CreateMultipleItems([FromBody] List<ItemCS> items)
    {
        if (items == null || items.Count == 0)
        {
            return BadRequest("No items to create.");
        }

        _itemService.CreateItems(items);
        return Ok();
    }
}
