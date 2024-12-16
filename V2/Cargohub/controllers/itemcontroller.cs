using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ServicesV2;

namespace ControllersV2;

[Route("api/v2/items")]
[ApiController]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;
    private readonly IInventoryService _inventoryService;
    ActionLogService _actionlogservice;

    // Constructor to initialize the ItemController with an IItemService instance
    public ItemController(IItemService itemService, IInventoryService inventoryService)
    {
        _itemService = itemService;
        _actionlogservice = new ActionLogService();
        _inventoryService = inventoryService;
    }

    // GET: items
    // Retrieves all items
    [HttpGet()]
    public ActionResult<IEnumerable<ItemCS>> GetAllItems()
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var items = _itemService.GetAllItems();
        return Ok(items);
    }

    // GET: items/5
    // Retrieves an item by its unique identifier (uid)
    [HttpGet("{uid}")]
    public ActionResult<ItemCS> GetByUid(string uid)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }
        
        var item = _itemService.GetItemById(uid);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    // GET: /items/{item_id}/inventory
    [HttpGet("{uid}/inventory")]
    public ActionResult<InventoryCS> GetInventoriesForItem([FromRoute] string uid)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var inventory = _inventoryService.GetInventoriesForItem(uid);
        if (inventory is null)
        {
            return NotFound("No inventory found for item with the given uid.");
        }
        return Ok(inventory);
    }

    [HttpGet("latest-actions")]
    public ActionResult<IEnumerable<object>> GetItemsWithLatestActions()
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

        if (userRole == null || !allowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }
        var items = _itemService.GetAllItems();
        var actions = _actionlogservice.GetLatestActionsForItems();

        var result = items.Select(item => new
        {
            Item = item,
            LatestAction = actions.FirstOrDefault(action => action.model == "item")
        });

        return Ok(result);
    }
    
    [HttpGet("latest-actions/{amount}")]
    public ActionResult<IEnumerable<object>> GetItemsWithLatestActions([FromRoute] int amount)
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

        if (userRole == null || !allowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var items = _itemService.GetAllItems();
        var actions = _actionlogservice.GetLatestActionsForItems();

        var result = items.Select(item => new
        {
            Item = item,
            LatestAction = actions.FirstOrDefault(action => action.model == "item")
        });
        var listed = result.ToList();
        while(listed.Count() > amount){
            listed.Remove(listed.Last());
        }

        return Ok(listed);
    }

    // POST: items
    [HttpPost()]
    public ActionResult<ItemCS> CreateItem([FromBody] ItemCS newItem)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }
        if (newItem == null)
        {
            return BadRequest("Item is null.");
        }

        var createdItem = _itemService.CreateItem(newItem);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "item created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return CreatedAtAction(nameof(GetByUid), new { uid = createdItem.uid }, createdItem);
    }

    // POST: /items/multiple
    [HttpPost("multiple")]
    public ActionResult<ItemCS> CreateMultipleItems([FromBody] List<ItemCS> newItems)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newItems is null)
        {
            return BadRequest("Item data is null");
        }

        var createdItems = _itemService.CreateMultipleItems(newItems);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "multiple items created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return StatusCode(StatusCodes.Status201Created, createdItems);
    }

    // PUT: items/5
    [HttpPut("{uid}")]
    public ActionResult<ItemCS> UpdateItem(string uid, [FromBody] ItemCS updatedItem)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales"};
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (updatedItem == null)
        {
            return BadRequest("Item is null.");
        }

        var existingItem = _itemService.GetItemById(uid);
        if (existingItem == null)
        {
            return NotFound();
        }

        var updatedItemResult = _itemService.UpdateItem(uid, updatedItem);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "item updated";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok(updatedItemResult);
    }
    // change the value of one property in an item object
    [HttpPatch("{uid}/{property}")]
    public ActionResult<ItemCS> PatchItem([FromRoute] string uid, [FromRoute] string property, [FromBody] object newvalue){
        if(string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(property) || newvalue is null){
            return BadRequest("Error in request");
        }
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales"};
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var result = _itemService.PatchItem(uid, property, newvalue);
        
        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "item updated";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok(result);
    } 
    [HttpDelete("{uid}")]
    public ActionResult DeleteItem(string uid)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var existingItem = _itemService.GetItemById(uid);
        if (existingItem == null)
        {
            return NotFound();
        }

        _itemService.DeleteItem(uid);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "item deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok();
    }

    // Delete: item/multiple/{id}
    // Deletes multiple items by their ids it can be used to delete multiple items at once so a list input is expected
    [HttpDelete("multiple")]
    public ActionResult DeleteMultipleItems([FromBody] List<string> uids)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (uids == null || uids.Count == 0)
        {
            return BadRequest("No items to delete.");
        }

        _itemService.DeleteItems(uids);
        
        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "multiple items deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok();
    } 
}
