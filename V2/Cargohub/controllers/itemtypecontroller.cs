using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ServicesV2;
using System.Threading.Tasks;

namespace ControllersV2;

[Route("api/v2/itemtypes")]
[ApiController]
public class ItemTypeController : ControllerBase
{
    private readonly IItemtypeService _itemtypeService;
    private readonly IItemService _itemService;
    ActionLogService _actionlogservice;

    // Constructor to initialize the ItemController with an IItemService instance
    public ItemTypeController(IItemtypeService itemtypeService, IItemService itemService)
    {
        _itemtypeService = itemtypeService;
        _itemService = itemService;
        _actionlogservice = new ActionLogService();
    }

    // GET: items
    // Retrieves all items
    [HttpGet()]
    public ActionResult<IEnumerable<ItemCS>> GetAllItemtypes()
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                               "Analyst", "Logistics", "Sales" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var itemtype = _itemtypeService.GetAllItemtypes();
        return Ok(itemtype);
    }

    //GET: /itemtypes/{itemtypeID}/items
    [HttpGet("{id}/items")]
    public ActionResult<IEnumerable<ItemCS>> GetAllItemsInItemType(int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                               "Analyst", "Logistics", "Sales" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var itemsInItemType = _itemService.GetAllItemsInItemType(id);
        if (itemsInItemType is null)
        {
            return BadRequest("No items found with the given ItemType ID");
        }
        return Ok(itemsInItemType);
    }

    // GET: itemtype/5
    [HttpGet("{id}")]
    public ActionResult<ItemTypeCS> GetItemById(int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                               "Analyst", "Logistics", "Sales" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var itemtype = _itemtypeService.GetItemById(id);
        if (itemtype == null)
        {
            return NotFound();
        }
        return Ok(itemtype);
    }

    [HttpGet("latest-actions")]
    public ActionResult<IEnumerable<object>> GetItemTypesWithLatestActions()
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

        if (userRole == null || !allowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }
        var itemtypes = _itemtypeService.GetAllItemtypes();
        var actions = _actionlogservice.GetLatestActionsForItem_Groups();

        var result = itemtypes.Select(itemtype => new
        {
            ItemType = itemtype,
            LatestAction = actions.FirstOrDefault(action => action.model == "item-type")
        });

        return Ok(result);
    }
    
    [HttpGet("latest-actions/{amount}")]
    public ActionResult<IEnumerable<object>> GetItemTypesWithLatestActions([FromRoute] int amount)
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

        if (userRole == null || !allowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var itemtype = _itemtypeService.GetAllItemtypes();
        var actions = _actionlogservice.GetLatestActionsForItem_Groups();

        var result = itemtype.Select(itemtype => new
        {
            ItemTypes = itemtype,
            LatestAction = actions.FirstOrDefault(action => action.model == "item-type")
        });
        var listed = result.ToList();
        while(listed.Count() > amount){
            listed.Remove(listed.Last());
        }

        return Ok(listed);
    }

    // POST: itemtype
    [HttpPost()]
    public async Task<IActionResult> CreateItemType([FromBody] ItemTypeCS itemtype)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (itemtype == null)
        {
            return BadRequest("ItemGroup cannot be null");
        }

        var createditemtype = _itemtypeService.CreateItemType(itemtype);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "item-type created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return CreatedAtAction(nameof(GetItemById), new { id = createditemtype.Id }, createditemtype);
    }

    // POST: /itemtypes/multiple
    [HttpPost("multiple")]
    public ActionResult<ItemTypeCS> CreateMultipleItemTypes([FromBody] List<ItemTypeCS> newItemTypes)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newItemTypes is null)
        {
            return BadRequest("ItemType data is null");
        }

        var createdItemTypes = _itemtypeService.CreateMultipleItemTypes(newItemTypes);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "multiple item-types created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return StatusCode(StatusCodes.Status201Created, createdItemTypes);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ItemTypeCS>> UpdateItemType(int id, [FromBody] ItemTypeCS itemType)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (id != itemType.Id)
        {
            return BadRequest();
        }

        var existingItemLine = _itemtypeService.GetItemById(id);
        if (existingItemLine == null)
        {
            return NotFound();
        }

        var updatedItemLine = await _itemtypeService.UpdateItemType(id, itemType);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "item-type updated";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok(updatedItemLine);
    }
    //zet een nieuwe value in een property van een item_type object 
    [HttpPatch("{id}/{property}")]
    public ActionResult<ItemTypeCS> PatchItemType([FromRoute] int id, [FromRoute] string property, [FromBody] object newvalue){
        if(int.IsNegative(id) || string.IsNullOrEmpty(property) || newvalue is null){
            return BadRequest("Errors in request");
        }
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var result = _itemtypeService.PatchItemType(id, property, newvalue);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "item-type patched";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok(result);
    }

    
    [HttpDelete("{id}")]
    public ActionResult DeleteItemType(int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var itemtype = _itemtypeService.GetItemById(id);
        
        if (itemtype == null)
        {
            return NotFound();
        }
        _itemtypeService.DeleteItemType(id);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "item-type deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);
        
        return Ok();
    }

    [HttpDelete("batch")]
    public ActionResult DeleteItemTypes(List<int> ids)
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
        _itemtypeService.DeleteItemTypes(ids);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "multiple item-types deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok("item types deleted");
    }
}