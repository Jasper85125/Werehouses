using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Services;
using System.Threading.Tasks;
using Controllers;

[Route("itemlines")]
[ApiController]
public class ItemLineController : ControllerBase
{
    private readonly IItemLineService _itemLineService;

    // Constructor to initialize the ItemController with an IItemService instance
    public ItemLineController(IItemLineService itemLineService)
    {
        _itemLineService = itemLineService;
    }

    // GET: api/items
    // Retrieves all items
    [HttpGet()]
    public ActionResult<IEnumerable<ItemCS>> GetAllItemLines()
    {
        var itemLine = _itemLineService.GetAllItemlines();
        return Ok(itemLine);
    }

    // GET: api/itemLine/5
    [HttpGet("{id}")]
    public ActionResult<ItemLineCS> GetItemLineById(int id)
    {
        var itemLine = _itemLineService.GetItemLineById(id);
        if (itemLine == null)
        {
            return NotFound();
        }
        return Ok(itemLine);
    }

    // POST: api/itemLine
    [HttpPost]
    public async Task<ActionResult<ItemLineCS>> AddItemLine([FromBody] ItemLineCS newItemLine)
    {
        if (newItemLine == null)
        {
            return BadRequest("ItemLine is null.");
        }

        var createdItemLine = await _itemLineService.AddItemLine(newItemLine);
        return CreatedAtAction(nameof(GetItemLineById), new { id = createdItemLine.Id }, createdItemLine);
    }
}
