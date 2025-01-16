using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using ServicesV2;

namespace ControllersV2;

public class shipmentFilter
{
    // public int Id { get; set; }
    public int order_id { get; set; }
    public int source_id { get; set; }
    public DateTime order_date { get; set; }
    public DateTime request_date { get; set; }
    public DateTime shipment_date { get; set; }
    public string? shipment_type { get; set; }
    public string? shipment_status { get; set; }
    public string? carrier_code { get; set; }
    public string? service_code { get; set; }
    public string? payment_type { get; set; }
    public string? transfer_mode { get; set; }
    public int total_package_count { get; set; }
    public double total_package_weight { get; set; }
    // public List<ItemIdAndAmount> Items { get; set; }
    // public DateTime created_at { get; set; }
    // public DateTime updated_at { get; set; }
}

[ApiController]
[Route("api/v2/shipments")]
public class ShipmentController : ControllerBase
{
    private readonly IShipmentService _shipmentService;
    public ShipmentController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    // GET: /shipments
    /*
    [HttpGet()]
    public ActionResult<IEnumerable<ShipmentCS>> GetAllShipments()
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var shipments = _shipmentService.GetAllShipments();
        return Ok(shipments);
    }
    */
    //example route: /shipments?page=1&pageSize=10&order_id=1
    [HttpGet()]
    public ActionResult<PaginationCS<ShipmentCS>> GetAllShipments(
        [FromQuery] shipmentFilter tofilter,
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
        if (tofilter == null)
        {
            tofilter = new shipmentFilter();
        }
        var items = _shipmentService.GetAllShipments();
        var query = items.AsQueryable();

        // Apply filters
        if (tofilter.order_id != 0)
        {
            query = query.Where(x => x.order_id == tofilter.order_id);
        }
        if (tofilter.source_id != 0)
        {
            query = query.Where(x => x.source_id == tofilter.source_id);
        }
        if (tofilter.order_date != DateTime.MinValue)
        {
            query = query.Where(x => x.order_date == tofilter.order_date);
        }
        if (tofilter.request_date != DateTime.MinValue)
        {
            query = query.Where(x => x.request_date == tofilter.request_date);
        }
        if (tofilter.shipment_date != DateTime.MinValue)
        {
            query = query.Where(x => x.shipment_date == tofilter.shipment_date);
        }
        if (tofilter.shipment_type != null)
        {
            query = query.Where(x => x.shipment_type == tofilter.shipment_type);
        }
        if (tofilter.shipment_status != null)
        {
            query = query.Where(x => x.shipment_status == tofilter.shipment_status);
        }
        if (tofilter.carrier_code != null)
        {
            query = query.Where(x => x.carrier_code == tofilter.carrier_code);
        }
        if (tofilter.service_code != null)
        {
            query = query.Where(x => x.service_code == tofilter.service_code);
        }
        if (tofilter.payment_type != null)
        {
            query = query.Where(x => x.payment_type == tofilter.payment_type);
        }
        if (tofilter.transfer_mode != null)
        {
            query = query.Where(x => x.transfer_mode == tofilter.transfer_mode);
        }
        if (tofilter.total_package_count != 0)
        {
            query = query.Where(x => x.total_package_count >= tofilter.total_package_count);
        }
        if (tofilter.total_package_weight != 0)
        {
            query = query.Where(x => x.total_package_weight >= tofilter.total_package_weight);
        }

        // Get the filtered count
        int filteredShipmentsCount = query.Count();

        // Pagination logic
        int totalPages = (int)Math.Ceiling(filteredShipmentsCount / (double)pageSize);
        var pagedShipments = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        // Return paginated and filtered result
        var result = new PaginationCS<ShipmentCS>()
        {
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            Data = pagedShipments
        };

        return Ok(result);
    }

    // GET: /shipments/{id}
    [HttpGet("{id}")]
    public ActionResult<ShipmentCS> GetShipmentById([FromRoute] int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var shipment = _shipmentService.GetShipmentById(id);
        if (shipment is null)
        {
            return NotFound();
        }
        return Ok(shipment);
    }
    //shipments/{shipment_id}/items
    [HttpGet("{shipment_id}/items")]
    public ActionResult<IEnumerable<ItemIdAndAmount>> GetItemsInShipment([FromRoute] int shipment_id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var items = _shipmentService.GetItemsInShipment(shipment_id);
        if (items is null)
        {
            return NotFound();
        }
        return Ok(items);
    }

    // POST: /shipments
    [HttpPost]
    public ActionResult<ShipmentCS> CreateShipment([FromBody] ShipmentCS newShipment)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newShipment is null)
        {
            return BadRequest("shipment data is null");
        }
        var shipment = _shipmentService.CreateShipment(newShipment);
        return CreatedAtAction(nameof(GetShipmentById), new { id = shipment.Id }, shipment);
    }

    // POST: /shipments/multiple
    [HttpPost("multiple")]
    public ActionResult<IEnumerable<ShipmentCS>> CreateMultipleShipments([FromBody] List<ShipmentCS> newShipments)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newShipments is null)
        {
            return BadRequest("Shipment data is null");
        }

        var createdShipment = _shipmentService.CreateMultipleShipments(newShipments);
        return StatusCode(StatusCodes.Status201Created, createdShipment);
    }


    // PUT: api/warehouse/5
    [HttpPut("{id}")]
    public ActionResult<ShipmentCS> UpdateShipment([FromRoute] int id, [FromBody] ShipmentCS updateShipment)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (id != updateShipment.Id)
        {
            return BadRequest();
        }

        var existingItemLine = _shipmentService.GetShipmentById(id);
        if (existingItemLine == null)
        {
            return NotFound();
        }

        var updatedItemLine = _shipmentService.UpdateShipment(id, updateShipment);
        return Ok(updatedItemLine);
    }

    [HttpPut("{shipmentId}/items")]
    public ActionResult<ShipmentCS> UpdateItemsinShipment(int shipmentId, [FromBody] List<ItemIdAndAmount> updateItems)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (updateItems is null)
        {
            return BadRequest("invalid id's/ items");
        }
        ShipmentCS updated = _shipmentService.UpdateItemsInShipment(shipmentId, updateItems);
        if (updated is null)
        {
            return BadRequest("invalid id's/ items");
        }
        return Ok(updated);
    }
    [HttpPatch("{id}/{property}")]
    public ActionResult<ShipmentCS> PatchShipment([FromRoute] int id, [FromRoute] string property, [FromBody] object newvalue)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (property is null || newvalue is null)
        {
            return BadRequest("invalid request");
        }
        var result = _shipmentService.PatchShipment(id, property, newvalue);
        return Ok(result);
    }

    // DELETE: api/warehouse/5
    [HttpDelete("{id}")]
    public ActionResult DeleteShipment(int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var shipment = _shipmentService.GetShipmentById(id);
        if (shipment is null)
        {
            return NotFound();
        }
        _shipmentService.DeleteShipment(id);
        return Ok();
    }

    // DELETE /shipments/{id}/items/{itemid}
    [HttpDelete("{id}/items/{itemid}")]
    public ActionResult DeleteItemFromShipment(int id, string itemid)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var shipment = _shipmentService.GetShipmentById(id);
        if (shipment is null)
        {
            return NotFound();
        }
        _shipmentService.DeleteItemFromShipment(id, itemid);
        return Ok();
    }

    [HttpDelete("batch")]
    public ActionResult DeleteShipments([FromBody] List<int> ids)
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
        _shipmentService.DeleteShipments(ids);
        return Ok("deleted shipments");
    }
}
