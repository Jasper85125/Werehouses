using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ServicesV1;
using System.Threading.Tasks;

namespace ControllersV1;

[Route("api/v1/itemgroups")]
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
    public ActionResult<List<ItemCS>> GetAllItemsFromItemGroupId(int id){
        var result = _itemgroupService.ItemsFromItemGroupId(id);
        if (result is null) return NotFound();
        return Ok(result);
    }


    // POST: itemgroups
    [HttpPost()]
    public async Task<IActionResult> CreateItemGroup([FromBody] ItemGroupCS itemGroup)
    {
        if (itemGroup == null)
        {
            return BadRequest("ItemGroup cannot be null");
        }

        var createdItemGroup = await _itemgroupService.CreateItemGroup(itemGroup);
        return CreatedAtAction(nameof(GetItemById), new { id = createdItemGroup.Id }, createdItemGroup);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ItemGroupCS>> UpdateItemGroup(int id, [FromBody] ItemGroupCS itemGroup)
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

        var updatedItemLine = await _itemgroupService.UpdateItemGroup(id, itemGroup);
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

}
