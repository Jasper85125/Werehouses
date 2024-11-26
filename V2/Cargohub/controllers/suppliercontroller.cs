using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using ServicesV2;

namespace ControllersV2;

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

    // POST: /supplier/multiple
        [HttpPost("multiple")]
        public ActionResult<IEnumerable<SupplierCS>> CreateMultipleSuppliers([FromBody] List<SupplierCS> newSupplier)
        {
            if (newSupplier is null)
            {
                return BadRequest("Supplier data is null");
            }

            var createdOrders = _supplierService.CreateMultipleSuppliers(newSupplier);
            return StatusCode(StatusCodes.Status201Created, createdOrders);
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

    [HttpPatch("{id}")]
    public ActionResult<SupplierCS> PatchSupplier([FromRoute] int id, [FromBody] SupplierCS patch)
    {
        var supplier = _supplierService.GetSupplierById(id);
        if (supplier is null)
        {
            return NotFound();
        }

        var updatedSupplier = _supplierService.PatchSupplier(id, patch);
        if (updatedSupplier is null)
        {
            return BadRequest("Failed to patch supplier.");
        }

        return Ok(updatedSupplier);
    }
    [HttpDelete("batch")]
    public ActionResult DeleteSuppliers([FromBody]List<int> ids){
        if(ids is null){
            return NotFound();
        }
        _supplierService.DeleteSuppliers(ids);
        return Ok("Deleted suppliers");
    }
}
