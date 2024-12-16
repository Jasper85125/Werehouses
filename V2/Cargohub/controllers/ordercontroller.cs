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
        ActionLogService _actionlogservice;
        public OrderController(IOrderService orderService)
        {
        _actionlogservice = new ActionLogService();
            _orderService = orderService;
        }

        // GET: /orders
        [HttpGet()]
        public ActionResult<IEnumerable<OrderCS>> GetAllOrders()
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager",
                                                                   "Floor Manager", "Sales", "Analyst", "Logistics" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();
            var WarehouseIDFromKey = (int)HttpContext.Items["WarehouseID"];

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                if (userRole == "Operative" || userRole == "Supervisor")
                {
                    var ordersForWarehouse = _orderService.GetOrdersByWarehouse(WarehouseIDFromKey);
                    return Ok(ordersForWarehouse);
                }
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

        [HttpGet("latest-actions")]
        public ActionResult<IEnumerable<object>> GetOrdersWithLatestActions()
        {
            var userRole = HttpContext.Items["UserRole"]?.ToString();
            List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

            if (userRole == null || !allowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }
            var orders = _orderService.GetAllOrders();
            var actions = _actionlogservice.GetLatestActionsForOrders();

            var result = orders.Select(order => new
            {
                Order = order,
                LatestAction = actions.FirstOrDefault(action => action.model == "order")
            });

            return Ok(result);
        }
        
        [HttpGet("latest-actions/{amount}")]
        public ActionResult<IEnumerable<object>> GetOrdersWithLatestActions([FromRoute] int amount)
        {
            var userRole = HttpContext.Items["UserRole"]?.ToString();
            List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

            if (userRole == null || !allowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }

            var orders = _orderService.GetAllOrders();
            var actions = _actionlogservice.GetLatestActionsForOrders();

            var result = orders.Select(order => new
            {
                Order = order,
                LatestAction = actions.FirstOrDefault(action => action.model == "order")
            });
            var listed = result.ToList();
            while(listed.Count() > amount){
                listed.Remove(listed.Last());
            }

            return Ok(listed);
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

            var actionlogs = _actionlogservice.GetAllActionLogs();
            ActionLogCS actionLog = new ActionLogCS();
            actionLog.performed_by = userRole;
            actionLog.id = actionlogs.Count()  + 1;
            actionLog.model = "order";
            actionLog.action = "order created";
            actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
            actionlogs.Add(actionLog);
            _actionlogservice.SaveActionLogs(actionlogs);

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

            var actionlogs = _actionlogservice.GetAllActionLogs();
            ActionLogCS actionLog = new ActionLogCS();
            actionLog.performed_by = userRole;
            actionLog.id = actionlogs.Count()  + 1;
            actionLog.model = "order";
            actionLog.action = "multiple orders created";
            actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
            actionlogs.Add(actionLog);
            _actionlogservice.SaveActionLogs(actionlogs);

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

            var actionlogs = _actionlogservice.GetAllActionLogs();
            ActionLogCS actionLog = new ActionLogCS();
            actionLog.performed_by = userRole;
            actionLog.id = actionlogs.Count()  + 1;
            actionLog.model = "order";
            actionLog.action = "order updated";
            actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
            actionlogs.Add(actionLog);
            _actionlogservice.SaveActionLogs(actionlogs);

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

            var actionlogs = _actionlogservice.GetAllActionLogs();
            ActionLogCS actionLog = new ActionLogCS();
            actionLog.performed_by = userRole;
            actionLog.id = actionlogs.Count()  + 1;
            actionLog.model = "order";
            actionLog.action = "items inside order updated";
            actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
            actionlogs.Add(actionLog);

            _actionlogservice.SaveActionLogs(actionlogs);

            return Ok(updatedOrder);
        }
        [HttpPatch("{id}/{property}")]
        public ActionResult<OrderCS> PatchOrder([FromRoute]int id, [FromRoute]string property, [FromBody]object newvalue){
            if(string.IsNullOrEmpty(property) || newvalue is null){
                return BadRequest("Missing inputs in request");
            }

            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }
            var result = _orderService.PatchOrder(id, property, newvalue);

            var actionlogs = _actionlogservice.GetAllActionLogs();
            ActionLogCS actionLog = new ActionLogCS();
            actionLog.performed_by = userRole;
            actionLog.id = actionlogs.Count()  + 1;
            actionLog.model = "order";
            actionLog.action = "order patched";
            actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
            actionlogs.Add(actionLog);

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

            var actionlogs = _actionlogservice.GetAllActionLogs();
            ActionLogCS actionLog = new ActionLogCS();
            actionLog.performed_by = userRole;
            actionLog.id = actionlogs.Count()  + 1;
            actionLog.model = "order";
            actionLog.action = "order deleted";
            actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
            actionlogs.Add(actionLog);

            return Ok("order deleted");
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

            var actionlogs = _actionlogservice.GetAllActionLogs();
            ActionLogCS actionLog = new ActionLogCS();
            actionLog.performed_by = userRole;
            actionLog.id = actionlogs.Count()  + 1;
            actionLog.model = "order";
            actionLog.action = "multiple orders deleted";
            actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
            actionlogs.Add(actionLog);

            return Ok("orders deleted");
        }
    }
}