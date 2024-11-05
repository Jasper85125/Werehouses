using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Services;

namespace Controllers;

[Route("items")]
[ApiController]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;

    // Constructor to initialize the ItemController with an IItemService instance
    public ItemController(IItemService itemService)
    {
        _itemService = itemService;
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

    // POST: items
    // Creates a new item
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
    // Updates an existing item
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
}
