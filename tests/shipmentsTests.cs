using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using Moq;
using Controllers;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;

namespace Tests
{
    [TestClass]
    public class ShipmentTest
    {
        private Mock<IShipmentService> _mockShipmentService;
        private ShipmentController _shipmentController;

        [TestInitialize]
        public void Setup()
        {
            _mockShipmentService = new Mock<IShipmentService>();
            _shipmentController = new ShipmentController(_mockShipmentService.Object);
        }

        [TestMethod]
        public void GetShipmentsTest_Exists()
        {
            //arrange
            var shipments = new List<ShipmentCS>
            {
                new ShipmentCS { Id = 1, order_id = 1, source_id = 24 },
                new ShipmentCS { Id = 2, order_id = 4, source_id = 10 },
            };
            _mockShipmentService.Setup(service => service.GetAllShipments()).Returns(shipments);
            
            //Act
            var value = _shipmentController.GetAllShipments();
            
            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<ShipmentCS>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
        }

        [TestMethod]
        public void GetShipmentByIdTest_Exists()
        {
            //arrange
            var shipments = new List<ShipmentCS>
            {
                new ShipmentCS { Id = 1, order_id = 1, source_id = 24 },
                new ShipmentCS { Id = 2, order_id = 4, source_id = 10 },
            };
            _mockShipmentService.Setup(service => service.GetShipmentById(1)).Returns(shipments[0]);
            
            //Act
            var value = _shipmentController.GetShipmentById(1);
            
            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as ShipmentCS;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(shipments[0].source_id, returnedItems.source_id);
        }

        [TestMethod]
        public void GetShipmentByIdTest_WrongId()
        {
            //arrange
            _mockShipmentService.Setup(service => service.GetShipmentById(1)).Returns((ShipmentCS)null);
            
            //Act
            var value = _shipmentController.GetShipmentById(1);
            
            //Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void CreateShipment_ReturnsCreatedResult_WithNewShipment()
        {
            // Arrange
            var shipment = new ShipmentCS { Id = 1, order_id = 1, source_id = 24 };
            _mockShipmentService.Setup(service => service.CreateShipment(shipment)).Returns(shipment);
            
            // Act
            var value = _shipmentController.CreateShipment(shipment);
            
            // Assert
            var createdResult = value.Result as CreatedAtActionResult;  // Use CreatedAtActionResult
            Assert.IsNotNull(createdResult);
            
            var returnedItems = createdResult.Value as ShipmentCS;
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(shipment.source_id, returnedItems.source_id);
        }

    }
}

