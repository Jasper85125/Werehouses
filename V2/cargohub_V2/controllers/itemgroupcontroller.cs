using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Services;

namespace Controllers;

[Route("itemgroups")]
[ApiController]
public class ItemGroupController : ControllerBase
{
    private readonly IitemGroupService _itemgroupService;

    public ItemGroupController(IitemGroupService itemgroupService)
    {
        _itemgroupService = itemgroupService;
    }

    // GET: itemgroup
    // Retrieves all itemgroups
    [HttpGet()]
    public ActionResult<IEnumerable<ItemGroupCS>> GetAllItemGroups()
    {
        var itemgroup = _itemgroupService.GetAllItemGroups();
        return Ok(itemgroup);
    }

    // GET: itemtype/5
    [HttpGet("{id}")]
    public ActionResult<ItemGroupCS> GetItemById(int id)
    {
        var itemtype = _itemgroupService.GetItemById(id);
        if (itemtype == null)
        {
            return NotFound();
        }
        return Ok(itemtype);
    }
    //GET: all item in item groups
    [HttpGet("{id}/items")]
    public ActionResult<List<ItemCS>> GetAllItemsFromItemGroupId(int id)
    {
        var result = _itemgroupService.ItemsFromItemGroupId(id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    // POST: itemgroups
    [HttpPost()]
    public IActionResult CreateItemGroup([FromBody] ItemGroupCS itemGroup)
    {
        if (itemGroup == null)
        {
            return BadRequest("ItemGroup cannot be null");
        }

        var createdItemGroup = _itemgroupService.CreateItemGroup(itemGroup);
        return CreatedAtAction(nameof(GetItemById), new { id = createdItemGroup.Id }, createdItemGroup);
    }

    // POST: /itemgroup/multiple
    [HttpPost("multiple")]
    public ActionResult<ItemGroupCS> CreateMultipleItemGroups([FromBody] List<ItemGroupCS> newItemGroup)
    {
        if (newItemGroup is null)
        {
            return BadRequest("Item group data is null");
        }

        var createdItemGroups = _itemgroupService.CreateMultipleItemGroups(newItemGroup);
        return StatusCode(StatusCodes.Status201Created, createdItemGroups);
    }

    [HttpPut("{id}")]
    public ActionResult<ItemGroupCS> UpdateItemGroup([FromRoute] int id, [FromBody] ItemGroupCS itemGroup)
    {
        if (id != itemGroup.Id)
        {
            return BadRequest();
        }

        var existingItemLine = _itemgroupService.GetItemById(id);
        if (existingItemLine == null)
        {
            return NotFound();
        }

        var updatedItemLine =  _itemgroupService.UpdateItemGroup(id, itemGroup);
        return Ok(updatedItemLine);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteItemGroup(int id)
    {
        var itemGroup = _itemgroupService.GetItemById(id);
        if (itemGroup == null)
        {
            return NotFound();
        }

        _itemgroupService.DeleteItemGroup(id);
        return Ok();
    }
    
    [HttpDelete("batch")]
    public ActionResult DeleteItemGroups([FromBody] List<int> ids){
        if(ids is null)
        {
            return NotFound();
        }
        _itemgroupService.DeleteItemGroups(ids);
        return Ok("Item Groups deleted");
    }

    [HttpPatch("{id}")]
    public ActionResult<ItemGroupCS> PatchItemGroup([FromRoute] int id, [FromBody] ItemGroupCS itemGroup)
    {
        var existingItemGroup = _itemgroupService.GetItemById(id);
        if (existingItemGroup == null)
        {
            return NotFound();
        }

        itemGroup.Id = id;
        var updatedItemGroup = _itemgroupService.PatchItemGroup(id, itemGroup);

        return Ok(updatedItemGroup);
    }
}
