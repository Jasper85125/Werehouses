using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using Services;

namespace Controllers;

[ApiController]
[Route("/suppliers")]
public class SupplierController : ControllerBase
{
    private readonly ISupplierService _supplierService;
    public SupplierController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    // GET: /suppliers
    [HttpGet()]
    public ActionResult<IEnumerable<SupplierCS>> GetAllSuppliers()
    {
        var suppliers = _supplierService.GetAllSuppliers();
        return Ok(suppliers);
    }

    // GET: /suppliers/{id}
    [HttpGet("{id}")]
    public ActionResult<SupplierCS> GetSupplierById([FromRoute]int id)
    {
        var supplier = _supplierService.GetSupplierById(id);
        if (supplier is null)
        {
            return NotFound();
        }
        return Ok(supplier);
    }

    // POST: /suppliers
    [HttpPost()]
        public ActionResult<SupplierCS> CreateLocation([FromBody] SupplierCS supplier)
        {
            if (supplier == null)
            {
                return BadRequest("Supplier is null.");
            }

            var createdSupplier = _supplierService.CreateSupplier(supplier);

            return CreatedAtAction(nameof(GetSupplierById), new { id = createdSupplier.Id }, createdSupplier);
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
