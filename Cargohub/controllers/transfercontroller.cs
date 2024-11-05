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

    // PUT: api/warehouse/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
        // Replace with your logic
    }

    // DELETE: api/warehouse/5
    [HttpDelete("{id}")]
    public ActionResult DeleteTransfer(int id)
    {
        var transfer = _transferService.GetTransferById(id);
        if (transfer is null)
        {
            return NotFound();
        }
        _transferService.DeleteTransfer(id);
        return Ok();
    }
}
