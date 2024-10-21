using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;

namespace inventories.Controllers
{
    [ApiController]
    [Route("/inventories")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // GET: /inventories
        [HttpGet()]
        public ActionResult<IEnumerable<InventoryCS>> GetAllInventories()
        {
            var inventories = _inventoryService.GetAllInventories();
            return Ok(inventories);
        }

        // GET: /inventories/{id}
        [HttpGet("{id}")]
        public ActionResult<InventoryCS> GetInventoryById([FromRoute]int id)
        {
            var inventory = _inventoryService.GetInventoryById(id);
            if (inventory is null)
            {
                return NotFound();
            }
            return Ok(inventory);
        }

        // POST: /inventories
        [HttpPost]
        public async Task Post([FromBody] InventoryCS inventory)
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