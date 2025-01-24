using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesV2;
using Moq;
using ControllersV2;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TestsV2
{
    [TestClass]
    public class OrderTest
    {
        private Mock<IOrderService> _mockOrderService;
        private OrderController _orderController;

        [TestInitialize]
        public void Setup()
        {
            _mockOrderService = new Mock<IOrderService>();
            _orderController = new OrderController(_mockOrderService.Object);

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../data/orders.json");
        var order = new OrderCS
        {
            Id = 1,
            source_id = 22,
            order_date = "2023-10-01T10:00:00Z",
            request_date = "2023-10-05T10:00:00Z",
            order_status = "Pending",
            warehouse_id = 1,
            ship_to = 1,
            bill_to = 3,
            shipment_id = 5,
            total_amount = 500,
            total_discount = 50,
            total_tax = 25,
            total_surcharge = 10
        };

        var orderList = new List<OrderCS> { order };
        var json = JsonConvert.SerializeObject(orderList, Formatting.Indented);

        // Create directory if it does not exist
        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Write the JSON data to the file
        File.WriteAllText(filePath, json);
        }

        [TestMethod]
        public void GetOrdersTest_Exists()
        {
            //arrange
            var orders = new List<OrderCS>
            {
                new OrderCS { Id = 1, source_id = 22 },
                new OrderCS { Id = 2, source_id = 10 },
            };
            _mockOrderService.Setup(service => service.GetAllOrders()).Returns(orders);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";
            httpContext.Items["WarehouseID"] = "1,2,3,4";

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _orderController.GetAllOrders(null, 1, 10);
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as PaginationCS<OrderCS>;

            //Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Data.Count());

            httpContext.Items["UserRole"] = "Soldier";
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _orderController.GetAllOrders(null, 1, 10);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetOrderByIdTest_Exists()
        {
            //arrange
            var orders = new List<OrderCS>
            {
                new OrderCS { Id = 1, source_id = 24 },
                new OrderCS { Id = 2, source_id = 10 },
            };
            _mockOrderService.Setup(service => service.GetOrderById(1)).Returns(orders[0]);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _orderController.GetOrderById(1);

            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as OrderCS;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(orders[0].source_id, returnedItems.source_id);

            httpContext.Items["UserRole"] = "Soldier";
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _orderController.GetOrderById(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetShipmentByIdTest_WrongId()
        {
            //arrange
            _mockOrderService.Setup(service => service.GetOrderById(1)).Returns((OrderCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _orderController.GetOrderById(1);

            //Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));

            httpContext.Items["UserRole"] = "Soldier";
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _orderController.GetOrderById(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetOrdersByClientTest_Exists()
        {
            //arrange
            var orders = new List<OrderCS>
            {
                new OrderCS { Id = 1,  ship_to = 24 },
                new OrderCS { Id = 2,  bill_to = 24 },
            };
            _mockOrderService.Setup(service => service.GetOrdersByClient(24)).Returns(orders);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _orderController.GetOrdersByClient(24);

            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<OrderCS>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());

            httpContext.Items["UserRole"] = "Soldier";
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _orderController.GetOrderById(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void CreateOrder_ReturnsCreatedResult_WithNewOrder()
        {
            // Arrange
            var newOrder = new OrderCS { Id = 1, source_id = 24, order_status = "Pending" };
            var createdOrder = new OrderCS { Id = 2, source_id = 24, order_status = "Pending" };

            _mockOrderService.Setup(service => service.CreateOrder(newOrder)).Returns(createdOrder);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _orderController.CreateOrder(newOrder);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Value, typeof(OrderCS));
            var returnedOrder = createdResult.Value as OrderCS;
            Assert.AreEqual(2, returnedOrder.Id);
            Assert.AreEqual(24, returnedOrder.source_id);
            Assert.AreEqual("Pending", returnedOrder.order_status);

            httpContext.Items["UserRole"] = "Soldier";
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _orderController.CreateOrder(newOrder);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void CreateMultipleOrders_ReturnsCreatedResult_WithNewOrders()
        {
            // Arrange
            var orders = new List<OrderCS>
            {
                new OrderCS { Id = 1, source_id = 24, order_date = "2019-04-03T11:33:15Z", request_date = "2019-04-07T11:33:15Z",
                                      order_status = "Delivered", warehouse_id = 1, ship_to = 1, bill_to = 3, shipment_id = 5, total_amount = 909,
                                      total_discount = 30, total_tax = 100, total_surcharge = 78 },
                new OrderCS { Id = 1, source_id = 24, order_date = "2019-04-03T11:33:15Z", request_date = "2019-04-07T11:33:15Z",
                                      order_status = "Delivered", warehouse_id = 1, ship_to = 1, bill_to = 3, shipment_id = 5, total_amount = 909,
                                      total_discount = 30, total_tax = 100, total_surcharge = 78 }
            };
            _mockOrderService.Setup(service => service.CreateMultipleOrders(orders)).Returns(orders);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _orderController.CreateMultipleOrders(orders);
            var createdResult = result.Result as ObjectResult;
            var returnedItems = createdResult.Value as List<OrderCS>;
            var firstOrder = returnedItems[0];

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(orders[0].source_id, firstOrder.source_id);
            Assert.AreEqual(orders[0].order_status, firstOrder.order_status);

            httpContext.Items["UserRole"] = "Soldier";
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _orderController.CreateMultipleOrders(orders);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void UpdateOrderTest_Success()
        {
            // Arrange
            var updatedOrder = new OrderCS { Id = 1, source_id = 24, order_status = "Shipped" };
            _mockOrderService.Setup(service => service.UpdateOrder(1, updatedOrder)).Returns(updatedOrder);
            _mockOrderService.Setup(service => service.GetOrderById(1)).Returns(updatedOrder);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _orderController.UpdateOrder(1, updatedOrder);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(OrderCS));
            var returnedOrder = okResult.Value as OrderCS;
            Assert.AreEqual(updatedOrder.source_id, returnedOrder.source_id);
            Assert.AreEqual(updatedOrder.order_status, returnedOrder.order_status);

            httpContext.Items["UserRole"] = "Soldier";
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _orderController.UpdateOrder(1, updatedOrder);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }


        [TestMethod]
        public void UpdateOrderTest_Failed()
        {
            // Arrange
            var updatedOrder = new OrderCS { Id = 1, source_id = 24, order_status = "Shipped" };
            _mockOrderService.Setup(service => service.UpdateOrder(1, updatedOrder)).Returns((OrderCS)null);  
            _mockOrderService.Setup(service => service.GetOrderById(1)).Returns((OrderCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _orderController.UpdateOrder(1, updatedOrder);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);

            httpContext.Items["UserRole"] = "Soldier";
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _orderController.UpdateOrder(1, updatedOrder);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }


        [TestMethod]
        public void DeleteOrderTest_Exist()
        {
            //arrange
            var order = new OrderCS
            {
                Id = 1,
                source_id = 24,
                order_date = "2019-04-03T11:33:15Z",
                request_date = "2019-04-07T11:33:15Z",
                order_status = "Delivered",
                warehouse_id = 1,
                ship_to = 1,
                bill_to = 3,
                shipment_id = 5,
                total_amount = 909,
                total_discount = 30,
                total_tax = 100,
                total_surcharge = 78
            };
            _mockOrderService.Setup(service => service.GetOrderById(1)).Returns(order);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _orderController.DeleteOrder(1);

            //assert
            Assert.IsInstanceOfType(result, typeof(OkResult));

            httpContext.Items["UserRole"] = "Soldier";
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _orderController.DeleteOrder(1);

            //assert
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetItemsByOrderIdTest_Exists()
        {
            // Arrange
            var items = new List<ItemIdAndAmount>
            {
                new ItemIdAndAmount { item_id = "ITEM1", amount = 10 },
                new ItemIdAndAmount { item_id = "ITEM2", amount = 5 }
            };
            _mockOrderService.Setup(service => service.GetItemsByOrderId(1)).Returns(items);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _orderController.GetItemsByOrderId(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<ItemIdAndAmount>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());

            httpContext.Items["UserRole"] = "Soldier";
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _orderController.GetItemsByOrderId(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetItemsByOrderIdTest_WrongId()
        {
            // Arrange
            _mockOrderService.Setup(service => service.GetItemsByOrderId(1)).Returns((List<ItemIdAndAmount>)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _orderController.GetItemsByOrderId(1);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

            httpContext.Items["UserRole"] = "Soldier";
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _orderController.GetItemsByOrderId(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void UpdateOrderItemsTest_Success()
        {
            // Arrange
            var items = new List<ItemIdAndAmount>
    {
        new ItemIdAndAmount { item_id = "ITEM1", amount = 10 },
        new ItemIdAndAmount { item_id = "ITEM2", amount = 5 }
    };
            var updatedOrder = new OrderCS { Id = 1, items = items };
            _mockOrderService.Setup(service => service.UpdateOrderItems(1, items)).Returns(updatedOrder);  
            _mockOrderService.Setup(service => service.GetOrderById(1)).Returns(updatedOrder);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin"; 

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _orderController.UpdateOrderItems(1, items);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(OrderCS));
            var returnedOrder = okResult.Value as OrderCS;
            Assert.AreEqual(2, returnedOrder.items.Count);
            Assert.AreEqual("ITEM1", returnedOrder.items[0].item_id);
            Assert.AreEqual(10, returnedOrder.items[0].amount);
            Assert.AreEqual("ITEM2", returnedOrder.items[1].item_id);
            Assert.AreEqual(5, returnedOrder.items[1].amount);

            httpContext.Items["UserRole"] = "Soldier";
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _orderController.UpdateOrderItems(1, items);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }


        [TestMethod]
        public void UpdateOrderItemsTest_Failed()
        {
            // Arrange
            var items = new List<ItemIdAndAmount>
    {
        new ItemIdAndAmount { item_id = "ITEM1", amount = 10 },
        new ItemIdAndAmount { item_id = "ITEM2", amount = 5 }
    };
            _mockOrderService.Setup(service => service.UpdateOrderItems(1, items)).Returns((OrderCS)null);
            _mockOrderService.Setup(service => service.GetOrderById(1)).Returns((OrderCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _orderController.UpdateOrderItems(1, items);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
        }


        [TestMethod]
        public void PatchOrder_succes()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            //Arrange
            var patchedorder = new OrderCS() { Id = 1, Reference = "lol, weirdo" };
            _mockOrderService.Setup(_ => _.PatchOrder(1, "Reference", "lol, weirdo")).Returns(patchedorder);
            //Act
            var result = _orderController.PatchOrder(1, "Reference", "lol, weirdo");
            var resultok = result.Result as OkObjectResult;
            var value = resultok.Value as OrderCS;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultok);
            Assert.IsNotNull(value);
            Assert.AreEqual(resultok.StatusCode, 200);
            Assert.AreEqual(value.Reference, patchedorder.Reference);

            httpContext.Items["UserRole"] = "Soldier";
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _orderController.PatchOrder(1, "Reference", "lol, weirdo");

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }
        [TestMethod]
        public void DeleteOrdersTest_Succes()
        {
            //Arrange
            var ordersToDelete = new List<int>() { 1, 2, 3 };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _orderController.DeleteOrders(ordersToDelete);
            var resultok = result as OkObjectResult;

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(resultok.StatusCode, 200);

            httpContext.Items["UserRole"] = "Soldier";
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _orderController.DeleteOrders(ordersToDelete);

            //assert
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        // test for the service

        [TestMethod]
        public void GetAllOrdersService_Test(){
            var orderService = new OrderService();
            var orders = orderService.GetAllOrders();
            Assert.IsNotNull(orders);
            Assert.AreEqual(1, orders.Count);

        }

        [TestMethod]
        public void GetOrdersByIdService_Test(){
            var orderService = new OrderService();
            var order = orderService.GetOrderById(1);
            Assert.IsNotNull(order);
            Assert.AreEqual("Pending", order.order_status);
        }

        [TestMethod]
        public void GetOrdersByShipmetIdService_Test(){
            var orderService = new OrderService();
            var order = orderService.GetOrdersByShipmentId(5);
            Assert.IsNotNull(order);
            Assert.AreEqual("Pending", order[0].order_status);
        }

        [TestMethod]
        public void CreateOrderService_Test(){
            var order = new OrderCS{
                Id = 2,
                source_id = 24,
                order_date = "2023-10-01T10:00:00Z",
                request_date = "2023-10-05T10:00:00Z",
                Reference = "OrderRef123",
                reference_extra = "ExtraRef",
                order_status = "Pending",
                Notes = "Order notes",
                shipping_notes = "Shipping notes",
                picking_notes = "Picking notes",
                warehouse_id = 1,
                ship_to = 1,
                bill_to = 3,
                shipment_id = 5,
                total_amount = 500,
                total_discount = 50,
                total_tax = 25,
                total_surcharge = 10,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow,
                items = new List<ItemIdAndAmount>
                {
                    new ItemIdAndAmount { item_id = "ITEM1", amount = 20 },
                    new ItemIdAndAmount { item_id = "ITEM2", amount = 5 }
                }
            };
            var orderService = new OrderService();
            var orders = orderService.CreateOrder(order);
            Assert.IsNotNull(orders);
            Assert.AreEqual("Pending",orders.order_status);

            var ordersUpdated = orderService.GetAllOrders();
            Assert.AreEqual(2, ordersUpdated.Count);

        }

        [TestMethod]
        public void CreateMultipleOrdersService_Test()
        {
            var orders = new List<OrderCS> {new OrderCS
            {
                source_id = 29,
                order_date = "2023-10-01T10:00:00Z",
                request_date = "2023-10-05T10:00:00Z",
                Reference = "OrderRef123",
                reference_extra = "ExtraRef",
                order_status = "Pending",
                Notes = "Order notes",
                shipping_notes = "Shipping notes",
                picking_notes = "Picking notes",
                warehouse_id = 1,
                ship_to = 1,
                bill_to = 3,
                shipment_id = 5,
                total_amount = 500,
                total_discount = 50,
                total_tax = 25,
                total_surcharge = 10,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow,
                items = new List<ItemIdAndAmount>
                {
                    new ItemIdAndAmount { item_id = "ITEM1", amount = 250 },
                    new ItemIdAndAmount { item_id = "ITEM2", amount = 5 }
                }
                }, new OrderCS
                {
                
                source_id = 27,
                order_date = "2043-10-01 T10:00:00Z",
                request_date = "2024-10-05T10:00:00Z",
                Reference = "OrderRf123",
                reference_extra = "xtraRef",
                order_status = "Pending",
                Notes = "Order tes",
                shipping_notes = "Shping notes",
                picking_notes = "Piing notes",
                warehouse_id = 2,
                ship_to = 2,
                bill_to = 3,
                shipment_id = 6,
                total_amount = 5100,
                total_discount = 520,
                total_tax = 252,
                total_surcharge = 120,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow,
                items = new List<ItemIdAndAmount>
                {
                    new ItemIdAndAmount { item_id = "ITEM1", amount = 1 },
                    new ItemIdAndAmount { item_id = "ITEM2", amount = 5 }
                }
            }};

            var orderService = new OrderService();
            var createdOrders = orderService.CreateMultipleOrders(orders);
            Assert.IsNotNull(createdOrders);
            Assert.AreEqual(2, createdOrders.Count);
            Assert.AreEqual("Pending", createdOrders[0].order_status);
            Assert.AreEqual("Pending", createdOrders[1].order_status);

            var ordersUpdated = orderService.GetAllOrders();
            //Assert.AreEqual(4, ordersUpdated.Count);
            Assert.AreEqual(3, ordersUpdated.Count);

        }
    }
}

