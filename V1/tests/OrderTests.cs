using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesV1;
using Moq;
using ControllersV1;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace TestsV1
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
                total_surcharge = 10,
                items = new List<ItemIdAndAmount> { new ItemIdAndAmount { item_id = "1", amount = 10 } }
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
            _mockOrderService.Setup(service => service.CreateOrder(newOrder)).Returns(createdOrder);

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
        public void UpdateOrderTest_Success()
        {
            // Arrange
            var updatedOrder = new OrderCS { Id = 1, source_id = 24, order_status = "Shipped" };
            _mockOrderService.Setup(service => service.UpdateOrder(1, updatedOrder)).Returns(updatedOrder);
            _mockOrderService.Setup(service => service.GetOrderById(1)).Returns(updatedOrder);

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
        }

        [TestMethod]
        public void UpdateOrderTest_Failed()
        {
            // Arrange
            var updatedOrder = new OrderCS { Id = 1, source_id = 24, order_status = "Shipped" };
            _mockOrderService.Setup(service => service.UpdateOrder(1, updatedOrder)).Returns((OrderCS)null);

            // Act
            var result = _orderController.UpdateOrder(1, updatedOrder);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
        }


        [TestMethod]
        public void DeleteOrderTest_Exist()
        {
            //arrange
            var order = new OrderCS { Id = 1, source_id = 24, order_status = "Pending" };
            _mockOrderService.Setup(service => service.GetOrderById(1)).Returns(order);

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
            _mockOrderService.Setup(service => service.UpdateOrderItems(1, items)).Returns(updatedOrder);
            _mockOrderService.Setup(service => service.GetOrderById(1)).Returns(updatedOrder);

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

            // Act
            var result = _orderController.UpdateOrderItems(1, items);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
        }

        // Test for the Service
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
        public void GetOrdersByClientService_Test(){
            var orderService = new OrderService();
            var orders = orderService.GetOrdersByClient(1);
            Assert.IsNotNull(orders);
            Assert.AreEqual(1,orders.Count);
            Assert.AreEqual("Pending",orders[0].order_status);
        }

        [TestMethod]
        public void GetOrdersByClientService_Test_Fail(){
            var orderService = new OrderService();
            var orders = orderService.GetOrdersByClient(10);
            Assert.IsNull(orders);
        }

        [TestMethod]
        public void GetOrdersByShipmetIdService_Test(){
            var orderService = new OrderService();
            var order = orderService.GetOrdersByShipmentId(5);
            Assert.IsNotNull(order);
            Assert.AreEqual("Pending", order[0].order_status);
        }

        [TestMethod]
        public void GetOrdersByShipmetIdService_Test_Fail(){
            var orderService = new OrderService();
            var order = orderService.GetOrdersByShipmentId(10);
            Assert.IsNull(order);
        }

        [TestMethod]
        public void UpdateOrderService_Test(){
            var order = new OrderCS
            {
                Id = 1,
                source_id = 99,
                order_date = "2023-10-01T10:00:00Z",
                request_date = "2023-10-05T10:00:00Z",
                order_status = "Completed",
                warehouse_id = 1,
                ship_to = 1,
                bill_to = 3,
                shipment_id = 5,
                total_amount = 500,
                total_discount = 50,
                total_tax = 25,
                total_surcharge = 10
            };

            var orderService = new OrderService();
            var orders = orderService.UpdateOrder(1,order);
            Assert.IsNotNull(orders);
            Assert.AreEqual("Completed", orders.order_status);
        }

        [TestMethod]
        public void UpdateOrderService_Test_Failed()
        {
            var order = new OrderCS
            {
                Id = 3,
                source_id = 99,
                order_date = "2023-10-01T10:00:00Z",
                request_date = "2023-10-05T10:00:00Z",
                order_status = "Completed",
                warehouse_id = 1,
                ship_to = 1,
                bill_to = 3,
                shipment_id = 5,
                total_amount = 500,
                total_discount = 50,
                total_tax = 25,
                total_surcharge = 10
            };

            var orderService = new OrderService();
            var orders = orderService.UpdateOrder(3,order);
            Assert.IsNull(orders);
        }
        
        [TestMethod]
        public void DeleteOrderService_Test()
        {
            var orderService = new OrderService();
            orderService.DeleteOrder(1);
            var orderUpdated = orderService.GetAllOrders();
            Assert.AreEqual(0, orderUpdated.Count);
        }

        [TestMethod]
        public void DeleteOrderService_Test_failed()
        {
            var orderService = new OrderService();
            orderService.DeleteOrder(3);
            var orderUpdated = orderService.GetAllOrders();
            Assert.AreEqual(1, orderUpdated.Count);
        }

         [TestMethod]
        public void GetItemsByOrderIdService_Test()
        {
            var orderService = new OrderService();
            var items = orderService.GetItemsByOrderId(1);
            Assert.IsNotNull(items);
            Assert.AreEqual("1",items[0].item_id);
        }

        [TestMethod]
        public void GetItemsByOrderIdService_Test_Fail()
        {
            var orderService = new OrderService();
            var items = orderService.GetItemsByOrderId(2);
            Assert.IsNull(items);
        }
    
        [TestMethod]
        public void UpdateOrderItemsService_Test()
        {
            var item = new List<ItemIdAndAmount> { new ItemIdAndAmount { item_id = "2", amount = 20 } };
            var orderService = new OrderService();
            var orders = orderService.UpdateOrderItems(1,item);
            Assert.IsNotNull(orders);
            Assert.AreEqual("2",orders.items[0].item_id);
        }

        [TestMethod]
        public void UpdateOrderItemsService_Test_Fail()
        {
            var item = new List<ItemIdAndAmount> { new ItemIdAndAmount { item_id = "2", amount = 20 } };
            var orderService = new OrderService();
            var orders = orderService.UpdateOrderItems(2,item);
            Assert.IsNull(orders);
        }
    }
}

