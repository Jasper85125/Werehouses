using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesV2;
using Moq;
using ControllersV2;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;

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
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _orderController.GetAllOrders();

            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<OrderCS>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
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
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
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
        }

        [TestMethod]
        public void GetShipmentByIdTest_WrongId()
        {
            //arrange
            _mockOrderService.Setup(service => service.GetOrderById(1)).Returns((OrderCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _orderController.GetOrderById(1);

            //Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
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
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
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
        }
        [TestMethod]
        public void CreateOrder_ReturnsCreatedResult_WithNewOrder()
        {
            // Arrange
            var newOrder = new OrderCS { Id = 1, source_id = 24, order_status = "Pending" };
            var createdOrder = new OrderCS { Id = 2, source_id = 24, order_status = "Pending" };

            // Set up the mock service to return the created order
            _mockOrderService.Setup(service => service.CreateOrder(newOrder)).Returns(createdOrder);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
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
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
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
        }

        [TestMethod]
        public void UpdateOrderTest_Success()
        {
            // Arrange
            var updatedOrder = new OrderCS { Id = 1, source_id = 24, order_status = "Shipped" };
            _mockOrderService.Setup(service => service.UpdateOrder(1, updatedOrder)).Returns(Task.FromResult(updatedOrder));
            _mockOrderService.Setup(service => service.GetOrderById(1)).Returns(updatedOrder);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _orderController.UpdateOrder(1, updatedOrder);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(ActionResult<OrderCS>));
            var okResult = result.Result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(OrderCS));
            var returnedOrder = okResult.Value as OrderCS;
            Assert.AreEqual(updatedOrder.source_id, returnedOrder.source_id);
            Assert.AreEqual(updatedOrder.order_status, returnedOrder.order_status);
        }

        [TestMethod]
        public void UpdateOrderTest_Failed()
        {
            // Arrange
            var updatedOrder = new OrderCS { Id = 1, source_id = 24, order_status = "Shipped" };
            _mockOrderService.Setup(service => service.UpdateOrder(1, updatedOrder)).Returns(Task.FromResult((OrderCS)null));

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _orderController.UpdateOrder(1, updatedOrder);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(ActionResult<OrderCS>));
            var notFoundResult = result.Result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
        }


        [TestMethod]
        public void DeleteOrderTest_Exist()
        {
            //arrange
            var order = new OrderCS { Id = 1, source_id = 24, order_date = "2019-04-03T11:33:15Z", request_date = "2019-04-07T11:33:15Z",
                                      order_status = "Delivered", warehouse_id = 1, ship_to = 1, bill_to = 3, shipment_id = 5, total_amount = 909,
                                      total_discount = 30, total_tax = 100, total_surcharge = 78 };
            _mockOrderService.Setup(service => service.GetOrderById(1)).Returns(order);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _orderController.DeleteOrder(1);

            //assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
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
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
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
        }

        [TestMethod]
        public void GetItemsByOrderIdTest_WrongId()
        {
            // Arrange
            _mockOrderService.Setup(service => service.GetItemsByOrderId(1)).Returns((List<ItemIdAndAmount>)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _orderController.GetItemsByOrderId(1);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
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
            _mockOrderService.Setup(service => service.UpdateOrderItems(1, items)).Returns(Task.FromResult(updatedOrder));
            _mockOrderService.Setup(service => service.GetOrderById(1)).Returns(updatedOrder);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _orderController.UpdateOrderItems(1, items);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(ActionResult<OrderCS>));
            var okResult = result.Result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(OrderCS));
            var returnedOrder = okResult.Value as OrderCS;
            Assert.AreEqual(2, returnedOrder.items.Count);
            Assert.AreEqual("ITEM1", returnedOrder.items[0].item_id);
            Assert.AreEqual(10, returnedOrder.items[0].amount);
            Assert.AreEqual("ITEM2", returnedOrder.items[1].item_id);
            Assert.AreEqual(5, returnedOrder.items[1].amount);
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
            _mockOrderService.Setup(service => service.UpdateOrderItems(1, items)).Returns(Task.FromResult((OrderCS)null));

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _orderController.UpdateOrderItems(1, items);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(ActionResult<OrderCS>));
            var notFoundResult = result.Result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
        }

        [TestMethod]
        public void DeleteOrdersTest_Succes(){
            //Arrange
            var ordersToDelete = new List<int>(){1,2,3};

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _orderController.DeleteOrders(ordersToDelete);
            var resultok = result as OkObjectResult;
            
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(resultok.StatusCode, 200);
        }
    }
}

