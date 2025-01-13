using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using ServicesV2;

namespace ControllersV2;

[ApiController]
[Route("api/v2/suppliers")]
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
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var suppliers = _supplierService.GetAllSuppliers();
        return Ok(suppliers);
    }

    // GET: /suppliers/{id}
    [HttpGet("{id}")]
    public ActionResult<SupplierCS> GetSupplierById([FromRoute] int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var supplier = _supplierService.GetSupplierById(id);
        if (supplier is null)
        {
            return NotFound();
        }
        return Ok(supplier);
    }

    // GET: /suppliers/{id}/items
    [HttpGet("{id}/items")]
    public ActionResult<IEnumerable<ItemCS>> GetItemsBySupplierId([FromRoute] int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var supplier = _supplierService.GetSupplierById(id);
        if (supplier is null)
        {
            return NotFound();
        }

        var items = _supplierService.GetItemsBySupplierId(id);
        return Ok(items);
    }

    // POST: /suppliers
    [HttpPost()]
    public ActionResult<SupplierCS> CreateSupplier([FromBody] SupplierCS supplier)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

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
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

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
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

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
  
    [HttpPatch("{id}")]
    public ActionResult<SupplierCS> PatchSupplier([FromRoute] int id, [FromBody] SupplierCS patch)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }
        
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

    // DELETE: api/warehouse/5
    [HttpDelete("{id}")]
    public ActionResult DeleteSupplier(int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var supplier = _supplierService.GetSupplierById(id);
        if (supplier is null)
        {
            return NotFound();
        }
        _supplierService.DeleteSupplier(id);
        return Ok();
    }

    [HttpDelete("batch")]
    public ActionResult DeleteSuppliers([FromBody] List<int> ids)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (ids is null)
        {
            return NotFound();
        }
        _supplierService.DeleteSuppliers(ids);
        return Ok("Deleted suppliers");
    }
}
