using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using Services;

namespace Controllers;

[ApiController]
[Route("/transfers")]
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
        var transfers = _transferService.GetAllTransfers();
        return Ok(transfers);
    }

    // GET: /transfers/{id}
    [HttpGet("{id}")]
    public ActionResult<TransferCS> GetTransferById([FromRoute]int id)
    {
        var inventory = _transferService.GetTransferById(id);
        if (inventory is null)
        {
            return NotFound();
        }
        return Ok(inventory);
    }

    // POST: /transfers
    [HttpPost]
    public ActionResult<TransferCS> Post([FromBody] TransferCS transfer)
    {
        if (transfer is null)
        {
            return BadRequest("transfer data is null");
        }
        var newTransfer = _transferService.CreateTransfer(transfer);
        return CreatedAtAction(nameof(GetTransferById), new { id = newTransfer.Id }, newTransfer);
    }

    // PUT: api/transfer/1
    [HttpPut("{id}")]
    public ActionResult<TransferCS> UpdateTransfer([FromRoute]int id, [FromBody] TransferCS newTransfer)
    {
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

    // DELETE: api/warehouse/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
        // Replace with your logic
    }
}
