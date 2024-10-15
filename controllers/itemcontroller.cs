using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace item.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        // GET: api/item
        [HttpGet()]
        public ActionResult<IEnumerable<ItemCS>> GetAllItems()
        {
            var items = _itemService.GetAllItems();
            return Ok(items);
        }

        // GET: api/item/5
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

        // POST: api/item
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/item/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/item/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}