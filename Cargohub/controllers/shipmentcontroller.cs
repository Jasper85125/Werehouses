using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using Services;

namespace Controllers;

[ApiController]
[Route("/shipments")]
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

    // POST: /shipments
    [HttpPost]
    public async Task Post([FromBody] ShipmentCS shipment)
    {
        
    }

    // PUT: api/warehouse/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
        // Replace with your logic
    }

    // DELETE: api/warehouse/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
        // Replace with your logic
    }
}
