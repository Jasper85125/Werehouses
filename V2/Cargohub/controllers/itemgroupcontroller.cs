using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ServicesV2;

namespace ControllersV2;

[Route("api/v2/itemgroups")]
[ApiController]
public class ItemGroupController : ControllerBase
{
    private readonly IitemGroupService _itemgroupService;
    ActionLogService _actionlogservice;

    public ItemGroupController(IitemGroupService itemgroupService)
    {
        _actionlogservice = new ActionLogService();
        _itemgroupService = itemgroupService;
    }

    // GET: itemgroup
    // Retrieves all itemgroups
    [HttpGet()]
    public ActionResult<IEnumerable<ItemGroupCS>> GetAllItemGroups()
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                               "Analyst", "Logistics", "Sales" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var itemgroup = _itemgroupService.GetAllItemGroups();
        return Ok(itemgroup);
    }

    // GET: itemtype/5
    [HttpGet("{id}")]
    public ActionResult<ItemGroupCS> GetItemById(int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                               "Analyst", "Logistics", "Sales" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var itemtype = _itemgroupService.GetItemById(id);
        if (itemtype == null)
        {
            return NotFound();
        }
        return Ok(itemtype);
    }

    //GET: all item in item groups
    [HttpGet("{id}/items")]
    public ActionResult<List<ItemCS>> GetAllItemsFromItemGroupId(int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                               "Analyst", "Logistics", "Sales" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var result = _itemgroupService.ItemsFromItemGroupId(id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet("latest-actions")]
    public ActionResult<IEnumerable<object>> GetItemGroupsWithLatestActions()
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

        if (userRole == null || !allowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }
        var itemgroups = _itemgroupService.GetAllItemGroups();
        var actions = _actionlogservice.GetLatestActionsForItem_Groups();

        var result = itemgroups.Select(itemgroup => new
        {
            ItemGroup = itemgroup,
            LatestAction = actions.FirstOrDefault(action => action.model == "item-group")
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

        var itemgroups = _itemgroupService.GetAllItemGroups();
        var actions = _actionlogservice.GetLatestActionsForItem_Groups();

        var result = itemgroups.Select(itemgroup => new
        {
            ItemGroup = itemgroup,
            LatestAction = actions.FirstOrDefault(action => action.model == "item-group")
        });
        var listed = result.ToList();
        while(listed.Count() > amount){
            listed.Remove(listed.Last());
        }

        return Ok(listed);
    }

    // POST: itemgroups
    [HttpPost()]
    public IActionResult CreateItemGroup([FromBody] ItemGroupCS itemGroup)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (itemGroup == null)
        {
            return BadRequest("ItemGroup cannot be null");
        }

        var createdItemGroup = _itemgroupService.CreateItemGroup(itemGroup);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "item-group created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return CreatedAtAction(nameof(GetItemById), new { id = createdItemGroup.Id }, createdItemGroup);
    }

    // POST: /itemgroup/multiple
    [HttpPost("multiple")]
    public ActionResult<ItemGroupCS> CreateMultipleItemGroups([FromBody] List<ItemGroupCS> newItemGroup)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newItemGroup is null)
        {
            return BadRequest("Item group data is null");
        }

        var createdItemGroups = _itemgroupService.CreateMultipleItemGroups(newItemGroup);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "multiple item-groups created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return StatusCode(StatusCodes.Status201Created, createdItemGroups);
    }

    [HttpPut("{id}")]
    public ActionResult<ItemGroupCS> UpdateItemGroup([FromRoute] int id, [FromBody] ItemGroupCS itemGroup)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (id != itemGroup.Id)
        {
            return BadRequest();
        }

        var existingItemLine = _itemgroupService.GetItemById(id);
        if (existingItemLine == null)
        {
            return NotFound();
        }

        var updatedItemLine = _itemgroupService.UpdateItemGroup(id, itemGroup);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "item-group updated";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok(updatedItemLine);
    }

    [HttpPatch("{id}")]
    public ActionResult<ItemGroupCS> PatchItemGroup([FromRoute] int id, [FromBody] ItemGroupCS itemGroup)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }
        
        var existingItemGroup = _itemgroupService.GetItemById(id);
        if (existingItemGroup == null)
        {
            return NotFound();
        }

        itemGroup.Id = id;
        var updatedItemGroup = _itemgroupService.PatchItemGroup(id, itemGroup);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "item-group patched";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok(updatedItemGroup);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteItemGroup(int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var itemGroup = _itemgroupService.GetItemById(id);
        if (itemGroup == null)
        {
            return NotFound();
        }

        _itemgroupService.DeleteItemGroup(id);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "item-group deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok();
    }

    [HttpDelete("batch")]
    public ActionResult DeleteItemGroups([FromBody] List<int> ids)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (ids is null)
        {
            return NotFound();
        }
        _itemgroupService.DeleteItemGroups(ids);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "multiple item-groups deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok("Item Groups deleted");
    }
}
