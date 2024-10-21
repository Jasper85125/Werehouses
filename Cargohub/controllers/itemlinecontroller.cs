using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using itemlines.Services;
using System.Threading.Tasks;

namespace itemlines.Controllers
{
    [Route("itemlines")]
    [ApiController]
    public class ItemLineController : ControllerBase
    {
        private readonly IItemLineService _itemLineService;

        // Constructor to initialize the ItemController with an IItemService instance
        public ItemLineController(IItemLineService itemLineService)
        {
            _itemLineService = itemLineService;
        }

        // GET: api/items
        // Retrieves all items
        [HttpGet()]
        public ActionResult<IEnumerable<ItemCS>> GetAllItemLines()
        {
            var itemLine = _itemLineService.GetAllItemline();
            return Ok(itemLine);
        }

        // GET: api/itemLine/5
        [HttpGet("{id}")]
        public ActionResult<ItemLineCS> GetItemById(int id)
        {
            var itemLine = _itemLineService.GetItemLineById(id);
            if (itemLine == null)
            {
                return NotFound();
            }
            return Ok(itemLine);
        }
        
        // POST: api/itemLine
        [HttpPost]
        public async Task<ActionResult<string>> PostItemLine([FromBody] string itemLine)
        {
            // Replace with actual logic to create a new item Line
            return CreatedAtAction(nameof(GetItemById), new { id = 1 }, itemLine);
        }

        // PUT: api/itemLine/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItemLine(int id, [FromBody] string itemLine)
        {
            // Replace with actual logic to update an existing item Line
            if (id != 1)
            {
                return BadRequest();
            }
            return NoContent();
        }

        // DELETE: api/itemLine/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemLine(int id)
        {
            // Replace with actual logic to delete an item Line
            if (id != 1)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}