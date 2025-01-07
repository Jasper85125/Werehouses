using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using ServicesV2;

namespace ControllersV2;

public class warehouseFilter
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Zip { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    // public Dictionary<string, string> Contact { get; set; }
    // public DateTime created_at { get; set; }
    // public DateTime updated_at { get; set; }
}

[ApiController]
[Route("api/v2/warehouses")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;
    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    // GET: /warehouses
    [HttpGet()]
    public ActionResult<IEnumerable<WarehouseCS>> GetAllWarehouses()
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Analyst", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var warehouses = _warehouseService.GetAllWarehouses();
        if (warehouses is null)
        {
            return NotFound();
        }
        return Ok(warehouses);
    }
    [HttpGet("page")]
    public ActionResult<PaginationCS<WarehouseCS>> GetAllItems(
        [FromQuery] warehouseFilter tofilter, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        List<string> listOfAllowedRoles = new List<string>()
        { "Admin", "Warehouse Manager", "Inventory Manager", "Floor Manager", "Sales", "Analyst", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }
        // Filter logic
        var warehouses = _warehouseService.GetAllWarehouses();
        var query = warehouses.AsQueryable();
        if (tofilter.Id != 0)
        {
            query = query.Where(x => x.Id == tofilter.Id);
        }
        if (tofilter.Code != null)
        {
            query = query.Where(x => x.Code == tofilter.Code);
        }
        if (tofilter.Name != null)
        {
            query = query.Where(x => x.Name == tofilter.Name);
        }
        if (tofilter.Address != null)
        {
            query = query.Where(x => x.Address == tofilter.Address);
        }
        if (tofilter.Zip != null)
        {
            query = query.Where(x => x.Zip == tofilter.Zip);
        }
        if (tofilter.City != null)
        {
            query = query.Where(x => x.City == tofilter.City);
        }
        if (tofilter.Province != null)
        {
            query = query.Where(x => x.Province == tofilter.Province);
        }
        if (tofilter.Country != null)
        {
            query = query.Where(x => x.Country == tofilter.Country);
        }
        var warehousesCount = query.Count();

        // Pagination logic
        int totalPages = (int)Math.Ceiling(warehousesCount / (double)pageSize);
        var pagedWarehouses = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        // Return paginated and filtered result
        var result = new PaginationCS<WarehouseCS>()
        {
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            Data = pagedWarehouses
        };

        return Ok(result);
    }

    // GET: /warehouses/{id}
    [HttpGet("{id}")]
    public ActionResult<WarehouseCS> GetWarehouseById([FromRoute] int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager", "Floor Manager", "Sales", "Analyst", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var warehouse = _warehouseService.GetWarehouseById(id);
        if (warehouse is null)
        {
            return NotFound();
        }
        return Ok(warehouse);
    }

    // POST: /warehouses
    [HttpPost()]
    public ActionResult<WarehouseCS> CreateWarehouse([FromBody] WarehouseCS newWarehouse)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newWarehouse is null)
        {
            return BadRequest("Warehouse data is null");
        }

        var createdWarehouse = _warehouseService.CreateWarehouse(newWarehouse);
        return CreatedAtAction(nameof(GetWarehouseById), new { id = createdWarehouse.Id }, createdWarehouse);
    }

    // POST: /warehouses/multiple
    [HttpPost("multiple")]
    public ActionResult<WarehouseCS> CreateMultipleWarehouse([FromBody] List<WarehouseCS> newWarehouse)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newWarehouse is null)
        {
            return BadRequest("Warehouse data is null");
        }

        var createdWarehouses = _warehouseService.CreateMultipleWarehouse(newWarehouse);
        return StatusCode(StatusCodes.Status201Created, createdWarehouses);
    }

    // PUT: /warehouses/{id}
    [HttpPut("{id}")]
    public ActionResult<WarehouseCS> UpdateWarehouse([FromRoute] int id, [FromBody] WarehouseCS newWarehouse)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newWarehouse is null)
        {
            return BadRequest("Warehouse is null.");
        }

        var updatedWarehouse = _warehouseService.UpdateWarehouse(id, newWarehouse);
        if (updatedWarehouse is null)
        {
            return NotFound("No warehouse found with the given id.");
        }
        return Ok(updatedWarehouse);
    }
    //PATCH: Warehouse/{id}/{property_to_change}
    //''   :     ''   /  ''/      contact == werkt niet
    [HttpPatch("{id}/{property}")]
    public ActionResult<WarehouseCS> PatchWarehouse([FromRoute] int id, [FromRoute] string property, [FromBody] object newvalue){
        if(newvalue is null){
            return NotFound("Erhm what?");
        }
        var result = _warehouseService.PatchWarehouse(id, property, newvalue);
        return Ok(result);
    }
    // DELETE: api/warehouse/5
    [HttpDelete("{id}")]
    public ActionResult DeleteWarehouse([FromRoute] int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var warehouse = _warehouseService.GetWarehouseById(id);
        if (warehouse is null)
        {
            return NotFound();
        }
        _warehouseService.DeleteWarehouse(id);

        return Ok();
    }

    [HttpDelete("batch")]
    public ActionResult DeleteWarehouses([FromBody] List<int> ids)
    {
        List<string> listOfAllowedRoles = new List<string> () {"Admin"};
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (ids is null)
        {
            return NotFound();
        }
        _warehouseService.DeleteWarehouses(ids);
        return Ok("Deleted warehouses");
    }

    // GET: /warehouses/latest
    [HttpGet("latest")]
    public ActionResult<WarehouseCS> GetLatestUpdatedWarehouse()
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Analyst", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var latestWarehouse = _warehouseService.GetLatestUpdatedWarehouse();
        if (latestWarehouse is null)
        {
            return NotFound();
        }
        return Ok(latestWarehouse);
    }
}
