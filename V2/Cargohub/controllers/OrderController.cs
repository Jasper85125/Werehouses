using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using ServicesV2;

namespace ControllersV2
{
    public class orderFilter()
    {
        public int Id { get; set; }
        public int source_id { get; set; }
        public string? order_date { get; set; }
        public string? request_date { get; set; }
        public string? order_status { get; set; }
        public int? warehouse_id { get; set; }
        public int? ship_to { get; set; }
        public int? bill_to { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int itemsCount { get; set; }
    }

    [ApiController]
    [Route("api/v2/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet]
        public ActionResult<PaginationCS<OrderCS>> GetAllOrders([FromQuery] orderFilter filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userRole = HttpContext.Items["UserRole"]?.ToString();
            if (!HttpContext.Items.TryGetValue("WarehouseID", out var warehouseIdObj) || !(warehouseIdObj is string warehouseID))
            {
                return BadRequest("WarehouseID is missing or invalid.");
            }

            var allowedRoles = new List<string> { "Admin", "Warehouse Manager", "Inventory Manager", "Floor Manager", "Sales", "Analyst", "Logistics" };

            if (string.IsNullOrEmpty(userRole) || !allowedRoles.Contains(userRole))
            {
                if (userRole == "Operative" || userRole == "Supervisor")
                {
                    var warehouseid = warehouseID.Split(',').Select(int.Parse).ToList();
                    var warehouseOrderslist = new List<List<OrderCS>>();
                    foreach (var id in warehouseid)
                    {
                        var warehouseOrders = _orderService.GetOrdersByWarehouse(id);
                        warehouseOrderslist.Add(warehouseOrders);
                    }
                    return Ok(warehouseOrderslist);
                }
                return Unauthorized();
            }

            filter ??= new orderFilter();
            var ordersQuery = _orderService.GetAllOrders().AsQueryable();

            if (filter.Id > 0) ordersQuery = ordersQuery.Where(o => o.Id == filter.Id);
            if (filter.source_id > 0) ordersQuery = ordersQuery.Where(o => o.source_id == filter.source_id);
            if (!string.IsNullOrWhiteSpace(filter.order_date)) ordersQuery = ordersQuery.Where(o => o.order_date == filter.order_date);
            if (!string.IsNullOrEmpty(filter.order_status)) ordersQuery = ordersQuery.Where(o => o.order_status == filter.order_status);
            if (filter.warehouse_id > 0) ordersQuery = ordersQuery.Where(o => o.warehouse_id == filter.warehouse_id);
            if (filter.ship_to > 0) ordersQuery = ordersQuery.Where(o => o.ship_to == filter.ship_to);
            if (filter.bill_to > 0) ordersQuery = ordersQuery.Where(o => o.bill_to == filter.bill_to);
            if (filter.itemsCount > 0) ordersQuery = ordersQuery.Where(o => o.items.Count() == filter.itemsCount);

            var ordersCount = ordersQuery.Count();
            var totalPages = (int)Math.Ceiling(ordersCount / (double)pageSize);
            if (page == 0)
            {
                page = totalPages;
            }
            page = Math.Max(1, Math.Min(page, totalPages));

            var data = ordersQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new PaginationCS<OrderCS>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = data
            });
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

        [HttpGet("{shipmentId}/shipments")]
        public ActionResult<List<ItemIdAndAmount>> GetOrdersByShipmentId([FromRoute] int shipmentId)
        {
            var orders = _orderService.GetOrdersByShipmentId(shipmentId);
            if (orders == null)
            {
                return NotFound();
            }
            return Ok(orders);
        }

        [HttpPost()]
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

            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
        }

        // POST: /orders/multiple
        [HttpPost("multiple")]
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
        public ActionResult<OrderCS> UpdateOrder(int id, [FromBody] OrderCS updateOrder)
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }

            var existingItemLine = _orderService.GetOrderById(id);
            if (existingItemLine == null)
            {
                return NotFound();
            }

            var updatedItemLine = _orderService.UpdateOrder(id, updateOrder);
            return Ok(updatedItemLine);
        }

        [HttpPut("{orderId}/items")]
        public ActionResult<OrderCS> UpdateOrderItems(int orderId, [FromBody] List<ItemIdAndAmount> items)
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

            var updatedOrder = _orderService.UpdateOrderItems(orderId, items); 
            return Ok(updatedOrder);
        }
        
        [HttpPatch("{id}")]
        public ActionResult<OrderCS> PatchOrder([FromRoute] int id, [FromQuery] string property, [FromBody] object newvalue)
        {
            List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
            var userRole = HttpContext.Items["UserRole"]?.ToString();

            if (userRole == null || !listOfAllowedRoles.Contains(userRole))
            {
                return Unauthorized();
            }
            
            if (string.IsNullOrEmpty(property) || newvalue is null)
            {
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
