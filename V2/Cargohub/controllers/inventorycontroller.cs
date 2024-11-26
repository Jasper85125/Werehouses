using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using ServicesV2;
using System.Reflection.Metadata.Ecma335;

namespace ControllersV2;

[ApiController]
[Route("/inventories")]
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
    // (GET) The system must allow users to fetch inventory totals for a specific item. the total is the total_on_hand + total_allocated it must return either a 200 or 404 status code and the sum of the total_on_hand and total_allocated for the item.
    [HttpGet("total/{item_id}")]
    public ActionResult<int> GetInventoryByItemId([FromRoute]string item_id)
    {
            // (GET) The system must allow users to fetch inventory totals for a specific item. the total is the total_on_hand + total_allocated it must return either a 200 or 404 status code and the sum of the total_on_hand and total_allocated for the item.
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

    // POST: /inventories/multiple
    [HttpPost("multiple")]
    public ActionResult<InventoryCS> CreateMultipleInventories([FromBody] List<InventoryCS> newInventory)
    {
        if (newInventory is null)
        {
            return BadRequest("Inventory data is null");
        }

        var createdInventories = _inventoryService.CreateMultipleInventories(newInventory);
        return StatusCode(StatusCodes.Status201Created, createdInventories);
    }

    // PUT: api/warehouse/5
    [HttpPut("{id}")]
    public ActionResult<InventoryCS> UpdateInventoryById(int id, [FromBody] InventoryCS value)
    {
        // Replace with your logic
        //make use of your update inventory by id service
        //include a check if value is null
        // if(value is null || value.created_at == default || value.updated_at == default 
        // || string.IsNullOrWhiteSpace(value.description) || value.item_id is null || value.item_reference is null 
        // || value.Locations is null){
        //     return BadRequest("request is invalid/contains invalid values");
        // }
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
    [HttpDelete("batch")]
    public ActionResult DeleteInventories ([FromBody] List<int> ids){
        if (ids is null){
            return NotFound();
        }
        _inventoryService.DeleteInventories(ids);
        return Ok("inventories deleted");
    }
}
