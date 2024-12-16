using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using ServicesV2;

namespace ControllersV2;

[ApiController]
[Route("api/v2/transfers")]
public class TransferController : ControllerBase
{
    private readonly ITransferService _transferService;
    ActionLogService _actionlogservice;
    public TransferController(ITransferService transferService)
    {
        _transferService = transferService;
        _actionlogservice = new ActionLogService();
    }

    // GET: /transfers
    [HttpGet()]
    public ActionResult<IEnumerable<TransferCS>> GetAllTransfers()
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                               "Floor Manager", "Analyst", "Supervisor", "Operative" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var transfers = _transferService.GetAllTransfers();
        return Ok(transfers);
    }

    // GET: /transfers/{id}
    [HttpGet("{id}")]
    public ActionResult<TransferCS> GetTransferById([FromRoute] int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Analyst", "Supervisor", "Operative" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var inventory = _transferService.GetTransferById(id);
        if (inventory is null)
        {
            return NotFound();
        }
        return Ok(inventory);
    }

    [HttpGet("{transfer_id}/items")]
    public ActionResult<IEnumerable<ItemIdAndAmount>> GetItemsInTransfer([FromRoute] int transfer_id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Analyst", "Supervisor", "Operative" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var items = _transferService.GetItemsInTransfer(transfer_id);
        if (items is null)
        {
            return NotFound();
        }
        return Ok(items);
    }

    [HttpGet("latest-actions")]
    public ActionResult<IEnumerable<object>> GetTransfersWithLatestActions()
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

        if (userRole == null || !allowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var transfers = _transferService.GetAllTransfers();
        var actions = _actionlogservice.GetLatestActionsForTransfers();

        var result = transfers.Select(transfer => new
        {
            Transfer = transfer,
            LatestAction = actions.FirstOrDefault(action => action.model == "transfer")
        });

        return Ok(result);
    }
    [HttpGet("latest-actions/{amount}")]
    public ActionResult<IEnumerable<object>> GetTransfersWithLatestActions([FromRoute] int amount)
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

        if (userRole == null || !allowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var transfers = _transferService.GetAllTransfers();
        var actions = _actionlogservice.GetLatestActionsForTransfers();

        var result = transfers.Select(transfer => new
        {
            Transfer = transfer,
            LatestAction = actions.FirstOrDefault(action => action.model == "transfer")
        });
        var listed = result.ToList();
        while(listed.Count() > amount){
            listed.Remove(listed.Last());
        }

        return Ok(listed);
    }

    // POST: /transfers
    [HttpPost]
    public ActionResult<TransferCS> CreateTransfer([FromBody] TransferCS transfer)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Supervisor", "Operative" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (transfer is null)
        {
            return BadRequest("transfer data is null");
        }
        var newTransfer = _transferService.CreateTransfer(transfer);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "transfer";
        actionLog.action = "transfer created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return CreatedAtAction(nameof(GetTransferById), new { id = newTransfer.Id }, newTransfer);
    }

    // POST: /transfers/multiple
    [HttpPost("multiple")]
    public ActionResult<IEnumerable<TransferCS>> CreateMultipleTransfers([FromBody] List<TransferCS> newTransfer)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Supervisor", "Operative" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newTransfer is null)
        {
            return BadRequest("Transfer data is null");
        }

        var createdTransfer = _transferService.CreateMultipleTransfers(newTransfer);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "transfer";
        actionLog.action = "multiple transfers created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return StatusCode(StatusCodes.Status201Created, createdTransfer);
    }

    // PUT: transfers/1
    [HttpPut("{id}")]
    public ActionResult<TransferCS> UpdateTransfer([FromRoute] int id, [FromBody] TransferCS newTransfer)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Supervisor", "Operative" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newTransfer is null)
        {
            return BadRequest("Warehouse is null.");
        }

        var updatedTransfer = _transferService.UpdateTransfer(id, newTransfer);
        if (updatedTransfer is null)
        {
            return NotFound("No warehouse found with the given id.");
        }

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "transfer";
        actionLog.action = "transfer updated";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok(updatedTransfer);
    }

    [HttpPut("{id}/commit")]
    public ActionResult<TransferCS> CommitTransfer([FromRoute] int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Supervisor", "Operative" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var updatedAction = _transferService.CommitTransfer(id);
        if (updatedAction is null)
        {
            return NotFound("There is no transfer with the given id!!");
        }

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "transfer";
        actionLog.action = "transfer commited";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok(updatedAction);
    }

    // DELETE: api/warehouse/5
    [HttpDelete("{id}")]
    public ActionResult DeleteTransfer(int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var transfer = _transferService.GetTransferById(id);
        if (transfer is null)
        {
            return NotFound();
        }
        _transferService.DeleteTransfer(id);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "transfer";
        actionLog.action = "transfer deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok();
    }

    [HttpDelete("batch")]
    public ActionResult DeleteTransfers(List<int> ids)
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
        _transferService.DeleteTransfers(ids);

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "transfer";
        actionLog.action = "multiple transfers deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok("deleted transfers");
    }

    // // GET: /transfers/latest
    // [HttpGet("latest")]
    // public ActionResult<IEnumerable<TransferCS>> GetLatestTransfers()
    // {
    //     List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager",
    //                                                             "Analyst", "Supervisor", "Operative" };
    //     var userRole = HttpContext.Items["UserRole"]?.ToString();

    //     if (userRole == null || !listOfAllowedRoles.Contains(userRole))
    //     {
    //         return Unauthorized();
    //     }

    //     var latestTransfers = _transferService.GetLatestTransfers();
    //     return Ok(latestTransfers);
    // }
}
