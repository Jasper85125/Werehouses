using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using Services;

namespace ControllersV1;

[ApiController]
[Route("api/v1/suppliers")]
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
    public ActionResult<SupplierCS> GetSupplierById([FromRoute] int id)
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
    public ActionResult<SupplierCS> CreateSupplier([FromBody] SupplierCS supplier)
    {
        if (supplier == null)
        {
            return BadRequest("Supplier is null.");
        }

        var createdSupplier = _supplierService.CreateSupplier(supplier);

        return CreatedAtAction(nameof(GetSupplierById), new { id = createdSupplier.Id }, createdSupplier);
    }

    // PUT: /suppliers/{id}
    [HttpPut("{id}")]
    public ActionResult<SupplierCS> UpdateSupplier([FromRoute] int id, [FromBody] SupplierCS newSupplier)
    {
        if (newSupplier is null)
        {
            return BadRequest("Supplier is null.");
        }

        var updatedSupplier = _supplierService.UpdateSupplier(id, newSupplier);
        if (updatedSupplier is null)
        {
            return BadRequest("No supplier found with the given id.");
        }
        return Ok(updatedSupplier);
    }

    // DELETE: api/warehouse/5
    [HttpDelete("{id}")]
    public ActionResult DeleteSupplier(int id)
    {
        var supplier = _supplierService.GetSupplierById(id);
        if (supplier is null)
        {
            return NotFound();
        }
        _supplierService.DeleteSupplier(id);
        return Ok();
    }

    // GET: /suppliers/{id}/items
    [HttpGet("{id}/items")]
    public ActionResult<IEnumerable<ItemCS>> GetItemsBySupplierId([FromRoute] int id)
    {
        var supplier = _supplierService.GetSupplierById(id);
        if (supplier is null)
        {
            return NotFound();
        }

        var items = _supplierService.GetItemsBySupplierId(id);
        return Ok(items);
    }
}
