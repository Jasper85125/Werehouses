using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;

namespace warehouses.Controllers
{
    [ApiController]
    [Route("/warehouses")]
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
            var warehouses = _warehouseService.GetAllWarehouses();
            if (warehouses is null)
            {
                return NotFound();
            }
            return Ok(warehouses);
        }

        // GET: /warehouses/{id}
        [HttpGet("{id}")]
        public ActionResult<WarehouseCS> GetWarehouseById([FromRoute]int id)
        {
            var warehouse = _warehouseService.GetWarehouseById(id);
            if (warehouse is null)
            {
                return NotFound();
            }
            return Ok(warehouse);
        }

        // POST: /warehouses
        [HttpPost]
        public async Task Post([FromBody] WarehouseCS warehouse)
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
        // POST: warehouses
        // Creates a new warehouse
        [HttpPost("warehouse")]
        public ActionResult CreateWarehouse([FromBody] WarehouseCS newWarehouse)
        {
            if (newWarehouse == null)
            {
                return BadRequest("Warehouse data is null");
            }

            _warehouseService.CreateWarehouse(newWarehouse);
            return Ok("warehouse created");
        }
    }
}