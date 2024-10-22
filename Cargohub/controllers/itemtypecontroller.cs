using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Services;
using System.Threading.Tasks;

namespace Controllers;

[Route("itemtypes")]
[ApiController]
public class ItemTypeController : ControllerBase
{
    private readonly IItemtypeService _itemtypeService;

    // Constructor to initialize the ItemController with an IItemService instance
    public ItemTypeController(IItemtypeService itemtypeService)
    {
        _itemtypeService = itemtypeService;
    }

    // GET: items
    // Retrieves all items
    [HttpGet()]
    public ActionResult<IEnumerable<ItemCS>> GetAllItemtypes()
    {
        var itemtype = _itemtypeService.GetAllItemtypes();
        return Ok(itemtype);
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

}