using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using ServicesV1;

namespace ControllersV1;
//test yamal delete later
[ApiController]
[Route("api/v1/inventories")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    // GET: /inventories
    [HttpGet()]
    public ActionResult<IEnumerable<InventoryCS>> GetAllInventories()
    {
        var inventories = _inventoryService.GetAllInventories();
        return Ok(inventories);
    }

    // GET: /inventories/{id}
    [HttpGet("{id}")]
    public ActionResult<InventoryCS> GetInventoryById([FromRoute]int id)
    {
        var inventory = _inventoryService.GetInventoryById(id);
        if (inventory is null)
        {
            return NotFound();
        }
        return Ok(inventory);
    }

    // GET: /inventories/total/{item_id}
    // (GET) The system must allow users to fetch inventory totals for a specific item.
    [HttpGet("total/{item_id}")]
    public ActionResult<int> GetInventoryByItemId([FromRoute]string item_id)
    {
            var inventory = _inventoryService.GetInventoriesForItem(item_id);
            if (inventory is null)
            {
                return NotFound();
            }
            var inventory_total = inventory.total_on_hand + inventory.total_allocated;
            return Ok(inventory_total);
       
    }
    // POST: /inventories
    [HttpPost()]
    public ActionResult<InventoryCS> CreateInventory([FromBody] InventoryCS inventory)
    {
        if (inventory is null)
        {
            return BadRequest("Inventory is null");
        }
        var newInventory = _inventoryService.CreateInventory(inventory);
        return CreatedAtAction(nameof(GetInventoryById), new { id = newInventory.Id }, newInventory);
    }

    // PUT: api/warehouse/5
    [HttpPut("{id}")]
    public ActionResult<InventoryCS> UpdateInventoryById(int id, [FromBody] InventoryCS value)
    {
        if(value is null)
        {
            return BadRequest("request is invalid/contains invalid values");
        }
        var patchedinventory = _inventoryService.UpdateInventoryById(id, value);
        return Ok(patchedinventory);
    }

    // DELETE: api/warehouse/5
    [HttpDelete("{id}")]
    public ActionResult DeleteInventory(int id)
    {
        var existingInventory = _inventoryService.GetInventoryById(id);
        if (existingInventory is null)
        {
            return NotFound();
        }
        _inventoryService.DeleteInventory(id);
        return Ok();
    }
}
