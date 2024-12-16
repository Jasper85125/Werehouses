using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using ServicesV2;
using System.Reflection.Metadata.Ecma335;

namespace ControllersV2;

[ApiController]
[Route("api/v2/inventories")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    ActionLogService _actionlogservice;
    public InventoryController(IInventoryService inventoryService)
    {
        _actionlogservice = new ActionLogService();
        _inventoryService = inventoryService;
    }

    // GET: /inventories
    [HttpGet()]
    public ActionResult<IEnumerable<InventoryCS>> GetAllInventories()
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var inventories = _inventoryService.GetAllInventories();
        return Ok(inventories);
    }

    // GET: /inventories/{id}
    [HttpGet("{id}")]
    public ActionResult<InventoryCS> GetInventoryById([FromRoute] int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

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
    public ActionResult<int> GetInventoryByItemId([FromRoute] string item_id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        // (GET) The system must allow users to fetch inventory totals for a specific item. the total is the total_on_hand + total_allocated it must return either a 200 or 404 status code and the sum of the total_on_hand and total_allocated for the item.
        var inventory = _inventoryService.GetInventoriesForItem(item_id);
        if (inventory is null)
        {
            return NotFound();
        }
        var inventory_total = inventory.total_on_hand + inventory.total_allocated;
        return Ok(inventory_total);

    }
    [HttpGet("latest-actions")]
    public ActionResult<IEnumerable<object>> GetInventoriesWithLatestActions()
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

        if (userRole == null || !allowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }
        var inventories = _inventoryService.GetAllInventories();
        var actions = _actionlogservice.GetLatestActionsForSuppliers();

        var result = inventories.Select(supplier => new
        {
            Inventory = supplier,
            LatestAction = actions.FirstOrDefault(action => action.model == "supplier")
        });

        return Ok(result);
    }
    
    [HttpGet("latest-actions/{amount}")]
    public ActionResult<IEnumerable<object>> GetSuppliersWithLatestActions([FromRoute] int amount)
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

        if (userRole == null || !allowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var inventories = _inventoryService.GetAllInventories();
        var actions = _actionlogservice.GetLatestActionsForSuppliers();

        var result = inventories.Select(inventory => new
        {
            Inventory = inventory,
            LatestAction = actions.FirstOrDefault(action => action.model == "inventory")
        });
        var listed = result.ToList();
        while(listed.Count() > amount){
            listed.Remove(listed.Last());
        }

        return Ok(listed);
    }

    // POST: /inventories
    [HttpPost()]
    public ActionResult<InventoryCS> CreateInventory([FromBody] InventoryCS inventory)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (inventory is null)
        {
            return BadRequest("Inventory is null");
        }

        var newInventory = _inventoryService.CreateInventory(inventory);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "inventory";
        actionLog.action = "inventory created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return CreatedAtAction(nameof(GetInventoryById), new { id = newInventory.Id }, newInventory);
    }

    // POST: /inventories/multiple
    [HttpPost("multiple")]
    public ActionResult<InventoryCS> CreateMultipleInventories([FromBody] List<InventoryCS> newInventory)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newInventory is null)
        {
            return BadRequest("Inventory data is null");
        }

        var createdInventories = _inventoryService.CreateMultipleInventories(newInventory);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "inventory";
        actionLog.action = "multiple inventories created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

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
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (value is null)
        {
            return BadRequest("request is invalid/contains invalid values");
        }
        var patchedinventory = _inventoryService.UpdateInventoryById(id, value);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "inventory";
        actionLog.action = "inventory updated";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);
        
        return Ok(patchedinventory);
    }

    // DELETE: api/warehouse/5
    [HttpDelete("{id}")]
    public ActionResult DeleteInventory(int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var existingInventory = _inventoryService.GetInventoryById(id);
        if (existingInventory is null)
        {
            return NotFound();
        }
        _inventoryService.DeleteInventory(id);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "inventory";
        actionLog.action = "inventory deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok();
    }
    [HttpDelete("batch")]
    public ActionResult DeleteInventories([FromBody] List<int> ids)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (ids is null)
        {
            return NotFound();
        }
        _inventoryService.DeleteInventories(ids);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "inventory";
        actionLog.action = "multiple inventories deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok("inventories deleted");
    }

    //PATCH: /inventories/{id}
    [HttpPatch("{id}")]
    public ActionResult<InventoryCS> PatchInventory(int id, [FromBody] InventoryCS patch)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (patch is null)
        {
            return BadRequest("patch document is null");
        }
        var patchedInventory = _inventoryService.PatchInventory(id, patch);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "inventory";
        actionLog.action = "inventory patched";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok(patchedInventory);
    }
}
