using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using ServicesV2;

namespace ControllersV2;

[ApiController]
[Route("api/v2/warehouses")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;
    ActionLogService _actionlogservice;

    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
        _actionlogservice = new ActionLogService();
    }

    // GET: /warehouses
    [HttpGet()]
    public ActionResult<IEnumerable<WarehouseCS>> GetAllWarehouses()
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Analyst", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

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
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager", "Floor Manager", "Sales", "Analyst", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var warehouse = _warehouseService.GetWarehouseById(id);
        if (warehouse is null)
        {
            return NotFound();
        }
        return Ok(warehouse);
    }

    [HttpGet("latest-actions")]
    public ActionResult<IEnumerable<object>> GetWarehousesWithLatestActions()
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

        if (userRole == null || !allowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var warehouses = _warehouseService.GetAllWarehouses();
        var actions = _actionlogservice.GetLatestActionsForWarehouses();

        var result = warehouses.Select(warehouse => new
        {
            Warehouse = warehouse,
            LatestAction = actions.FirstOrDefault(action => action.model == "warehouse")
        });

        return Ok(result);
    }
    [HttpGet("latest-actions/{amount}")]
    public ActionResult<IEnumerable<object>> GetWarehousesWithLatestActions([FromRoute] int amount)
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

        if (userRole == null || !allowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var warehouses = _warehouseService.GetAllWarehouses();
        var actions = _actionlogservice.GetLatestActionsForWarehouses();

        var result = warehouses.Select(warehouse => new
        {
            Warehouse = warehouse,
            LatestAction = actions.FirstOrDefault(action => action.model == "warehouse")
        });
        var listed = result.ToList();
        while(listed.Count() > amount){
            listed.Remove(listed.Last());
        }

        return Ok(listed);
    }

    // POST: /warehouses
    [HttpPost()]
    public ActionResult<WarehouseCS> CreateWarehouse([FromBody] WarehouseCS newWarehouse)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newWarehouse is null)
        {
            return BadRequest("Warehouse data is null");
        }

        var createdWarehouse = _warehouseService.CreateWarehouse(newWarehouse);
        
        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "warehouse";
        actionLog.action = "warehouse created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return CreatedAtAction(nameof(GetWarehouseById), new { id = createdWarehouse.Id }, createdWarehouse);
    }

    // POST: /warehouses/multiple
    [HttpPost("multiple")]
    public ActionResult<WarehouseCS> CreateMultipleWarehouse([FromBody] List<WarehouseCS> newWarehouse)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newWarehouse is null)
        {
            return BadRequest("Warehouse data is null");
        }

        var createdWarehouses = _warehouseService.CreateMultipleWarehouse(newWarehouse);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "warehouse";
        actionLog.action = "multiple warehouse created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return StatusCode(StatusCodes.Status201Created, createdWarehouses);
    }

    // PUT: /warehouses/{id}
    [HttpPut("{id}")]
    public ActionResult<WarehouseCS> UpdateWarehouse([FromRoute] int id, [FromBody] WarehouseCS newWarehouse)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newWarehouse is null)
        {
            return BadRequest("Warehouse is null.");
        }

        var updatedWarehouse = _warehouseService.UpdateWarehouse(id, newWarehouse);
        if (updatedWarehouse is null)
        {
            return NotFound("No warehouse found with the given id.");
        }

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "warehouse";
        actionLog.action = "warehouse updated";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok(updatedWarehouse);
    }
    //PATCH: Warehouse/{id}/{property_to_change}
    //''   :     ''   /  ''/      contact == werkt niet
    [HttpPatch("{id}/{property}")]
    public ActionResult<WarehouseCS> PatchWarehouse([FromRoute] int id, [FromRoute] string property, [FromBody] object newvalue){
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if(newvalue is null || int.IsNegative(id) || string.IsNullOrEmpty(property)){
            return NotFound("Erhm what?");
        }
        var result = _warehouseService.PatchWarehouse(id, property, newvalue);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "warehouse";
        actionLog.action = "warehouse patched";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok(result);
    }
    // DELETE: api/warehouse/5
    [HttpDelete("{id}")]
    public ActionResult DeleteWarehouse([FromRoute] int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var warehouse = _warehouseService.GetWarehouseById(id);
        if (warehouse is null)
        {
            return NotFound();
        }
        _warehouseService.DeleteWarehouse(id);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "warehouse";
        actionLog.action = "warehouse deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok();
    }

    [HttpDelete("batch")]
    public ActionResult DeleteWarehouses([FromBody] List<int> ids)
    {
        List<string> listOfAllowedRoles = new List<string> () {"Admin"};
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (ids is null)
        {
            return NotFound();
        }
        _warehouseService.DeleteWarehouses(ids);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "warehouse";
        actionLog.action = "multiple warehouses deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok("Deleted warehouses");
    }

    // // GET: /warehouses/latest
    // [HttpGet("latest")]
    // public ActionResult<WarehouseCS> GetLatestUpdatedWarehouse()
    // {
    //     List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Analyst", "Logistics" };
    //     var userRole = HttpContext.Items["UserRole"]?.ToString();

    //     if (userRole == null || !listOfAllowedRoles.Contains(userRole))
    //     {
    //         return Unauthorized();
    //     }

    //     var latestWarehouse = _warehouseService.GetLatestUpdatedWarehouse();
    //     if (latestWarehouse is null)
    //     {
    //         return NotFound();
    //     }
    //     return Ok(latestWarehouse);
    // }
}
