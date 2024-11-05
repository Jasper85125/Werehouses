using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using Moq;
using Controllers;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;

namespace Tests
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
        public void CreateOrder_ReturnsCreatedResult_WithNewOrder()
        {
            // Arrange
            var newOrder = new OrderCS { Id = 1, source_id = 24, order_status = "Pending" };
            var createdOrder = new OrderCS { Id = 2, source_id = 24, order_status = "Pending" };
            
            // Set up the mock service to return the created order
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
        
        public void DeleteOrderTest_Exist(){
            //arrange
            var order = new OrderCS { Id = 1, source_id = 24, order_status = "Pending" };
            _mockOrderService.Setup(service => service.GetOrderById(1)).Returns(order);

            //act
            var result = _orderController.DeleteOrder(1);

            //assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
    }
}

