using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using Services;

namespace Controllers;

[ApiController]
[Route("/warehouses")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;
    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    // GET: /warehouses
    [HttpGet()]
    public ActionResult<IEnumerable<WarehouseCS>> GetAllWarehouses()
    {
        var warehouses = _warehouseService.GetAllWarehouses();
        if (warehouses is null)
        {
            return NotFound();
        }
        return Ok(warehouses);
    }

    // GET: /warehouses/{id}
    [HttpGet("{id}")]
    public ActionResult<WarehouseCS> GetWarehouseById([FromRoute] int id)
    {
        var warehouse = _warehouseService.GetWarehouseById(id);
        if (warehouse is null)
        {
            return NotFound();
        }
        return Ok(warehouse);
    }

    // POST: /warehouses
    [HttpPost()]
    public ActionResult<WarehouseCS> CreateWarehouse([FromBody] WarehouseCS newWarehouse)
    {
        if (newWarehouse is null)
        {
            return BadRequest("Warehouse data is null");
        }

        var createdWarehouse = _warehouseService.CreateWarehouse(newWarehouse);
        return CreatedAtAction(nameof(GetWarehouseById), new { id = createdWarehouse.Id }, createdWarehouse);
    }

    // POST: /warehouses
    [HttpPost("multiple")]
    public ActionResult<WarehouseCS> CreateWarehouse([FromBody] List<WarehouseCS> newWarehouse)
    {
        if (newWarehouse is null)
        {
            return BadRequest("Warehouse data is null");
        }

        var createdWarehouses = _warehouseService.CreateMultipleWarehouse(newWarehouse);
        return StatusCode(StatusCodes.Status201Created, createdWarehouses);
    }

    // PUT: /warehouses/{id}
    [HttpPut("{id}")]
    public ActionResult<WarehouseCS> UpdateWarehouse([FromRoute]int id, [FromBody] WarehouseCS newWarehouse)
    {
        if (newWarehouse is null)
        {
            return BadRequest("Warehouse is null.");
        }

        var updatedWarehouse = _warehouseService.UpdateWarehouse(id, newWarehouse);
        if (updatedWarehouse is null)
        {
            return NotFound("No warehouse found with the given id.");
        }
        return Ok(updatedWarehouse);
    }

    // DELETE: api/warehouse/5
    [HttpDelete("{id}")]
    public ActionResult DeleteWarehouse([FromRoute] int id)
    {
        var warehouse = _warehouseService.GetWarehouseById(id);
        if (warehouse is null)
        {
            return NotFound();
        }
        _warehouseService.DeleteWarehouse(id);
        
        return Ok();
    }
}
