using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ServicesV2;
using System.Threading.Tasks;

namespace ControllersV2;

[Route("api/v2/itemlines")]
[ApiController]
public class ItemLineController : ControllerBase
{
    private readonly IItemLineService _itemLineService;
    ActionLogService _actionlogservice;

    // Constructor to initialize the ItemController with an IItemService instance
    public ItemLineController(IItemLineService itemLineService)
    {
        _itemLineService = itemLineService;
        _actionlogservice = new ActionLogService();
    }

    // GET: api/items
    // Retrieves all items
    [HttpGet()]
    public ActionResult<IEnumerable<ItemCS>> GetAllItemLines()
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                               "Analyst", "Logistics", "Sales" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var itemLine = _itemLineService.GetAllItemlines();
        return Ok(itemLine);
    }

    // GET: api/itemlines/5
    [HttpGet("{id}")]
    public ActionResult<ItemLineCS> GetItemLineById(int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                               "Analyst", "Logistics", "Sales" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var itemLine = _itemLineService.GetItemLineById(id);
        if (itemLine == null)
        {
            return NotFound();
        }
        return Ok(itemLine);
    }

    // GET: /{id}/items
    [HttpGet("{id}/items")]
    public ActionResult<IEnumerable<ItemCS>> GetItemsByItemLineId([FromRoute] int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                               "Analyst", "Logistics", "Sales" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var Itemline = _itemLineService.GetItemLineById(id);
        if (Itemline is null)
        {
            return NotFound();
        }

        var items = _itemLineService.GetItemsByItemLineId(id);
        return Ok(items);
    }

    [HttpGet("latest-actions")]
    public ActionResult<IEnumerable<object>> GetItemLineWithLatestActions()
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

        if (userRole == null || !allowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }
        var itemlines = _itemLineService.GetAllItemlines();
        var actions = _actionlogservice.GetLatestActionsForItem_Groups();

        var result = itemlines.Select(itemline => new
        {
            ItemLine = itemline,
            LatestAction = actions.FirstOrDefault(action => action.model == "item-line")
        });

        return Ok(result);
    }
    
    [HttpGet("latest-actions/{amount}")]
    public ActionResult<IEnumerable<object>> GetItemLinesWithLatestActions([FromRoute] int amount)
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

        if (userRole == null || !allowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var itemlines = _itemLineService.GetAllItemlines();
        var actions = _actionlogservice.GetLatestActionsForItem_Groups();

        var result = itemlines.Select(itemline => new
        {
            ItemLine = itemline,
            LatestAction = actions.FirstOrDefault(action => action.model == "item-line")
        });
        var listed = result.ToList();
        while(listed.Count() > amount){
            listed.Remove(listed.Last());
        }

        return Ok(listed);
    }

    // POST: api/itemines
    [HttpPost]
    public IActionResult AddItemLine([FromBody] ItemLineCS newItemLine)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newItemLine == null)
        {
            return BadRequest("ItemLine is null.");
        }

        var createdItemLine = _itemLineService.AddItemLine(newItemLine);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "item-line created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return CreatedAtAction(nameof(GetItemLineById), new { id = createdItemLine.Id }, createdItemLine);
    }

    // POST: /itemlines/multiple
    [HttpPost("multiple")]
    public ActionResult<ItemLineCS> CreateMultipleItemLines([FromBody] List<ItemLineCS> newItemLines)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newItemLines is null)
        {
            return BadRequest("Item line data is null");
        }

        var createdItemLines = _itemLineService.CreateMultipleItemLines(newItemLines);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "multiple item-lines created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return StatusCode(StatusCodes.Status201Created, createdItemLines);
    }

    // PUT: api/itemLine/5
    [HttpPut("{id}")]
    public ActionResult<ItemLineCS> UpdateItemLine(int id, [FromBody] ItemLineCS itemLine)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (id != itemLine.Id)
        {
            return BadRequest();
        }

        var existingItemLine = _itemLineService.GetItemLineById(id);
        if (existingItemLine == null)
        {
            return NotFound();
        }

        var updatedItemLine = _itemLineService.UpdateItemLine(id, itemLine);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "item-line updated";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok(updatedItemLine);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteItemLine(int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var itemLine = _itemLineService.GetItemLineById(id);
        if (itemLine == null)
        {
            return NotFound();
        }

        _itemLineService.DeleteItemLine(id);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "item-line deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok();
    }

    [HttpDelete("batch")]
    public ActionResult DeleteItemLines([FromBody] List<int> ids)
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
        _itemLineService.DeleteItemLines(ids);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "multiple item-lines deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok("Item lines deleted");
    }

    [HttpPatch("{id}")]
    public ActionResult<ItemLineCS> PatchItemLine([FromRoute] int id, [FromBody] ItemLineCS itemLine)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        itemLine.Id = id;
        if (id != itemLine.Id)
        {
            return BadRequest();
        }

        var existingItemLine = _itemLineService.GetItemLineById(id);
        if (existingItemLine == null)
        {
            return NotFound();
        }

        var updatedItemLine = _itemLineService.PatchItemLine(id, itemLine);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "item";
        actionLog.action = "item-line patched";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok(updatedItemLine);
    }
}
