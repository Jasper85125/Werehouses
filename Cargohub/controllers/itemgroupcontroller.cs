using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using itemgroup.Services;
using System.Threading.Tasks;

namespace itemgroup.Controllers
{
    [Route("itemgroups")]
    [ApiController]
    public class itemgroupController : ControllerBase
    {
        private readonly IitemgroupService _itemgroupService;

        // Constructor to initialize the ItemController with an IItemService instance
        public itemgroupController(IitemgroupService itemgroupService)
        {
            _itemgroupService = itemgroupService;
        }

        // GET: api/itemgroup
        // Retrieves all itemgroups
        [HttpGet()]
        public ActionResult<IEnumerable<ItemGroupCS>> GetAllItemGroups()
        {
            var itemgroup = _itemgroupService.GetAllItemGroups();
            return Ok(itemgroup);
        }

        // GET: api/itemtype/5
        [HttpGet("{id}")]
        public ActionResult<ItemGroupCS> GetItemById(int id)
        {
            var itemtype = _itemgroupService.GetItemById(id);
            if (itemtype == null)
            {
                return NotFound();
            }
            return Ok(itemtype);
        }
        
        // POST: api/itemtype
        [HttpPost]
        public async Task<ActionResult<string>> PostItemGroup([FromBody] string itemType)
        {
            // Replace with actual logic to create a new item type
            return null;
        }

        // PUT: api/itemtype/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItemGroup(int id, [FromBody] string itemType)
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
        public async Task<IActionResult> DeleteItemGroup(int id)
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