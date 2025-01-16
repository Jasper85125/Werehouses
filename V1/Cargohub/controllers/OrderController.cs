using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using ServicesV1;

namespace ControllersV1
{
    [ApiController]
    [Route("api/v1/orders")]
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
            var orders = _orderService.GetAllOrders();
            return Ok(orders);
        }

        // GET: /orders/{id}
        [HttpGet("{id}")]
        public ActionResult<OrderCS> GetOrderById([FromRoute] int id)
        {
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
            var orders = _orderService.GetOrdersByClient(client_id);
            if (orders is null)
            {
                return NotFound();
            }
            return Ok(orders);
        }
        

        // PUT: api/warehouse/5
        [HttpPut("{id}")]
        public async Task<ActionResult<OrderCS>> UpdateOrder(int id, [FromBody] OrderCS updateOrder)
        {
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

        // DELETE: api/warehouse/5
        [HttpDelete("{id}")]
        public ActionResult DeleteOrder(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order is null)
            {
                return NotFound();
            }
            _orderService.DeleteOrder(id);
            return Ok();
        }

        [HttpPost()]
        public ActionResult<OrderCS> CreateOrder([FromBody] OrderCS order)
        {
            if (order == null)
            {
                return BadRequest("Order is null.");
            }

            var createdOrder = _orderService.CreateOrder(order);

            // Return the CreatedAtAction result, which includes the route to the GetOrderById action for the newly created order.
            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpGet("{orderId}/items")]
        public ActionResult<List<ItemIdAndAmount>> GetItemsByOrderId(int orderId)
        {
            var items = _orderService.GetItemsByOrderId(orderId);
            if (items == null)
            {
                return NotFound();
            }
            return Ok(items);
        }

        [HttpPut("{orderId}/items")]
        public async Task<ActionResult<OrderCS>> UpdateOrderItems(int orderId, [FromBody] List<ItemIdAndAmount> items)
        {
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

    }
}
