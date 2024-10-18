using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace item.Controllers
{
    [Route("items")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        // Constructor to initialize the ItemController with an IItemService instance
        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        // GET: api/items
        // Retrieves all items
        [HttpGet()]
        public ActionResult<IEnumerable<ItemCS>> GetAllItems()
        {
            var items = _itemService.GetAllItems();
            return Ok(items);
        }

        // GET: api/items/5
        // Retrieves an item by its unique identifier (uid)
        [HttpGet("{uid}")]
        public ActionResult<ItemCS> GetByUid(string uid)
        {
            var item = _itemService.GetItemById(uid);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        // POST: api/items
        // Creates a new item (implementation needed)
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/items/5
        // Updates an existing item by its id (implementation needed)
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/items/5
        // Deletes an item by its id (implementation needed)
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}