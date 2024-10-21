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
    public async Task Post([FromBody] TransferCS inventory)
    {
        
    }

    // PUT: api/warehouse/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
        // Replace with your logic
    }

    // DELETE: api/warehouse/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
        // Replace with your logic
    }
}
