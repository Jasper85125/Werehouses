using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ServicesV1;
using System.Threading.Tasks;

namespace ControllersV1;

[Route("api/v1/itemtypes")]
[ApiController]
public class ItemTypeController : ControllerBase
{
    private readonly IItemtypeService _itemtypeService;
    private readonly IItemService _itemService;

    // Constructor to initialize the ItemController with an IItemService instance
    public ItemTypeController(IItemtypeService itemtypeService, IItemService itemService)
    {
        _itemtypeService = itemtypeService;
        _itemService = itemService;
    }

    // GET: items
    // Retrieves all items
    [HttpGet()]
    public ActionResult<IEnumerable<ItemCS>> GetAllItemtypes()
    {
        var itemtype = _itemtypeService.GetAllItemtypes();
        return Ok(itemtype);
    }

    //GET: /itemtypes/{itemtypeID}/items
    [HttpGet("{id}/items")]
    public ActionResult<IEnumerable<ItemCS>> GetAllItemsInItemType(int id)
    {
        var itemsInItemType = _itemService.GetAllItemsInItemType(id);
        if (itemsInItemType is null)
        {
            return BadRequest("No items found with the given ItemType ID");
        }
        return Ok(itemsInItemType);
    }

    // GET: itemtype/5
    [HttpGet("{id}")]
    public ActionResult<ItemTypeCS> GetItemById(int id)
    {
        var itemtype = _itemtypeService.GetItemById(id);
        if (itemtype == null)
        {
            return NotFound();
        }
        return Ok(itemtype);
    }

    // POST: itemtype
    [HttpPost()]
    public async Task<IActionResult> CreateItemType([FromBody] ItemTypeCS itemtype)
    {
        if (itemtype == null)
        {
            return BadRequest("ItemGroup cannot be null");
        }

        var createditemtype = await _itemtypeService.CreateItemType(itemtype);
        return CreatedAtAction(nameof(GetItemById), new { id = createditemtype.Id }, createditemtype);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ItemTypeCS>> UpdateItemType(int id, [FromBody] ItemTypeCS itemType)
    {
        if (id != itemType.Id)
        {
            return BadRequest();
        }

        var existingItemLine = _itemtypeService.GetItemById(id);
        if (existingItemLine == null)
        {
            return NotFound();
        }

        var updatedItemLine = await _itemtypeService.UpdateItemType(id, itemType);
        return Ok(updatedItemLine);
    }
    
    [HttpDelete("{id}")]
    public ActionResult DeleteItemType(int id)
    {
        var itemtype = _itemtypeService.GetItemById(id);
        if (itemtype == null)
        {
            return NotFound();
        }
        _itemtypeService.DeleteItemType(id);
        return Ok();
    }

}