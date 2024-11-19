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

    // PUT: api/itemLine/5
    [HttpPut("{id}")]
    public async Task<ActionResult<ItemLineCS>> UpdateItemLine(int id, [FromBody] ItemLineCS itemLine)
    {
        if (id != itemLine.Id)
        {
            return BadRequest();
        }

        var existingItemLine = _itemLineService.GetItemLineById(id);
        if (existingItemLine == null)
        {
            return NotFound();
        }

        var updatedItemLine = await _itemLineService.UpdateItemLine(id, itemLine);
        return Ok(updatedItemLine);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteItemLine(int id)
    {
        var itemLine = _itemLineService.GetItemLineById(id);
        if (itemLine == null)
        {
            return NotFound();
        }

        _itemLineService.DeleteItemLine(id);
        return Ok();
    }

    // GET: /{id}/items
    [HttpGet("{id}/items")]
    public ActionResult<IEnumerable<ItemCS>> GetItemsByItemLineId([FromRoute] int id)
    {
        var Itemline = _itemLineService.GetItemLineById(id);
        if (Itemline is null)
        {
            return NotFound();
        }

        var items = _itemLineService.GetItemsByItemLineId(id);
        return Ok(items);
    }

    [HttpDelete("batch")]
    public ActionResult DeleteItemLines([FromBody]List<int> ids){
        if(ids is null){
            return NotFound();
        }
        _itemLineService.DeleteItemLines(ids);
        return Ok("Item lines deleted");
    }

    [HttpPatch("{id}")]
    public ActionResult<ItemLineCS> PatchItemLine(int id, [FromBody] ItemLineCS itemLine)
    {
        if (id != itemLine.Id)
        {
            return BadRequest();
        }

        var existingItemLine = _itemLineService.GetItemLineById(id);
        if (existingItemLine == null)
        {
            return NotFound();
        }

        var updatedItemLine = _itemLineService.PatchItemLine(id, itemLine);
        return Ok(updatedItemLine);
    }
}
