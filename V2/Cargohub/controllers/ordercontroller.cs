using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using ServicesV2;

namespace ControllersV2
{
    [ApiController]
    [Route("api/v2/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: /orders
        [HttpGet()]
        public ActionResult<IEnumerable<OrderCS>> GetAllOrders()
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }

            var orders = _orderService.GetAllOrders();
            return Ok(orders);
        }

        // GET: /orders/{id}
        [HttpGet("{id}")]
        public ActionResult<OrderCS> GetOrderById([FromRoute] int id)
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }

            var orders = _orderService.GetOrderById(id);
            if (orders is null)
            {
                return NotFound();
            }
            return Ok(orders);
        }

        //get orders for clients using the shipp_to and bill_to fields
        [HttpGet("clients/{client_id}")]
        public ActionResult<IEnumerable<OrderCS>> GetOrdersByClient([FromRoute] int client_id)
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }

            var orders = _orderService.GetOrdersByClient(client_id);
            if (orders is null)
            {
                return NotFound();
            }
            return Ok(orders);
        }

        [HttpGet("{orderId}/items")]
        public ActionResult<List<ItemIdAndAmount>> GetItemsByOrderId(int orderId)
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics",
                                                                   "Operative", "Supervisor" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }

            var items = _orderService.GetItemsByOrderId(orderId);
            if (items == null)
            {
                return NotFound();
            }
            return Ok(items);
        }

        [HttpPost("orders")]
        public ActionResult<OrderCS> CreateOrder([FromBody] OrderCS order)
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }

            if (order == null)
            {
                return BadRequest("Order is null.");
            }

            var createdOrder = _orderService.CreateOrder(order);

            // Return the CreatedAtAction result, which includes the route to the GetOrderById action for the newly created order.
            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
        }

        // POST: /orders/multiple
        [HttpPost("orders/multiple")]
        public ActionResult<IEnumerable<OrderCS>> CreateMultipleOrders([FromBody] List<OrderCS> newOrders)
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }

            if (newOrders is null)
            {
                return BadRequest("Order data is null");
            }

            var createdOrders = _orderService.CreateMultipleOrders(newOrders);
            return StatusCode(StatusCodes.Status201Created, createdOrders);
        }

        // PUT: api/warehouse/5
        [HttpPut("{id}")]
        public async Task<ActionResult<OrderCS>> UpdateOrder(int id, [FromBody] OrderCS updateOrder)
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }

            if (id != updateOrder.Id)
            {
                return BadRequest();
            }

            var existingItemLine = _orderService.GetOrderById(id);
            if (existingItemLine == null)
            {
                return NotFound();
            }

            var updatedItemLine = await _orderService.UpdateOrder(id, updateOrder);
            return Ok(updatedItemLine);
        }

        [HttpPut("{orderId}/items")]
        public async Task<ActionResult<OrderCS>> UpdateOrderItems(int orderId, [FromBody] List<ItemIdAndAmount> items)
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }
            
            var order = _orderService.GetOrderById(orderId);
            if (order == null)
            {
                return NotFound();
            }

            if (items == null)
            {
                return BadRequest("The item field is required.");
            }

            var updatedOrder = await _orderService.UpdateOrderItems(orderId, items);
            return Ok(updatedOrder);
        }
        [HttpPatch("{id}/{property}")]
        public ActionResult<OrderCS> PatchOrder([FromRoute]int id, [FromRoute]string property, [FromBody]object newvalue){
            if(string.IsNullOrEmpty(property) || newvalue is null){
                return BadRequest("Missing inputs in request");
            }
            var result = _orderService.PatchOrder(id, property, newvalue);
            return Ok(result);
        }

        // DELETE: api/warehouse/5
        [HttpDelete("{id}")]
        public ActionResult DeleteOrder(int id)
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }

            var order = _orderService.GetOrderById(id);
            if (order is null)
            {
                return NotFound();
            }
            _orderService.DeleteOrder(id);
            return Ok();
        }

        [HttpDelete("batch")]
        public ActionResult DeleteOrders([FromBody] List<int> ids)
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
            _orderService.DeleteOrders(ids);
            return Ok("orders deleted");
        }
    }
}