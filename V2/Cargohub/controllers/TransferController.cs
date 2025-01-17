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
    public TransferController(ITransferService transferService)
    {
        _transferService = transferService;
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
        return Ok(updatedAction);
    }
    //http://localhost:5002/api/v2/transfers/1?property=Reference
    [HttpPatch("{id}")]
    public ActionResult<TransferCS> PatchTransfer([FromRoute] int id, [FromQuery] string property, [FromBody] object newvalue){
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Supervisor", "Operative" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if(string.IsNullOrEmpty(property) || newvalue is null){
            return BadRequest("Invalid input");
        }
        var patched = _transferService.PatchTransfer(id, property, newvalue);
        if(patched is null){
            return NotFound("No transfer found with the given id.");
        }
        return Ok(patched);

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
        return Ok("deleted transfers");
    }

    // GET: /transfers/latest
    [HttpGet("latest")]
    public ActionResult<IEnumerable<TransferCS>> GetLatestTransfers()
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager",
                                                                "Analyst", "Supervisor", "Operative" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var latestTransfers = _transferService.GetLatestTransfers();
        return Ok(latestTransfers);
    }
}
