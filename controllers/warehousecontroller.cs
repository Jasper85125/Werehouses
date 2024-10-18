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
        public ActionResult<IEnumerable<WarehouseCS>> Get()
        {
            var warehouses = _warehouseService.GetAllWarehouses();
            return Ok(warehouses);
        }

        // GET: /warehouses/{id}
        [HttpGet("{id}")]
        public async Task<WarehouseCS> GetWarehouseById([FromRoute]int id)
        {
            var path = $"data/warehouses.json";
            if(!System.IO.File.Exists(path)) return null;
            string jsonString = await System.IO.File.ReadAllTextAsync(path);
            var employeeList = JsonSerializer.Deserialize<List<WarehouseCS>>(jsonString);
            var employee = employeeList.FirstOrDefault(e => e.Id == id);
            return employee;
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
    }
}