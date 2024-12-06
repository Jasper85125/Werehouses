using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using ServicesV2;

namespace ControllersV2;

[ApiController]
[Route("api/v2/shipments")]
public class ShipmentController : ControllerBase
{
    private readonly IShipmentService _shipmentService;
    public ShipmentController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    // GET: /shipments
    [HttpGet()]
    public ActionResult<IEnumerable<ShipmentCS>> GetAllShipments()
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var shipments = _shipmentService.GetAllShipments();
        return Ok(shipments);
    }

    // GET: /shipments/{id}
    [HttpGet("{id}")]
    public ActionResult<ShipmentCS> GetShipmentById([FromRoute] int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var shipment = _shipmentService.GetShipmentById(id);
        if (shipment is null)
        {
            return NotFound();
        }
        return Ok(shipment);
    }
    //shipments/{shipment_id}/items
    [HttpGet("{shipment_id}/items")]
    public ActionResult<IEnumerable<ItemIdAndAmount>> GetItemsInShipment([FromRoute] int shipment_id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var items = _shipmentService.GetItemsInShipment(shipment_id);
        if (items is null)
        {
            return NotFound();
        }
        return Ok(items);
    }

    // POST: /shipments
    [HttpPost]
    public ActionResult<ShipmentCS> CreateShipment([FromBody] ShipmentCS newShipment)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newShipment is null)
        {
            return BadRequest("shipment data is null");
        }
        var shipment = _shipmentService.CreateShipment(newShipment);
        return CreatedAtAction(nameof(GetShipmentById), new { id = shipment.Id }, shipment);
    }

    // POST: /shipments/multiple
    [HttpPost("multiple")]
    public ActionResult<IEnumerable<ShipmentCS>> CreateMultipleShipments([FromBody] List<ShipmentCS> newShipments)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newShipments is null)
        {
            return BadRequest("Shipment data is null");
        }

        var createdShipment = _shipmentService.CreateMultipleShipments(newShipments);
        return StatusCode(StatusCodes.Status201Created, createdShipment);
    }


    // PUT: api/warehouse/5
    [HttpPut("{id}")]
    public Task<ActionResult<ShipmentCS>> UpdateShipment(int id, [FromBody] ShipmentCS updateShipment)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Task.FromResult<ActionResult<ShipmentCS>>(Unauthorized());
        }

        if (id != updateShipment.Id)
        {
            return Task.FromResult<ActionResult<ShipmentCS>>(BadRequest());
        }

        var existingItemLine = _shipmentService.GetShipmentById(id);
        if (existingItemLine == null)
        {
            return Task.FromResult<ActionResult<ShipmentCS>>(NotFound());
        }

        var updatedItemLine = _shipmentService.UpdateShipment(id, updateShipment);
        return Task.FromResult<ActionResult<ShipmentCS>>(Ok(updatedItemLine));
    }

    [HttpPut("{shipmentId}/items")]
    public ActionResult<ShipmentCS> UpdateItemsinShipment(int shipmentId, [FromBody] List<ItemIdAndAmount> updateItems)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (updateItems is null)
        {
            return BadRequest("invalid id's/ items");
        }
        ShipmentCS updated = _shipmentService.UpdateItemsInShipment(shipmentId, updateItems);
        if (updated is null)
        {
            return BadRequest("invalid id's/ items");
        }
        return Ok(updated);
    }

    // DELETE: api/warehouse/5
    [HttpDelete("{id}")]
    public ActionResult DeleteShipment(int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var shipment = _shipmentService.GetShipmentById(id);
        if (shipment is null)
        {
            return NotFound();
        }
        _shipmentService.DeleteShipment(id);
        return Ok();
    }

    // DELETE /shipments/{id}/items/{itemid}
    [HttpDelete("{id}/items/{itemid}")]
    public ActionResult DeleteItemFromShipment(int id, string itemid)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var shipment = _shipmentService.GetShipmentById(id);
        if (shipment is null)
        {
            return NotFound();
        }
        _shipmentService.DeleteItemFromShipment(id, itemid);
        return Ok();
    }

    [HttpDelete("batch")]
    public ActionResult DeleteShipments([FromBody] List<int> ids)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (ids is null)
        {
            return NotFound();
        }
        _shipmentService.DeleteShipments(ids);
        return Ok("deleted shipments");
    }
}
