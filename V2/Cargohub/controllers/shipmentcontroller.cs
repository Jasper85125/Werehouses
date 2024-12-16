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
    ActionLogService _actionlogservice;
    public ShipmentController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
        _actionlogservice = new ActionLogService();
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

    [HttpGet("latest-actions")]
    public ActionResult<IEnumerable<object>> GetShipmentsWithLatestActions()
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

        if (userRole == null || !allowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }
        var shipments = _shipmentService.GetAllShipments();
        var actions = _actionlogservice.GetLatestActionsForShipments();

        var result = shipments.Select(shipment => new
        {
            Shipment = shipment,
            LatestAction = actions.FirstOrDefault(action => action.model == "shipment")
        });

        return Ok(result);
    }
    
    [HttpGet("latest-actions/{amount}")]
    public ActionResult<IEnumerable<object>> GetShipmentsWithLatestActions([FromRoute] int amount)
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

        if (userRole == null || !allowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var shipments = _shipmentService.GetAllShipments();
        var actions = _actionlogservice.GetLatestActionsForShipments();

        var result = shipments.Select(shipment => new
        {
            Shipment = shipment,
            LatestAction = actions.FirstOrDefault(action => action.model == "shipment")
        });
        var listed = result.ToList();
        while(listed.Count() > amount){
            listed.Remove(listed.Last());
        }

        return Ok(listed);
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

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "shipment";
        actionLog.action = "shipment created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

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

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "shipment";
        actionLog.action = "multiple shipments created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

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

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "shipment";
        actionLog.action = "shipment updated";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

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

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "shipment";
        actionLog.action = "items inside shipment updated";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok(updated);
    }
    [HttpPatch("{id}/{property}")]
    public ActionResult<ShipmentCS> PatchShipment([FromRoute] int id, [FromRoute] string property, [FromBody] object newvalue)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }
        
        if (property is null || newvalue is null)
        {
            return BadRequest("invalid request");
        }
        var result = _shipmentService.PatchShipment(id, property, newvalue);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "shipment";
        actionLog.action = "shipment patched";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok(result);
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

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "shipment";
        actionLog.action = "shipment deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

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

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "shipment";
        actionLog.action = "item inside shipment created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

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

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "shipment";
        actionLog.action = "multiple shipments deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok("deleted shipments");
    }
}
