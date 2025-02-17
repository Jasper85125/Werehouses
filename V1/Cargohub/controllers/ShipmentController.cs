using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using ServicesV1;

namespace ControllersV1;

[ApiController]
[Route("api/v1/shipments")]
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
        var shipments = _shipmentService.GetAllShipments();
        return Ok(shipments);
    }

    // GET: /shipments/{id}
    [HttpGet("{id}")]
    public ActionResult<ShipmentCS> GetShipmentById([FromRoute]int id)
    {
        var shipment = _shipmentService.GetShipmentById(id);
        if (shipment is null)
        {
            return NotFound();
        }
        return Ok(shipment);
    }
    [HttpGet("{shipment_id}/items")]
    public ActionResult<IEnumerable<ItemIdAndAmount>> GetItemsInShipment([FromRoute]int shipment_id)
    {
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
        if (newShipment is null)
        {
            return BadRequest("shipment data is null");
        }
        var shipment = _shipmentService.CreateShipment(newShipment);
        return CreatedAtAction(nameof(GetShipmentById), new { id = shipment.Id }, shipment);
    }
  

    // PUT: api/warehouse/5
    [HttpPut("{id}")]
    public ActionResult<ShipmentCS> UpdateShipment([FromRoute]int id, [FromBody] ShipmentCS updateShipment)
    {
        if (id != updateShipment.Id)
        {
            return BadRequest();
        }

        var existingItemLine = _shipmentService.GetShipmentById(id);
        if (existingItemLine == null)
        {
            return NotFound();
        }

        var updatedItemLine = _shipmentService.UpdateShipment(id, updateShipment);
        return Ok(updatedItemLine);
    }

    [HttpPut("{shipmentId}/items")]
    public ActionResult<ShipmentCS> UpdateItemsinShipment(int shipmentId, [FromBody] List<ItemIdAndAmount> updateItems){
        if(updateItems is null)
        {
            return BadRequest("invalid id's/ items");
        }
        ShipmentCS updated = _shipmentService.UpdateItemsInShipment(shipmentId, updateItems);
        if(updated is null){
            return BadRequest("invalid id's/ items");
        }
        return Ok(updated);
    }


    // DELETE: api/warehouse/5
    [HttpDelete("{id}")]
    public ActionResult DeleteShipment(int id)
    {
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
        var shipment = _shipmentService.GetShipmentById(id);
        if (shipment is null)
        {
            return NotFound();
        }
        _shipmentService.DeleteItemFromShipment(id, itemid);
        return Ok();
    }
}
