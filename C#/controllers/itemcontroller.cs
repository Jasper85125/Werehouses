using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        // GET: api/item
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "item1", "item2" };
        }

        // GET: api/item/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "item" + id;
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