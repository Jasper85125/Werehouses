using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using itemtype.Services;
using System.Threading.Tasks;

namespace itemtype.Controllers
{
    [Route("itemtypes")]
    [ApiController]
    public class ItemTypeController : ControllerBase
    {
        private readonly IItemtypeService _itemtypeService;

        // Constructor to initialize the ItemController with an IItemService instance
        public ItemTypeController(IItemtypeService itemtypeService)
        {
            _itemtypeService = itemtypeService;
        }

        // GET: api/items
        // Retrieves all items
        [HttpGet()]
        public ActionResult<IEnumerable<ItemCS>> GetAllItemtypes()
        {
            var itemtype = _itemtypeService.GetAllItemtypes();
            return Ok(itemtype);
        }

        // GET: api/itemtype/5
        [HttpGet("{id}")]
        public ActionResult<ItemTypeCS> GetItemById(int id)
        {
            var itemtype = _itemtypeService.GetItemById(id);
            if (itemtype == null)
            {
                return NotFound();
            }
            return Ok(itemtype);
        }
        
        // POST: api/itemtype
        [HttpPost]
        public async Task<ActionResult<string>> PostItemType([FromBody] string itemType)
        {
            // Replace with actual logic to create a new item type
            return CreatedAtAction(nameof(GetItemById), new { id = 1 }, itemType);
        }

        // PUT: api/itemtype/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItemType(int id, [FromBody] string itemType)
        {
            // Replace with actual logic to update an existing item type
            if (id != 1)
            {
                return BadRequest();
            }
            return NoContent();
        }

        // DELETE: api/itemtype/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemType(int id)
        {
            // Replace with actual logic to delete an item type
            if (id != 1)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}